using HotelManagement.BLL.DTOs;
using HotelManagement.DAL;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    /// <summary>
    /// Nghiệp vụ CRUD Phòng. Dùng chung IUnitOfWork đã có (PhongRepository,
    /// LoaiPhongRepository) từ Epic 2, không tạo repository mới.
    /// </summary>
    public class PhongService : IPhongService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhongService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PhongDto>> LayDanhSachAsync()
        {
            var dsPhong = await _unitOfWork.PhongRepository.GetAllAsync();
            var tenLoaiPhongTheoId = await LayTenLoaiPhongTheoIdAsync();

            return dsPhong.Select(p => ChuyenDto(p, tenLoaiPhongTheoId))
                          .OrderBy(x => x.SoPhong)
                          .ToList();
        }

        public async Task<PhongDto?> LayTheoIdAsync(int id)
        {
            var entity = await _unitOfWork.PhongRepository.GetByIdAsync(id);
            if (entity is null) return null;

            var tenLoaiPhongTheoId = await LayTenLoaiPhongTheoIdAsync();
            return ChuyenDto(entity, tenLoaiPhongTheoId);
        }

        public async Task<List<PhongDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await LayDanhSachAsync();

            var ketQua = await _unitOfWork.PhongRepository.FindAsync(
                p => p.Sophong.Contains(tuKhoa));
            var tenLoaiPhongTheoId = await LayTenLoaiPhongTheoIdAsync();

            return ketQua.Select(p => ChuyenDto(p, tenLoaiPhongTheoId))
                         .OrderBy(x => x.SoPhong)
                         .ToList();
        }

        public async Task ThemAsync(PhongDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: true);

            var entity = new Phong
            {
                Sophong = dto.SoPhong.Trim(),
                IdLoaiphong = dto.IdLoaiPhong,
                Trangthai = dto.TrangThai
            };

            await _unitOfWork.PhongRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CapNhatAsync(PhongDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: false);

            var entity = await _unitOfWork.PhongRepository.GetByIdAsync(dto.Id);
            if (entity is null)
                throw new InvalidOperationException("Không tìm thấy phòng cần cập nhật.");

            entity.Sophong = dto.SoPhong.Trim();
            entity.IdLoaiphong = dto.IdLoaiPhong;
            entity.Trangthai = dto.TrangThai;

            _unitOfWork.PhongRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task XoaAsync(int id)
        {
            var entity = await _unitOfWork.PhongRepository.GetByIdAsync(id);
            if (entity is null)
                throw new InvalidOperationException("Không tìm thấy phòng cần xóa.");

            // Chỉ cho xóa phòng đang ở trạng thái Trống, tránh xóa nhầm phòng đang
            // phục vụ khách hoặc đang có booking liên quan (ChiTietDatPhong sẽ kiểm tra
            // kỹ hơn khi Epic 3 hoàn thiện nghiệp vụ Đặt phòng).
            if (entity.Trangthai != "Trống")
                throw new InvalidOperationException(
                    "Chỉ có thể xóa phòng đang ở trạng thái Trống. " +
                    "Vui lòng chuyển phòng về trạng thái Trống trước khi xóa.");

            _unitOfWork.PhongRepository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CapNhatTrangThaiAsync(int id, string trangThaiMoi)
        {
            if (!IPhongService.DanhSachTrangThai.Contains(trangThaiMoi))
                throw new InvalidOperationException($"Trạng thái '{trangThaiMoi}' không hợp lệ.");

            var entity = await _unitOfWork.PhongRepository.GetByIdAsync(id);
            if (entity is null)
                throw new InvalidOperationException("Không tìm thấy phòng.");

            entity.Trangthai = trangThaiMoi;
            _unitOfWork.PhongRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task KiemTraHopLeAsync(PhongDto dto, bool laThem)
        {
            if (string.IsNullOrWhiteSpace(dto.SoPhong))
                throw new InvalidOperationException("Số phòng không được để trống.");

            if (!IPhongService.DanhSachTrangThai.Contains(dto.TrangThai))
                throw new InvalidOperationException("Trạng thái phòng không hợp lệ.");

            var loaiPhong = await _unitOfWork.LoaiPhongRepository.GetByIdAsync(dto.IdLoaiPhong);
            if (loaiPhong is null)
                throw new InvalidOperationException("Vui lòng chọn loại phòng hợp lệ.");

            var soChuan = dto.SoPhong.Trim();
            var trungSo = await _unitOfWork.PhongRepository.FindAsync(
                p => p.Sophong.ToLower() == soChuan.ToLower()
                     && (laThem || p.IdPhong != dto.Id));

            if (trungSo.Any())
                throw new InvalidOperationException("Số phòng đã tồn tại, vui lòng chọn số khác.");
        }

        private async Task<Dictionary<int, string>> LayTenLoaiPhongTheoIdAsync()
        {
            var dsLoaiPhong = await _unitOfWork.LoaiPhongRepository.GetAllAsync();
            return dsLoaiPhong.ToDictionary(lp => lp.IdLoaiphong, lp => lp.Ten);
        }

        private static PhongDto ChuyenDto(Phong entity, Dictionary<int, string> tenLoaiPhongTheoId) => new()
        {
            Id = entity.IdPhong,
            SoPhong = entity.Sophong,
            IdLoaiPhong = entity.IdLoaiphong,
            TenLoaiPhong = tenLoaiPhongTheoId.TryGetValue(entity.IdLoaiphong, out var ten) ? ten : "",
            TrangThai = entity.Trangthai
        };
    }
}