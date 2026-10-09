using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    /// <summary>
    /// UnitOfWork chịu trách nhiệm đảm bảo tính atomic khi 1 nghiệp vụ cần ghi dữ liệu
    /// lên nhiều bảng cùng lúc - quan trọng nhất là khi tạo 1 DatPhong kèm nhiều ChiTietDatPhong
    /// (nhiều phòng trong 1 lần đặt): hoặc tất cả cùng thành công, hoặc rollback toàn bộ.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IDatPhongRepository DatPhongRepository { get; }
        ITaiKhoanRepository TaiKhoanRepository { get; }

        // Các bảng danh mục/đơn giản dùng thẳng Repository<T> generic, không cần interface riêng
        IRepository<Phong> PhongRepository { get; }
        IRepository<KhachHang> KhachHangRepository { get; }
        IRepository<NhanSu> NhanSuRepository { get; }
        IRepository<LoaiPhong> LoaiPhongRepository { get; }
        IRepository<DichVu> DichVuRepository { get; }
        IRepository<VaiTro> VaiTroRepository { get; }
        IRepository<NhatKyHoatDong> NhatKyHoatDongRepository { get; }

        /// <summary>Lưu toàn bộ thay đổi đang pending xuống database (không kèm transaction tường minh).</summary>
        Task<int> SaveChangesAsync();

        /// <summary>Bắt đầu transaction tường minh - dùng cho nghiệp vụ nhiều bước (vd: đặt nhiều phòng).</summary>
        Task BeginTransactionAsync();

        /// <summary>Commit transaction đã mở bằng BeginTransactionAsync.</summary>
        Task CommitTransactionAsync();

        /// <summary>Rollback transaction nếu có lỗi xảy ra giữa chừng.</summary>
        Task RollbackTransactionAsync();
    }
}