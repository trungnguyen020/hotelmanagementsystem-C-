using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelManagement.DAL.Repositories
{
    /// <summary>
    /// Implementation mặc định của IRepository&lt;T&gt;, dùng chung DbContext với UnitOfWork
    /// (không tự tạo/đóng DbContext riêng, để đảm bảo mọi repository trong 1 UnitOfWork
    /// thao tác trên cùng 1 transaction).
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly HotelManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(HotelManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(params object[] keyValues)
            => await _dbSet.FindAsync(keyValues);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.AsNoTracking().ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AsNoTracking().Where(predicate).ToListAsync();

        public virtual async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public virtual void Update(T entity)
            => _dbSet.Update(entity);

        public virtual void Remove(T entity)
            => _dbSet.Remove(entity);
    }
}