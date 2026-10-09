using Microsoft.EntityFrameworkCore;
using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    public class TaiKhoanRepository : Repository<TaiKhoan>, ITaiKhoanRepository
    {
        public TaiKhoanRepository(HotelManagementDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan?> GetByTenDangNhapAsync(string tenDangNhap)
            => await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(tk => tk.TenDangnhap == tenDangNhap);

        public async Task<TaiKhoan?> GetByTenDangNhapKemVaiTroAsync(string tenDangNhap)
            => await _dbSet
                .Include(tk => tk.IdNhansuNavigation)
                .Include(tk => tk.IdVaitroNavigation)
                .FirstOrDefaultAsync(tk => tk.TenDangnhap == tenDangNhap);
    }
}