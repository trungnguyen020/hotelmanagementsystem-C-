/* ===================================================================================
   ĐỒ ÁN HỆ THỐNG QUẢN LÝ KHÁCH SẠN (HOTEL MANAGEMENT SYSTEM)
   Tệp: HotelManagementDB_Full.sql
   Mô tả: Script khởi tạo toàn bộ CSDL HotelManagementDB từ đầu (13 bảng)
          kèm theo bộ Dữ liệu mẫu (Seed Data) chuẩn chỉnh để chạy thử ngay.
   Tương thích: Microsoft SQL Server 2019 / 2022 / Azure SQL
   =================================================================================== */

USE master;
GO

-- 1. XÓA DATABASE CŨ NẾU ĐÃ TỒN TẠI VÀ TẠO MỚI
IF DB_ID(N'HotelManagementDB') IS NOT NULL
BEGIN
    ALTER DATABASE HotelManagementDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HotelManagementDB;
END
GO

CREATE DATABASE HotelManagementDB;
GO

USE HotelManagementDB;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* ===================================================================================
   PHẦN I: TẠO CẤU TRÚC 13 BẢNG CƠ SỞ DỮ LIỆU
   =================================================================================== */

-- 1. BẢNG VAITRO
CREATE TABLE dbo.VaiTro (
    id_vaitro   INT IDENTITY(1,1) NOT NULL,
    ten_vaitro  NVARCHAR(50) NOT NULL,
    created_at  DATETIME2 NOT NULL CONSTRAINT DF_VaiTro_CreatedAt DEFAULT (sysdatetime()),
    updated_at  DATETIME2 NULL,
    CONSTRAINT PK_VaiTro PRIMARY KEY (id_vaitro),
    CONSTRAINT UQ_VaiTro_Ten UNIQUE (ten_vaitro)
);
GO

-- 2. BẢNG NHANSU
CREATE TABLE dbo.NhanSu (
    id_nhansu   INT IDENTITY(1,1) NOT NULL,
    hoten       NVARCHAR(100) NOT NULL,
    ngaysinh    DATE NOT NULL,
    cccd        VARCHAR(12) NOT NULL,
    sdt         VARCHAR(15) NOT NULL,
    created_at  DATETIME2 NOT NULL CONSTRAINT DF_NhanSu_CreatedAt DEFAULT (sysdatetime()),
    updated_at  DATETIME2 NULL,
    CONSTRAINT PK_NhanSu PRIMARY KEY (id_nhansu),
    CONSTRAINT UQ_NhanSu_CCCD UNIQUE (cccd),
    CONSTRAINT CK_NhanSu_CCCD CHECK (LEN(cccd) = 12 AND cccd NOT LIKE '%[^0-9]%')
);
GO

-- 3. BẢNG TAIKHOAN
CREATE TABLE dbo.TaiKhoan (
    id_taikhoan         INT IDENTITY(1,1) NOT NULL,
    ten_dangnhap        VARCHAR(50) NOT NULL,
    matkhau_hash        VARCHAR(255) NOT NULL,
    id_nhansu           INT NULL,
    id_khachhang        INT NULL,
    id_vaitro           INT NOT NULL,
    trangthai           NVARCHAR(20) NOT NULL CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT (N'Hoạt động'),
    solan_saimatkhau    INT NOT NULL CONSTRAINT DF_TaiKhoan_SoLanSai DEFAULT (0),
    created_at          DATETIME2 NOT NULL CONSTRAINT DF_TaiKhoan_CreatedAt DEFAULT (sysdatetime()),
    updated_at          DATETIME2 NULL,
    CONSTRAINT PK_TaiKhoan PRIMARY KEY (id_taikhoan),
    CONSTRAINT UQ_TaiKhoan_TenDangNhap UNIQUE (ten_dangnhap),
    CONSTRAINT FK_TaiKhoan_NhanSu FOREIGN KEY (id_nhansu) REFERENCES dbo.NhanSu(id_nhansu),
    CONSTRAINT FK_TaiKhoan_KhachHang FOREIGN KEY (id_khachhang) REFERENCES dbo.KhachHang(id_khachhang),
    CONSTRAINT FK_TaiKhoan_VaiTro FOREIGN KEY (id_vaitro) REFERENCES dbo.VaiTro(id_vaitro),
    CONSTRAINT CK_TaiKhoan_TrangThai CHECK (trangthai IN (N'Hoạt động', N'Khóa')),
    CONSTRAINT CK_TaiKhoan_DoiTuong CHECK (
        (id_nhansu IS NOT NULL AND id_khachhang IS NULL) OR 
        (id_nhansu IS NULL AND id_khachhang IS NOT NULL)
    )
);
GO

CREATE UNIQUE NONCLUSTERED INDEX UQ_TaiKhoan_NhanSu ON dbo.TaiKhoan(id_nhansu) WHERE id_nhansu IS NOT NULL;
CREATE UNIQUE NONCLUSTERED INDEX UQ_TaiKhoan_KhachHang ON dbo.TaiKhoan(id_khachhang) WHERE id_khachhang IS NOT NULL;
GO

-- 4. BẢNG KHACHHANG
CREATE TABLE dbo.KhachHang (
    id_khachhang    INT IDENTITY(1,1) NOT NULL,
    ten             NVARCHAR(100) NOT NULL,
    sdt             VARCHAR(15) NOT NULL,
    cccd            VARCHAR(12) NOT NULL,
    gioitinh        NVARCHAR(10) NULL,
    ngaysinh        DATE NULL,
    email           VARCHAR(100) NOT NULL,
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_KhachHang_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_KhachHang PRIMARY KEY (id_khachhang),
    CONSTRAINT UQ_KhachHang_CCCD UNIQUE (cccd),
    CONSTRAINT CK_KhachHang_CCCD CHECK (LEN(cccd) = 12 AND cccd NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_KhachHang_GioiTinh CHECK (gioitinh IS NULL OR gioitinh IN (N'Nam', N'Nữ', N'Khác'))
);
GO

-- 5. BẢNG LOAIPHONG
CREATE TABLE dbo.LoaiPhong (
    id_loaiphong    INT IDENTITY(1,1) NOT NULL,
    ten             NVARCHAR(100) NOT NULL,
    mucgia          DECIMAL(12,2) NOT NULL,
    succhua         INT NOT NULL,
    mota            NVARCHAR(500) NULL,
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_LoaiPhong_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_LoaiPhong PRIMARY KEY (id_loaiphong),
    CONSTRAINT CK_LoaiPhong_MucGia CHECK (mucgia >= 0),
    CONSTRAINT CK_LoaiPhong_SucChua CHECK (succhua > 0)
);
GO

-- 6. BẢNG PHONG
CREATE TABLE dbo.Phong (
    id_phong        INT IDENTITY(1,1) NOT NULL,
    sophong         VARCHAR(10) NOT NULL,
    id_loaiphong    INT NOT NULL,
    trangthai       NVARCHAR(20) NOT NULL CONSTRAINT DF_Phong_TrangThai DEFAULT (N'Trống'),
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_Phong_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_Phong PRIMARY KEY (id_phong),
    CONSTRAINT UQ_Phong_SoPhong UNIQUE (sophong),
    CONSTRAINT FK_Phong_LoaiPhong FOREIGN KEY (id_loaiphong) REFERENCES dbo.LoaiPhong(id_loaiphong),
    CONSTRAINT CK_Phong_TrangThai CHECK (trangthai IN (N'Trống', N'Đang ở', N'Đang dọn', N'Bảo trì'))
);
GO

-- 7. BẢNG DICHVU
CREATE TABLE dbo.DichVu (
    id_dichvu       INT IDENTITY(1,1) NOT NULL,
    ten_dichvu      NVARCHAR(100) NOT NULL,
    mucgia          DECIMAL(12,2) NOT NULL,
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_DichVu_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_DichVu PRIMARY KEY (id_dichvu),
    CONSTRAINT CK_DichVu_MucGia CHECK (mucgia >= 0)
);
GO

-- 8. BẢNG DATPHONG
CREATE TABLE dbo.DatPhong (
    id_datphong         INT IDENTITY(1,1) NOT NULL,
    mabooking           VARCHAR(20) NOT NULL,
    id_khachhang        INT NOT NULL,
    id_nhansu           INT NOT NULL,
    ngaydat             DATETIME2 NOT NULL CONSTRAINT DF_DatPhong_NgayDat DEFAULT (sysdatetime()),
    ngaynhan_dukien     DATE NOT NULL,
    ngaytra_dukien      DATE NOT NULL,
    ngaynhan_thucte     DATETIME2 NULL,
    ngaytra_thucte      DATETIME2 NULL,
    songuoi             INT NOT NULL,
    trangthai           NVARCHAR(20) NOT NULL CONSTRAINT DF_DatPhong_TrangThai DEFAULT (N'Đã xác nhận'),
    created_at          DATETIME2 NOT NULL CONSTRAINT DF_DatPhong_CreatedAt DEFAULT (sysdatetime()),
    updated_at          DATETIME2 NULL,
    CONSTRAINT PK_DatPhong PRIMARY KEY (id_datphong),
    CONSTRAINT UQ_DatPhong_MaBooking UNIQUE (mabooking),
    CONSTRAINT FK_DatPhong_KhachHang FOREIGN KEY (id_khachhang) REFERENCES dbo.KhachHang(id_khachhang),
    CONSTRAINT FK_DatPhong_NhanSu FOREIGN KEY (id_nhansu) REFERENCES dbo.NhanSu(id_nhansu),
    CONSTRAINT CK_DatPhong_NgayTra CHECK (ngaytra_dukien > ngaynhan_dukien),
    CONSTRAINT CK_DatPhong_SoNguoi CHECK (songuoi > 0),
    CONSTRAINT CK_DatPhong_TrangThai CHECK (trangthai IN (N'Đã xác nhận', N'Đã check-in', N'Đã check-out', N'Đã hủy'))
);
GO

CREATE INDEX IX_DatPhong_KhoangNgay ON dbo.DatPhong (ngaynhan_dukien, ngaytra_dukien);
GO

-- 9. BẢNG CHITIETDATPHONG
CREATE TABLE dbo.ChiTietDatPhong (
    id              INT IDENTITY(1,1) NOT NULL,
    id_datphong     INT NOT NULL,
    id_phong        INT NOT NULL,
    dongia          DECIMAL(12,2) NOT NULL,
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_ChiTietDatPhong_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_ChiTietDatPhong PRIMARY KEY (id),
    CONSTRAINT FK_CTDP_DatPhong FOREIGN KEY (id_datphong) REFERENCES dbo.DatPhong(id_datphong) ON DELETE CASCADE,
    CONSTRAINT FK_CTDP_Phong FOREIGN KEY (id_phong) REFERENCES dbo.Phong(id_phong),
    CONSTRAINT UQ_CTDP_DatPhong_Phong UNIQUE (id_datphong, id_phong),
    CONSTRAINT CK_CTDP_DonGia CHECK (dongia >= 0)
);
GO

-- 10. BẢNG CHITIETDICHVU
CREATE TABLE dbo.ChiTietDichVu (
    id              INT IDENTITY(1,1) NOT NULL,
    id_datphong     INT NOT NULL,
    id_dichvu       INT NOT NULL,
    soluong         INT NOT NULL,
    dongia          DECIMAL(12,2) NOT NULL,
    thoigian        DATETIME2 NOT NULL CONSTRAINT DF_CTDV_ThoiGian DEFAULT (sysdatetime()),
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_ChiTietDichVu_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_ChiTietDichVu PRIMARY KEY (id),
    CONSTRAINT FK_CTDV_DatPhong FOREIGN KEY (id_datphong) REFERENCES dbo.DatPhong(id_datphong) ON DELETE CASCADE,
    CONSTRAINT FK_CTDV_DichVu FOREIGN KEY (id_dichvu) REFERENCES dbo.DichVu(id_dichvu),
    CONSTRAINT CK_CTDV_SoLuong CHECK (soluong > 0),
    CONSTRAINT CK_CTDV_DonGia CHECK (dongia >= 0)
);
GO

-- 11. BẢNG HOADON
CREATE TABLE dbo.HoaDon (
    id_hoadon               INT IDENTITY(1,1) NOT NULL,
    id_datphong             INT NOT NULL,
    ngaylap                 DATETIME2 NOT NULL CONSTRAINT DF_HoaDon_NgayLap DEFAULT (sysdatetime()),
    tienphong               DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_TienPhong DEFAULT (0),
    tiendichvu              DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_TienDichVu DEFAULT (0),
    giamgia                 DECIMAL(14,2) NOT NULL CONSTRAINT DF_HoaDon_GiamGia DEFAULT (0),
    tonggia                 AS (tienphong + tiendichvu - giamgia) PERSISTED,
    phuongthuc_thanhtoan    NVARCHAR(20) NULL,
    trangthai_thanhtoan     NVARCHAR(20) NOT NULL CONSTRAINT DF_HoaDon_TrangThai DEFAULT (N'Chưa thanh toán'),
    created_at              DATETIME2 NOT NULL CONSTRAINT DF_HoaDon_CreatedAt DEFAULT (sysdatetime()),
    updated_at              DATETIME2 NULL,
    CONSTRAINT PK_HoaDon PRIMARY KEY (id_hoadon),
    CONSTRAINT UQ_HoaDon_DatPhong UNIQUE (id_datphong),
    CONSTRAINT FK_HoaDon_DatPhong FOREIGN KEY (id_datphong) REFERENCES dbo.DatPhong(id_datphong),
    CONSTRAINT CK_HoaDon_PTTT CHECK (phuongthuc_thanhtoan IS NULL OR phuongthuc_thanhtoan IN (N'Tiền mặt', N'Chuyển khoản', N'VNPay')),
    CONSTRAINT CK_HoaDon_TrangThai CHECK (trangthai_thanhtoan IN (N'Chưa thanh toán', N'Đã thanh toán'))
);
GO

-- 12. BẢNG LICHSUGUITHONGBAO
CREATE TABLE dbo.LichSuGuiThongBao (
    id_lichsu       INT IDENTITY(1,1) NOT NULL,
    id_datphong     INT NULL,
    id_hoadon       INT NULL,
    loai_thongbao   NVARCHAR(50) NOT NULL,
    nguoi_nhan      NVARCHAR(100) NOT NULL,
    tieu_de         NVARCHAR(255) NULL,
    trang_thai      NVARCHAR(20) NOT NULL,
    ly_do_loi       NVARCHAR(500) NULL,
    thoi_gian_gui   DATETIME2 NOT NULL CONSTRAINT DF_LichSuGuiThongBao_ThoiGianGui DEFAULT (sysdatetime()),
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_LichSuGuiThongBao_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_LichSuGuiThongBao PRIMARY KEY (id_lichsu),
    CONSTRAINT FK_LichSuGuiThongBao_DatPhong FOREIGN KEY (id_datphong) REFERENCES dbo.DatPhong(id_datphong) ON DELETE SET NULL,
    CONSTRAINT FK_LichSuGuiThongBao_HoaDon FOREIGN KEY (id_hoadon) REFERENCES dbo.HoaDon(id_hoadon) ON DELETE SET NULL
);
GO

CREATE NONCLUSTERED INDEX IX_LichSuGuiThongBao_ThoiGian ON dbo.LichSuGuiThongBao(thoi_gian_gui DESC);
CREATE NONCLUSTERED INDEX IX_LichSuGuiThongBao_DatPhong ON dbo.LichSuGuiThongBao(id_datphong);
CREATE NONCLUSTERED INDEX IX_LichSuGuiThongBao_HoaDon ON dbo.LichSuGuiThongBao(id_hoadon);
GO

-- 13. BẢNG NHATKYHOATDONG
CREATE TABLE dbo.NhatKyHoatDong (
    id_nhatky       INT IDENTITY(1,1) NOT NULL,
    id_taikhoan     INT NOT NULL,
    hanhdong        NVARCHAR(50) NOT NULL,
    thoigian        DATETIME2 NOT NULL CONSTRAINT DF_NhatKyHoatDong_ThoiGian DEFAULT (sysdatetime()),
    ghichu          NVARCHAR(255) NULL,
    created_at      DATETIME2 NOT NULL CONSTRAINT DF_NhatKyHoatDong_CreatedAt DEFAULT (sysdatetime()),
    updated_at      DATETIME2 NULL,
    CONSTRAINT PK_NhatKyHoatDong PRIMARY KEY (id_nhatky),
    CONSTRAINT FK_NhatKyHoatDong_TaiKhoan FOREIGN KEY (id_taikhoan) REFERENCES dbo.TaiKhoan(id_taikhoan),
    CONSTRAINT CK_NhatKyHoatDong_HanhDong CHECK (hanhdong IN (N'Đăng nhập', N'Đăng xuất'))
);
GO

CREATE NONCLUSTERED INDEX IX_NhatKyHoatDong_TaiKhoan ON dbo.NhatKyHoatDong(id_taikhoan, thoigian DESC);
GO

/* ===================================================================================
   PHẦN II: DỮ LIỆU MẪU (SEED DATA)
   =================================================================================== */

-- 1. Vai trò
INSERT INTO dbo.VaiTro (ten_vaitro) VALUES (N'Admin'), (N'LeTan'), (N'KhachHang');
GO

-- 2. Nhân sự
INSERT INTO dbo.NhanSu (hoten, ngaysinh, cccd, sdt) VALUES
    (N'Nguyễn Văn Quân (Admin)', '1990-05-10', '079090001234', '0901234567'),
    (N'Trần Thị Lễ Tân', '1998-08-20', '079098005678', '0912345678'),
    (N'Lê Thị Thu Ngân', '2000-01-15', '079099009876', '0988776655');
GO

-- 3. Tài khoản đăng nhập
INSERT INTO dbo.TaiKhoan (ten_dangnhap, matkhau_hash, id_nhansu, id_vaitro, trangthai, solan_saimatkhau) VALUES
    ('admin',        '$2a$11$FEwLOM5vc8p2YlVdh1JzpuD/F5Ab4yfvdoFRUeQqsI78jPGPmvXHS', 1, 1, N'Hoạt động', 0),
    ('letan1',       '$2a$11$8pXNPeK3P8mu5XRIEBF1lukK4qj1mCizlbq3GnVdTnnrq4CLSJMuG', 2, 2, N'Hoạt động', 0),
    ('receptionist', '$2a$11$8pXNPeK3P8mu5XRIEBF1lukK4qj1mCizlbq3GnVdTnnrq4CLSJMuG', 3, 2, N'Hoạt động', 0);
GO

-- 4. Loại phòng
INSERT INTO dbo.LoaiPhong (ten, mucgia, succhua, mota) VALUES
    (N'Phòng Standard',  500000, 2, N'Phòng tiêu chuẩn 1 giường đôi, đầy đủ tiện nghi, view thành phố.'),
    (N'Phòng Deluxe',    800000, 2, N'Phòng cao cấp 1 giường King, view biển thoáng mát, kèm minibar miễn phí.'),
    (N'Phòng Suite',    1500000, 4, N'Căn hộ cao cấp 2 phòng ngủ, phòng khách riêng, ban công ngắm hoàng hôn.');
GO

-- 5. Danh sách phòng
INSERT INTO dbo.Phong (sophong, id_loaiphong, trangthai) VALUES
    ('101', 1, N'Trống'),
    ('102', 1, N'Trống'),
    ('103', 1, N'Đang dọn'),
    ('201', 2, N'Trống'),
    ('202', 2, N'Trống'),
    ('203', 2, N'Đang dọn'),
    ('301', 3, N'Trống'),
    ('302', 3, N'Trống'),
    ('303', 3, N'Bảo trì');
GO

-- 6. Dịch vụ
INSERT INTO dbo.DichVu (ten_dichvu, mucgia) VALUES
    (N'Minibar nước ngọt / snack', 50000),
    (N'Giặt ủi quần áo lấy liền',   30000),
    (N'Đưa đón sân bay 4 chỗ',     200000),
    (N'Buffet ăn sáng cao cấp',    120000),
    (N'Dịch vụ Spa & Massage',     300000);
GO

-- 7. Khách hàng mẫu
INSERT INTO dbo.KhachHang (ten, sdt, cccd, gioitinh, ngaysinh, email) VALUES
    (N'Lê Văn Khách',     '0987654321', '079095009999', N'Nam', '1995-03-15', 'khachmau@gmail.com'),
    (N'Trần Thị Mai',     '0912345678', '079096008888', N'Nữ',  '1997-07-22', 'maitran@gmail.com'),
    (N'Nguyễn Đức Trung', '0862198497', '038204025011', N'Nam', '2004-03-31', 'trungfc147123@gmail.com');
GO

-- 8. Booking mẫu và Hóa đơn mẫu
INSERT INTO dbo.DatPhong (mabooking, id_khachhang, id_nhansu, ngaydat, ngaynhan_dukien, ngaytra_dukien, ngaynhan_thucte, ngaytra_thucte, songuoi, trangthai)
VALUES ('BK261001080000', 1, 1, '2026-10-01 08:00:00', '2026-10-01', '2026-10-03', '2026-10-01 14:00:00', '2026-10-03 11:30:00', 2, N'Đã check-out');
GO

INSERT INTO dbo.ChiTietDatPhong (id_datphong, id_phong, dongia)
VALUES (1, 1, 500000);
GO

INSERT INTO dbo.ChiTietDichVu (id_datphong, id_dichvu, soluong, dongia, thoigian)
VALUES (1, 1, 2, 50000, '2026-10-02 19:00:00');
GO

INSERT INTO dbo.HoaDon (id_datphong, ngaylap, tienphong, tiendichvu, giamgia, phuongthuc_thanhtoan, trangthai_thanhtoan)
VALUES (1, '2026-10-03 11:45:00', 1000000, 100000, 0, N'Tiền mặt', N'Đã thanh toán');
GO

INSERT INTO dbo.DatPhong (mabooking, id_khachhang, id_nhansu, ngaydat, ngaynhan_dukien, ngaytra_dukien, ngaynhan_thucte, ngaytra_thucte, songuoi, trangthai)
VALUES ('BK261015093000', 2, 2, '2026-10-15 09:30:00', '2026-10-15', '2026-10-17', '2026-10-15 14:30:00', '2026-10-17 11:00:00', 2, N'Đã check-out');
GO

INSERT INTO dbo.ChiTietDatPhong (id_datphong, id_phong, dongia)
VALUES (2, 4, 800000);
GO

INSERT INTO dbo.ChiTietDichVu (id_datphong, id_dichvu, soluong, dongia, thoigian)
VALUES (2, 3, 1, 200000, '2026-10-15 15:00:00');
GO

INSERT INTO dbo.HoaDon (id_datphong, ngaylap, tienphong, tiendichvu, giamgia, phuongthuc_thanhtoan, trangthai_thanhtoan)
VALUES (2, '2026-10-17 11:15:00', 1600000, 200000, 100000, N'Chuyển khoản', N'Đã thanh toán');
GO

INSERT INTO dbo.DatPhong (mabooking, id_khachhang, id_nhansu, ngaydat, ngaynhan_dukien, ngaytra_dukien, ngaynhan_thucte, ngaytra_thucte, songuoi, trangthai)
VALUES ('BK261101140000', 3, 2, '2026-11-01 14:00:00', '2026-11-10', '2026-11-12', NULL, NULL, 4, N'Đã xác nhận');
GO

INSERT INTO dbo.ChiTietDatPhong (id_datphong, id_phong, dongia)
VALUES (3, 7, 1500000);
GO

PRINT N'=======================================================================';
PRINT N'ĐÃ TẠO DATABASE HOTELMANAGEMENTDB VÀ NẠP DỮ LIỆU MẪU THÀNH CÔNG!';
PRINT N'=======================================================================';
GO
