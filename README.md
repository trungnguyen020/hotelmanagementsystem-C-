# 🏨 Hotel Management System — Hệ thống Quản lý Khách sạn

---

## 📋 Mô tả

Ứng dụng WinForms quản lý vận hành khách sạn đầy đủ nghiệp vụ: quản lý danh mục phòng — loại phòng — dịch vụ — khách hàng — nhân sự — tài khoản, đặt phòng — check-in — check-out, thanh toán (tiền mặt / chuyển khoản QR), xuất hóa đơn PDF và báo cáo doanh thu Excel.

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | .NET 10 — Windows Forms (WinForms) |
| Ngôn ngữ | C# 13 |
| Cơ sở dữ liệu | SQL Server (LocalDB / Express) |
| ORM | Entity Framework Core 10 (Database-First scaffold) |
| Xuất PDF | QuestPDF |
| Xuất Excel | ClosedXML |
| Kiến trúc | 4 tầng: UI → BLL → DAL → Models |
| Quản lý tiến độ | Jira (project DCKLTTQ) |

## 📁 Cấu trúc Solution

```
HotelManagement/                  ← UI Layer (WinForms)
HotelManagement.BLL/              ← Business Logic Layer (Services, DTOs)
HotelManagement.DAL/              ← Data Access Layer (EF Core, Repositories, UoW)
HotelManagement.Models/           ← Domain Models / Entities
```

## ⚡ Hướng dẫn chạy

### Yêu cầu

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB, Express, hoặc Developer Edition)

### Bước 1 — Tạo cơ sở dữ liệu

Chạy script SQL tạo database `HotelManagementDB` (file script nằm trong thư mục `Database/` hoặc liên hệ tác giả).

### Bước 2 — Cấu hình Connection String

Tạo file `HotelManagement/appsettings.Development.json` (file này **không** được commit lên Git):

```json
{
  "ConnectionStrings": {
    "HotelManagementDB": "Server=YOUR_SERVER_NAME;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Thay `YOUR_SERVER_NAME` bằng tên SQL Server instance trên máy bạn (ví dụ: `DESKTOP-ABC123`, `.\\SQLEXPRESS`, `(localdb)\\MSSQLLocalDB`).

### Bước 3 — Build & Run

```bash
dotnet build
dotnet run --project HotelManagement
```

Hoặc mở file `HotelManagement.sln` bằng Visual Studio 2022/2025 → **F5**.

## 📊 Tính năng chính

- ✅ CRUD danh mục: Phòng, Loại phòng, Dịch vụ, Khách hàng, Nhân sự, Tài khoản
- ✅ Đặt phòng (Booking) với kiểm tra trùng lịch và tính năng auto-fill
- ✅ Sơ đồ phòng trực quan (Room Map) vẽ bằng GDI+
- ✅ Check-in / Check-out với tra cứu mã booking
- ✅ Thanh toán: tiền mặt / chuyển khoản (QR tĩnh)
- ✅ Xuất hóa đơn PDF (QuestPDF)
- ✅ Báo cáo doanh thu & hiệu suất phòng — xuất Excel (ClosedXML)
- 🔲 Dashboard tổng quan (đang phát triển)
- 🔲 Publish Self-Contained + Installer

## 📜 License

Đồ án phục vụ mục đích học tập. Mọi quyền thuộc về tác giả.
