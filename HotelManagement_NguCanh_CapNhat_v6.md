# HỆ THỐNG QUẢN LÝ KHÁCH SẠN — Tài liệu ngữ cảnh cập nhật (v6)

Đồ án môn Lập trình Windows (WinForms .NET 10 + C#)
Cập nhật sau khi hoàn thành DCKLTTQ-69 (Tìm phòng trống) và DCKLTTQ-70 (Form Đặt phòng
master-detail) trong Epic 3. Việc tiếp theo là DCKLTTQ-71 (Check-in bằng tra mã booking).

## 1. Thông tin chung (không đổi so với các bản trước)

| Hạng mục | Nội dung |
|---|---|
| Môn học | Lập trình Windows (WinForms .NET 10 + C#) |
| Hình thức đánh giá | Demo + Vấn đáp; thang điểm 10; độ chia nhỏ nhất 0.25 |
| Quy mô nhóm | 1 người thực hiện |
| Đề tài | Phần mềm quản lý khách sạn (desktop WinForms) |
| Cơ sở dữ liệu | SQL Server — HotelManagementDB (12 bảng) |
| Quản lý tiến độ | Jira (project DCKLTTQ) qua Atlassian MCP connector |
| Nhịp độ làm việc | 2 task/ngày (để kiểm soát khối lượng, làm một mình) |

Kiến trúc, quy ước đặt tên Entity, danh sách 12 bảng, và các quyết định thiết kế đã chốt giữ
nguyên như tài liệu gốc (`HotelManagement_NguCanh_CapNhat_v3.md`). Tài liệu v5 bổ sung tiến độ
DCKLTTQ-65 đến 68. Tài liệu này (v6) bổ sung tiếp DCKLTTQ-69 và DCKLTTQ-70.

## 2. Tiến độ Jira (project DCKLTTQ) tính đến thời điểm hiện tại

| Epic / Issue | Nội dung | Trạng thái |
|---|---|---|
| Epic 1 (DCKLTTQ-50) | Database & Kiến trúc nền tảng | Done |
| Epic 2 (DCKLTTQ-55) | Repository, Đăng nhập & Phân quyền | Done |
| **Epic 3 (DCKLTTQ-64)** | **Module lõi: CRUD, đặt phòng, check-in/check-out, sơ đồ phòng GDI+** | **Đang làm** |
| DCKLTTQ-65 | CRUD Loại phòng | Done |
| DCKLTTQ-66 | CRUD Phòng | Done |
| DCKLTTQ-67 | CRUD Khách hàng | Done |
| DCKLTTQ-68 | CRUD Nhân sự (kèm tạo tài khoản đăng nhập) | Done |
| DCKLTTQ-69 | Tìm phòng trống theo ngày/loại phòng/số khách | **Done** |
| DCKLTTQ-70 | Form Đặt phòng master-detail nhiều phòng | **Done** |
| **DCKLTTQ-71** | **Check-in bằng tra mã booking** | **To Do — việc tiếp theo** |
| DCKLTTQ-72 | Check-out và thêm dịch vụ phát sinh | To Do |
| DCKLTTQ-73 | Sơ đồ phòng vẽ bằng GDI+ | To Do |
| Epic 4 (DCKLTTQ-74) | Hóa đơn, báo cáo PDF/Excel, email, QR | To Do |
| Epic 5 (DCKLTTQ-83) | Dashboard, Kiosk, đa ngôn ngữ | To Do |
| Epic 6 (DCKLTTQ-87) | Chatbot AI (Gemini), tính năng mở rộng | To Do |
| Epic 7 (DCKLTTQ-92) | Hoàn thiện, bảo mật, publish, chuẩn bị bảo vệ | To Do |
| Bổ sung (DCKLTTQ-98) | Thiết kế giao diện (UI/UX) | To Do — làm sau Epic 3-7 |

Quy tắc đã áp dụng: chỉ chuyển issue sang **Done** sau khi đã build và kiểm chứng chạy thực tế
trên dữ liệu thật — không đánh Done chỉ vì đã viết xong code.

## 3. Entity thật đã đối chiếu (quan trọng — thay thế các giả định ở v5)

Trong lúc làm DCKLTTQ-69, đã xem code thật của `IDatPhongRepository`/`DatPhongRepository` (viết
sẵn từ trước, không phải giả định). Các tên property/cột sau đây đã **xác nhận chính xác**,
theo đúng quy ước scaffold (chỉ viết hoa chữ đầu mỗi đoạn tách bởi `_`, không viết hoa từng từ
con bên trong đoạn):

| Entity | Property/Method đã xác nhận |
|---|---|
| `DatPhong` | `IdDatphong`, `Mabooking`, `Trangthai`, `NgaynhanDukien`, `NgaytraDukien`, `ChiTietDatPhongs`, `ChiTietDichVus`, `IdKhachhangNavigation`, `HoaDon` |
| `ChiTietDatPhong` | `IdPhong`, `IdDatphong`, `IdDatphongNavigation`, `IdPhongNavigation` |
| `ChiTietDichVu` | `IdDichvuNavigation` |
| `Phong` | `IdPhong`, `SoPhong`, `IdLoaiphong`, `Trangthai` (ràng buộc CK_Phong_TrangThai, có giá trị `"Bảo trì"`) |

**Còn chưa xác nhận** (đang dùng giả định, cần đối chiếu khi build DCKLTTQ-69/70): property
`SucChua`, `TenLoaiPhong` (hoặc `Succhua`, `Tenloaiphong`...), `Gia` trên entity `LoaiPhong`, và
navigation `IdLoaiphongNavigation` trên `Phong`. Nếu entity thật khác tên, chỉ cần sửa trong
`DatPhongRepository.GetPhongTrongAsync` và `DatPhongService.TimPhongTrongAsync`, không đổi cấu
trúc Form.

`IDatPhongRepository` đã có sẵn từ trước (không phải viết mới ở DCKLTTQ-69):
`IsPhongTrongAsync`, `GetPhongTrongAsync`, `GetByMaBookingAsync`, `GetChiTietDayDuAsync`. Việc
của DCKLTTQ-69 chỉ là **mở rộng** `GetPhongTrongAsync` thêm tham số `soKhach` (lọc theo sức
chứa), giữ nguyên toàn bộ logic giao nhau khoảng ngày đã có (điều kiện
`NgaynhanDukien < ngayTra && NgaytraDukien > ngayNhan`, chỉ 2 trạng thái "Đã xác nhận"/"Đã
check-in" coi là đang giữ phòng).

## 4. Mã nguồn đã viết trong DCKLTTQ-69 và DCKLTTQ-70

### Tầng DAL
- Mở rộng `IDatPhongRepository.GetPhongTrongAsync(DateOnly ngayNhan, DateOnly ngayTra, int?
  idLoaiPhong = null, int? soKhach = null)` — thêm lọc theo sức chứa tối thiểu của loại phòng.
  `Phong` query luôn `Include(p => p.IdLoaiphongNavigation)` (không chỉ khi có `soKhach`) để
  tránh `NullReferenceException` khi map DTO ở tầng BLL.

### Tầng BLL
- `DTOs/PhongTrongDto.cs` — `IdPhong`, `SoPhong`, `IdLoaiPhong`, `TenLoaiPhong`, `Gia`,
  `SucChua`.
- `Services/IDatPhongService.cs` / `DatPhongService.cs` — `TimPhongTrongAsync(ngayNhan, ngayTra,
  idLoaiPhong, soKhach)`, validate: `ngayNhan < ngayTra`, `ngayNhan` không ở quá khứ, `soKhach >
  0` nếu có. Dùng `IUnitOfWork.DatPhongRepository` (giả định tên property trên `IUnitOfWork`,
  cần đối chiếu nếu build lỗi).
- Đã đăng ký `IDatPhongService`/`DatPhongService` vào DI trong `Program.cs`.

### Tầng UI (DCKLTTQ-70 — Form Đặt phòng master-detail)
- Form "master" nhập thông tin đặt phòng (khách hàng, ngày nhận/trả); "detail" là danh sách
  nhiều phòng chọn trong 1 lượt đặt, dựng theo đúng khuôn mẫu Form độc lập đã tách từ Epic 2
  (control dựng bằng code, không dùng Designer).
- Tái sử dụng `IKhachHangService` (tìm/chọn khách hàng đã có) và `PhongTrongDto` từ
  `IDatPhongService.TimPhongTrongAsync` (DCKLTTQ-69) để đổ danh sách phòng trống cho người dùng
  chọn.

> Lưu ý: chi tiết implement cụ thể của Form (tên control, layout, cách tạo `DatPhong` +
> nhiều `ChiTietDatPhong` trong 1 transaction qua `IUnitOfWork.BeginTransactionAsync`) không có
> trong ngữ cảnh hội thoại trước — nếu cần debug hoặc mở rộng DCKLTTQ-70, nên dán kèm code Form
> thật vào hội thoại mới.

## 5. Việc tiếp theo — DCKLTTQ-71 (Check-in bằng tra mã booking)

- Dùng `IDatPhongRepository.GetByMaBookingAsync(string maBooking)` đã có sẵn để tra booking theo
  mã, và `GetChiTietDayDuAsync(int idDatPhong)` để lấy đầy đủ chi tiết phòng/dịch vụ/khách hàng
  phục vụ màn hình check-in.
- Nghiệp vụ check-in: tìm booking theo mã → kiểm tra trạng thái phải là "Đã xác nhận" → cập nhật
  `Trangthai` của `DatPhong` sang "Đã check-in" và cập nhật trạng thái các `Phong` liên quan
  sang "Đang ở" (bảng `Phong` có ràng buộc CK_Phong_TrangThai, cần xác nhận lại đúng chuỗi giá
  trị, ví dụ "Đang ở" hay cách viết khác).
- Sau DCKLTTQ-71 là DCKLTTQ-72 (Check-out + dịch vụ phát sinh) và DCKLTTQ-73 (sơ đồ phòng
  GDI+), hoàn tất các task còn lại của Epic 3.

## 6. Đoạn ngữ cảnh để dán vào cuộc hội thoại mới

Sao chép đoạn dưới đây (cùng với file này và `HotelManagement_NguCanh_CapNhat_v3.md` nếu cần)
khi bắt đầu cuộc hội thoại mới:

> Tôi đang làm đồ án môn Lập trình Windows (WinForms .NET 10 + C#, SQL Server, làm một mình)
> với đề tài Hệ thống quản lý khách sạn. File markdown gốc (v3) có kiến trúc, thiết kế database,
> quy ước đặt tên Entity. File markdown này (v6) cập nhật tiến độ mới nhất: đã hoàn thành
> DCKLTTQ-65 đến 70 trong Epic 3 (CRUD Loại phòng/Phòng/Khách hàng/Nhân sự, Tìm phòng trống,
> Form Đặt phòng master-detail), tất cả đã kiểm chứng chạy thật và cập nhật Done trên Jira (qua
> Atlassian MCP connector). Mục 3 của file này có các tên property Entity thật đã đối chiếu
> (DatPhong, ChiTietDatPhong, ChiTietDichVu, Phong) — dùng đúng các tên này, không đoán lại theo
> PascalCase chuẩn thông thường. Việc tiếp theo là DCKLTTQ-71 (Check-in bằng tra mã booking),
> dùng `GetByMaBookingAsync`/`GetChiTietDayDuAsync` đã có sẵn trong `IDatPhongRepository`. Hãy
> đọc kỹ tài liệu, phản hồi bằng tiếng Việt theo phong cách chuyên nghiệp, và tiếp tục từ
> DCKLTTQ-71 theo đúng quy ước đặt tên Entity và kiến trúc đã có.
