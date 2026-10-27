using Microsoft.EntityFrameworkCore;
using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    public class DatPhongRepository : Repository<DatPhong>, IDatPhongRepository
    {
        // Giá trị đúng theo ràng buộc CK_DatPhong_TrangThai trong script SQL (giữ nguyên dấu).
        private static readonly string[] TrangThaiGiuPhong = { "Đã xác nhận", "Đã check-in" };

        // Giá trị đúng theo ràng buộc CK_Phong_TrangThai.
        private const string TrangThaiBaoTri = "Bảo trì";

        public DatPhongRepository(HotelManagementDbContext context) : base(context)
        {
        }

        public async Task<bool> IsPhongTrongAsync(int idPhong, DateOnly ngayNhan, DateOnly ngayTra, int? excludeIdDatPhong = null)
        {
            // NOTE: tên property IdPhong/IdDatphongNavigation trên ChiTietDatPhong suy theo đúng
            // quy luật scaffold đã xác nhận ở DatPhong/TaiKhoan - đối chiếu lại khi có file thật.
            var query = _context.Set<ChiTietDatPhong>()
                .Include(ct => ct.IdDatphongNavigation)
                .Where(ct => ct.IdPhong == idPhong
                    && TrangThaiGiuPhong.Contains(ct.IdDatphongNavigation.Trangthai)
                    && ct.IdDatphongNavigation.NgaynhanDukien < ngayTra
                    && ct.IdDatphongNavigation.NgaytraDukien > ngayNhan);

            if (excludeIdDatPhong.HasValue)
                query = query.Where(ct => ct.IdDatphong != excludeIdDatPhong.Value);

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Phong>> GetPhongTrongAsync(
            DateOnly ngayNhan, DateOnly ngayTra, int? idLoaiPhong = null, int? soKhach = null)
        {
            var phongQuery = _context.Set<Phong>().AsNoTracking().AsQueryable();

            if (idLoaiPhong.HasValue)
                phongQuery = phongQuery.Where(p => p.IdLoaiphong == idLoaiPhong.Value);

            // TODO: xác nhận lại tên property "sức chứa" trên LoaiPhong khi có entity thật.
            // Giả định cột là "succhua" (1 đoạn, không "_") -> property "Succhua".
            if (soKhach.HasValue)
            {
                phongQuery = phongQuery
                    .Include(p => p.IdLoaiphongNavigation)
                    .Where(p => p.IdLoaiphongNavigation.Succhua >= soKhach.Value);
            }

            var idPhongDangBiGiu = _context.Set<ChiTietDatPhong>()
                .Include(ct => ct.IdDatphongNavigation)
                .Where(ct => TrangThaiGiuPhong.Contains(ct.IdDatphongNavigation.Trangthai)
                    && ct.IdDatphongNavigation.NgaynhanDukien < ngayTra
                    && ct.IdDatphongNavigation.NgaytraDukien > ngayNhan)
                .Select(ct => ct.IdPhong);

            return await phongQuery
                .Where(p => !idPhongDangBiGiu.Contains(p.IdPhong) && p.Trangthai != TrangThaiBaoTri)
                .ToListAsync();
        }

        public async Task<DatPhong?> GetByMaBookingAsync(string maBooking)
            => await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(dp => dp.Mabooking == maBooking);

        public async Task<DatPhong?> GetChiTietDayDuAsync(int idDatPhong)
            => await _dbSet
                .Include(dp => dp.ChiTietDatPhongs)
                    .ThenInclude(ct => ct.IdPhongNavigation)
                        .ThenInclude(p => p.IdLoaiphongNavigation)
                .Include(dp => dp.ChiTietDichVus).ThenInclude(cv => cv.IdDichvuNavigation)
                .Include(dp => dp.IdKhachhangNavigation)
                .Include(dp => dp.HoaDon)
                .FirstOrDefaultAsync(dp => dp.IdDatphong == idDatPhong);

        public async Task<IEnumerable<DatPhong>> TimKiemBookingAsync(
            string? tenKhachHang = null,
            string? sdt = null,
            string? cccd = null,
            string? soPhong = null,
            string? trangThai = null)
        {
            var query = _dbSet.AsNoTracking()
                .Include(dp => dp.IdKhachhangNavigation)
                .Include(dp => dp.ChiTietDatPhongs)
                    .ThenInclude(ct => ct.IdPhongNavigation)
                .AsQueryable();

            // Lọc theo tên khách hàng (tìm kiếm chứa - case-insensitive)
            if (!string.IsNullOrWhiteSpace(tenKhachHang))
            {
                string ten = tenKhachHang.Trim();
                query = query.Where(dp => dp.IdKhachhangNavigation.Ten.Contains(ten));
            }

            // Lọc theo SĐT (tìm kiếm chứa)
            if (!string.IsNullOrWhiteSpace(sdt))
            {
                string s = sdt.Trim();
                query = query.Where(dp => dp.IdKhachhangNavigation.Sdt.Contains(s));
            }

            // Lọc theo CCCD (tìm kiếm chứa)
            if (!string.IsNullOrWhiteSpace(cccd))
            {
                string c = cccd.Trim();
                query = query.Where(dp => dp.IdKhachhangNavigation.Cccd.Contains(c));
            }

            // Lọc theo số phòng (kiểm tra booking có chứa phòng có số phòng khớp)
            if (!string.IsNullOrWhiteSpace(soPhong))
            {
                string sp = soPhong.Trim();
                query = query.Where(dp =>
                    dp.ChiTietDatPhongs.Any(ct => ct.IdPhongNavigation.Sophong.Contains(sp)));
            }

            // Lọc theo trạng thái booking (chính xác)
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                string tt = trangThai.Trim();
                query = query.Where(dp => dp.Trangthai == tt);
            }

            // Sắp xếp theo ngày đặt mới nhất, giới hạn 100 kết quả
            return await query
                .OrderByDescending(dp => dp.Ngaydat)
                .Take(100)
                .ToListAsync();
        }
    }
}