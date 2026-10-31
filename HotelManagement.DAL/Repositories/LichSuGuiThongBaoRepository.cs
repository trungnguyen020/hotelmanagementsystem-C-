using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    public class LichSuGuiThongBaoRepository : Repository<LichSuGuiThongBao>, ILichSuGuiThongBaoRepository
    {
        public LichSuGuiThongBaoRepository(HotelManagementDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LichSuGuiThongBao>> LayTheoDatPhongAsync(int idDatPhong)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ls => ls.IdDatphongNavigation)
                .Where(ls => ls.IdDatphong == idDatPhong)
                .OrderByDescending(ls => ls.ThoiGianGui)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuGuiThongBao>> LayTheoHoaDonAsync(int idHoaDon)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ls => ls.IdHoadonNavigation)
                .Where(ls => ls.IdHoadon == idHoaDon)
                .OrderByDescending(ls => ls.ThoiGianGui)
                .ToListAsync();
        }

        public async Task<IEnumerable<LichSuGuiThongBao>> LayDanhSachMoiNhatAsync(int soLuong = 100)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ls => ls.IdDatphongNavigation)
                .Include(ls => ls.IdHoadonNavigation)
                    .ThenInclude(hd => hd!.IdDatphongNavigation)
                .OrderByDescending(ls => ls.ThoiGianGui)
                .Take(soLuong)
                .ToListAsync();
        }
    }
}

