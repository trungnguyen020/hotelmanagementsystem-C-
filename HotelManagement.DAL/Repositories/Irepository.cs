using System.Linq.Expressions;

namespace HotelManagement.DAL.Repositories
{
    /// <summary>
    /// Generic repository interface - chuẩn hóa thao tác CRUD cho mọi entity.
    /// Các repository nghiệp vụ (IDatPhongRepository, ITaiKhoanRepository...) kế thừa interface này
    /// và bổ sung thêm các phương thức truy vấn đặc thù.
    /// </summary>
    /// <typeparam name="T">Kiểu entity (phải là class, map với 1 bảng trong DbContext)</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Lấy entity theo khóa chính (hỗ trợ khóa chính ghép bằng params).</summary>
        Task<T?> GetByIdAsync(params object[] keyValues);

        /// <summary>Lấy toàn bộ dữ liệu (AsNoTracking - chỉ dùng để đọc/hiển thị).</summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>Tìm theo điều kiện tùy ý (AsNoTracking).</summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>Thêm entity mới vào DbContext (chưa SaveChanges - do UnitOfWork quyết định khi commit).</summary>
        Task AddAsync(T entity);

        /// <summary>Đánh dấu entity đã chỉnh sửa để EF Core track thay đổi.</summary>
        void Update(T entity);

        /// <summary>Đánh dấu entity cần xóa.</summary>
        void Remove(T entity);
    }
}