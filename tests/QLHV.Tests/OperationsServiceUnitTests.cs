using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Contracts.Results;
using QLHV.Core.Logging;
using QLHV.Core.Security;
using QLHV.Features.Catalog.Repositories.Fake;
using QLHV.Features.Catalog.Services;
using QLHV.Features.Training.Repositories.Fake;
using QLHV.Features.Training.Services;

namespace QLHV.Tests;

[TestClass]
public class OperationsServiceUnitTests
{
    private FakeTrainingStore _store = null!;
    private FakeLopHocRepository _lopHocRepo = null!;
    private FakeDangKyLopHocRepository _dangKyRepo = null!;
    private FakeLichHocRepository _lichHocRepo = null!;
    private FakeKetQuaHocTapRepository _ketQuaRepo = null!;
    private FakePhieuThuRepository _phieuThuRepo = null!;
    private FakeKhoaHocRepository _khoaHocRepo = null!;
    private FakeKhoaHocMonHocRepository _khoaHocMonHocRepo = null!;
    private FakeHocVienRepository _hocVienRepo = null!;
    private FakeMonHocRepository _monHocRepo = null!;
    private FakeGiangVienRepository _giangVienRepo = null!;
    private FakePhongHocRepository _phongHocRepo = null!;
    private FakeTaiKhoanRepository _taiKhoanRepo = null!;
    private CurrentUserService _currentUserService = null!;
    private IAppLogger _logger = null!;

    [TestInitialize]
    public void Setup()
    {
        _store = new FakeTrainingStore();
        _khoaHocRepo = new FakeKhoaHocRepository();
        _khoaHocMonHocRepo = new FakeKhoaHocMonHocRepository();
        _hocVienRepo = new FakeHocVienRepository();
        _monHocRepo = new FakeMonHocRepository();
        _giangVienRepo = new FakeGiangVienRepository();
        _phongHocRepo = new FakePhongHocRepository();
        _taiKhoanRepo = new FakeTaiKhoanRepository(new Pbkdf2PasswordHasher());

        _lopHocRepo = new FakeLopHocRepository(_store, _phongHocRepo);
        _dangKyRepo = new FakeDangKyLopHocRepository(_store);
        _lichHocRepo = new FakeLichHocRepository(_store, _monHocRepo, _giangVienRepo, _phongHocRepo, _khoaHocMonHocRepo);
        _ketQuaRepo = new FakeKetQuaHocTapRepository(_store);
        _phieuThuRepo = new FakePhieuThuRepository(_store);

        _currentUserService = new CurrentUserService(new AuthorizationService());
        _logger = new FileAppLogger();
    }

    private void SetUser(string username, VaiTro role, int? giangVienId = null)
    {
        _currentUserService.SetCurrentUser(new TaiKhoanDto
        {
            Id = 99,
            TenDangNhap = username,
            VaiTro = role,
            GiangVienId = giangVienId,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        });
    }

    // 1. Phân quyền và tạo Lớp học
    [TestMethod]
    public async Task LopHocService_Create_WithoutPermission_ReturnsForbidden()
    {
        SetUser("thungan", VaiTro.THUNGAN); // Thu ngân không có TrainingManage
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        var result = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_TEST",
            TenLopHoc = "Lớp Test",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.FORBIDDEN, result.Error?.Code);
    }

    [TestMethod]
    public async Task LopHocService_Create_Valid_Admin_Success()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        var result = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_NEW_01",
            TenLopHoc = "Lớp Lập trình mới",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1,
            SiSoToiDa = 35
        });

        Assert.IsTrue(result.IsSuccess);
        Assert.IsGreaterThan(0, result.Data!.Id);
        Assert.AreEqual("LH_NEW_01", result.Data.MaLopHoc);
        Assert.AreEqual(1, result.Data.GiangVienChuNhiemId);
    }

    [TestMethod]
    public async Task LopHocService_CannotReduceSiSoBelowActiveEnrollments()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        // Lop 1 có 2 học viên đang hoạt động (dk1, dk2)
        var result = await service.UpdateAsync(new LopHocDto
        {
            Id = 1,
            TenLopHoc = "Lớp Chuyên viên .NET K01",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1,
            SiSoToiDa = 1 // Nhỏ hơn 2
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, result.Error?.Code);
    }

    // 2. Xếp lớp học viên và snapshot học phí
    [TestMethod]
    public async Task DangKyLopHocService_Enroll_SnapshotsTuitionFee()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new DangKyLopHocService(_dangKyRepo, _lopHocRepo, _hocVienRepo, _khoaHocRepo, _currentUserService, _logger);

        // Lop 2 thuộc Khóa học 2 (Học phí niêm yết 5.500.000)
        var result = await service.EnrollAsync(2, 2); // Xếp học viên 2 vào lớp 2

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(5500000.00m, result.Data!.HocPhiPhaiThu);
        Assert.AreEqual(5500000.00m, result.Data.ConNo);
    }

    [TestMethod]
    public async Task DangKyLopHocService_CannotCancelEnrollmentWithPaidReceipts()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new DangKyLopHocService(_dangKyRepo, _lopHocRepo, _hocVienRepo, _khoaHocRepo, _currentUserService, _logger);

        // dk1 đã nộp 8.500.000 VNĐ -> không thể hủy
        var result = await service.CancelEnrollmentAsync(1);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, result.Error?.Code);
    }

    // 3. Lịch học và kiểm tra xung đột thời gian
    [TestMethod]
    public async Task LichHocService_DetectsTimeConflict_OverlappingSchedules()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new LichHocService(_lichHocRepo, _lopHocRepo, _monHocRepo, _giangVienRepo, _phongHocRepo, _khoaHocMonHocRepo, _currentUserService, _logger);

        // Buổi học đã có: P101, ngày 2026-03-02 từ 08:00 đến 11:00
        // Thử thêm buổi học mới: P101, ngày 2026-03-02 từ 09:30 đến 11:30 (bị giao nhau)
        var result = await service.CreateAsync(new LichHocDto
        {
            LopHocId = 1,
            MonHocId = 1,
            GiangVienId = 2,
            PhongHocId = 1,
            NgayHoc = new DateTime(2026, 3, 2),
            GioBatDau = new TimeSpan(9, 30, 0),
            GioKetThuc = new TimeSpan(11, 30, 0)
        });

        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains(result.Error?.Message ?? "", "Phòng học");
    }

    // 4. Nhập điểm và phân quyền giảng viên
    [TestMethod]
    public async Task KetQuaHocTapService_GiangVien_CanOnlyGradeAssignedClasses()
    {
        // GV001 (TS. Dũng) được phân công dạy LH_NET_01 (Lop 1, Mon 1). Không dạy LH_WF_02 (Lop 2, Mon 2)
        SetUser("giangvien01", VaiTro.GIANGVIEN, giangVienId: 1);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        // Thử chấm điểm cho lớp 2 môn 2
        var result = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 3,
            LopHocId = 2,
            MonHocId = 2,
            DiemTongKet = 9.0m
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.FORBIDDEN, result.Error?.Code);
    }

    [TestMethod]
    public async Task KetQuaHocTapService_Grade_MustBeInRange0To10()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        var result = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 1,
            LopHocId = 1,
            MonHocId = 1,
            DiemTongKet = 11.5m
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, result.Error?.Code);
    }

    [TestMethod]
    public async Task KetQuaHocTapService_BatchRejectsMixedClasses()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        // Batch chứa học viên thuộc lớp 1 và học viên thuộc lớp 2
        var list = new List<KetQuaHocTapDto>
        {
            new KetQuaHocTapDto { DangKyLopHocId = 1, LopHocId = 1, MonHocId = 1, DiemTongKet = 8.0m },
            new KetQuaHocTapDto { DangKyLopHocId = 3, LopHocId = 2, MonHocId = 1, DiemTongKet = 7.0m }
        };

        var result = await service.SaveGradesBatchAsync(list);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, result.Error?.Code);
    }

    // 5. Học phí và chống thu vượt số tiền còn nợ
    [TestMethod]
    public async Task HocPhiService_Pay_Overpayment_Fails()
    {
        SetUser("thungan", VaiTro.THUNGAN);
        var service = new HocPhiService(_phieuThuRepo, _dangKyRepo, _lopHocRepo, _currentUserService, _logger);

        // dk1: Phải thu 8.500.000, đã thu đủ 8.500.000 trong seed
        var result = await service.ThuHocPhiAsync(1, 500000m, HinhThucThanhToan.TIEN_MAT);
        Assert.IsFalse(result.IsSuccess);
        StringAssert.Contains(result.Error?.Message ?? "", "học phí");

        // dk2: Phải thu 8.500.000, đã thu 0, còn nợ 8.500.000. Thử thu 9.000.000
        var resultOver = await service.ThuHocPhiAsync(2, 9000000m, HinhThucThanhToan.CHUYEN_KHOAN);
        Assert.IsFalse(resultOver.IsSuccess);
        StringAssert.Contains(resultOver.Error?.Message ?? "", "vượt quá");
    }

    [TestMethod]
    public async Task HocPhiService_CancelReceipt_RequiresReason()
    {
        SetUser("thungan", VaiTro.THUNGAN);
        var service = new HocPhiService(_phieuThuRepo, _dangKyRepo, _lopHocRepo, _currentUserService, _logger);

        var result = await service.HuyPhieuThuAsync(1, "");
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, result.Error?.Code);

        var successResult = await service.HuyPhieuThuAsync(1, "Nhập nhầm thông tin");
        Assert.IsTrue(successResult.IsSuccess);

        var pt = await _phieuThuRepo.GetByIdAsync(1);
        Assert.AreEqual(TrangThaiPhieuThu.DaHuy, pt!.TrangThai);
        Assert.AreEqual("Nhập nhầm thông tin", pt.LyDoHuy);
    }

    // 6. Kiểm tra các vai trò tra cứu dữ liệu form đúng ngữ cảnh (GIANGVIEN, THUNGAN, QUANLY)
    [TestMethod]
    public async Task GiangVien_CanLoadOnlyAssignedClassesAndSubjects()
    {
        SetUser("giangvien01", VaiTro.GIANGVIEN, giangVienId: 1);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        var lops = await service.GetLopHocOptionsAsync();
        Assert.IsTrue(lops.IsSuccess);
        Assert.HasCount(1, lops.Data!);
        Assert.AreEqual(1, lops.Data![0].Id); // Chỉ có lớp 1

        var mons = await service.GetMonHocOptionsAsync(1);
        Assert.IsTrue(mons.IsSuccess);
        Assert.HasCount(1, mons.Data!);
        Assert.AreEqual(1, mons.Data![0].Id); // Chỉ có môn 1
    }

    [TestMethod]
    public async Task ThuNgan_CanLoadClassOptionsForTuition()
    {
        SetUser("thungan", VaiTro.THUNGAN);
        var service = new HocPhiService(_phieuThuRepo, _dangKyRepo, _lopHocRepo, _currentUserService, _logger);

        var lops = await service.GetLopHocOptionsAsync();
        Assert.IsTrue(lops.IsSuccess);
        Assert.IsGreaterThan(0, lops.Data!.Count);
    }

    [TestMethod]
    public async Task QuanLy_CanLoadReportFilters()
    {
        SetUser("quanly", VaiTro.QUANLY);
        var service = new BaoCaoService(_store != null ? new FakeBaoCaoRepository(_store) : null!, _lopHocRepo, _currentUserService, _logger);

        var lops = await service.GetLopHocFilterOptionsAsync();
        Assert.IsTrue(lops.IsSuccess);
        Assert.IsGreaterThan(0, lops.Data!.Count);
    }

    // 7. Các kiểm thử bổ sung cho Hardening-02
    [TestMethod]
    public async Task KetQuaHocTapService_GiangVien_InactiveSchedule_GradingForbidden()
    {
        // GV001 dạy Lop 1 Mon 1. Giả lập lịch học bị hủy (TrangThai = NgungHoatDong)
        SetUser("giangvien01", VaiTro.GIANGVIEN, giangVienId: 1);
        foreach (var s in _store.LichHocs.Where(x => x.GiangVienId == 1))
        {
            s.TrangThai = TrangThaiHoatDong.NgungHoatDong;
        }

        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        var result = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 1,
            LopHocId = 1,
            MonHocId = 1,
            DiemTongKet = 8.5m
        });

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.FORBIDDEN, result.Error?.Code);
    }

    [TestMethod]
    public async Task KetQuaHocTapService_CrossClassInjection_OverridesToActualClass()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        // UI cố tình gửi LopHocId = 999 nhưng DangKyLopHocId = 1 (thực chất thuộc lớp 1)
        var dto = new KetQuaHocTapDto
        {
            DangKyLopHocId = 1,
            LopHocId = 999,
            MonHocId = 1,
            DiemTongKet = 7.5m
        };

        var result = await service.SaveGradeAsync(dto);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, dto.LopHocId, "Service phải ghi đè LopHocId theo bản ghi DangKy thật.");
    }

    [TestMethod]
    public async Task KetQuaHocTapService_BlankVsZeroGrade_HandledCorrectly()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new KetQuaHocTapService(_ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        // 1. Điểm 0.00 là hợp lệ
        var res0 = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 2,
            LopHocId = 1,
            MonHocId = 1,
            DiemTongKet = 0.00m
        });
        Assert.IsTrue(res0.IsSuccess);
        Assert.AreEqual(0.00m, res0.Data!.DiemTongKet);

        var inRepo = await _ketQuaRepo.GetByDangKyAndMonAsync(2, 1);
        Assert.IsNotNull(inRepo);
        Assert.AreEqual(0.00m, inRepo.DiemTongKet);

        // 2. Điểm null (blank) -> Xóa bản ghi điểm
        var resNull = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 2,
            LopHocId = 1,
            MonHocId = 1,
            DiemTongKet = null
        });
        Assert.IsTrue(resNull.IsSuccess);

        var inRepoAfter = await _ketQuaRepo.GetByDangKyAndMonAsync(2, 1);
        Assert.IsNull(inRepoAfter, "Xóa điểm trắng phải xóa bản ghi điểm trong repository.");
    }

    [TestMethod]
    public async Task DangKyLopHocService_EnrollValidations_FakeAndSqlParity()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new DangKyLopHocService(_dangKyRepo, _lopHocRepo, _hocVienRepo, _khoaHocRepo, _currentUserService, _logger);

        // Học viên không tồn tại
        var resNotFound = await service.EnrollAsync(1, 99999);
        Assert.IsFalse(resNotFound.IsSuccess);
        Assert.AreEqual(ErrorCodes.NOT_FOUND, resNotFound.Error?.Code);

        // Học viên ngừng hoạt động
        var hvInactive = await _hocVienRepo.AddAsync(new HocVienDto
        {
            MaHocVien = "HV_INACT",
            HoTen = "Học viên ngừng",
            TrangThai = TrangThaiHoatDong.NgungHoatDong
        });
        var resInactive = await service.EnrollAsync(1, hvInactive.Id);
        Assert.IsFalse(resInactive.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resInactive.Error?.Code);

        // Học viên thật hợp lệ -> Fake repo phải lưu đúng tên, mã thật
        var hvReal = await _hocVienRepo.AddAsync(new HocVienDto
        {
            MaHocVien = "HV_REAL_01",
            HoTen = "Trần Thật",
            DienThoai = "0999888777",
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });
        var resReal = await service.EnrollAsync(2, hvReal.Id);
        Assert.IsTrue(resReal.IsSuccess);
        Assert.AreEqual("HV_REAL_01", resReal.Data!.MaHocVien);
        Assert.AreEqual("Trần Thật", resReal.Data.HoTenHocVien);
        Assert.AreEqual("0999888777", resReal.Data.DienThoaiHocVien);
    }

    [TestMethod]
    public async Task LichHocService_Validations_SubjectMustBeInCourse_AndDateRange()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new LichHocService(_lichHocRepo, _lopHocRepo, _monHocRepo, _giangVienRepo, _phongHocRepo, _khoaHocMonHocRepo, _currentUserService, _logger);

        // Lop 2 (KhoaHocId = 2) chỉ có môn 1 và 2. Thử xếp môn 3 (Quản trị CSDL) vào lớp 2 -> Vi phạm nghiệp vụ
        var resMonInvalid = await service.CreateAsync(new LichHocDto
        {
            LopHocId = 2,
            MonHocId = 3,
            GiangVienId = 1,
            PhongHocId = 1,
            NgayHoc = new DateTime(2026, 4, 1),
            GioBatDau = new TimeSpan(8, 0, 0),
            GioKetThuc = new TimeSpan(11, 0, 0)
        });
        Assert.IsFalse(resMonInvalid.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resMonInvalid.Error?.Code);

        // Ngày học trước ngày bắt đầu của lớp (2026-03-01)
        var resDateBefore = await service.CreateAsync(new LichHocDto
        {
            LopHocId = 1,
            MonHocId = 1,
            GiangVienId = 1,
            PhongHocId = 1,
            NgayHoc = new DateTime(2026, 1, 1),
            GioBatDau = new TimeSpan(8, 0, 0),
            GioKetThuc = new TimeSpan(11, 0, 0)
        });
        Assert.IsFalse(resDateBefore.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, resDateBefore.Error?.Code);
    }

    [TestMethod]
    public async Task LopHocService_Deactivate_FailsWhenActiveScheduleExists()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        // Lop 1 có lịch học active trong seed
        var res = await service.DeactivateAsync(1);
        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, res.Error?.Code);
    }

    [TestMethod]
    public async Task HocPhiService_ThuHocPhi_FailsWhenEnrollmentCancelled()
    {
        SetUser("thungan", VaiTro.THUNGAN);
        var service = new HocPhiService(_phieuThuRepo, _dangKyRepo, _lopHocRepo, _currentUserService, _logger);

        // Hủy đăng ký dk2 (chưa đóng tiền nên hủy được)
        var dk2 = _store.DangKyLopHocs.First(x => x.Id == 2);
        dk2.TrangThai = TrangThaiHoatDong.NgungHoatDong;

        var result = await service.ThuHocPhiAsync(2, 1000000m, HinhThucThanhToan.TIEN_MAT);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, result.Error?.Code);
    }

    [TestMethod]
    public async Task BaoCao_FakeReport_MatchesSqlSemantics_DynamicCourseSubjectMatrix()
    {
        var repo = new FakeBaoCaoRepository(_store, _khoaHocMonHocRepo, _monHocRepo);

        // Báo cáo điểm lớp 1: phải có đầy đủ học viên hoạt động nhân với các môn của khóa học
        var gradesReport = await repo.GetBaoCaoKetQuaHocTapAsync(1, null);
        Assert.IsNotNull(gradesReport);
        Assert.IsGreaterThan(0, gradesReport.Count);

        // Không có dòng nào hardcode môn học giả không thuộc khóa học
        foreach (var item in gradesReport)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.MaMonHoc));
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.TenMonHoc));
        }

        // Tỷ lệ môn học
        var stats = await repo.GetThongKeTyLeMonHocAsync(1);
        Assert.IsNotNull(stats);
        Assert.IsGreaterThan(0, stats.Count);
    }

    [TestMethod]
    public async Task HocPhiService_CancelReceipt_ConcurrentExecution_OnlyOneSucceeds()
    {
        SetUser("thungan", VaiTro.THUNGAN);
        var service = new HocPhiService(_phieuThuRepo, _dangKyRepo, _lopHocRepo, _currentUserService, _logger);

        // Tạo phiếu thu mới
        var pt = await _phieuThuRepo.AddAsync(new PhieuThuDto
        {
            MaPhieuThu = "PT_CONCURRENT",
            DangKyLopHocId = 2,
            SoTien = 1000000m,
            HinhThuc = HinhThucThanhToan.TIEN_MAT,
            NguoiThuTaiKhoanId = 1,
            TrangThai = TrangThaiPhieuThu.HieuLuc
        });

        var task1 = Task.Run(() => service.HuyPhieuThuAsync(pt.Id, "Hủy luồng 1"));
        var task2 = Task.Run(() => service.HuyPhieuThuAsync(pt.Id, "Hủy luồng 2"));

        var results = await Task.WhenAll(task1, task2);
        int successCount = results.Count(r => r.IsSuccess);

        Assert.AreEqual(1, successCount, "Chỉ 1 luồng hủy phiếu thu thành công trong Fake mode.");
    }

    [TestMethod]
    public async Task FakeBaoCaoRepository_ReturnsAccurateRealData_AndNullVsZeroGrades()
    {
        var repo = new FakeBaoCaoRepository(_store, _khoaHocMonHocRepo, _monHocRepo, _hocVienRepo, _khoaHocRepo);

        // 1. Kiểm tra Báo cáo học viên theo lớp: tên khóa, ngày sinh, email thật
        var hocVienReport = await repo.GetBaoCaoHocVienTheoLopAsync(1);
        Assert.IsNotNull(hocVienReport);
        Assert.HasCount(2, hocVienReport, "Lớp 1 mặc định có 2 học viên trong Fake store.");

        var hv1 = hocVienReport.First(x => x.MaHocVien == "HV001");
        Assert.AreEqual("Chuyên viên Lập trình .NET Toàn diện", hv1.TenKhoaHoc, "Tên khóa học phải lấy từ khóa học thật.");
        Assert.AreEqual(new DateTime(2003, 5, 12), hv1.NgaySinh, "Ngày sinh phải lấy từ hồ sơ học viên thật.");
        Assert.AreEqual("an.nguyen@example.com", hv1.Email, "Email phải lấy từ hồ sơ học viên thật.");
        Assert.AreNotEqual("Khóa đào tạo tiêu chuẩn", hv1.TenKhoaHoc, "Không được dùng fallback cứng.");

        // 2. Kiểm tra Kết quả học tập phân biệt null vs 0.00
        // Trong seed: dk1-mh1 có 8.50, dk1-mh2 có 7.00; dk2-mh1 có null (chưa nhập)
        // Ta thêm một bản ghi điểm 0.00 cho dk2-mh2
        lock (_store.SyncLock)
        {
            _store.KetQuaHocTaps.Add(new KetQuaHocTapDto
            {
                Id = 999,
                DangKyLopHocId = 2, // dk2
                LopHocId = 1,
                MonHocId = 2,
                DiemTongKet = 0.00m
            });
        }

        var kqReport = await repo.GetBaoCaoKetQuaHocTapAsync(1, null);
        Assert.IsNotNull(kqReport);
        Assert.HasCount(6, kqReport, "Lớp 1 có 2 học viên * 3 môn = 6 dòng ma trận.");

        var itemZero = kqReport.First(x => x.MaHocVien == "HV002" && x.MaMonHoc == "MH02");
        Assert.IsTrue(itemZero.DiemTongKet.HasValue, "Điểm 0.00 phải có giá trị (HasValue = true).");
        Assert.AreEqual(0.00m, itemZero.DiemTongKet.Value, "Điểm 0.00 phải được bảo toàn.");
        Assert.IsFalse(itemZero.Dat, "0.00 điểm là không đạt.");

        var itemNull = kqReport.First(x => x.MaHocVien == "HV002" && x.MaMonHoc == "MH03");
        Assert.IsFalse(itemNull.DiemTongKet.HasValue, "Môn chưa nhập điểm phải có DiemTongKet là null.");
        Assert.AreEqual("Chưa nhập", itemNull.KetQua);

        // 3. Thống kê tỷ lệ môn học
        var stats = await repo.GetThongKeTyLeMonHocAsync(1);
        Assert.IsNotNull(stats);
        Assert.HasCount(3, stats);

        var stat1 = stats.First(x => x.MaMonHoc == "MH01");
        Assert.AreEqual(2, stat1.TongSoHocVien);
        Assert.AreEqual(2, stat1.SoLuongDaCham);
        Assert.AreEqual(0, stat1.SoLuongChuaNhap);
        Assert.AreEqual(1, stat1.SoLuongDat); // 8.50 đạt, 4.50 không đạt

        var stat2 = stats.First(x => x.MaMonHoc == "MH02");
        Assert.AreEqual(2, stat2.TongSoHocVien);
        Assert.AreEqual(1, stat2.SoLuongDaCham);
        Assert.AreEqual(1, stat2.SoLuongChuaNhap);
        Assert.AreEqual(0, stat2.SoLuongDat); // 0.00 không đạt
    }

    [TestMethod]
    public async Task KetQuaHocTapService_BlankSemantics_ZeroGrade_AndEmptyBatchExecution()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new KetQuaHocTapService(
            _ketQuaRepo, _dangKyRepo, _lopHocRepo, _monHocRepo, _khoaHocMonHocRepo, _lichHocRepo, _currentUserService, _logger);

        // 1. Nhập điểm 0.00
        var saveZeroRes = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 1,
            MonHocId = 1,
            DiemTongKet = 0.00m
        });
        Assert.IsTrue(saveZeroRes.IsSuccess);
        Assert.AreEqual(0.00m, saveZeroRes.Data!.DiemTongKet);

        var checkZero = await _ketQuaRepo.GetByDangKyAndMonAsync(1, 1);
        Assert.IsNotNull(checkZero);
        Assert.AreEqual(0.00m, checkZero.DiemTongKet);

        // 2. Chuyển điểm thành blank (null) -> xóa điểm khỏi DB
        var saveBlankRes = await service.SaveGradeAsync(new KetQuaHocTapDto
        {
            DangKyLopHocId = 1,
            MonHocId = 1,
            DiemTongKet = null
        });
        Assert.IsTrue(saveBlankRes.IsSuccess);
        var checkBlank = await _ketQuaRepo.GetByDangKyAndMonAsync(1, 1);
        Assert.IsNull(checkBlank, "Điểm null (blank) phải xóa bản ghi điểm.");

        // 3. Batch toàn bộ blank: Tạo 2 điểm trước rồi xóa bằng batch blank
        await service.SaveGradeAsync(new KetQuaHocTapDto { DangKyLopHocId = 1, MonHocId = 2, DiemTongKet = 7.0m });
        await service.SaveGradeAsync(new KetQuaHocTapDto { DangKyLopHocId = 2, MonHocId = 2, DiemTongKet = 6.0m });

        var batchBlankRes = await service.SaveGradesBatchAsync(new List<KetQuaHocTapDto>
        {
            new KetQuaHocTapDto { DangKyLopHocId = 1, MonHocId = 2, DiemTongKet = null },
            new KetQuaHocTapDto { DangKyLopHocId = 2, MonHocId = 2, DiemTongKet = null }
        });
        Assert.IsTrue(batchBlankRes.IsSuccess);
        Assert.AreEqual(2, batchBlankRes.Data);

        var checkB1 = await _ketQuaRepo.GetByDangKyAndMonAsync(1, 2);
        var checkB2 = await _ketQuaRepo.GetByDangKyAndMonAsync(2, 2);
        Assert.IsNull(checkB1);
        Assert.IsNull(checkB2);
    }

    [TestMethod]
    public async Task LopHocService_Deactivate_OnlyWhenNoActiveChildren()
    {
        SetUser("daotao", VaiTro.DAOTAO);
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        // Tạo 1 lớp mới hoàn toàn trống
        var createRes = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_EMPTY",
            TenLopHoc = "Lớp học trống",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1,
            NgayBatDau = DateTime.Today,
            SiSoToiDa = 20
        });
        Assert.IsTrue(createRes.IsSuccess);
        int emptyLopId = createRes.Data!.Id;

        // 1. Lớp không có học viên và không có lịch -> Ngừng thành công
        var deactEmptyRes = await service.DeactivateAsync(emptyLopId);
        Assert.IsTrue(deactEmptyRes.IsSuccess, "Lớp không có học viên/lịch phải ngừng hoạt động thành công.");

        // 2. Lớp có học viên active -> Bị chặn
        // Lớp 1 trong seed đang có 2 học viên active
        var deactWithStudentsRes = await service.DeactivateAsync(1);
        Assert.IsFalse(deactWithStudentsRes.IsSuccess, "Lớp có học viên đang học phải bị từ chối ngừng hoạt động.");
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, deactWithStudentsRes.Error?.Code);

        // 3. Lớp không có học viên nhưng có lịch active -> Bị chặn
        // Tạo lớp mới, không thêm học viên nào nhưng thêm 1 lịch active
        var createLopWithSchRes = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_SCH_ONLY",
            TenLopHoc = "Lớp chỉ có lịch",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1,
            NgayBatDau = DateTime.Today,
            SiSoToiDa = 20
        });
        Assert.IsTrue(createLopWithSchRes.IsSuccess);
        int schOnlyLopId = createLopWithSchRes.Data!.Id;

        var addSchRes = await _lichHocRepo.CreateAtomicAsync(new LichHocDto
        {
            LopHocId = schOnlyLopId,
            MonHocId = 1,
            GiangVienId = 1,
            PhongHocId = 1,
            NgayHoc = DateTime.Today.AddDays(1),
            GioBatDau = new TimeSpan(8, 0, 0),
            GioKetThuc = new TimeSpan(11, 0, 0),
            NoiDung = "Buổi 1"
        });
        Assert.IsTrue(addSchRes.Success);

        var deactWithSchRes = await service.DeactivateAsync(schOnlyLopId);
        Assert.IsFalse(deactWithSchRes.IsSuccess, "Lớp không có học viên nhưng còn lịch active phải bị từ chối ngừng hoạt động.");
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, deactWithSchRes.Error?.Code);
    }

    [TestMethod]
    public async Task FakeLichHoc_AtomicInvariants_EnforcesActiveEntities_AndCapacity()
    {
        // 1. Tạo lịch với phòng có sức chứa nhỏ hơn sĩ số tối đa của lớp (Lớp 1 sĩ số tối đa 30)
        var smallRoom = await _phongHocRepo.AddAsync(new PhongHocDto
        {
            MaPhong = "P_TINY",
            TenPhong = "Phòng siêu nhỏ",
            SucChua = 15,
            TrangThai = TrangThaiHoatDong.DangHoatDong
        });

        var resSmallRoom = await _lichHocRepo.CreateAtomicAsync(new LichHocDto
        {
            LopHocId = 1,
            MonHocId = 1,
            GiangVienId = 1,
            PhongHocId = smallRoom.Id,
            NgayHoc = new DateTime(2026, 3, 20),
            GioBatDau = new TimeSpan(13, 0, 0),
            GioKetThuc = new TimeSpan(16, 0, 0),
            NoiDung = "Phòng nhỏ"
        });
        Assert.IsFalse(resSmallRoom.Success, "Sức chứa phòng học nhỏ hơn sĩ số lớp phải bị từ chối.");
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resSmallRoom.ErrorCode);

        // 2. Tạo lịch với môn không thuộc khóa học của lớp (Môn 3 không thuộc Khóa 2 của Lớp 2)
        var resWrongSubject = await _lichHocRepo.CreateAtomicAsync(new LichHocDto
        {
            LopHocId = 2,
            MonHocId = 3,
            GiangVienId = 1,
            PhongHocId = 1,
            NgayHoc = new DateTime(2026, 4, 15),
            GioBatDau = new TimeSpan(13, 0, 0),
            GioKetThuc = new TimeSpan(16, 0, 0),
            NoiDung = "Môn sai"
        });
        Assert.IsFalse(resWrongSubject.Success, "Môn học không thuộc khóa của lớp phải bị từ chối.");
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resWrongSubject.ErrorCode);
    }

    // =========================================================================
    // STUDENT PROFILE EXPANSION & VALIDATION TESTS (STUDENT-PROFILE-AND-TRAINING-UI-05)
    // =========================================================================

    [TestMethod]
    public async Task HocVienService_Create_ValidFullProfile_Success()
    {
        SetUser("admin", VaiTro.ADMIN);
        var fakeTaiKhoanRepo = new FakeTaiKhoanRepository(new Pbkdf2PasswordHasher());
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, fakeTaiKhoanRepo);

        var dto = new HocVienDto
        {
            MaHocVien = "HV_PROFILE_01",
            HoTen = "Phạm Nhật Minh",
            NgaySinh = new DateTime(2000, 1, 1),
            GioiTinh = "Nam",
            DienThoai = "0988776655",
            Email = "minh.pn@example.com",
            DiaChi = "Hà Nội",
            SoDinhDanh = "001200001234",
            NgayCapGiayTo = new DateTime(2021, 1, 1),
            NoiCapGiayTo = "Cục Cảnh sát QLHC về TTXH",
            HoTenNguoiGiamHo = "Phạm Văn Cha",
            DienThoaiNguoiGiamHo = "0911223344",
            QuanHeNguoiGiamHo = "Bố",
            NhanVienPhuTrachTaiKhoanId = 2, // daotao account (active DAOTAO)
            NguonTuyenSinh = "Facebook",
            NgheNghiep = "Kỹ sư",
            DonViCongTac = "FPT Software",
            GhiChuDacBiet = "Học viên xuất sắc"
        };

        var result = await service.CreateAsync(dto);
        Assert.IsTrue(result.IsSuccess, result.Message);
        Assert.IsGreaterThan(0, result.Data!.Id);

        // Fetch back and assert masked SoDinhDanh
        var fetched = await _hocVienRepo.GetByIdAsync(result.Data.Id);
        Assert.IsNotNull(fetched);
        Assert.AreEqual("001200001234", fetched.SoDinhDanh);
        Assert.AreEqual("********1234", fetched.SoDinhDanhMasked);
        Assert.AreEqual("Facebook", fetched.NguonTuyenSinh);
        Assert.AreEqual(2, fetched.NhanVienPhuTrachTaiKhoanId);
    }

    [TestMethod]
    public async Task HocVienService_SoDinhDanh_Enforces9Or12Digits()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        // 1. Invalid: 10 digits
        var res10 = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_SDD_10",
            HoTen = "Nguyễn Văn Mười",
            SoDinhDanh = "1234567890",
            NgayCapGiayTo = DateTime.Today.AddDays(-10),
            NoiCapGiayTo = "Công an TP"
        });
        Assert.IsFalse(res10.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_FORMAT, res10.Error?.Code);

        // 2. Invalid: Non-digits
        var resChars = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_SDD_CHAR",
            HoTen = "Nguyễn Văn Chữ",
            SoDinhDanh = "00120000ABCD",
            NgayCapGiayTo = DateTime.Today.AddDays(-10),
            NoiCapGiayTo = "Công an TP"
        });
        Assert.IsFalse(resChars.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_FORMAT, resChars.Error?.Code);

        // 3. Valid: 9 digits
        var res9 = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_SDD_9",
            HoTen = "Nguyễn Văn Chín",
            SoDinhDanh = "123456789",
            NgayCapGiayTo = DateTime.Today.AddDays(-10),
            NoiCapGiayTo = "Công an TP"
        });
        Assert.IsTrue(res9.IsSuccess, res9.Message);
        Assert.AreEqual("*****6789", res9.Data!.SoDinhDanhMasked);
    }

    [TestMethod]
    public async Task HocVienService_SoDinhDanh_Duplicate_Rejected()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        // Học viên 1 đã có SoDinhDanh = "001203001234" trong seed
        var resDup = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_DUP_SDD",
            HoTen = "Trùng Định Danh",
            SoDinhDanh = "001203001234",
            NgayCapGiayTo = DateTime.Today.AddDays(-10),
            NoiCapGiayTo = "Cục Cảnh sát"
        });
        Assert.IsFalse(resDup.IsSuccess);
        Assert.AreEqual(ErrorCodes.DUPLICATE_CODE, resDup.Error?.Code);
    }

    [TestMethod]
    public async Task HocVienService_PaperworkGroup_AllOrNothing_AndDateRules()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        // 1. Chỉ có SoDinhDanh, thiếu Ngày cấp và Nơi cấp -> Bị từ chối
        var resMissing = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_MISSING_PW",
            HoTen = "Thiếu Giấy Tờ",
            SoDinhDanh = "001200009999"
        });
        Assert.IsFalse(resMissing.IsSuccess);
        Assert.AreEqual(ErrorCodes.VALIDATION_ERROR, resMissing.Error?.Code);

        // 2. Ngày cấp trong tương lai -> Bị từ chối
        var resFutureDate = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_FUTURE_PW",
            HoTen = "Ngày Cấp Tương Lai",
            SoDinhDanh = "001200009999",
            NgayCapGiayTo = DateTime.Today.AddDays(5),
            NoiCapGiayTo = "Công an TP"
        });
        Assert.IsFalse(resFutureDate.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, resFutureDate.Error?.Code);

        // 3. Ngày cấp trước ngày sinh -> Bị từ chối
        var resBeforeBirth = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_BEFORE_BIRTH",
            HoTen = "Cấp Trước Sinh",
            NgaySinh = new DateTime(2005, 1, 1),
            SoDinhDanh = "001200009999",
            NgayCapGiayTo = new DateTime(2004, 1, 1),
            NoiCapGiayTo = "Công an TP"
        });
        Assert.IsFalse(resBeforeBirth.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, resBeforeBirth.Error?.Code);
    }

    [TestMethod]
    public async Task HocVienService_Under18_RequiresGuardianInfo()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        // Học viên 15 tuổi
        var resMinor = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_MINOR_NO_G",
            HoTen = "Em Bé Thiếu Giám Hộ",
            NgaySinh = DateTime.Today.AddYears(-15)
        });
        Assert.IsFalse(resMinor.IsSuccess, "Học viên dưới 18 tuổi thiếu người giám hộ phải bị từ chối.");
        Assert.AreEqual(ErrorCodes.REQUIRED, resMinor.Error?.Code);

        // Học viên 15 tuổi có người giám hộ đầy đủ
        var resMinorWithG = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_MINOR_OK",
            HoTen = "Em Bé Có Giám Hộ",
            NgaySinh = DateTime.Today.AddYears(-15),
            HoTenNguoiGiamHo = "Mẹ Bé",
            DienThoaiNguoiGiamHo = "0987654321",
            QuanHeNguoiGiamHo = "Mẹ"
        });
        Assert.IsTrue(resMinorWithG.IsSuccess, resMinorWithG.Message);
    }

    [TestMethod]
    public async Task HocVienService_StaffInCharge_EnforcesRoleAndActiveStatus()
    {
        SetUser("admin", VaiTro.ADMIN);
        var fakeTaiKhoanRepo = new FakeTaiKhoanRepository(new Pbkdf2PasswordHasher());
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, fakeTaiKhoanRepo);

        // 1. Gán tài khoản bị khóa (id=6 bikhoa) -> Bị từ chối
        var resInactive = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_STAFF_LOCKED",
            HoTen = "Gán Nhân Viên Khóa",
            NhanVienPhuTrachTaiKhoanId = 6
        });
        Assert.IsFalse(resInactive.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resInactive.Error?.Code);

        // 2. Gán tài khoản sai vai trò (id=3 thungan, id=4 giangvien) -> Bị từ chối
        var resWrongRole = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_STAFF_WRONG_ROLE",
            HoTen = "Gán Thu Ngân Làm Phụ Trách",
            NhanVienPhuTrachTaiKhoanId = 3
        });
        Assert.IsFalse(resWrongRole.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resWrongRole.Error?.Code);

        // 3. Gán tài khoản hợp lệ (id=1 admin) -> Thành công
        var resAdminStaff = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_STAFF_OK",
            HoTen = "Gán Admin Hợp Lệ",
            NhanVienPhuTrachTaiKhoanId = 1
        });
        Assert.IsTrue(resAdminStaff.IsSuccess, resAdminStaff.Message);
    }

    [TestMethod]
    public async Task HocVienService_RecruitmentSource_ValidatesAgainstStandardList()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        // Nguồn không hợp lệ
        var resInvalid = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_SRC_INVALID",
            HoTen = "Nguồn Bậy Bạ",
            NguonTuyenSinh = "Nguồn Tự Bịa Đặt"
        });
        Assert.IsFalse(resInvalid.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_FORMAT, resInvalid.Error?.Code);

        // Nguồn chuẩn: "Tiktok"
        var resTiktok = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_SRC_TIKTOK",
            HoTen = "Nguồn Tiktok Hợp Lệ",
            NguonTuyenSinh = "Tiktok"
        });
        Assert.IsTrue(resTiktok.IsSuccess, resTiktok.Message);
    }

    [TestMethod]
    public async Task HocVienService_GhiChuDacBiet_Enforces1000CharsLimit()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new HocVienService(_hocVienRepo, _currentUserService, _logger, _taiKhoanRepo);

        var longNote = new string('A', 1001);
        var resLong = await service.CreateAsync(new HocVienDto
        {
            MaHocVien = "HV_NOTE_LONG",
            HoTen = "Ghi Chú Quá Dài",
            GhiChuDacBiet = longNote
        });
        Assert.IsFalse(resLong.IsSuccess);
        Assert.AreEqual(ErrorCodes.INVALID_RANGE, resLong.Error?.Code);
    }

    [TestMethod]
    public async Task LopHocService_GiangVienChuNhiem_MandatoryAndActive()
    {
        SetUser("admin", VaiTro.ADMIN);
        var service = new LopHocService(_lopHocRepo, _khoaHocRepo, _giangVienRepo, _dangKyRepo, _lichHocRepo, _ketQuaRepo, _currentUserService, _logger);

        // 1. Thiếu giảng viên chủ nhiệm -> Bị từ chối
        var resMissing = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_NO_ADVISOR",
            TenLopHoc = "Lớp Thiếu GVCN",
            KhoaHocId = 1,
            GiangVienChuNhiemId = null
        });
        Assert.IsFalse(resMissing.IsSuccess);
        Assert.AreEqual(ErrorCodes.REQUIRED, resMissing.Error?.Code);

        // 2. GV chủ nhiệm không tồn tại -> Bị từ chối
        var resNotExist = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_NOT_EXIST_GV",
            TenLopHoc = "Lớp GVCN Không Tồn Tại",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 999
        });
        Assert.IsFalse(resNotExist.IsSuccess);
        Assert.AreEqual(ErrorCodes.BUSINESS_RULE_VIOLATION, resNotExist.Error?.Code);

        // 3. GV chủ nhiệm hợp lệ -> Thành công và có tên GVCN
        var resOk = await service.CreateAsync(new LopHocDto
        {
            MaLopHoc = "LH_ADVISOR_OK",
            TenLopHoc = "Lớp GVCN Chuẩn",
            KhoaHocId = 1,
            GiangVienChuNhiemId = 1
        });
        Assert.IsTrue(resOk.IsSuccess, resOk.Message);
        Assert.AreEqual(1, resOk.Data!.GiangVienChuNhiemId);
        Assert.IsNotNull(resOk.Data.HoTenGiangVienChuNhiem);
    }

    [TestMethod]
    public async Task DangKyLopHoc_CourseMapping_AndMultiCourseHistory()
    {
        SetUser("admin", VaiTro.ADMIN);
        var dkService = new DangKyLopHocService(
            _dangKyRepo, _lopHocRepo, _hocVienRepo, _khoaHocRepo, _currentUserService, _logger);

        // Học viên 1 đã có đăng ký trong FakeTrainingStore seed
        var historyRes = await dkService.GetAllAsync(new DangKyLopHocFilter { HocVienId = 1 });
        Assert.IsTrue(historyRes.IsSuccess);
        Assert.IsNotEmpty(historyRes.Data!);

        foreach (var item in historyRes.Data!)
        {
            Assert.IsGreaterThan(0, item.KhoaHocId, "Mỗi lượt ghi danh phải chứa KhoaHocId hợp lệ.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.TenKhoaHoc), "Mỗi lượt ghi danh phải chứa TenKhoaHoc hợp lệ.");
        }
    }

    [TestMethod]
    public async Task FakeTrainingStore_EnrollAtomic_And_ThuHocPhiAtomic_Populate_CourseContract()
    {
        SetUser("admin", VaiTro.ADMIN);
        // Enroll student 2 into class 2 (which has KhoaHocId = 2)
        var enrollResult = await _dangKyRepo.EnrollAtomicAsync(2, 2, 5500000m, "Kiểm thử hợp đồng khóa học");
        Assert.IsTrue(enrollResult.Success);
        Assert.IsNotNull(enrollResult.Data);
        Assert.AreEqual(2, enrollResult.Data.KhoaHocId);
        Assert.IsFalse(string.IsNullOrWhiteSpace(enrollResult.Data.TenKhoaHoc));

        // Pay tuition atomically
        var payResult = await _phieuThuRepo.ThuHocPhiAtomicAsync(enrollResult.Data.Id, 1000000m, HinhThucThanhToan.TIEN_MAT, 1, "Thu đợt 1");
        Assert.IsTrue(payResult.Success);
        Assert.IsNotNull(payResult.Data);
        Assert.AreEqual(2, payResult.Data.KhoaHocId);
        Assert.IsFalse(string.IsNullOrWhiteSpace(payResult.Data.TenKhoaHoc));

        // Results also populated
        var kq = await _ketQuaRepo.GetByDangKyAndMonAsync(1, 1);
        Assert.IsNotNull(kq);
        Assert.AreEqual(1, kq.KhoaHocId);
        Assert.IsFalse(string.IsNullOrWhiteSpace(kq.TenKhoaHoc));
    }
}
