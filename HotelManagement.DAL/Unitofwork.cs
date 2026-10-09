using Microsoft.EntityFrameworkCore.Storage;
using HotelManagement.Models.Entities;

namespace HotelManagement.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelManagementDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy-init: chỉ khởi tạo repository khi thực sự được truy cập lần đầu
        private IDatPhongRepository? _datPhongRepository;
        private ITaiKhoanRepository? _taiKhoanRepository;
        private IRepository<Phong>? _phongRepository;
        private IRepository<KhachHang>? _khachHangRepository;
        private IRepository<NhanSu>? _nhanSuRepository;
        private IRepository<LoaiPhong>? _loaiPhongRepository;
        private IRepository<DichVu>? _dichVuRepository;
        private IRepository<VaiTro>? _vaiTroRepository;
        private IRepository<NhatKyHoatDong>? _nhatKyHoatDongRepository;

        public UnitOfWork(HotelManagementDbContext context)
        {
            _context = context;
        }

        public IDatPhongRepository DatPhongRepository
            => _datPhongRepository ??= new DatPhongRepository(_context);

        public ITaiKhoanRepository TaiKhoanRepository
            => _taiKhoanRepository ??= new TaiKhoanRepository(_context);

        public IRepository<Phong> PhongRepository
            => _phongRepository ??= new Repository<Phong>(_context);

        public IRepository<KhachHang> KhachHangRepository
            => _khachHangRepository ??= new Repository<KhachHang>(_context);

        public IRepository<NhanSu> NhanSuRepository
            => _nhanSuRepository ??= new Repository<NhanSu>(_context);

        public IRepository<LoaiPhong> LoaiPhongRepository
            => _loaiPhongRepository ??= new Repository<LoaiPhong>(_context);

        public IRepository<DichVu> DichVuRepository
            => _dichVuRepository ??= new Repository<DichVu>(_context);

        public IRepository<VaiTro> VaiTroRepository
            => _vaiTroRepository ??= new Repository<VaiTro>(_context);

        public IRepository<NhatKyHoatDong> NhatKyHoatDongRepository
            => _nhatKyHoatDongRepository ??= new Repository<NhatKyHoatDong>(_context);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
            => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("Chưa gọi BeginTransactionAsync() trước khi Commit.");

            try
            {
                await SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}