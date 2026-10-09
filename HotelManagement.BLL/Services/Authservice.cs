using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

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
                return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau,
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
            }

            // Dùng bản có Include VaiTro/NhanSu, KHÔNG AsNoTracking - vì cần Update lại
            // SolanSaimatkhau/Trangthai trong cùng 1 DbContext khi xác thực sai.
            var taiKhoan = await _uow.TaiKhoanRepository.GetByTenDangNhapKemVaiTroAsync(tenDangNhap.Trim());

            if (taiKhoan is null)
            {
                return TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau, "Sai tên đăng nhập hoặc mật khẩu.");
            }

            if (taiKhoan.Trangthai == TrangThaiKhoa)
            {
                return TaoKetQua(TrangThaiDangNhap.TaiKhoanBiKhoa,
                    $"Tài khoản đã bị khóa do nhập sai mật khẩu quá {SoLanSaiToiDa} lần. Vui lòng liên hệ Admin.");
            }

            bool matKhauDung = false;
            try
            {
                matKhauDung = BCrypt.Net.BCrypt.Verify(matKhau, taiKhoan.MatkhauHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Nếu hash trong DB không phải chuẩn BCrypt (ví dụ insert tay bằng plaintext)
                matKhauDung = false;
            }

            if (!matKhauDung)
            {
                taiKhoan.SolanSaimatkhau++;

                bool viBiKhoa = taiKhoan.SolanSaimatkhau >= SoLanSaiToiDa;
                if (viBiKhoa)
                    taiKhoan.Trangthai = TrangThaiKhoa;

                _uow.TaiKhoanRepository.Update(taiKhoan);
                await _uow.SaveChangesAsync();

                return viBiKhoa
                    ? TaoKetQua(TrangThaiDangNhap.TaiKhoanBiKhoa,
                        $"Sai mật khẩu {SoLanSaiToiDa} lần liên tiếp. Tài khoản đã bị khóa.")
                    : TaoKetQua(TrangThaiDangNhap.SaiTenDangNhapHoacMatKhau,
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

            return new KetQuaDangNhap
            {
                TrangThai = TrangThaiDangNhap.ThanhCong,
                TaiKhoan = taiKhoan,
                ThongBao = "Đăng nhập thành công."
            };
        }

        public async Task DangXuatAsync(int idTaiKhoan)
            => await GhiNhatKyAsync(idTaiKhoan, "Đăng xuất");

        public string HashMatKhau(string matKhauTho)
            => BCrypt.Net.BCrypt.HashPassword(matKhauTho);

        public async Task<(bool Success, string Message)> DoiMatKhauAsync(int idTaiKhoan, string matKhauCu, string matKhauMoi)
        {
            var taiKhoan = await _uow.TaiKhoanRepository.GetByIdAsync(idTaiKhoan);
            if (taiKhoan == null) return (false, "Tài khoản không tồn tại.");

            bool matKhauDung = false;
            try
            {
                matKhauDung = BCrypt.Net.BCrypt.Verify(matKhauCu, taiKhoan.MatkhauHash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                matKhauDung = false;
            }

            if (!matKhauDung)
                return (false, "Mật khẩu cũ không chính xác.");

            taiKhoan.MatkhauHash = HashMatKhau(matKhauMoi);
            // Bỏ _uow.TaiKhoanRepository.Update(taiKhoan) vì entity đã được track bởi DbContext
            // EF Core sẽ tự phát hiện property MatkhauHash bị thay đổi để Update đúng field đó.
            await _uow.SaveChangesAsync();

            // Không ghi log "Đổi mật khẩu" vì CSDL có CHECK CONSTRAINT chỉ cho phép "Đăng nhập" và "Đăng xuất"
            // await GhiNhatKyAsync(idTaiKhoan, "Đổi mật khẩu");
            
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