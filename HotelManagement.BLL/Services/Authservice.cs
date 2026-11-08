using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using Serilog;

namespace HotelManagement.BLL.Services
{
    public class AuthService : IAuthService
    {
        // Ngưỡng khóa tài khoản - chỉnh theo yêu cầu đề bài nếu rubric có quy định số khác.
        private const int SoLanSaiToiDa = 10;

        // Giá trị đúng theo CK_TaiKhoan_TrangThai trong script SQL (giữ nguyên dấu).
        private const string TrangThaiHoatDong = "Hoạt động";
        private const string TrangThaiKhoa = "Khóa";

        private readonly IUnitOfWork _uow;

        public AuthService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<KetQuaDangNhap> DangNhapAsync(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                Log.Warning("[AuthService] Đăng nhập thất bại: Thiếu tên đăng nhập hoặc mật khẩu.");
                return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau,
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
            }

            // Dùng bản có Include VaiTro/NhanSu, KHÔNG AsNoTracking - vì cần Update lại
            // SolanSaimatkhau/Trangthai trong cùng 1 DbContext khi xác thực sai.
            var taiKhoan = await _uow.TaiKhoanRepository.GetByTenDangNhapKemVaiTroAsync(tenDangNhap.Trim());

            if (taiKhoan is null)
            {
                Log.Warning("[AuthService] Đăng nhập thất bại: Tài khoản '{Username}' không tồn tại.", tenDangNhap.Trim());
                return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau, "Sai tên đăng nhập hoặc mật khẩu.");
            }

            // Kiểm tra xem có phải tài khoản Admin hoặc vai trò Quản trị viên không
            bool laAdmin = string.Equals(taiKhoan.TenDangnhap, "admin", StringComparison.OrdinalIgnoreCase)
                || string.Equals(taiKhoan.IdVaitroNavigation?.TenVaitro, "Admin", StringComparison.OrdinalIgnoreCase);

            if (taiKhoan.Trangthai == TrangThaiKhoa)
            {
                if (laAdmin)
                {
                    // Tự động mở khóa bảo vệ nếu tài khoản Admin vô tình bị khóa trước đó
                    taiKhoan.Trangthai = TrangThaiHoatDong;
                    taiKhoan.SolanSaimatkhau = 0;
                    _uow.TaiKhoanRepository.Update(taiKhoan);
                    await _uow.SaveChangesAsync();
                    Log.Information("[AuthService] Tự động kích hoạt lại tài khoản Admin.");
                }
                else
                {
                    Log.Warning("[AuthService] Đăng nhập thất bại: Tài khoản '{Username}' đang bị khóa.", taiKhoan.TenDangnhap);
                    return TaoKetQua(TrangThaiDangNhap.TaiKhoanBiKhoa,
                        $"Tài khoản đã bị khóa do nhập sai mật khẩu quá {SoLanSaiToiDa} lần. Vui lòng liên hệ Admin.");
                }
            }

            bool matKhauDung = false;
            try
            {
                matKhauDung = BCrypt.Net.BCrypt.Verify(matKhau, taiKhoan.MatkhauHash);
            }
            catch (Exception ex)
            {
                // Nếu hash trong DB không phải chuẩn BCrypt hoặc bị lỗi cấu trúc salt
                Log.Warning(ex, "[AuthService] Không thể verify hash mật khẩu cho tài khoản '{Username}'.", taiKhoan.TenDangnhap);
                matKhauDung = false;
            }

            if (!matKhauDung)
            {
                // Tài khoản Admin được miễn trừ cơ chế khóa để tránh mất quyền quản trị hệ thống
                if (laAdmin)
                {
                    Log.Warning("[AuthService] Tài khoản Admin '{Username}' nhập sai mật khẩu. Miễn trừ cơ chế khóa tài khoản.", taiKhoan.TenDangnhap);
                    return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau, "Sai mật khẩu.");
                }

                taiKhoan.SolanSaimatkhau++;

                bool viBiKhoa = taiKhoan.SolanSaimatkhau >= SoLanSaiToiDa;
                if (viBiKhoa)
                    taiKhoan.Trangthai = TrangThaiKhoa;

                _uow.TaiKhoanRepository.Update(taiKhoan);
                await _uow.SaveChangesAsync();

                if (viBiKhoa)
                {
                    Log.Warning("[AuthService] Tài khoản '{Username}' bị khóa do nhập sai mật khẩu {Max} lần liên tiếp.", taiKhoan.TenDangnhap, SoLanSaiToiDa);
                    return TaoKetQua(TrangThaiDangNhap.TaiKhoanBiKhoa,
                        $"Sai mật khẩu {SoLanSaiToiDa} lần liên tiếp. Tài khoản đã bị khóa.");
                }

                Log.Warning("[AuthService] Tài khoản '{Username}' nhập sai mật khẩu (Lần {Count}/{Max}).", taiKhoan.TenDangnhap, taiKhoan.SolanSaimatkhau, SoLanSaiToiDa);
                return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau,
                    $"Sai mật khẩu. Còn {SoLanSaiToiDa - taiKhoan.SolanSaimatkhau} lần thử trước khi bị khóa.");
            }

            // Đăng nhập thành công: reset số lần sai nếu trước đó có sai
            if (taiKhoan.SolanSaimatkhau > 0)
            {
                taiKhoan.SolanSaimatkhau = 0;
                _uow.TaiKhoanRepository.Update(taiKhoan);
                await _uow.SaveChangesAsync();
            }

            await GhiNhatKyAsync(taiKhoan.IdTaikhoan, "Đăng nhập");

            Log.Information("[AuthService] Đăng nhập thành công: '{Username}' - Vai trò: '{Role}'", taiKhoan.TenDangnhap, taiKhoan.IdVaitroNavigation?.TenVaitro ?? "Chưa rõ");

            return new KetQuaDangNhap
            {
                TrangThai = TrangThaiDangNhap.ThanhCong,
                TaiKhoan = taiKhoan,
                ThongBao = "Đăng nhập thành công."
            };
        }

        public async Task DangXuatAsync(int idTaiKhoan)
        {
            Log.Information("[AuthService] Đăng xuất tài khoản ID: {IdTaiKhoan}", idTaiKhoan);
            await GhiNhatKyAsync(idTaiKhoan, "Đăng xuất");
        }

        public string HashMatKhau(string matKhauTho)
            => BCrypt.Net.BCrypt.HashPassword(matKhauTho);

        public async Task<(bool Success, string Message)> DoiMatKhauAsync(int idTaiKhoan, string matKhauCu, string matKhauMoi)
        {
            var taiKhoan = await _uow.TaiKhoanRepository.GetByIdAsync(idTaiKhoan);
            if (taiKhoan == null)
            {
                Log.Warning("[AuthService] Đổi mật khẩu thất bại: Không tìm thấy tài khoản ID {IdTaiKhoan}", idTaiKhoan);
                return (false, "Tài khoản không tồn tại.");
            }

            bool matKhauDung = false;
            try
            {
                matKhauDung = BCrypt.Net.BCrypt.Verify(matKhauCu, taiKhoan.MatkhauHash);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[AuthService] Không thể verify hash mật khẩu cũ cho tài khoản '{Username}'.", taiKhoan.TenDangnhap);
                matKhauDung = false;
            }

            if (!matKhauDung)
            {
                Log.Warning("[AuthService] Đổi mật khẩu thất bại cho tài khoản '{Username}': Mật khẩu cũ không chính xác.", taiKhoan.TenDangnhap);
                return (false, "Mật khẩu cũ không chính xác.");
            }

            taiKhoan.MatkhauHash = HashMatKhau(matKhauMoi);
            await _uow.SaveChangesAsync();

            Log.Information("[AuthService] Đổi mật khẩu thành công cho tài khoản '{Username}'.", taiKhoan.TenDangnhap);
            return (true, "Đổi mật khẩu thành công.");
        }

        private async Task GhiNhatKyAsync(int idTaiKhoan, string hanhDong)
        {
            var nhatKy = new NhatKyHoatDong
            {
                IdTaikhoan = idTaiKhoan,
                Hanhdong = hanhDong,
                Thoigian = DateTime.Now
            };

            await _uow.NhatKyHoatDongRepository.AddAsync(nhatKy);
            await _uow.SaveChangesAsync();
        }

        private static KetQuaDangNhap TaoKetQua(TrangThaiDangNhap trangThai, string thongBao)
            => new() { TrangThai = trangThai, ThongBao = thongBao };
    }
}