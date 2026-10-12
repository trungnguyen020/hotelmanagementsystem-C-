# HỆ THỐNG QUẢN LÝ KHÁCH SẠN

Tài liệu ngữ cảnh cập nhật — Đồ án môn Lập trình Windows (WinForms .NET 10 + C#)
Cập nhật sau khi hoàn thành Epic 2, tạo Epic bổ sung "Thiết kế giao diện", và tách Form1
thành FormDangNhap + FormChinh.

## Trạng thái dự án tại thời điểm tạo tài liệu

Đề tài đã được giảng viên duyệt. Đã hoàn thành trọn vẹn Epic 1 (Database & Kiến trúc nền
tảng) và Epic 2 (Repository Pattern, Đăng nhập & Phân quyền) trên Jira — bao gồm cả code lẫn
cập nhật trạng thái issue. Đã tạo thêm 1 Epic ngoài kế hoạch 7 giai đoạn gốc: "Thiết kế giao
diện (UI/UX) cho đẹp" (trạng thái To Do, sẽ làm sau khi các Epic nghiệp vụ 3-7 hoàn thiện phần
lớn chức năng). Sau khi hoàn thành Epic 2, đã tái cấu trúc tầng UI: tách Form1 (gộp chung đăng
nhập + menu) thành 2 Form độc lập FormDangNhap và FormChinh để dễ fix lỗi và chuẩn bị cho các
Form CRUD của Epic 3. Tiến độ được theo dõi trên Jira (project DCKLTTQ) qua kết nối Atlassian
MCP trực tiếp từ Claude.

## 1. Thông tin chung

| Hạng mục | Nội dung |
|---|---|
| Môn học | Lập trình Windows (WinForms .NET 10 + C#) |
| Hình thức đánh giá | Demo + Vấn đáp; thang điểm 10; độ chia nhỏ nhất 0.25 |
| Quy mô nhóm | 1 người thực hiện (cần kiểm soát khối lượng công việc) |
| Đề tài | Phần mềm quản lý khách sạn (desktop WinForms) |
| Cơ sở dữ liệu | SQL Server — database HotelManagementDB (12 bảng: 11 bảng cốt lõi + NhatKyHoatDong) |
| Quản lý tiến độ | Jira (project DCKLTTQ) — 7 Epic giai đoạn gốc + 1 Epic bổ sung (Thiết kế giao diện) |

## 2. Mô tả nghiệp vụ tổng quan

Ứng dụng desktop WinForms quản lý khách sạn, hỗ trợ lễ tân tiếp nhận đặt phòng (trực tiếp hoặc
đặt trước qua điện thoại), check-in/check-out, thanh toán; Admin theo dõi hoạt động kinh doanh.

- **Admin** — quản lý phòng/loại phòng, giá, nhân viên, phân quyền, cấu hình hệ thống, quản lý
  đặt phòng và dịch vụ, xem báo cáo doanh thu/tỷ lệ lấp đầy phòng.
- **Lễ tân (Staff)** — tiếp nhận đặt phòng, xác nhận và gửi email kèm mã booking, check-in/
  check-out bằng tra mã booking, tính hóa đơn, thêm dịch vụ đi kèm, thanh toán (tiền mặt/VNPay
  sandbox), in hóa đơn, cập nhật trạng thái phòng.
- **Khách hàng** — nhận mã booking qua email; tự tra cứu trạng thái đặt phòng bằng mã booking
  và SĐT tại màn hình Kiosk ở sảnh khách sạn.
- **Chatbot AI (Gemini API)** tại Kiosk — tư vấn phòng trống, giá, chính sách check-in/check-out
  và hủy phòng; hội thoại multi-turn, fallback khi API lỗi.

## 3. Các quyết định thiết kế đã chốt

| Vấn đề | Quyết định |
|---|---|
| Số vai trò | Admin + Lễ tân; Khách hàng không có tài khoản, chỉ tra cứu qua Kiosk |
| Đặt phòng online | Không làm web. "Đặt trước" = khách gọi điện, lễ tân nhập hệ thống, hệ thống gửi email mã booking |
| Kênh thông báo | Chỉ Email (MailKit + Gmail SMTP + App Password); bỏ SMS |
| Tra cứu tại Kiosk | Tra cứu bằng mã booking và SĐT (khuyến nghị yêu cầu nhập đồng thời cả hai) |
| Số phòng mỗi booking | Cho phép nhiều phòng (mô hình master-detail) |
| Kiến trúc | Layered Architecture + Repository Pattern + UnitOfWork + DTO — đã dựng và triển khai xong |
| NLP/ML | Không tự huấn luyện mô hình; dùng Gemini API cho chatbot |
| Quy ước đặt tên Entity | EF Core Scaffold-DbContext mặc định: property scalar ghép PascalCase không giữ dấu "_" (cột `id_datphong` → property `IdDatphong`); navigation phía có FK đặt tên `{TênCộtFK}Navigation` (`IdNhansuNavigation`, `IdVaitroNavigation`); navigation/collection phía đối diện giữ nguyên tên entity (`ChiTietDatPhongs`, `TaiKhoan`, `HoaDon`). Cột DATE map sang `DateOnly`, DATETIME2 map sang `DateTime`. |

> **Lưu ý khi tự viết code mới:** quy ước scaffold mặc định của EF Core chỉ viết hoa **chữ cái
> đầu mỗi đoạn** tách bởi dấu `_`, không viết hoa từng từ con bên trong đoạn đó. Ví dụ thực tế
> đã xuất hiện trong tài liệu: `Trangthai` (không phải `TrangThai`), `TenVaitro` (không phải
> `TenVaiTro`), `SolanSaimatkhau`. Khi đoán tên property cho entity chưa xem code thật (`KhachHang`,
> `NhanSu`, `LoaiPhong`...), nên ưu tiên kiểm tra lại theo đúng quy ước này thay vì dùng PascalCase
> "chuẩn" thông thường.

## 4. Kiến trúc phần mềm — đã dựng và triển khai

Solution gồm 4 project, tham chiếu đúng chiều UI → BLL → DAL → Models:

```
HotelManagement.sln
|-- HotelManagement (UI — WinForms: FormDangNhap = màn hình đăng nhập, FormChinh = shell menu theo vai trò)
|-- HotelManagement.BLL (Services: IAuthService/AuthService — nghiệp vụ đăng nhập, BCrypt, khóa tài khoản)
|-- HotelManagement.DAL (DbContext, Repositories: IRepository<T>, IUnitOfWork, IDatPhongRepository,
|                         ITaiKhoanRepository...)
|-- HotelManagement.Models (Entities — scaffold từ Database First, 12 bảng)
```

Entity và DbContext được sinh bằng EF Core Scaffold-DbContext (Database First) từ database đã
tạo sẵn, không viết tay. Connection string đọc từ `appsettings.json`, đăng ký DbContext +
Repository/UnitOfWork/AuthService qua Dependency Injection trong `Program.cs` (dùng
`ServiceCollection` thủ công, không phải `WebApplicationBuilder`) — đã kiểm chứng ứng dụng đọc
được dữ liệu thật từ SQL Server và đăng nhập thành công.

## 5. Thiết kế cơ sở dữ liệu

### Nhóm bảng cốt lõi (11 bảng, đã tạo từ Giai đoạn 1)

| Bảng | Vai trò |
|---|---|
| `VaiTro` | Danh mục vai trò: Admin, LeTan |
| `NhanSu` | Thông tin nhân sự (lễ tân, admin); CCCD UNIQUE, tuổi kiểm tra ở BLL |
| `TaiKhoan` | Tài khoản đăng nhập, liên kết NhanSu + VaiTro, hash mật khẩu BCrypt, `SolanSaimatkhau`, `Trangthai` (Hoạt động/Khóa) |
| `LoaiPhong` | Danh mục loại phòng: giá, sức chứa, mô tả |
| `Phong` | Phòng cụ thể, trạng thái: Trống/Đang ở/Đang dọn/Bảo trì |
| `DichVu` | Danh mục dịch vụ: minibar, giặt ủi, spa... |
| `KhachHang` | Thông tin khách, CCCD UNIQUE, email bắt buộc |
| `DatPhong` | Booking: mã booking UNIQUE, trạng thái (Đã xác nhận/Đã check-in/Đã check-out/Đã hủy), ngày nhận/trả dự kiến (DateOnly) và thực tế (DateTime) |
| `ChiTietDatPhong` | Chi tiết phòng trong 1 booking (master-detail nhiều phòng) |
| `ChiTietDichVu` | Dịch vụ phát sinh trong lưu trú, đơn giá tại thời điểm dùng |
| `HoaDon` | Hóa đơn, quan hệ 1-1 với DatPhong, tổng tiền tính tự động (computed column) |

### Bảng mở rộng mới tạo (thuộc Epic 2)

| Bảng | Vai trò |
|---|---|
| `NhatKyHoatDong` | Ghi nhật ký Đăng nhập/Đăng xuất: `id_nhatky`, `id_taikhoan` (FK), `hanhdong`, `thoigian`, `ghichu` |

Nhóm bảng mở rộng còn lại (LichSuGuiThongBao, LichSuThanhToan, LichSuChat, Quyền động) sẽ bổ
sung ở các giai đoạn sau, theo đúng tiêu chí rubric tương ứng.

## 6. Mã nguồn đã viết trong Epic 2

### Tầng DAL (HotelManagement.DAL/Repositories)

- `IRepository<T>` / `Repository<T>` — generic CRUD (GetByIdAsync, GetAllAsync, FindAsync,
  AddAsync, Update, Remove), dùng chung DbContext với UnitOfWork.
- `IUnitOfWork` / `UnitOfWork` — lazy-init các repository (PhongRepository, KhachHangRepository,
  NhanSuRepository, LoaiPhongRepository, DichVuRepository, NhatKyHoatDongRepository,
  DatPhongRepository, TaiKhoanRepository); quản lý transaction tường minh
  (`BeginTransactionAsync`/`CommitTransactionAsync`/`RollbackTransactionAsync`) — bắt buộc cho
  nghiệp vụ đặt nhiều phòng trong 1 booking (atomic).
- `IDatPhongRepository` / `DatPhongRepository` — logic quan trọng nhất hệ thống:
  `IsPhongTrongAsync` (kiểm tra giao nhau khoảng ngày bằng điều kiện
  `A.NgayNhan < B.NgayTra VÀ A.NgayTra > B.NgayNhan`), `GetPhongTrongAsync`,
  `GetByMaBookingAsync`, `GetChiTietDayDuAsync`. Chỉ 2 trạng thái "Đã xác nhận"/"Đã check-in"
  coi là đang giữ phòng.
- `ITaiKhoanRepository` / `TaiKhoanRepository` — `GetByTenDangNhapAsync`,
  `GetByTenDangNhapKemVaiTroAsync` (Include `IdNhansuNavigation` + `IdVaitroNavigation`, không
  `AsNoTracking` vì cần Update khi xác thực sai).

### Tầng BLL (HotelManagement.BLL/Services)

- `IAuthService` / `AuthService` — `DangNhapAsync`: kiểm tra tài khoản tồn tại, tài khoản bị
  khóa (`Trangthai == "Khóa"`), xác thực `BCrypt.Verify`; sai → tăng `SolanSaimatkhau`, khóa khi
  đạt ngưỡng 5 lần; đúng → reset về 0, ghi nhật ký "Đăng nhập". `DangXuatAsync`: ghi nhật ký
  "Đăng xuất". `HashMatKhau`: băm mật khẩu mới bằng BCrypt.
- Package NuGet đã cài: `BCrypt.Net-Next` (vào project HotelManagement.BLL).

### Tầng UI — đã tách thành 2 Form riêng biệt (không dùng MDI)

Ban đầu gộp chung vào Form1, sau đó tách thành FormDangNhap và FormChinh để dễ fix và chuẩn bị
cho Epic 3 (mỗi màn hình CRUD sẽ là 1 Form riêng mở từ menu của FormChinh). Cả hai Form đều
dựng control bằng code trong constructor (không qua Visual Studio Designer kéo-thả).

- **FormDangNhap** — chỉ chứa panel đăng nhập (TextBox tên đăng nhập/mật khẩu, nút Đăng nhập,
  label thông báo lỗi). Là Form khởi động của ứng dụng (`Program.cs` gọi
  `Application.Run(new FormDangNhap())`).
- **FormChinh** — nhận TaiKhoan qua constructor, chứa MenuStrip chính; mục "Quản lý nhân sự" và
  "Báo cáo doanh thu" chỉ hiện khi `IdVaitroNavigation.TenVaitro == "Admin"`. Mở Maximized ngay
  khi hiện.
- **Luồng chuyển Form**: đăng nhập thành công → `FormDangNhap.Hide()` rồi tạo và `Show()`
  FormChinh. FormChinh có sự kiện public `YeuCauDangXuat` — khi bấm "Đăng xuất", FormChinh gọi
  `IAuthService.DangXuatAsync` (ghi nhật ký), phát sự kiện này rồi tự `Close()`; FormDangNhap
  lắng nghe sự kiện đó để `Show()` lại chính mình.
- **Phân biệt Đăng xuất vs Thoát ứng dụng**: FormDangNhap cũng lắng nghe `FormClosed` của
  FormChinh — nếu lúc đó FormDangNhap vẫn chưa `Visible` (nghĩa là người dùng bấm nút X chứ
  không qua đăng xuất) thì mới `Close()` chính FormDangNhap để thoát hẳn ứng dụng.
- Mỗi thao tác đăng nhập/đăng xuất resolve `IAuthService` qua 1 scope DI riêng
  (`Program.ServiceProvider.CreateScope()`) để tránh dùng chung 1 DbContext xuyên suốt vòng đời
  ứng dụng.

**Còn nợ lại (chưa làm):** giao diện cả 2 Form hiện tại chỉ chạy đúng chức năng, chưa đẹp — đã
tạo Epic riêng "Thiết kế giao diện (UI/UX)" để làm sau khi các Epic nghiệp vụ hoàn thiện. Khi
làm Epic 3, các Form CRUD mới (Phòng, Khách hàng, Đặt phòng...) sẽ theo đúng khuôn mẫu Form
độc lập này, mở từ menu của FormChinh.

## 7. Tiến độ đã hoàn thành trên Jira (project DCKLTTQ)

### Epic 1 — Database & Kiến trúc nền tảng (DCKLTTQ-50) — Done

Thiết kế ERD, viết script SQL 11 bảng cốt lõi, dựng solution 4 project, scaffold Entity/
DbContext, cấu hình DI, kiểm chứng kết nối thực tế.

### Epic 2 — Repository, Đăng nhập & Phân quyền (DCKLTTQ-55) — Done

| Mã Jira | Nội dung | Trạng thái |
|---|---|---|
| DCKLTTQ-56 | Viết IRepository generic và Repository | Done |
| DCKLTTQ-57 | Viết IUnitOfWork quản lý transaction | Done |
| DCKLTTQ-58 | Viết IDatPhongRepository kiểm tra phòng trống theo khoảng ngày | Done |
| DCKLTTQ-59 | Viết ITaiKhoanRepository | Done |
| DCKLTTQ-60 | Đăng ký Repository và UnitOfWork vào DI container | Done |
| DCKLTTQ-61 | Viết IAuthService: hash BCrypt, giới hạn sai mật khẩu | Done |
| DCKLTTQ-62 | Form đăng nhập và ẩn/hiện menu theo vai trò | Done |
| DCKLTTQ-63 | Ghi nhật ký hoạt động khi đăng nhập/đăng xuất | Done |

### Epic bổ sung — Thiết kế giao diện (DCKLTTQ-98) — To Do

| Mã Jira | Nội dung | Trạng thái |
|---|---|---|
| DCKLTTQ-99 | Chọn bảng màu và font chữ chung cho toàn hệ thống | To Do |
| DCKLTTQ-100 | Thiết kế logo khách sạn | To Do |
| DCKLTTQ-101 | Thiết kế lại giao diện Form đăng nhập (màu sắc, bố cục, logo) | To Do |
| DCKLTTQ-102 | Chuẩn hóa style chung cho các control (Button, TextBox, DataGridView...) | To Do |
| DCKLTTQ-103 | Thiết kế giao diện màn hình chính theo vai trò Admin/LeTan | To Do |

> **Lưu ý quan trọng về Jira:** Project DCKLTTQ hiện có 2 bộ Epic 1-7 trùng lặp (ví dụ "Giai
> đoạn 2" tồn tại ở cả DCKLTTQ-7 và DCKLTTQ-55). Bộ đang hoạt động / đã liên kết đúng task con
> là bộ bắt đầu từ DCKLTTQ-50 trở đi (Epic 1 = 50, Epic 2 = 55, Epic 3 = 64, Epic 4 = 74, Epic
> 5 = 83, Epic 6 = 87, Epic 7 = 92). Bộ còn lại (DCKLTTQ-2, 7, 8-15, 16, 26, 35, 39, 44...) là
> các issue mồ côi, chưa gắn Epic nào — nhiều khả năng dư thừa từ một lần import trước đó, có
> thể cân nhắc dọn dẹp sau.

## 8. Backlog Jira — tổng quan các Epic

| Epic | Nội dung | Trạng thái |
|---|---|---|
| 1 (DCKLTTQ-50) | Database & Kiến trúc nền tảng | Done |
| 2 (DCKLTTQ-55) | Repository, Đăng nhập & Phân quyền | Done |
| 3 (DCKLTTQ-64) | Module lõi: CRUD, đặt phòng, check-in/check-out, sơ đồ phòng GDI+ | To Do — tiếp theo |
| 4 (DCKLTTQ-74) | Hóa đơn, báo cáo PDF/Excel, email, QR | To Do |
| 5 (DCKLTTQ-83) | Dashboard, Kiosk, đa ngôn ngữ | To Do |
| 6 (DCKLTTQ-87) | Chatbot AI (Gemini), tính năng mở rộng | To Do |
| 7 (DCKLTTQ-92) | Hoàn thiện, bảo mật, publish, chuẩn bị bảo vệ | To Do |
| Bổ sung (DCKLTTQ-98) | Thiết kế giao diện (UI/UX) cho đẹp | To Do — làm sau Epic 3-7 |

## 9. Việc tiếp theo — Epic 3: Module lõi nghiệp vụ

- CRUD Khách hàng, CRUD Nhân sự, CRUD Phòng/Loại phòng, CRUD Dịch vụ (dùng `IRepository<T>`
  đã có).
- Nghiệp vụ Đặt phòng: tìm phòng trống theo ngày/loại phòng (đã có
  `IDatPhongRepository.GetPhongTrongAsync`), tạo DatPhong + nhiều ChiTietDatPhong trong 1
  transaction (dùng `IUnitOfWork.BeginTransactionAsync`).
- Check-in/Check-out bằng tra mã booking (`IDatPhongRepository.GetByMaBookingAsync` /
  `GetChiTietDayDuAsync`).
- Sơ đồ phòng trực quan bằng GDI+ (vẽ trạng thái Trống/Đang ở/Đang dọn/Bảo trì theo màu).
- Import danh sách phòng từ Excel có validate (đã thấy trong backlog Jira: DCKLTTQ liên quan
  "Import danh sach phong tu Excel co validate").

## 10. Ghi chú kỹ thuật quan trọng cần nhớ

### Scaffold / EF Core

Hướng Database First: mỗi khi sửa script SQL, chạy lại lệnh scaffold với cờ `--force` để đồng
bộ entity. Không sửa tay entity đã sinh vì lần scaffold sau sẽ ghi đè. Lệnh chuẩn:

```
dotnet ef dbcontext scaffold
"Server=...;Database=HotelManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
Microsoft.EntityFrameworkCore.SqlServer --context HotelManagementDbContext --context-dir . --output-dir
../HotelManagement.Models/Entities --namespace HotelManagement.Models.Entities --context-namespace
HotelManagement.DAL --no-onconfiguring --force
```

### Dependency Injection (Program.cs)

`Program.cs` dùng `ServiceCollection` thủ công (KHÔNG phải `WebApplicationBuilder`/
`Host.CreateApplicationBuilder`): khai báo `var services = new ServiceCollection();` rồi
`services.AddScoped(...)` — không dùng cú pháp `builder.Services`. Đã đăng ký: DbContext,
`IUnitOfWork`/`UnitOfWork`, `IRepository<>`/`Repository<>` generic,
`IDatPhongRepository`/`DatPhongRepository`, `ITaiKhoanRepository`/`TaiKhoanRepository`,
`IAuthService`/`AuthService`.

### Email

Một tài khoản Gmail dùng chung của khách sạn; MailKit kết nối `smtp.gmail.com` cổng 587
StartTls, gửi bất đồng bộ; App Password 16 ký tự lưu ở User Secrets.

### Chatbot AI (Gemini API)

Đặt tại Kiosk; system prompt định hướng nghiệp vụ khách sạn; truyền kèm dữ liệu thực từ DB vào
ngữ cảnh; giữ lịch sử hội thoại multi-turn; fallback thân thiện khi API lỗi hoặc mất mạng.

## 11. Đoạn ngữ cảnh để dán vào cuộc hội thoại mới

Sao chép đoạn dưới đây (thay cho file PDF gốc) khi bắt đầu cuộc hội thoại mới:

> Tôi đang làm đồ án môn Lập trình Windows (WinForms .NET 10 + C#, SQL Server, làm một mình)
> với đề tài Hệ thống quản lý khách sạn; đề tài đã được giảng viên duyệt. Tài liệu markdown
> đính kèm tổng hợp toàn bộ ý tưởng, quyết định thiết kế, thiết kế database (12 bảng), quy ước
> đặt tên Entity do EF Core scaffold sinh ra, kiến trúc 4 tầng, và tiến độ Jira (project
> DCKLTTQ, quản lý qua Atlassian MCP connector).
>
> Đã hoàn thành và kiểm chứng: Epic 1 (database + solution 4 tầng + scaffold EF Core + kết nối
> DI) và Epic 2 trọn vẹn (IRepository/Repository generic, IUnitOfWork với transaction,
> IDatPhongRepository kiểm tra phòng trống theo khoảng ngày, ITaiKhoanRepository, IAuthService
> với BCrypt + khóa tài khoản sau 5 lần sai, ghi nhật ký NhatKyHoatDong khi đăng nhập/đăng
> xuất) — tất cả đã cập nhật Done trên Jira. Đã tạo thêm 1 Epic ngoài kế hoạch gốc: "Thiết kế
> giao diện (UI/UX)" (To Do, làm sau khi xong Epic 3-7).
>
> Sau Epic 2, đã tách Form1 thành FormDangNhap (màn hình đăng nhập) và FormChinh (shell menu
> theo vai trò) — 2 Form độc lập, không dùng MDI; FormChinh có sự kiện YeuCauDangXuat để báo
> cho FormDangNhap hiện lại khi đăng xuất, phân biệt rõ với việc đóng ứng dụng bằng nút X. Các
> Form CRUD của Epic 3 sẽ theo đúng khuôn mẫu Form độc lập này, mở từ menu của FormChinh.
>
> Việc tiếp theo: bắt đầu Epic 3 — CRUD Khách hàng/Nhân sự/Phòng/Dịch vụ (mỗi module 1 Form
> riêng), nghiệp vụ Đặt phòng (tạo DatPhong + nhiều ChiTietDatPhong trong 1 transaction qua
> IUnitOfWork), Check-in/Check-out bằng mã booking, sơ đồ phòng bằng GDI+.
>
> Hãy đọc kỹ tài liệu, phản hồi bằng tiếng Việt theo phong cách chuyên nghiệp, và bắt đầu từ
> Epic 3 theo đúng quy ước đặt tên Entity và kiến trúc đã có (dùng IUnitOfWork/IRepository sẵn
> có và khuôn mẫu Form độc lập đã tách, không viết lại từ đầu).
