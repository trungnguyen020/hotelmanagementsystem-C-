# HỆ THỐNG QUẢN LÝ KHÁCH SẠN — Tài liệu ngữ cảnh cập nhật (v5)

Đồ án môn Lập trình Windows (WinForms .NET 10 + C#)
Cập nhật sau khi hoàn thành DCKLTTQ-67 (CRUD Khách hàng) và DCKLTTQ-68 (CRUD Nhân sự) trong Epic 3.

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
nguyên như tài liệu gốc (`HotelManagement_NguCanh_CapNhat_v3.pdf`). Tài liệu này chỉ bổ sung
phần tiến độ mới và các quy ước phát sinh trong quá trình làm DCKLTTQ-67/68.

## 2. Tiến độ Jira (project DCKLTTQ) tính đến thời điểm hiện tại

| Epic / Issue | Nội dung | Trạng thái |
|---|---|---|
| Epic 1 (DCKLTTQ-50) | Database & Kiến trúc nền tảng | Done |
| Epic 2 (DCKLTTQ-55) | Repository, Đăng nhập & Phân quyền | Done |
| **Epic 3 (DCKLTTQ-64)** | **Module lõi: CRUD, đặt phòng, check-in/check-out, sơ đồ phòng GDI+** | **Đang làm** |
| DCKLTTQ-65 | CRUD Loại phòng | Done |
| DCKLTTQ-66 | CRUD Phòng | Done |
| DCKLTTQ-67 | CRUD Khách hàng | **Done** |
| DCKLTTQ-68 | CRUD Nhân sự (kèm tạo tài khoản đăng nhập) | **Done** |
| **DCKLTTQ-69** | **Tìm phòng trống theo ngày/loại phòng/số khách** | **To Do — việc tiếp theo** |
| **DCKLTTQ-70** | **Form Đặt phòng master-detail nhiều phòng** | **To Do — việc tiếp theo** |
| DCKLTTQ-71 | Check-in bằng tra mã booking | To Do |
| DCKLTTQ-72 | Check-out và thêm dịch vụ phát sinh | To Do |
| DCKLTTQ-73 | Sơ đồ phòng vẽ bằng GDI+ | To Do |
| Epic 4 (DCKLTTQ-74) | Hóa đơn, báo cáo PDF/Excel, email, QR | To Do |
| Epic 5 (DCKLTTQ-83) | Dashboard, Kiosk, đa ngôn ngữ | To Do |
| Epic 6 (DCKLTTQ-87) | Chatbot AI (Gemini), tính năng mở rộng | To Do |
| Epic 7 (DCKLTTQ-92) | Hoàn thiện, bảo mật, publish, chuẩn bị bảo vệ | To Do |
| Bổ sung (DCKLTTQ-98) | Thiết kế giao diện (UI/UX) | To Do — làm sau Epic 3-7 |

Quy tắc đã áp dụng: chỉ chuyển issue sang **Done** sau khi đã build và kiểm chứng chạy thực tế
trên dữ liệu thật — không đánh Done chỉ vì đã viết xong code.

## 3. Mã nguồn đã viết trong DCKLTTQ-67 và DCKLTTQ-68

### Tầng BLL (HotelManagement.BLL)
- `DTOs/KhachHangDto.cs`, `DTOs/NhanSuDto.cs`, `DTOs/VaiTroLookupDto.cs`
- `Services/IKhachHangService.cs` / `KhachHangService.cs` — CRUD Khách hàng, validate: họ tên
  không rỗng, CCCD đúng 12 chữ số và UNIQUE, email bắt buộc + đúng định dạng.
- `Services/INhanSuService.cs` / `NhanSuService.cs` — CRUD Nhân sự. **Thêm mới nhân sự sẽ tạo
  kèm tài khoản đăng nhập (TaiKhoan) trong cùng một thao tác** (username, mật khẩu, vai trò),
  theo đúng luồng nghiệp vụ thực tế đã chốt lại khi làm task này (ban đầu để ngỏ ở tài liệu v4,
  nay đã quyết định gộp). Có `GetDanhSachVaiTroAsync()` để đổ ComboBox chọn vai trò. **Xóa nhân
  sự sẽ xóa luôn tài khoản gắn kèm** (thay vì chặn xóa như cách tiếp cận ban đầu), vì tài khoản
  giờ gắn liền vòng đời với nhân sự. Sửa thông tin nhân sự KHÔNG cho đổi tài khoản/mật khẩu/vai
  trò — việc đó để làm ở một chức năng riêng sau này (ví dụ gắn vào module Đăng nhập & Phân
  quyền của Epic 2), tránh gộp quá nhiều việc vào 1 CRUD đơn giản.
- `Services/IPasswordHasher.cs` / `PasswordHasher.cs` — **PLACEHOLDER băm mật khẩu (PBKDF2)**.
  ⚠️ Cần kiểm tra lại: nếu Epic 2 (màn hình đăng nhập) đã có sẵn cơ chế băm mật khẩu riêng, phải
  xóa 2 file này và dùng lại đúng cơ chế đã có — nếu băm khác thuật toán, nhân sự mới tạo sẽ
  không đăng nhập được dù đúng mật khẩu. (Việc này cần làm sớm, trước khi demo.)

### Tầng UI (HotelManagement/Forms)
- `FormKhachHang.cs` — dựng control hoàn toàn bằng code, theo đúng khuôn mẫu FormLoaiPhong/
  FormPhong, tự tạo `IServiceScope` riêng.
- `FormNhanSu.cs` — tương tự, có thêm GroupBox "Tài khoản đăng nhập": mở để nhập (tên đăng
  nhập, mật khẩu, xác nhận mật khẩu, ComboBox vai trò) khi ở chế độ **thêm mới**; tự khóa lại và
  chỉ hiển thị thông tin tài khoản hiện có (readonly) khi ở chế độ **sửa**.
- Cả hai Form cần được gắn vào menu (xem file hướng dẫn tích hợp `TichHop_DCKLTTQ-67-68.md` đã
  gửi kèm — dùng đúng tên biến `menuChinh`, không phải `menuStrip`).

### Giả định tên property entity (cần đối chiếu lại với entity thật đã scaffold)

| Entity | Property giả định |
|---|---|
| `KhachHang` | `IdKhachHang`, `HoTen`, `Cccd`, `Email`, `SoDienThoai`, `DiaChi` |
| `NhanSu` | `IdNhanSu`, `HoTen`, `Cccd`, `SoDienThoai`, `Email`, `ChucVu`, `IdTaiKhoan` (FK) |
| `TaiKhoan` | `IdTaiKhoan`, `TenDangNhap`, `MatKhauHash`, `IdVaiTro` (FK) |
| `VaiTro` | `IdVaiTro`, `TenVaiTro` |

Nếu entity thật khác tên cột, chỉ cần sửa trong Service tương ứng, không đổi cấu trúc Form.
Cũng cần đối chiếu tên method thật trên `IUnitOfWork`/`IRepository<T>` (code vừa viết giả định
dạng generic `_unitOfWork.Repository<T>()...`).

## 4. Bài học / quyết định đã chốt trong lúc làm DCKLTTQ-67/68

- Tên biến MenuStrip chính trong `FormChinh.cs` là **`menuChinh`** (nhắc lại từ v4).
- **Quyết định về tài khoản nhân sự** (vốn để ngỏ ở v4): chọn tạo tài khoản đăng nhập ngay lúc
  thêm nhân sự mới, không tách thành task riêng. Lý do: đúng luồng nghiệp vụ thực tế (có nhân
  sự thì phải đăng nhập được ngay), và tránh phải quay lại sửa `NhanSu`/`FormNhanSu` một lần
  nữa sau này.
- Vì quyết định trên, **logic xóa nhân sự cũng đổi theo**: xóa luôn `TaiKhoan` gắn kèm thay vì
  chặn xóa khi còn tài khoản (khác với bản thiết kế ban đầu ở v4).
- Cần kiểm tra kỹ cơ chế băm mật khẩu trước khi build thật (xem mục 3) — đây là rủi ro lớn nhất
  của 2 task này nếu bỏ sót.

## 5. Việc tiếp theo — DCKLTTQ-69 (Tìm phòng trống) và DCKLTTQ-70 (Form Đặt phòng)

- **DCKLTTQ-69 — Tìm phòng trống theo ngày/loại phòng/số khách**: cần join `Phong` với
  `LoaiPhong` và (khi có) bảng Đặt phòng/ChiTietDatPhong để loại các phòng đã được đặt trong
  khoảng ngày yêu cầu; lọc theo `SucChua` của loại phòng ≥ số khách nhập vào. Đây là bước chuẩn
  bị dữ liệu cho DCKLTTQ-70.
- **DCKLTTQ-70 — Form Đặt phòng master-detail nhiều phòng**: form "master" là thông tin đặt
  phòng (khách hàng, ngày nhận/trả), "detail" là danh sách nhiều phòng được chọn trong 1 lượt
  đặt. Cần tái sử dụng `IKhachHangService` (tìm/chọn khách hàng đã có) và kết quả tìm phòng
  trống từ DCKLTTQ-69.
- Cả hai đều phức tạp hơn CRUD đơn giản trước đó — có thể cần tách DCKLTTQ-69 ra làm trước, kiểm
  thử kỹ phần lọc phòng trống, rồi mới ráp vào Form ở DCKLTTQ-70, vẫn giữ nhịp 2 task/ngày nếu
  khối lượng cho phép, nếu không thì linh hoạt điều chỉnh.

## 6. Đoạn ngữ cảnh để dán vào cuộc hội thoại mới

Sao chép đoạn dưới đây (cùng với file PDF gốc `HotelManagement_NguCanh_CapNhat_v3.pdf` nếu cần)
khi bắt đầu cuộc hội thoại mới:

> Tôi đang làm đồ án môn Lập trình Windows (WinForms .NET 10 + C#, SQL Server, làm một mình)
> với đề tài Hệ thống quản lý khách sạn. Tài liệu PDF đính kèm (v3) tổng hợp ý tưởng, thiết kế
> database, kiến trúc, và tiến độ gốc. File markdown này (v5) cập nhật thêm: đã hoàn thành
> DCKLTTQ-65, 66, 67, 68 (CRUD Loại phòng/Phòng/Khách hàng/Nhân sự) trong Epic 3, theo đúng
> khuôn mẫu Form độc lập dựng bằng code + IUnitOfWork/IRepository sẵn có từ Epic 2. Riêng
> DCKLTTQ-68 đã quyết định tạo kèm tài khoản đăng nhập ngay lúc thêm nhân sự mới. Việc tiếp
> theo là DCKLTTQ-69 (Tìm phòng trống theo ngày/loại phòng/số khách) và DCKLTTQ-70 (Form Đặt
> phòng master-detail nhiều phòng). Hãy đọc kỹ cả 2 tài liệu, phản hồi bằng tiếng Việt theo
> phong cách chuyên nghiệp, và tiếp tục từ DCKLTTQ-69 theo đúng quy ước đặt tên Entity và kiến
> trúc đã có.
