# 🏨 Hướng dẫn Cài đặt & Vận hành Hệ thống Quản lý Khách sạn

Tài liệu hướng dẫn dành cho người chấm bài và người vận hành hệ thống Quản lý Khách sạn (WinForms .NET 10).

---

## 💻 1. Yêu cầu Hệ thống

- **Hệ điều hành:** Windows 10 / 11 (64-bit).
- **Cơ sở dữ liệu:** Microsoft SQL Server 2019 trở lên (hỗ trợ cả bản LocalDB, Express, Developer hoặc Enterprise).
- **Công cụ quản lý CSDL:** SQL Server Management Studio (SSMS) hoặc Azure Data Studio.
- **Lưu ý về .NET:** Bản chạy tại `D:\HotelManagement_Publish\` là **Self-Contained Single-File**, máy người chấm **không cần cài đặt .NET SDK hay .NET Runtime** vẫn chạy được bình thường.

---

## 🗄️ 2. Khởi tạo Cơ sở Dữ liệu (1-Click SQL)

Dự án cung cấp tệp script khởi tạo CSDL trọn gói **`HotelManagementDB_Full.sql`** (nằm ở thư mục gốc dự án hoặc trong thư mục `D:\HotelManagement_Publish\`):

1. Mở **SQL Server Management Studio (SSMS)** và kết nối tới SQL Server trên máy bạn.
2. Mở tệp **`HotelManagementDB_Full.sql`**.
3. Nhấn **Execute (F5)** để thực thi.

> **Kết quả:** Script tự động khởi tạo database `HotelManagementDB`, tạo đầy đủ 13 bảng nghiệp vụ và nạp sẵn bộ **dữ liệu mẫu** (loại phòng, danh sách phòng, dịch vụ, khách hàng, booking và hóa đơn mẫu) để kiểm thử ngay.

---

## ⚙️ 3. Cấu hình Connection String

1. Mở thư mục chứa bản chạy: `D:\HotelManagement_Publish\`.
2. Mở tệp **`appsettings.json`** bằng Notepad hoặc trình soạn thảo bất kỳ.
3. Chỉnh sửa `Server=...` cho đúng với tên SQL Server trên máy của bạn:

```json
{
  "ConnectionStrings": {
    "HotelManagementDB": "Server=YOUR_SQL_SERVER_NAME;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "YOUR_EMAIL@gmail.com",
    "SenderName": "Khách sạn DCKLTTQ",
    "AppPassword": "YOUR_GMAIL_APP_PASSWORD"
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY",
    "Model": "gemini-2.5-flash"
  }
}
```

> **Mẹo:** 
> - Nếu dùng SQL Server mặc định trên máy cục bộ, giá trị `Server` thường là `.` hoặc `localhost` hoặc `.\\SQLEXPRESS`.
> - Bản publish local đã được cấu hình sẵn các key kiểm thử phục vụ việc chấm bài. Bạn chỉ cần chỉnh lại mục `Server` nếu tên máy của bạn khác.

---

## ▶️ 4. Chạy Ứng dụng

1. Vào thư mục `D:\HotelManagement_Publish\`.
2. Nhấp đúp chuột vào tệp **`HotelManagement.exe`** để khởi chạy.
3. Ứng dụng tự động ghi log hoạt động và lỗi phát sinh tại thư mục `logs/log-YYYYMMDD.txt` cùng cấp với file `.exe`.

---

## 🔑 5. Tài khoản Đăng nhập Mẫu

| Tài khoản | Tên đăng nhập | Mật khẩu | Phạm vi sử dụng |
|---|---|---|---|
| **Quản trị viên (Admin)** | `admin` | `admin123` | Toàn quyền quản trị danh mục, nhân sự, tài khoản, Dashboard và Báo cáo doanh thu |
| **Lễ tân (Receptionist)** | `letan1` *(hoặc `receptionist`)* | `rec123` | Quản lý sơ đồ phòng, tiếp nhận đặt phòng, Check-in, Check-out, Hóa đơn |
| **Kiosk tự phục vụ** | *(Không cần đăng nhập)* | *(Trực tiếp)* | Tra cứu mã booking, Chatbot AI tư vấn thông tin khách sạn |

---

## 💡 6. Ghi chú Thiết kế Quan trọng

- **Giao diện Desktop WinForms:** Tối ưu hóa cho môi trường quầy lễ tân (POS) và màn hình cảm ứng Kiosk, hỗ trợ vẽ sơ đồ phòng thời gian thực (GDI+) và in hóa đơn PDF trực tiếp.
- **Kiosk tự phục vụ không cần tài khoản:** Khách hàng tại sảnh có thể tự tra cứu thông tin đặt phòng qua Mã Booking hoặc SĐT và trò chuyện với Chatbot Gemini AI mà không có quyền truy cập vào chức năng nội bộ.
- **Bảo vệ tài khoản Admin:** Cơ chế chống dò mật khẩu sẽ khóa tài khoản nhân viên sau 10 lần nhập sai, riêng tài khoản `admin` được miễn trừ khóa để tránh mất quyền quản trị hệ thống.
- **Xử lý bất đồng bộ:** Toàn bộ tác vụ nặng (Import Excel, Báo cáo, Hóa đơn PDF, Email, AI) đều chạy trên background thread (`Task.Run` / `IProgress<T>` / `CancellationToken`), có nút Hủy và không gây treo giao diện.
- **Bảo mật & Nhật ký:** Không hardcode thông tin nhạy cảm vào mã nguồn; hệ thống sử dụng Serilog ghi log xoay vòng theo ngày và xử lý ngoại lệ toàn cục tránh crash đột ngột.
