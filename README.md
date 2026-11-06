# 🏨 Hotel Management System — Hệ thống Quản lý Khách sạn

---

## 📋 Mô tả

Ứng dụng desktop Windows Forms (.NET 10) quản lý vận hành khách sạn đầy đủ quy trình nghiệp vụ: quản lý danh mục phòng — loại phòng — dịch vụ — khách hàng — nhân sự — tài khoản, đặt phòng — check-in — check-out — hủy đặt phòng, thanh toán (tiền mặt / chuyển khoản VietQR), xuất hóa đơn PDF tiêu chuẩn, báo cáo doanh thu Excel, Dashboard biểu đồ thống kê (LiveCharts2), Kiosk tự phục vụ tra cứu mã booking tích hợp Chatbot tư vấn thông minh (Google Gemini AI), và gửi email tự động (MailKit).

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | .NET 10 — Windows Forms (WinForms) |
| Ngôn ngữ | C# 13 |
| Cơ sở dữ liệu | Microsoft SQL Server (LocalDB / Express / Developer) |
| ORM | Entity Framework Core 10 (Database-First scaffold, Repository Pattern + UnitOfWork) |
| Biểu đồ thống kê | LiveCharts2 (Cartesian & Pie Charts) |
| Trí tuệ nhân tạo | Google Gemini AI (REST API generativelanguage.googleapis.com) |
| Xuất PDF & Mã QR | QuestPDF (Community License) + ZXing.Net |
| Báo cáo & Nhập liệu Excel | ClosedXML (.xlsx import có validate toàn diện + export) |
| Gửi Email thông báo | MailKit (Gmail SMTP StartTls) |
| Bảo mật cấu hình | .NET User Secrets (`secrets.json`, không lộ credential ra Git) |
| Kiến trúc | Layered Architecture: UI (`HotelManagement`) → BLL (`HotelManagement.BLL`) → DAL (`HotelManagement.DAL`) → Models (`HotelManagement.Models`) |

---

## 📁 Cấu trúc Solution

```
HotelManagement/                  ← UI Layer (WinForms, Forms, AppTheme, Program)
HotelManagement.BLL/              ← Business Logic Layer (Services, DTOs, Pdf)
HotelManagement.DAL/              ← Data Access Layer (EF Core DbContext, Repositories, UnitOfWork)
HotelManagement.Models/           ← Domain Models / Entities (scaffold từ DB)
Mau_Excel/                        ← Tệp Excel biểu mẫu import phòng
```

---

## ⚡ Hướng dẫn cấu hình và chạy dự án (Setup Guide)

> **Tuân thủ tiêu chí bảo mật (DCKLTTQ-94 - Rubric H2):** Toàn bộ connection string, App Password Gmail và API Key Gemini **KHÔNG** hardcode trong code và **KHÔNG** commit lên Git. Các thông tin này được quản lý an toàn qua **.NET User Secrets** trong môi trường phát triển và chấm bài.

### 1. Yêu cầu môi trường
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Microsoft SQL Server (LocalDB, Express, hoặc Developer Edition)
- Hệ điều hành: Windows 10 / 11

### 2. Khởi tạo Cơ sở Dữ liệu
1. Mở SQL Server Management Studio (SSMS) hoặc Azure Data Studio.
2. Mở và thực thi tệp `SQLQuery1.sql` tại thư mục gốc để khởi tạo schema database `HotelManagementDB` và dữ liệu danh mục mẫu (Loại phòng, Dịch vụ, Nhân sự, Tài khoản).
3. Mở và thực thi tệp `TaoBang_LichSuGuiThongBao_Va_AuditColumns.sql` để tạo bảng nhật ký gửi thông báo và các cột audit.

### 3. Cấu hình User Secrets (Dành cho người chấm bài)

Mở terminal (PowerShell hoặc CMD) tại thư mục gốc của dự án (`d:\DAMH\Winform\HotelManagement`):

#### Bước 3.1: Cấu hình Connection String SQL Server (Bắt buộc)
Thay `YOUR_SQL_SERVER_NAME` bằng tên SQL Server trên máy bạn (ví dụ: `.\\SQLEXPRESS`, `(localdb)\\MSSQLLocalDB`, hoặc tên máy tính):
```powershell
dotnet user-secrets set "ConnectionStrings:HotelManagementDB" "Server=YOUR_SQL_SERVER_NAME;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;" --project HotelManagement
```

#### Bước 3.2: Cấu hình Gemini API Key cho Chatbot Kiosk (Tùy chọn)
Để sử dụng tính năng Chatbot tư vấn phòng và chính sách bằng Gemini AI tại màn hình Kiosk (DCKLTTQ-88):
```powershell
dotnet user-secrets set "Gemini:ApiKey" "YOUR_GEMINI_API_KEY" --project HotelManagement
```

#### Bước 3.3: Cấu hình Email SMTP Gmail (Tùy chọn)
Để sử dụng tính năng gửi email xác nhận đặt phòng và hóa đơn PDF tự động (DCKLTTQ-81):
```powershell
dotnet user-secrets set "Email:SenderEmail" "your_email@gmail.com" --project HotelManagement
dotnet user-secrets set "Email:AppPassword" "your_16_character_app_password" --project HotelManagement
```

#### Bước 3.4: Kiểm tra lại các secrets đã cấu hình
```powershell
dotnet user-secrets list --project HotelManagement
```

### 4. Biên dịch và Chạy ứng dụng

Chạy lệnh CLI:
```powershell
dotnet build
dotnet run --project HotelManagement
```
Hoặc mở file `HotelManagement.slnx` bằng Visual Studio 2022/2025 → nhấn **F5**.

### 5. Tài khoản đăng nhập mặc định
- **Quản trị viên (Admin):**
  - Tên đăng nhập: `admin`
  - Mật khẩu: `admin123`
- **Lễ tân (Receptionist):**
  - Tên đăng nhập: `receptionist`
  - Mật khẩu: `rec123`

---

## 📊 Tính năng chính đã hoàn thành

- ✅ **CRUD danh mục:** Phòng, Loại phòng, Dịch vụ, Khách hàng, Nhân sự, Tài khoản (phân quyền Admin / Lễ tân).
- ✅ **Sơ đồ phòng trực quan (Room Map):** Vẽ bằng GDI+ hiển thị trạng thái màu sắc theo thời gian thực (Trống, Đang ở, Đang dọn, Bảo trì).
- ✅ **Đặt phòng (Booking):** Kiểm tra trùng lịch phòng, tự động gợi ý phòng trống, tính tổng tiền dự kiến.
- ✅ **Check-in / Check-out:** Tra cứu mã booking, nhận phòng, trả phòng, hủy đặt phòng an toàn.
- ✅ **Thanh toán đa phương thức:** Tiền mặt (tính tiền thừa), Chuyển khoản VietQR tĩnh/động.
- ✅ **Xuất hóa đơn PDF tiêu chuẩn:** Sinh file PDF chuyên nghiệp bằng QuestPDF kèm mã QR tra cứu (ZXing).
- ✅ **Báo cáo doanh thu & Hiệu suất phòng:** Lọc đa tiêu chí, phân tích breakdown, xuất Excel định dạng chuẩn (ClosedXML).
- ✅ **Import danh sách phòng từ Excel:** Đọc file ClosedXML, gom toàn bộ lỗi validate từng dòng, lưu transaction (DCKLTTQ-79).
- ✅ **Gửi email thông báo tự động:** Xác nhận đặt phòng kèm mã booking + hóa đơn PDF đính kèm qua Gmail SMTP (MailKit, DCKLTTQ-81/82).
- ✅ **Dashboard biểu đồ quản trị:** Thống kê doanh thu cột, tỷ lệ lấp đầy đường, cơ cấu trạng thái phòng tròn (LiveCharts2, DCKLTTQ-84).
- ✅ **Kiosk tra cứu & Chatbot Gemini AI:** Khách tự tra cứu booking bằng mã/SĐT, Chatbot AI tư vấn thông tin khách sạn (DCKLTTQ-85, DCKLTTQ-88).
- ✅ **Xử lý tác vụ nặng bất đồng bộ:** Áp dụng mô hình chuẩn `Task.Run`, `IProgress<T>`, `CancellationToken`, nút Hủy và xử lý `OperationCanceledException` (DCKLTTQ-93).
- ✅ **Cấu hình bảo mật:** User Secrets phân tách độc lập, không lộ API key hay connection string ra Git (DCKLTTQ-94).

---

## 📜 License

Đồ án phục vụ mục đích học tập môn Đồ án Kiến trúc & Lập trình Windows. Mọi quyền thuộc về tác giả.
