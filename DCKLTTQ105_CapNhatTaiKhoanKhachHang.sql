/* ===================================================================================
   ĐỒ ÁN HỆ THỐNG QUẢN LÝ KHÁCH SẠN
   Mã task: DCKLTTQ-105 — Thiết kế lại CSDL để KhachHang có tài khoản đăng nhập (Phương án A)
   Database: HotelManagementDB
   Tương thích: SQL Server 2019 / 2022
   =================================================================================== */

USE HotelManagementDB;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Bổ sung giá trị vai trò 'KhachHang' vào bảng VaiTro
IF NOT EXISTS (SELECT 1 FROM dbo.VaiTro WHERE ten_vaitro = N'KhachHang')
BEGIN
    INSERT INTO dbo.VaiTro (ten_vaitro) VALUES (N'KhachHang');
    PRINT N'1. Đã thêm vai trò KhachHang vào bảng VaiTro.';
END
ELSE
BEGIN
    PRINT N'1. Vai trò KhachHang đã tồn tại trong bảng VaiTro.';
END
GO

-- 2. Gỡ bỏ khóa ngoại và ràng buộc UNIQUE cũ trên cột id_nhansu để chuyển thành NULLABLE
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TaiKhoan_NhanSu')
BEGIN
    ALTER TABLE dbo.TaiKhoan DROP CONSTRAINT FK_TaiKhoan_NhanSu;
    PRINT N'2.1. Đã gỡ bỏ khóa ngoại FK_TaiKhoan_NhanSu.';
END
GO

IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_TaiKhoan_NhanSu')
BEGIN
    ALTER TABLE dbo.TaiKhoan DROP CONSTRAINT UQ_TaiKhoan_NhanSu;
    PRINT N'2.2. Đã gỡ bỏ ràng buộc UQ_TaiKhoan_NhanSu cũ.';
END
GO

-- 3. Đổi cột id_nhansu thành cho phép NULL
ALTER TABLE dbo.TaiKhoan ALTER COLUMN id_nhansu INT NULL;
PRINT N'3. Đã chuyển cột id_nhansu thành NULLABLE.';
GO

-- 4. Tạo lại khóa ngoại FK_TaiKhoan_NhanSu
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TaiKhoan_NhanSu')
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD CONSTRAINT FK_TaiKhoan_NhanSu 
        FOREIGN KEY (id_nhansu) REFERENCES dbo.NhanSu(id_nhansu);
    PRINT N'4. Đã tạo lại khóa ngoại FK_TaiKhoan_NhanSu.';
END
GO

-- 5. Tạo Filtered Unique Index cho id_nhansu (chỉ bắt buộc duy nhất khi id_nhansu IS NOT NULL)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_TaiKhoan_NhanSu')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_TaiKhoan_NhanSu 
        ON dbo.TaiKhoan(id_nhansu) WHERE id_nhansu IS NOT NULL;
    PRINT N'5. Đã tạo Filtered Unique Index UQ_TaiKhoan_NhanSu.';
END
GO

-- 6. Thêm cột id_khachhang (NULLABLE) nếu chưa có
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.TaiKhoan') AND name = 'id_khachhang')
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD id_khachhang INT NULL;
    PRINT N'6. Đã thêm cột id_khachhang vào bảng TaiKhoan.';
END
ELSE
BEGIN
    PRINT N'6. Cột id_khachhang đã tồn tại.';
END
GO

-- 7. Tạo khóa ngoại FK_TaiKhoan_KhachHang
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TaiKhoan_KhachHang')
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD CONSTRAINT FK_TaiKhoan_KhachHang 
        FOREIGN KEY (id_khachhang) REFERENCES dbo.KhachHang(id_khachhang);
    PRINT N'7. Đã tạo khóa ngoại FK_TaiKhoan_KhachHang.';
END
GO

-- 8. Tạo Filtered Unique Index cho id_khachhang (1 khách hàng có tối đa 1 tài khoản)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_TaiKhoan_KhachHang')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UQ_TaiKhoan_KhachHang 
        ON dbo.TaiKhoan(id_khachhang) WHERE id_khachhang IS NOT NULL;
    PRINT N'8. Đã tạo Filtered Unique Index UQ_TaiKhoan_KhachHang.';
END
GO

-- 9. Thêm CHECK CONSTRAINT đảm bảo đúng 1 trong 2 cột (id_nhansu, id_khachhang) có giá trị
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_TaiKhoan_DoiTuong')
BEGIN
    ALTER TABLE dbo.TaiKhoan ADD CONSTRAINT CK_TaiKhoan_DoiTuong CHECK (
        (id_nhansu IS NOT NULL AND id_khachhang IS NULL) OR 
        (id_nhansu IS NULL AND id_khachhang IS NOT NULL)
    );
    PRINT N'9. Đã tạo CHECK CONSTRAINT CK_TaiKhoan_DoiTuong.';
END
GO

PRINT N'=======================================================================';
PRINT N'HOÀN TẤT CẬP NHẬT DATABASE CHO TÀI KHOẢN KHÁCH HÀNG (DCKLTTQ-105)!';
PRINT N'=======================================================================';
GO

