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

## 🔑 5. Tài khoản Đăng nhập Mẫu & Hướng dẫn Demo

| Tài khoản | Tên đăng nhập | Mật khẩu | Phạm vi sử dụng |
|---|---|---|---|
| **Quản trị viên (Admin)** | `admin` | `admin123` | Toàn quyền quản trị danh mục (phòng, loại phòng, dịch vụ, tiện nghi), nhân sự, tài khoản, Dashboard và Báo cáo doanh thu |
| **Lễ tân (Receptionist)** | `letan1` *(hoặc `receptionist`)* | `letan123` | Quản lý sơ đồ phòng (tìm kiếm & phân trang), tiếp nhận đặt phòng, Check-in, Check-out, Thanh toán hóa đơn (chọn dịch vụ miễn phí, giảm giá, in PDF, gửi email) |
| **Khách hàng trực tuyến** | `trungnguyen` *(hoặc tự Đăng ký)* | `khach123` | Cổng Khách hàng: Tự đặt phòng trực tuyến (kèm chọn dịch vụ), xem lịch sử đặt phòng, quản lý thông tin cá nhân & đổi mật khẩu |
| **Kiosk tự phục vụ** | *(Không cần đăng nhập)* | *(Trực tiếp)* | Tra cứu mã booking, Chatbot AI tư vấn thông tin khách sạn |

---

### 🌐 Hướng dẫn Demo Luồng Khách hàng Mới:
1. **Đăng ký tài khoản mới:** Tại màn hình Đăng nhập, bấm **"Đăng ký khách hàng"**. Nhập họ tên, CCCD (12 số), SĐT, Email và Mật khẩu. Hệ thống tự động tạo hồ sơ khách hàng và tài khoản đăng nhập.
2. **Đăng nhập Cổng khách hàng:** Đăng nhập bằng tài khoản vừa tạo hoặc tài khoản mẫu `trungnguyen` / `khach123`.
3. **Tự đặt phòng & dịch vụ:**
   - Chọn ngày nhận phòng, ngày trả phòng (validate không được trước thời điểm hiện tại), số lượng khách và loại phòng mong muốn.
   - Bấm **"Tìm phòng"** để xem danh sách phòng trống khả dụng.
   - Tích chọn phòng muốn đặt.
   - Chọn thêm dịch vụ phát sinh mong muốn kèm số lượng (phân loại rõ ràng: **Nước uống**, **Thức ăn**, **Dịch vụ**).
   - Bấm **"Xác nhận đặt phòng"**: Hệ thống bảo đảm an toàn giao dịch đa luồng (Serializable Isolation Level), tạo booking và tự động gửi email xác nhận đặt phòng kèm thông tin chi tiết.
4. **Xem lịch sử & Quản lý tài khoản:** Chuyển qua tab **"Lịch sử đặt phòng"** để theo dõi trạng thái các đơn đặt (Đã xác nhận / Đã check-in / Đã check-out) hoặc tab **"Thông tin cá nhân"** để đổi mật khẩu.

---

## 💡 6. Ghi chú Thiết kế & Tính năng Nổi bật

- **Quản lý Dịch vụ & Thanh toán Khuyến mãi:**
  - Phân loại danh mục dịch vụ rõ ràng: **Nước uống**, **Thức ăn**, **Dịch vụ**.
  - Khi thanh toán hóa đơn (`FormThanhToan`), hỗ trợ tính năng **Chọn dịch vụ miễn phí**: Thu ngân có thể chọn cụ thể từng dịch vụ được tặng/miễn phí (100%) hoặc chọn tất cả dịch vụ để áp dụng giảm trừ trực tiếp trên tổng hóa đơn.
- **Giá phòng linh hoạt:** Quản lý đồng thời **Giá theo ngày** và **Giá theo giờ** cho từng Loại phòng.
- **Giao diện Desktop WinForms Hiện đại:** Chuẩn màu Emerald Green & Cream White, bảng điều khiển tìm kiếm với cơ chế Debounce 300ms, xuất dữ liệu ra file Excel định dạng chuẩn chỉ với 1 click.
- **Bảo vệ tài khoản & Quản trị nhanh:** Cơ chế chống dò mật khẩu khóa tài khoản sau 10 lần nhập sai (miễn trừ admin). Hỗ trợ nút Đặt lại mật khẩu nhanh và Khóa / Mở khóa tài khoản ngay trên danh sách.
- **Xử lý bất đồng bộ:** Toàn bộ tác vụ nặng (Báo cáo, Hóa đơn PDF, Email SMTP, AI Gemini) đều chạy trên background thread (`Task.Run` / `IProgress<T>` / `CancellationToken`), không gây đơ hoặc giật lag giao diện.
