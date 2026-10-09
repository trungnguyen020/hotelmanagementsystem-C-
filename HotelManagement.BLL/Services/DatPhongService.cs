using System;
using System.Collections.Generic;
using System.Text;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;

namespace HotelManagement.BLL.Services
{
    public class DatPhongService : IDatPhongService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DatPhongService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<Phongtrongdto>> TimPhongTrongAsync(
            DateOnly ngayNhan, DateOnly ngayTra, int? idLoaiPhong, int? soKhach)
        {
            if (ngayNhan >= ngayTra)
                throw new ArgumentException("Ngày nhận phòng phải trước ngày trả phòng.");

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentException("Ngày nhận phòng không được ở quá khứ.");

            if (soKhach.HasValue && soKhach.Value <= 0)
                throw new ArgumentException("Số khách phải lớn hơn 0.");

            var danhSachPhong = await _unitOfWork.DatPhongRepository
                .GetPhongTrongAsync(ngayNhan, ngayTra, idLoaiPhong, soKhach);

            return danhSachPhong.Select(p => new Phongtrongdto
            {
                IdPhong = p.IdPhong,
                SoPhong = p.Sophong,
                IdLoaiPhong = p.IdLoaiphong,
                TenLoaiPhong = p.IdLoaiphongNavigation.Ten,
                Gia = p.IdLoaiphongNavigation.Mucgia,
                SucChua = p.IdLoaiphongNavigation.Succhua
            }).ToList();
        }

        public async Task<(bool Success, string Message)> DatPhongAsync(
            int idKhachHang, int idNhanSu, DateOnly ngayNhan, DateOnly ngayTra, int soNguoi, List<int> danhSachIdPhong)
        {
            if (danhSachIdPhong == null || !danhSachIdPhong.Any())
                return (false, "Vui lòng chọn ít nhất 1 phòng để đặt.");

            if (ngayNhan >= ngayTra)
                return (false, "Ngày nhận phòng phải trước ngày trả phòng.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Kiểm tra lại lần cuối xem phòng còn trống không (tránh race condition)
                foreach (var idPhong in danhSachIdPhong)
                {
                    bool isTrong = await _unitOfWork.DatPhongRepository.IsPhongTrongAsync(idPhong, ngayNhan, ngayTra);
                    if (!isTrong)
                    {
                        var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                        await _unitOfWork.RollbackTransactionAsync();
                        return (false, $"Phòng {phong?.Sophong} đã được đặt trong khoảng thời gian này.");
                    }
                }

                // Sinh mã booking ngẫu nhiên / unique
                string maBooking = "BK" + DateTime.Now.ToString("yyMMddHHmmss");

                var datPhong = new HotelManagement.Models.Entities.DatPhong
                {
                    IdKhachhang = idKhachHang,
                    IdNhansu = idNhanSu,
                    NgaynhanDukien = ngayNhan,
                    NgaytraDukien = ngayTra,
                    Songuoi = soNguoi,
                    Mabooking = maBooking,
                    Trangthai = "Đã xác nhận",
                    // Ngaydat sẽ được tự động tạo do DF_DatPhong_NgayDat
                };

                await _unitOfWork.DatPhongRepository.AddAsync(datPhong);
                await _unitOfWork.SaveChangesAsync(); // Lưu để lấy IdDatphong

                // Thêm chi tiết đặt phòng
                foreach (var idPhong in danhSachIdPhong)
                {
                    var phong = await _unitOfWork.PhongRepository.GetByIdAsync(idPhong);
                    // Lưu ý: Cần join sang LoaiPhong để lấy đúng giá phòng hiện tại,
                    // do GetByIdAsync không tự Include, ta có thể tự Get LoaiPhong
                    decimal donGia = 0;
                    if (phong != null)
                    {
                        var loaiPhong = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(phong.IdLoaiphong);
                        donGia = loaiPhong?.Mucgia ?? 0;
                    }

                    var ctDatPhong = new HotelManagement.Models.Entities.ChiTietDatPhong
                    {
                        IdDatphong = datPhong.IdDatphong,
                        IdPhong = idPhong,
                        Dongia = donGia
                    };

                    // Add bằng reflection hoặc DbContext nếu không có repository riêng
                    // Ta chưa viết repository riêng cho ChiTietDatPhong nên dùng DbContext thông qua Repository Generic
                    // May thay ta không khai báo DbSet cho ChiTietDatPhong trong IUnitOfWork?
                    // Không sao, Entity Framework tự track navigation property ChiTietDatPhongs
                    datPhong.ChiTietDatPhongs.Add(ctDatPhong);
                }

                // Lưu các chi tiết đặt phòng vừa thêm qua navigation property
                await _unitOfWork.SaveChangesAsync();
                
                await _unitOfWork.CommitTransactionAsync();
                
                return (true, $"Đặt phòng thành công! Mã booking: {maBooking}");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Có lỗi xảy ra khi đặt phòng: " + ex.Message);
            }
        }
    }
}
