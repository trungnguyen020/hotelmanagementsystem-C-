using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Forms
{
    public partial class FormDatPhong : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private readonly IServiceScope _scope;
        private readonly IDatPhongService _datPhongService;
        private readonly IKhachHangService _khachHangService;
        private readonly ILoaiPhongService _loaiPhongService;

        private KhachHangDto? _khachHangDuocChon;

        private BindingList<Phongtrongdto> _dsPhongChon = new();

        public FormDatPhong(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();
            
            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) => await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var loaiPhongs = await _loaiPhongService.LayDanhSachAsync();
            var lst = loaiPhongs.ToList();
            lst.Insert(0, new LoaiPhongDto { Id = 0, TenLoaiPhong = "-- Tất cả --" });
            _cboLoaiPhong.DataSource = lst;
            _cboLoaiPhong.DisplayMember = "TenLoaiPhong";
            _cboLoaiPhong.ValueMember = "Id";
        }

        private async Task TimKhachHangAsync()
        {
            string tuKhoa = _txtTimKhachHang.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa)) return;

            var dsKH = await _khachHangService.TimKiemAsync(tuKhoa);
            var kh = dsKH.FirstOrDefault();
            if (kh == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng nào khớp với thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _khachHangDuocChon = null;
                _lblThongTinKhachHang.Text = "(Chưa chọn khách hàng)";
                return;
            }

            _khachHangDuocChon = kh;
            _lblThongTinKhachHang.Text = $"Họ tên: {kh.Ten}\nCCCD: {kh.Cccd}\nSĐT: {kh.Sdt}";
        }

        private async Task TimPhongAsync()
        {
            int? idLoaiPhong = _cboLoaiPhong.SelectedValue as int?;
            if (idLoaiPhong == 0) idLoaiPhong = null;

            try
            {
                var kq = await _datPhongService.TimPhongTrongAsync(
                    DateOnly.FromDateTime(_dtpNgayNhan.Value),
                    DateOnly.FromDateTime(_dtpNgayTra.Value),
                    idLoaiPhong,
                    (int)_numSoKhach.Value);

                _dgvPhongTrong.DataSource = kq;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnChonPhong_Click(object? sender, EventArgs e)
        {
            if (_dgvPhongTrong.SelectedRows.Count == 0) return;

            foreach (DataGridViewRow row in _dgvPhongTrong.SelectedRows)
            {
                if (row.DataBoundItem is Phongtrongdto p)
                {
                    if (!_dsPhongChon.Any(x => x.IdPhong == p.IdPhong))
                    {
                        _dsPhongChon.Add(p);
                    }
                }
            }
            TinhTongTien();
        }

        private void BtnXoaPhongChon_Click(object? sender, EventArgs e)
        {
            if (_dgvPhongChon.SelectedRows.Count == 0) return;

            var rowsToRemove = new List<Phongtrongdto>();
            foreach (DataGridViewRow row in _dgvPhongChon.SelectedRows)
            {
                if (row.DataBoundItem is Phongtrongdto p)
                    rowsToRemove.Add(p);
            }

            foreach (var p in rowsToRemove)
                _dsPhongChon.Remove(p);

            TinhTongTien();
        }

        private void TinhTongTien()
        {
            decimal tongGia1Ngay = _dsPhongChon.Sum(x => x.Gia);
            int soNgay = (int)(_dtpNgayTra.Value.Date - _dtpNgayNhan.Value.Date).TotalDays;
            if (soNgay <= 0) soNgay = 1;
            
            _lblTongTien.Text = $"Tổng tiền tạm tính: {(tongGia1Ngay * soNgay):N0} VND ({soNgay} đêm)";
        }

        private async Task DatPhongAsync()
        {
            if (_khachHangDuocChon == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dsPhongChon.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 phòng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dsIdPhong = _dsPhongChon.Select(p => p.IdPhong).ToList();

            var (success, msg, maBooking) = await _datPhongService.DatPhongAsync(
                _khachHangDuocChon.IdKhachHang,
                _taiKhoanDangNhap.IdNhansu,
                DateOnly.FromDateTime(_dtpNgayNhan.Value),
                DateOnly.FromDateTime(_dtpNgayTra.Value),
                (int)_numSoKhach.Value,
                dsIdPhong
            );

            if (success)
            {
                if (!string.IsNullOrEmpty(maBooking))
                {
                    using var frmXacNhan = new FormXacNhanDatPhong(maBooking);
                    frmXacNhan.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                Close();
            }
            else
            {
                MessageBox.Show(msg, "Lỗi đặt phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
