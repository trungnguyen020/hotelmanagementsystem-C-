using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotelManagement.BLL.DTOs;
using HotelManagement.BLL.Services;
using HotelManagement.DAL.Repositories;
using HotelManagement.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HotelManagement.Forms
{
    public partial class FormDatPhongKhachHang : Form
    {
        private readonly TaiKhoan _taiKhoanDangNhap;
        private KhachHang? _khachHang;
        private List<Phongtrongdto> _danhSachPhongTrong = new();
        private List<DichVuDto> _danhSachDichVu = new();

        public FormDatPhongKhachHang(TaiKhoan taiKhoanDangNhap)
        {
            _taiKhoanDangNhap = taiKhoanDangNhap ?? throw new ArgumentNullException(nameof(taiKhoanDangNhap));
            InitializeComponent();
            SetupGridColumns();
            SetupServiceGridColumns();

            dtpNgayNhan.ValueChanged += (_, _) =>
            {
                if (dtpNgayTra.Value <= dtpNgayNhan.Value)
                {
                    dtpNgayTra.Value = dtpNgayNhan.Value.Date.AddDays(1);
                }
                dtpNgayTra.MinDate = dtpNgayNhan.Value.Date.AddDays(1);
                _ = TimPhongTrongAsync();
            };

            this.Load += async (_, _) =>
            {
                dtpNgayNhan.Value = DateTime.Today;
                dtpNgayTra.Value = DateTime.Today.AddDays(1);
                await TaiThongTinKhachHangAsync();
                await TaiDanhSachLoaiPhongAsync();
                await TaiDanhSachDichVuAsync();
                await TimPhongTrongAsync();
            };
        }

        private void SetupGridColumns()
        {
            dgvPhongTrong.Columns.Clear();

            var chkCol = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn",
                Width = 60,
                ReadOnly = false
            };
            dgvPhongTrong.Columns.Add(chkCol);

            dgvPhongTrong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoPhong",
                HeaderText = "Số phòng",
                DataPropertyName = "SoPhong",
                ReadOnly = true
            });

            dgvPhongTrong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenLoaiPhong",
                HeaderText = "Loại phòng",
                DataPropertyName = "TenLoaiPhong",
                ReadOnly = true
            });

            dgvPhongTrong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SucChua",
                HeaderText = "Sức chứa (người)",
                DataPropertyName = "SucChua",
                ReadOnly = true
            });

            dgvPhongTrong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Gia",
                HeaderText = "Đơn giá (VNĐ/đêm)",
                DataPropertyName = "Gia",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" },
                ReadOnly = true
            });

            dgvPhongTrong.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == 0)
                {
                    CapNhatTongTienDuKien();
                }
            };

            dgvPhongTrong.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvPhongTrong.IsCurrentCellDirty && dgvPhongTrong.CurrentCell.ColumnIndex == 0)
                {
                    dgvPhongTrong.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        private void SetupServiceGridColumns()
        {
            dgvDichVu.Columns.Clear();

            var chkCol = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn",
                Width = 50,
                ReadOnly = false
            };
            dgvDichVu.Columns.Add(chkCol);

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenDichVu",
                HeaderText = "Tên dịch vụ",
                DataPropertyName = "TenDichVu",
                ReadOnly = true
            });

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MucGia",
                HeaderText = "Đơn giá (VNĐ)",
                DataPropertyName = "MucGia",
                ReadOnly = true,
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "SL",
                Width = 45,
                ReadOnly = false
            });

            dgvDichVu.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 3)
                {
                    CapNhatTongTienDuKien();
                }
            };

            dgvDichVu.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvDichVu.IsCurrentCellDirty && dgvDichVu.CurrentCell.ColumnIndex == 0)
                {
                    dgvDichVu.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        private async Task TaiDanhSachDichVuAsync()
        {
            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var dvService = scope.ServiceProvider.GetRequiredService<IDichVuService>();
                _danhSachDichVu = await dvService.LayDanhSachAsync();

                dgvDichVu.Rows.Clear();
                foreach (var dv in _danhSachDichVu)
                {
                    int rowIndex = dgvDichVu.Rows.Add(false, dv.TenDichVu, dv.MucGia, 1);
                    dgvDichVu.Rows[rowIndex].Tag = dv;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[FormDatPhongKhachHang] Không thể tải danh sách dịch vụ: {Message}", ex.Message);
            }
        }

        private async Task TaiThongTinKhachHangAsync()
        {
            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                if (_taiKhoanDangNhap.IdKhachhang.HasValue)
                {
                    _khachHang = await uow.KhachHangRepository.GetByIdAsync(_taiKhoanDangNhap.IdKhachhang.Value);
                }

                if (_khachHang != null)
                {
                    lblHoTenKhach.Text = $"Họ tên: {_khachHang.Ten}";
                    lblCccdKhach.Text = $"CCCD: {_khachHang.Cccd}";
                    lblSdtKhach.Text = $"SĐT: {_khachHang.Sdt}";
                    lblEmailKhach.Text = $"Email: {_khachHang.Email}";
                }
                else
                {
                    lblHoTenKhach.Text = $"Tài khoản: {_taiKhoanDangNhap.TenDangnhap}";
                    lblCccdKhach.Text = "CCCD: Chưa cập nhật";
                    lblSdtKhach.Text = "SĐT: Chưa cập nhật";
                    lblEmailKhach.Text = "Email: Chưa cập nhật";
                }
                CanChinhThongTinKhach();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[FormDatPhongKhachHang] Không thể tải thông tin khách hàng: {Message}", ex.Message);
            }
        }

        private async Task TaiDanhSachLoaiPhongAsync()
        {
            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var loaiPhongService = scope.ServiceProvider.GetRequiredService<ILoaiPhongService>();
                var ds = await loaiPhongService.LayDanhSachAsync();

                cboLoaiPhong.Items.Clear();
                cboLoaiPhong.Items.Add(new { Id = (int?)null, Ten = "-- Tất cả loại phòng --" });
                foreach (var lp in ds)
                {
                    cboLoaiPhong.Items.Add(new { Id = (int?)lp.Id, Ten = lp.TenLoaiPhong });
                }

                cboLoaiPhong.DisplayMember = "Ten";
                cboLoaiPhong.ValueMember = "Id";
                cboLoaiPhong.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "[FormDatPhongKhachHang] Lỗi tải loại phòng: {Message}", ex.Message);
            }
        }

        private async Task TimPhongTrongAsync()
        {
            lblThongBao.Text = string.Empty;
            var ngayNhan = DateOnly.FromDateTime(dtpNgayNhan.Value);
            var ngayTra = DateOnly.FromDateTime(dtpNgayTra.Value);

            if (ngayNhan >= ngayTra)
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Ngày nhận phòng phải trước ngày trả phòng.";
                return;
            }

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Today))
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Ngày nhận phòng không được ở quá khứ.";
                return;
            }

            int? idLoaiPhong = null;
            if (cboLoaiPhong.SelectedItem != null)
            {
                dynamic selected = cboLoaiPhong.SelectedItem;
                idLoaiPhong = selected.Id;
            }

            int soKhach = (int)numSoKhach.Value;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var datPhongService = scope.ServiceProvider.GetRequiredService<IDatPhongService>();

                _danhSachPhongTrong = await datPhongService.TimPhongTrongAsync(ngayNhan, ngayTra, idLoaiPhong, soKhach);
                dgvPhongTrong.Rows.Clear();

                foreach (var p in _danhSachPhongTrong)
                {
                    int rowIndex = dgvPhongTrong.Rows.Add(false, p.SoPhong, p.TenLoaiPhong, p.SucChua, p.Gia);
                    dgvPhongTrong.Rows[rowIndex].Tag = p;
                }

                CapNhatTongTienDuKien();

                if (!_danhSachPhongTrong.Any())
                {
                    lblThongBao.ForeColor = AppTheme.StatusWarningText;
                    lblThongBao.Text = "Không có phòng nào trống phù hợp với thời gian và số lượng khách đã chọn.";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormDatPhongKhachHang] Lỗi tra cứu phòng trống: {Message}", ex.Message);
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Lỗi khi tìm phòng trống: " + ex.Message;
            }
        }

        private void CapNhatTongTienDuKien()
        {
            int soPhongDaChon = 0;
            decimal tongTienPhong = 0;
            int soDem = (dtpNgayTra.Value.Date - dtpNgayNhan.Value.Date).Days;
            if (soDem <= 0) soDem = 1;

            foreach (DataGridViewRow row in dgvPhongTrong.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Chon"].Value);
                if (isChecked && row.Tag is Phongtrongdto p)
                {
                    soPhongDaChon++;
                    tongTienPhong += (p.Gia * soDem);
                }
            }

            int soDichVuDaChon = 0;
            decimal tongTienDichVu = 0;

            foreach (DataGridViewRow row in dgvDichVu.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Chon"].Value);
                if (isChecked && row.Tag is DichVuDto dv)
                {
                    int sl = 1;
                    if (int.TryParse(row.Cells["SoLuong"].Value?.ToString(), out int parsedSl) && parsedSl > 0)
                    {
                        sl = parsedSl;
                    }
                    soDichVuDaChon++;
                    tongTienDichVu += (dv.MucGia * sl);
                }
            }

            decimal tongCong = tongTienPhong + tongTienDichVu;
            string thongTinDichVu = soDichVuDaChon > 0 ? $" + {soDichVuDaChon} dịch vụ ({tongTienDichVu:N0} đ)" : "";
            lblTongTienDuKien.Text = $"Đã chọn: {soPhongDaChon} phòng ({soDem} đêm){thongTinDichVu} | Ước tính: {tongCong:N0} VNĐ";
        }

        private async void BtnXacNhanDat_Click(object? sender, EventArgs e)
        {
            lblThongBao.Text = string.Empty;

            if (_khachHang == null && !_taiKhoanDangNhap.IdKhachhang.HasValue)
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Tài khoản khách hàng chưa được liên kết thông tin. Vui lòng liên hệ lễ tân.";
                return;
            }

            int idKhachHang = _khachHang?.IdKhachhang ?? _taiKhoanDangNhap.IdKhachhang!.Value;

            var dsIdPhong = new List<int>();
            foreach (DataGridViewRow row in dgvPhongTrong.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Chon"].Value);
                if (isChecked && row.Tag is Phongtrongdto p)
                {
                    dsIdPhong.Add(p.IdPhong);
                }
            }

            if (!dsIdPhong.Any())
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Vui lòng tích chọn ít nhất 1 phòng trong danh sách phía trên.";
                return;
            }

            // Thu thập các dịch vụ kèm theo đã chọn (nếu có)
            var dsDichVuChon = new List<(int IdDichVu, int SoLuong)>();
            foreach (DataGridViewRow row in dgvDichVu.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Chon"].Value);
                if (isChecked && row.Tag is DichVuDto dv)
                {
                    int sl = 1;
                    if (int.TryParse(row.Cells["SoLuong"].Value?.ToString(), out int parsedSl) && parsedSl > 0)
                    {
                        sl = parsedSl;
                    }
                    dsDichVuChon.Add((dv.IdDichVu, sl));
                }
            }

            var ngayNhan = DateOnly.FromDateTime(dtpNgayNhan.Value);
            var ngayTra = DateOnly.FromDateTime(dtpNgayTra.Value);

            if (ngayNhan < DateOnly.FromDateTime(DateTime.Now))
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Ngày nhận phòng không được trước thời điểm hiện tại.";
                MessageBox.Show("Ngày nhận phòng không được trước thời điểm hiện tại.", "Lỗi hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ngayTra <= ngayNhan)
            {
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Ngày trả phòng phải sau ngày nhận phòng.";
                MessageBox.Show("Ngày trả phòng phải sau ngày nhận phòng.", "Lỗi hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soKhach = (int)numSoKhach.Value;

            string chiTietDv = dsDichVuChon.Any() ? $"\n- Kèm theo {dsDichVuChon.Count} dịch vụ đã chọn" : "";
            var xacNhan = MessageBox.Show(
                $"Xác nhận đặt {dsIdPhong.Count} phòng từ ngày {ngayNhan:dd/MM/yyyy} đến {ngayTra:dd/MM/yyyy} cho {soKhach} khách?{chiTietDv}",
                "Xác nhận đặt phòng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes)
                return;

            btnXacNhanDat.Enabled = false;
            Cursor = Cursors.WaitCursor;

            try
            {
                using var scope = Program.ServiceProvider.CreateScope();
                var datPhongService = scope.ServiceProvider.GetRequiredService<IDatPhongService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var (success, message, maBooking) = await datPhongService.KhachHangDatPhongAsync(
                    idKhachHang, ngayNhan, ngayTra, soKhach, dsIdPhong, dsDichVuChon);

                if (!success)
                {
                    lblThongBao.ForeColor = AppTheme.StatusDangerText;
                    lblThongBao.Text = message;
                    // Tự động load lại phòng trống để loại bỏ phòng người khác vừa đặt
                    await TimPhongTrongAsync();
                    return;
                }

                // Đặt thành công -> Gửi email xác nhận & ghi lịch sử thông báo
                if (!string.IsNullOrEmpty(maBooking))
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            using var bgScope = Program.ServiceProvider.CreateScope();
                            var bgEmailService = bgScope.ServiceProvider.GetRequiredService<IEmailService>();
                            var bgUow = bgScope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                            var emailResult = await bgEmailService.GuiEmailXacNhanDatPhongAsync(maBooking);

                            // Lấy IdDatphong để ghi lịch sử thông báo
                            var dp = await bgUow.DatPhongRepository.GetByMaBookingAsync(maBooking);
                            var lichSu = new LichSuGuiThongBao
                            {
                                IdDatphong = dp?.IdDatphong,
                                LoaiThongbao = "Xác nhận đặt phòng",
                                NguoiNhan = emailResult.NguoiNhan ?? (_khachHang?.Email ?? string.Empty),
                                TieuDe = emailResult.TieuDe ?? "Xác nhận đặt phòng khách sạn",
                                TrangThai = emailResult.ThanhCong ? "Thành công" : "Thất bại",
                                LyDoLoi = emailResult.LyDoLoi,
                                ThoiGianGui = emailResult.ThoiGianGui ?? DateTime.Now
                            };

                            await bgUow.LichSuGuiThongBaoRepository.AddAsync(lichSu);
                            await bgUow.SaveChangesAsync();
                        }
                        catch (Exception exEmail)
                        {
                            Log.Warning(exEmail, "[FormDatPhongKhachHang] Lỗi khi gửi email chạy nền: {Message}", exEmail.Message);
                        }
                    });
                }

                var hoiXemLichSu = MessageBox.Show(
                    $"{message}\n\nEmail xác nhận và mã booking đã được gửi đến: {_khachHang?.Email ?? "email của bạn"}.\n\nBạn có muốn chuyển sang mục 'Lịch sử đặt phòng' để xem đơn vừa đặt không?",
                    "Đặt phòng thành công",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (hoiXemLichSu == DialogResult.Yes)
                {
                    if (this.Parent?.Parent is TabControl tc && tc.Parent is FormKhachHangPortal portal)
                    {
                        portal.ChuyenTab(1);
                    }
                }

                // Làm mới danh sách phòng trống
                await TimPhongTrongAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[FormDatPhongKhachHang] Ngoại lệ khi khách đặt phòng: {Message}", ex.Message);
                lblThongBao.ForeColor = AppTheme.StatusDangerText;
                lblThongBao.Text = "Đã xảy ra sự cố khi đặt phòng: " + ex.Message;
            }
            finally
            {
                btnXacNhanDat.Enabled = true;
                Cursor = Cursors.Default;
            }
        }
    }
}
