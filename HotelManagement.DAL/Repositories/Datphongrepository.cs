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
                .Include(dp => dp.ChiTietDatPhongs).ThenInclude(ct => ct.IdPhongNavigation)
                .Include(dp => dp.ChiTietDichVus).ThenInclude(cv => cv.IdDichvuNavigation)
                .Include(dp => dp.IdKhachhangNavigation)
                .Include(dp => dp.HoaDon)
                .FirstOrDefaultAsync(dp => dp.IdDatphong == idDatPhong);
    }
}