using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    /// <summary>
    /// Repository quản lý lịch sử gửi thông báo (DCKLTTQ-82).
    /// </summary>
    public interface ILichSuGuiThongBaoRepository : IRepository<LichSuGuiThongBao>
    {
        Task<IEnumerable<LichSuGuiThongBao>> LayTheoDatPhongAsync(int idDatPhong);
        Task<IEnumerable<LichSuGuiThongBao>> LayTheoHoaDonAsync(int idHoaDon);
        Task<IEnumerable<LichSuGuiThongBao>> LayDanhSachMoiNhatAsync(int soLuong = 100);
    }
}

