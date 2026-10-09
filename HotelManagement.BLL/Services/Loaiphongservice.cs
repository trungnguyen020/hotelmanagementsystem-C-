using HotelManagement.BLL.DTOs;
using HotelManagement.DAL;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Nghiệp vụ CRUD Loại phòng, dùng IUnitOfWork/IRepository đã có sẵn
    /// từ Epic 2 (LoaiPhongRepository được lazy-init trong UnitOfWork).
    /// </summary>
    public class LoaiPhongService : ILoaiPhongService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoaiPhongService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LoaiPhongDto>> LayDanhSachAsync()
        {
            var danhSach = await _unitOfWork.LoaiPhongRepository.GetAllAsync();
            return danhSach.Select(ChuyenDto)
                           .OrderBy(x => x.TenLoaiPhong)
                           .ToList();
        }

        public async Task<LoaiPhongDto?> LayTheoIdAsync(int id)
        {
            var entity = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(id);
            return entity is null ? null : ChuyenDto(entity);
        }

        public async Task<List<LoaiPhongDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await LayDanhSachAsync();

            var ketQua = await _unitOfWork.LoaiPhongRepository.FindAsync(
                lp => lp.Ten.Contains(tuKhoa));

            return ketQua.Select(ChuyenDto)
                         .OrderBy(x => x.TenLoaiPhong)
                         .ToList();
        }

        public async Task ThemAsync(LoaiPhongDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: true);

            var entity = new LoaiPhong
            {
                Ten = dto.TenLoaiPhong.Trim(),
                Mucgia = dto.Gia,
                Succhua = dto.SucChua,
                Mota = dto.MoTa?.Trim()
            };

            await _unitOfWork.LoaiPhongRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CapNhatAsync(LoaiPhongDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: false);

            var entity = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(dto.Id);
            if (entity is null)
                throw new InvalidOperationException("Không tìm thấy loại phòng cần cập nhật.");

            entity.Ten = dto.TenLoaiPhong.Trim();
            entity.Mucgia = dto.Gia;
            entity.Succhua = dto.SucChua;
            entity.Mota = dto.MoTa?.Trim();

            _unitOfWork.LoaiPhongRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task XoaAsync(int id)
        {
            var entity = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(id);
            if (entity is null)
                throw new InvalidOperationException("Không tìm thấy loại phòng cần xóa.");

            // Ràng buộc nghiệp vụ: không cho xóa loại phòng còn phòng đang sử dụng.
            var conPhongSuDung = await _unitOfWork.PhongRepository.FindAsync(
                p => p.IdLoaiphong == id);

            if (conPhongSuDung.Any())
                throw new InvalidOperationException(
                    "Không thể xóa: vẫn còn phòng đang thuộc loại phòng này. " +
                    "Hãy chuyển các phòng đó sang loại khác trước khi xóa.");

            _unitOfWork.LoaiPhongRepository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task KiemTraHopLeAsync(LoaiPhongDto dto, bool laThem)
        {
            if (string.IsNullOrWhiteSpace(dto.TenLoaiPhong))
                throw new InvalidOperationException("Tên loại phòng không được để trống.");

            if (dto.Gia <= 0)
                throw new InvalidOperationException("Giá phòng phải lớn hơn 0.");

            if (dto.SucChua <= 0)
                throw new InvalidOperationException("Sức chứa phải lớn hơn 0.");

            var tenChuan = dto.TenLoaiPhong.Trim();
            var trungTen = await _unitOfWork.LoaiPhongRepository.FindAsync(
                lp => lp.Ten.ToLower() == tenChuan.ToLower()
                      && (laThem || lp.IdLoaiphong != dto.Id));

            if (trungTen.Any())
                throw new InvalidOperationException("Tên loại phòng đã tồn tại, vui lòng chọn tên khác.");
        }

        private static LoaiPhongDto ChuyenDto(LoaiPhong entity) => new()
        {
            Id = entity.IdLoaiphong,
            TenLoaiPhong = entity.Ten,
            Gia = entity.Mucgia,
            SucChua = entity.Succhua,
            MoTa = entity.Mota
        };
    }
}