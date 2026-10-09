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

        // UI Controls
        private DateTimePicker _dtpNgayNhan = null!;
        private DateTimePicker _dtpNgayTra = null!;
        private ComboBox _cboLoaiPhong = null!;
        private NumericUpDown _numSoKhach = null!;
        private Button _btnTimPhong = null!;

        private TextBox _txtTimKhachHang = null!;
        private Button _btnTimKhachHang = null!;
        private Label _lblThongTinKhachHang = null!;
        private KhachHangDto? _khachHangDuocChon;

        private DataGridView _dgvPhongTrong = null!;
        private DataGridView _dgvPhongChon = null!;
        private Label _lblTongTien = null!;
        private Button _btnDatPhong = null!;
        
        private BindingList<Phongtrongdto> _dsPhongChon = new();

        public FormDatPhong(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap;
            _scope = Program.ServiceProvider.CreateScope();
            _datPhongService = _scope.ServiceProvider.GetRequiredService<IDatPhongService>();
            _khachHangService = _scope.ServiceProvider.GetRequiredService<IKhachHangService>();
            _loaiPhongService = _scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();

            InitializeComponent();
            KhoiTaoGiaoDien();
            
            FormClosed += (_, _) => _scope.Dispose();
            Load += async (_, _) => await LoadDataAsync();
        }

        private void InitializeComponent()
        {
            // Được gọi thủ công ở KhoiTaoGiaoDien
        }

        private void KhoiTaoGiaoDien()
        {
            Text = "Đặt phòng";
            Size = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;

            // --- Pannel Filter ---
            var pnlFilter = new GroupBox { Text = "Tìm phòng trống", Dock = DockStyle.Top, Height = 80 };
            
            var lblNgayNhan = new Label { Text = "Ngày nhận:", Location = new Point(20, 30), AutoSize = true };
            _dtpNgayNhan = new DateTimePicker { Location = new Point(100, 27), Width = 120, Format = DateTimePickerFormat.Short };
            
            var lblNgayTra = new Label { Text = "Ngày trả:", Location = new Point(240, 30), AutoSize = true };
            _dtpNgayTra = new DateTimePicker { Location = new Point(310, 27), Width = 120, Format = DateTimePickerFormat.Short };
            _dtpNgayTra.Value = DateTime.Now.AddDays(1);

            var lblLoaiPhong = new Label { Text = "Loại phòng:", Location = new Point(450, 30), AutoSize = true };
            _cboLoaiPhong = new ComboBox { Location = new Point(530, 27), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            
            var lblSoKhach = new Label { Text = "Số khách:", Location = new Point(700, 30), AutoSize = true };
            _numSoKhach = new NumericUpDown { Location = new Point(770, 27), Width = 60, Minimum = 1, Maximum = 50, Value = 1 };
            
            _btnTimPhong = new Button { Text = "Tìm phòng", Location = new Point(850, 25), Width = 100, Height = 30 };
            _btnTimPhong.Click += async (_, _) => await TimPhongAsync();

            pnlFilter.Controls.AddRange(new Control[] {
                lblNgayNhan, _dtpNgayNhan, lblNgayTra, _dtpNgayTra,
                lblLoaiPhong, _cboLoaiPhong, lblSoKhach, _numSoKhach, _btnTimPhong
            });

            // --- Panel Khách Hàng ---
            var pnlKhachHang = new GroupBox { Text = "Khách hàng", Dock = DockStyle.Left, Width = 300 };
            
            var lblTimKH = new Label { Text = "Nhập CCCD / SĐT:", Location = new Point(10, 30), AutoSize = true };
            _txtTimKhachHang = new TextBox { Location = new Point(10, 50), Width = 180 };
            _btnTimKhachHang = new Button { Text = "Tra cứu", Location = new Point(200, 48), Width = 80, Height = 25 };
            _btnTimKhachHang.Click += async (_, _) => await TimKhachHangAsync();

            _lblThongTinKhachHang = new Label 
            { 
                Text = "(Chưa chọn khách hàng)", 
                Location = new Point(10, 90), 
                AutoSize = true, 
                MaximumSize = new Size(280, 0)
            };

            var btnThemKH = new Button { Text = "Khách hàng mới...", Location = new Point(10, 160), Width = 130, Height = 30 };
            btnThemKH.Click += (_, _) =>
            {
                var frmKH = new FormKhachHang();
                frmKH.ShowDialog(this);
            };

            pnlKhachHang.Controls.AddRange(new Control[] { lblTimKH, _txtTimKhachHang, _btnTimKhachHang, _lblThongTinKhachHang, btnThemKH });

            // --- Center Panel ---
            var pnlCenter = new Panel { Dock = DockStyle.Fill };
            
            var grpPhongTrong = new GroupBox { Text = "Danh sách phòng trống", Dock = DockStyle.Top, Height = 300 };
            _dgvPhongTrong = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            var btnChonPhong = new Button { Text = "Thêm phòng đã chọn ↓", Dock = DockStyle.Bottom, Height = 30 };
            btnChonPhong.Click += BtnChonPhong_Click;

            grpPhongTrong.Controls.Add(_dgvPhongTrong);
            grpPhongTrong.Controls.Add(btnChonPhong);

            var grpPhongChon = new GroupBox { Text = "Phòng đã chọn để đặt", Dock = DockStyle.Fill };
            _dgvPhongChon = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            _dgvPhongChon.DataSource = _dsPhongChon;
            
            var pnlDatPhong = new Panel { Dock = DockStyle.Bottom, Height = 60 };
            var btnXoaPhongChon = new Button { Text = "Bỏ chọn ↑", Location = new Point(20, 15), Width = 100, Height = 30 };
            btnXoaPhongChon.Click += BtnXoaPhongChon_Click;

            _lblTongTien = new Label { Text = "Tổng tiền tạm tính: 0", Location = new Point(300, 20), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            
            _btnDatPhong = new Button { Text = "ĐẶT PHÒNG", Location = new Point(620, 10), Width = 120, Height = 40, BackColor = Color.LightGreen };
            _btnDatPhong.Click += async (_, _) => await DatPhongAsync();

            pnlDatPhong.Controls.AddRange(new Control[] { btnXoaPhongChon, _lblTongTien, _btnDatPhong });
            
            grpPhongChon.Controls.Add(_dgvPhongChon);
            grpPhongChon.Controls.Add(pnlDatPhong);

            pnlCenter.Controls.Add(grpPhongChon);
            pnlCenter.Controls.Add(grpPhongTrong);

            Controls.Add(pnlCenter);
            Controls.Add(pnlKhachHang);
            Controls.Add(pnlFilter);
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

            var (success, msg) = await _datPhongService.DatPhongAsync(
                _khachHangDuocChon.IdKhachHang,
                _taiKhoanDangNhap.IdNhansu,
                DateOnly.FromDateTime(_dtpNgayNhan.Value),
                DateOnly.FromDateTime(_dtpNgayTra.Value),
                (int)_numSoKhach.Value,
                dsIdPhong
            );

            if (success)
            {
                MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show(msg, "Lỗi đặt phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
