using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagement.BLL.DTOs;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;

namespace HotelManagement.BLL.Services
{
    public class DichVuService : IDichVuService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DichVuService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DichVuDto>> LayDanhSachAsync()
        {
            var danhSach = await _unitOfWork.DichVuRepository.GetAllAsync();
            return danhSach.Select(ChuyenDto)
                           .OrderBy(x => x.TenDichVu)
                           .ToList();
        }

        public async Task<DichVuDto?> LayTheoIdAsync(int id)
        {
            var entity = await _unitOfWork.DichVuRepository.GetByIdAsync(id);
            return entity is null ? null : ChuyenDto(entity);
        }

        public async Task<List<DichVuDto>> TimKiemAsync(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return await LayDanhSachAsync();

            string tk = tuKhoa.Trim();
            var ketQua = await _unitOfWork.DichVuRepository.FindAsync(
                dv => dv.TenDichvu.Contains(tk) || (dv.LoaiDichvu != null && dv.LoaiDichvu.Contains(tk)));

            return ketQua.Select(ChuyenDto)
                         .OrderBy(x => x.TenDichVu)
                         .ToList();
        }

        public async Task ThemAsync(DichVuDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: true);

            var entity = new DichVu
            {
                TenDichvu = dto.TenDichVu.Trim(),
                LoaiDichvu = string.IsNullOrWhiteSpace(dto.LoaiDichVu) ? "Dịch vụ" : dto.LoaiDichVu.Trim(),
                Mucgia = dto.MucGia,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.DichVuRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            dto.IdDichVu = entity.IdDichvu;
        }

        public async Task CapNhatAsync(DichVuDto dto)
        {
            await KiemTraHopLeAsync(dto, laThem: false);

            var entity = await _unitOfWork.DichVuRepository.GetByIdAsync(dto.IdDichVu);
            if (entity is null)
                throw new KeyNotFoundException($"Không tìm thấy dịch vụ ID = {dto.IdDichVu}.");

            entity.TenDichvu = dto.TenDichVu.Trim();
            entity.LoaiDichvu = string.IsNullOrWhiteSpace(dto.LoaiDichVu) ? "Dịch vụ" : dto.LoaiDichVu.Trim();
            entity.Mucgia = dto.MucGia;
            entity.UpdatedAt = DateTime.Now;

            _unitOfWork.DichVuRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task XoaAsync(int id)
        {
            var entity = await _unitOfWork.DichVuRepository.GetByIdAsync(id);
            if (entity is null)
                throw new KeyNotFoundException($"Không tìm thấy dịch vụ ID = {id}.");

            var coChiTiet = await _unitOfWork.ChiTietDichVuRepository.FindAsync(ct => ct.IdDichvu == id);
            if (coChiTiet.Any())
                throw new InvalidOperationException("Không thể xóa dịch vụ này vì đã có dữ liệu giao dịch sử dụng.");

            _unitOfWork.DichVuRepository.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task KiemTraHopLeAsync(DichVuDto dto, bool laThem)
        {
            if (string.IsNullOrWhiteSpace(dto.TenDichVu))
                throw new ArgumentException("Tên dịch vụ không được để trống.");

            if (dto.MucGia < 0)
                throw new ArgumentException("Mức giá dịch vụ không được là số âm.");

            string tenChuanHoa = dto.TenDichVu.Trim().ToLower();
            var trungTen = await _unitOfWork.DichVuRepository.FindAsync(
                dv => dv.TenDichvu.ToLower() == tenChuanHoa &&
                      (laThem || dv.IdDichvu != dto.IdDichVu));

            if (trungTen.Any())
                throw new InvalidOperationException($"Tên dịch vụ '{dto.TenDichVu}' đã tồn tại trong hệ thống.");
        }

        private static DichVuDto ChuyenDto(DichVu entity) => new()
        {
            IdDichVu = entity.IdDichvu,
            TenDichVu = entity.TenDichvu,
            LoaiDichVu = entity.LoaiDichvu ?? "Dịch vụ",
            MucGia = entity.Mucgia
        };
    }
}
