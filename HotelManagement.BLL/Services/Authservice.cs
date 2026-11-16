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
            if (string.IsNullOrWhiteSpace(matKhauCu))
                return (false, "Vui lòng nhập mật khẩu hiện tại.");

            if (string.IsNullOrWhiteSpace(matKhauMoi))
                return (false, "Vui lòng nhập mật khẩu mới.");

            if (matKhauMoi.Length < 6)
                return (false, "Mật khẩu mới phải có độ dài tối thiểu 6 ký tự.");

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
                Log.Warning("[AuthService] Đổi mật khẩu thất bại cho tài khoản '{Username}': Mật khẩu hiện tại không chính xác.", taiKhoan.TenDangnhap);
                return (false, "Mật khẩu hiện tại không chính xác.");
            }

            // Kiểm tra mật khẩu mới phải khác mật khẩu hiện tại (DCKLTTQ-110)
            bool trungMatKhauCu = false;
            try
            {
                trungMatKhauCu = BCrypt.Net.BCrypt.Verify(matKhauMoi, taiKhoan.MatkhauHash);
            }
            catch
            {
                trungMatKhauCu = false;
            }

            if (trungMatKhauCu)
            {
                return (false, "Mật khẩu mới phải khác mật khẩu hiện tại.");
            }

            taiKhoan.MatkhauHash = HashMatKhau(matKhauMoi);
            taiKhoan.UpdatedAt = DateTime.Now;
            _uow.TaiKhoanRepository.Update(taiKhoan);
            await _uow.SaveChangesAsync();

            // Ghi nhật ký vào NhatKyHoatDong (DCKLTTQ-110)
            await GhiNhatKyAsync(idTaiKhoan, "Đổi mật khẩu");

            Log.Information("[AuthService] Đổi mật khẩu thành công cho tài khoản '{Username}'.", taiKhoan.TenDangnhap);
            return (true, "Đổi mật khẩu thành công!");
        }

        public async Task<(bool Success, string Message)> DangKyKhachHangAsync(HotelManagement.BLL.DTOs.KhachHangDangKyDto dto)
        {
            if (dto == null)
                return (false, "Dữ liệu đăng ký không hợp lệ.");

            // 1. Kiểm tra không được để trống
            if (string.IsNullOrWhiteSpace(dto.HoTen) ||
                string.IsNullOrWhiteSpace(dto.Cccd) ||
                string.IsNullOrWhiteSpace(dto.Sdt) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.TenDangNhap) ||
                string.IsNullOrWhiteSpace(dto.MatKhau) ||
                string.IsNullOrWhiteSpace(dto.XacNhanMatKhau))
            {
                return (false, "Vui lòng nhập đầy đủ tất cả các trường thông tin bắt buộc.");
            }

            string hoTen = dto.HoTen.Trim();
            string cccd = dto.Cccd.Trim();
            string sdt = dto.Sdt.Trim();
            string email = dto.Email.Trim();
            string tenDangNhap = dto.TenDangNhap.Trim();
            string matKhau = dto.MatKhau;
            string xacNhanMatKhau = dto.XacNhanMatKhau;

            // 2. Validate định dạng
            if (!System.Text.RegularExpressions.Regex.IsMatch(cccd, @"^\d{12}$"))
                return (false, "CCCD phải gồm đúng 12 chữ số.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return (false, "Địa chỉ email không đúng định dạng.");

            if (matKhau.Length < 6)
                return (false, "Mật khẩu phải có độ dài tối thiểu 6 ký tự.");

            if (matKhau != xacNhanMatKhau)
                return (false, "Mật khẩu và xác nhận mật khẩu không khớp.");

            try
            {
                // 3. Kiểm tra tên đăng nhập trước
                var taiKhoanTonTai = await _uow.TaiKhoanRepository.GetByTenDangNhapAsync(tenDangNhap);
                if (taiKhoanTonTai != null)
                {
                    Log.Warning("[AuthService] Đăng ký khách hàng thất bại: Tên đăng nhập '{Username}' đã tồn tại.", tenDangNhap);
                    return (false, "Tên đăng nhập đã được sử dụng. Vui lòng chọn tên đăng nhập khác.");
                }

                // Lấy vai trò "KhachHang"
                var dsVaiTro = await _uow.VaiTroRepository.FindAsync(v => v.TenVaitro == "KhachHang");
                var vaiTroKhachHang = dsVaiTro.FirstOrDefault();
                if (vaiTroKhachHang == null)
                {
                    Log.Error("[AuthService] Không tìm thấy vai trò 'KhachHang' trong cơ sở dữ liệu.");
                    return (false, "Hệ thống chưa cấu hình vai trò Khách hàng. Vui lòng liên hệ quản trị viên.");
                }

                // 4. Kiểm tra CCCD trong bảng KhachHang
                var dsKhachHangCccd = await _uow.KhachHangRepository.FindAsync(k => k.Cccd == cccd);
                var khachHangCu = dsKhachHangCccd.FirstOrDefault();

                int idKhachHangMucTieu;

                await _uow.BeginTransactionAsync();

                if (khachHangCu != null)
                {
                    // Đã có hồ sơ khách hàng cũ (ví dụ do lễ tân tạo khi đặt phòng qua điện thoại)
                    // Kiểm tra xem khách hàng này ĐÃ CÓ TÀI KHOẢN chưa
                    var dsTaiKhoanCu = await _uow.TaiKhoanRepository.FindAsync(t => t.IdKhachhang == khachHangCu.IdKhachhang);
                    if (dsTaiKhoanCu.Any())
                    {
                        await _uow.RollbackTransactionAsync();
                        Log.Warning("[AuthService] Đăng ký thất bại: Khách hàng CCCD '{Cccd}' đã có tài khoản liên kết.", cccd);
                        return (false, "Thông tin không khớp với hồ sơ đã có, vui lòng liên hệ lễ tân để được hỗ trợ.");
                    }

                    // Xác thực 2 yếu tố: SĐT nhập vào phải khớp với SĐT đã lưu trong hồ sơ cũ
                    if (!string.Equals(khachHangCu.Sdt.Trim(), sdt, StringComparison.OrdinalIgnoreCase))
                    {
                        await _uow.RollbackTransactionAsync();
                        Log.Warning("[AuthService] Đăng ký thất bại: SĐT '{Sdt}' không khớp với hồ sơ CCCD '{Cccd}'.", sdt, cccd);
                        // Không tiết lộ CCCD đúng nhưng SĐT sai để đảm bảo bảo mật
                        return (false, "Thông tin không khớp với hồ sơ đã có, vui lòng liên hệ lễ tân để được hỗ trợ.");
                    }

                    // Khớp SĐT -> Liên kết trực tiếp tài khoản mới vào hồ sơ KhachHang cũ
                    idKhachHangMucTieu = khachHangCu.IdKhachhang;

                    // Cập nhật bổ sung Email hoặc Tên nếu hồ sơ cũ chưa có hoặc thay đổi
                    if (string.IsNullOrWhiteSpace(khachHangCu.Email) || khachHangCu.Email != email)
                    {
                        khachHangCu.Email = email;
                        khachHangCu.UpdatedAt = DateTime.Now;
                        _uow.KhachHangRepository.Update(khachHangCu);
                        await _uow.SaveChangesAsync();
                    }
                }
                else
                {
                    // CCCD chưa tồn tại -> Tạo mới bản ghi KhachHang
                    var khachHangMoi = new KhachHang
                    {
                        Ten = hoTen,
                        Cccd = cccd,
                        Sdt = sdt,
                        Email = email,
                        CreatedAt = DateTime.Now
                    };

                    await _uow.KhachHangRepository.AddAsync(khachHangMoi);
                    await _uow.SaveChangesAsync(); // Cần lưu để SQL Server sinh IdKhachhang (Identity)
                    idKhachHangMucTieu = khachHangMoi.IdKhachhang;
                }

                // Tạo TaiKhoan gắn với idKhachHangMucTieu
                var taiKhoanMoi = new TaiKhoan
                {
                    TenDangnhap = tenDangNhap,
                    MatkhauHash = HashMatKhau(matKhau),
                    IdNhansu = null,
                    IdKhachhang = idKhachHangMucTieu,
                    IdVaitro = vaiTroKhachHang.IdVaitro,
                    Trangthai = TrangThaiHoatDong,
                    SolanSaimatkhau = 0,
                    CreatedAt = DateTime.Now
                };

                await _uow.TaiKhoanRepository.AddAsync(taiKhoanMoi);
                await _uow.SaveChangesAsync();

                await _uow.CommitTransactionAsync();

                Log.Information("[AuthService] Đăng ký tài khoản khách hàng thành công: Username '{Username}', KhachHang ID: {IdKhachHang} (Khách cũ liên kết: {IsKhachCu})",
                    tenDangNhap, idKhachHangMucTieu, khachHangCu != null);

                return (true, khachHangCu != null 
                    ? "Đăng ký thành công! Hệ thống đã liên kết tài khoản với hồ sơ và lịch sử đặt phòng trước đây của bạn." 
                    : "Đăng ký tài khoản khách hàng thành công.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                Log.Error(ex, "[AuthService] Lỗi ngoại lệ khi đăng ký tài khoản khách hàng '{Username}': {Message}", tenDangNhap, ex.Message);
                return (false, "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau.");
            }
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