using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;

namespace QLHV.Features.Training.Repositories.Fake;

/// <summary>
/// Thread-safe shared in-memory data store cho toàn bộ nghiệp vụ Vận hành (Training) ở chế độ Fake.
/// Đảm bảo tính nhất quán dữ liệu giữa Lớp học, Xếp lớp, Lịch học, Điểm số, Phiếu thu và Báo cáo.
/// </summary>
public class FakeTrainingStore
{
    private readonly object _lock = new();

    public List<LopHocDto> LopHocs { get; } = new();
    public List<DangKyLopHocDto> DangKyLopHocs { get; } = new();
    public List<LichHocDto> LichHocs { get; } = new();
    public List<KetQuaHocTapDto> KetQuaHocTaps { get; } = new();
    public List<PhieuThuDto> PhieuThus { get; } = new();

    private int _lopHocId = 0;
    private int _dangKyId = 0;
    private int _lichHocId = 0;
    private int _ketQuaId = 0;
    private int _phieuThuId = 0;

    public FakeTrainingStore()
    {
        SeedDefaultData();
    }

    private void SeedDefaultData()
    {
        lock (_lock)
        {
            // Seed 2 LopHoc
            var lop1 = new LopHocDto
            {
                Id = ++_lopHocId,
                MaLopHoc = "LH_NET_01",
                TenLopHoc = "Lớp Chuyên viên .NET K01",
                KhoaHocId = 1,
                TenKhoaHoc = "Chuyên viên Lập trình .NET Toàn diện",
                NgayBatDau = new DateTime(2026, 3, 1),
                NgayKetThuc = new DateTime(2026, 7, 1),
                SiSoToiDa = 30,
                SiSoHienTai = 2,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                GhiChu = "Lớp học ban ngày thứ 2-4-6",
                GiangVienChuNhiemId = 1,
                HoTenGiangVienChuNhiem = "ThS. Nguyễn Văn A"
            };
            var lop2 = new LopHocDto
            {
                Id = ++_lopHocId,
                MaLopHoc = "LH_WF_02",
                TenLopHoc = "Lớp WinForms Nâng Cao K02",
                KhoaHocId = 2,
                TenKhoaHoc = "Lập trình C# WinForms Cơ bản & Nâng cao",
                NgayBatDau = new DateTime(2026, 3, 15),
                NgayKetThuc = new DateTime(2026, 6, 15),
                SiSoToiDa = 25,
                SiSoHienTai = 1,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                GhiChu = "Lớp học buổi tối thứ 3-5-7",
                GiangVienChuNhiemId = 2,
                HoTenGiangVienChuNhiem = "TS. Trần Thị B"
            };
            LopHocs.Add(lop1);
            LopHocs.Add(lop2);

            // Seed 3 DangKyLopHoc
            var dk1 = new DangKyLopHocDto
            {
                Id = ++_dangKyId,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 1,
                MaHocVien = "HV001",
                HoTenHocVien = "Nguyễn Văn An",
                DienThoaiHocVien = "0981234567",
                NgayDangKy = new DateTime(2026, 2, 15),
                HocPhiPhaiThu = 8500000.00m,
                DaThu = 8500000.00m,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                GhiChu = "Đăng ký khóa .NET K01"
            };
            var dk2 = new DangKyLopHocDto
            {
                Id = ++_dangKyId,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 2,
                MaHocVien = "HV002",
                HoTenHocVien = "Trần Thị Bình",
                DienThoaiHocVien = "0912345678",
                NgayDangKy = new DateTime(2026, 2, 18),
                HocPhiPhaiThu = 8500000.00m,
                DaThu = 0m,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                GhiChu = "Đăng ký khóa .NET K01"
            };
            var dk3 = new DangKyLopHocDto
            {
                Id = ++_dangKyId,
                LopHocId = lop2.Id,
                MaLopHoc = lop2.MaLopHoc,
                TenLopHoc = lop2.TenLopHoc,
                KhoaHocId = lop2.KhoaHocId,
                TenKhoaHoc = lop2.TenKhoaHoc,
                HocVienId = 3,
                MaHocVien = "HV003",
                HoTenHocVien = "Lê Hoàng Cường",
                DienThoaiHocVien = "0903456789",
                NgayDangKy = new DateTime(2026, 2, 20),
                HocPhiPhaiThu = 5500000.00m,
                DaThu = 5500000.00m,
                TrangThai = TrangThaiHoatDong.DangHoatDong,
                GhiChu = "Đăng ký khóa WinForms K02"
            };
            DangKyLopHocs.Add(dk1);
            DangKyLopHocs.Add(dk2);
            DangKyLopHocs.Add(dk3);

            // Seed 4 LichHoc
            LichHocs.Add(new LichHocDto
            {
                Id = ++_lichHocId,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                MonHocId = 1,
                MaMonHoc = "MH01",
                TenMonHoc = "Lập trình C# cơ bản",
                GiangVienId = 1,
                MaGiangVien = "GV001",
                HoTenGiangVien = "TS. Phạm Văn Dũng",
                PhongHocId = 1,
                MaPhong = "P101",
                TenPhong = "Phòng Lab Máy tính 01",
                NgayHoc = new DateTime(2026, 3, 2),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(11, 0, 0),
                NoiDung = "C# Cơ bản - Bài 1: Cú pháp và Kiểu dữ liệu",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            LichHocs.Add(new LichHocDto
            {
                Id = ++_lichHocId,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                MonHocId = 1,
                MaMonHoc = "MH01",
                TenMonHoc = "Lập trình C# cơ bản",
                GiangVienId = 1,
                MaGiangVien = "GV001",
                HoTenGiangVien = "TS. Phạm Văn Dũng",
                PhongHocId = 1,
                MaPhong = "P101",
                TenPhong = "Phòng Lab Máy tính 01",
                NgayHoc = new DateTime(2026, 3, 4),
                GioBatDau = new TimeSpan(8, 0, 0),
                GioKetThuc = new TimeSpan(11, 0, 0),
                NoiDung = "C# Cơ bản - Bài 2: Lập trình hướng đối tượng OOP",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            LichHocs.Add(new LichHocDto
            {
                Id = ++_lichHocId,
                LopHocId = lop2.Id,
                MaLopHoc = lop2.MaLopHoc,
                TenLopHoc = lop2.TenLopHoc,
                MonHocId = 2,
                MaMonHoc = "MH02",
                TenMonHoc = "Lập trình Windows Forms",
                GiangVienId = 2,
                MaGiangVien = "GV002",
                HoTenGiangVien = "ThS. Đỗ Thị Hương",
                PhongHocId = 2,
                MaPhong = "P102",
                TenPhong = "Phòng Lab Máy tính 02",
                NgayHoc = new DateTime(2026, 3, 17),
                GioBatDau = new TimeSpan(18, 0, 0),
                GioKetThuc = new TimeSpan(21, 0, 0),
                NoiDung = "WinForms - Bài 1: Form Layout & Controls cơ bản",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            LichHocs.Add(new LichHocDto
            {
                Id = ++_lichHocId,
                LopHocId = lop2.Id,
                MaLopHoc = lop2.MaLopHoc,
                TenLopHoc = lop2.TenLopHoc,
                MonHocId = 2,
                MaMonHoc = "MH02",
                TenMonHoc = "Lập trình Windows Forms",
                GiangVienId = 2,
                MaGiangVien = "GV002",
                HoTenGiangVien = "ThS. Đỗ Thị Hương",
                PhongHocId = 2,
                MaPhong = "P102",
                TenPhong = "Phòng Lab Máy tính 02",
                NgayHoc = new DateTime(2026, 3, 19),
                GioBatDau = new TimeSpan(18, 0, 0),
                GioKetThuc = new TimeSpan(21, 0, 0),
                NoiDung = "WinForms - Bài 2: Data Binding & DataGridView",
                TrangThai = TrangThaiHoatDong.DangHoatDong
            });

            // Seed 2 KetQuaHocTap
            KetQuaHocTaps.Add(new KetQuaHocTapDto
            {
                Id = ++_ketQuaId,
                DangKyLopHocId = dk1.Id,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 1,
                MaHocVien = "HV001",
                HoTenHocVien = "Nguyễn Văn An",
                MonHocId = 1,
                MaMonHoc = "MH01",
                TenMonHoc = "Lập trình C# cơ bản",
                DiemTongKet = 8.50m,
                NguoiCapNhatTaiKhoanId = 1,
                NguoiCapNhatTen = "admin",
                NgayCapNhat = DateTime.Now,
                GhiChu = "Hoàn thành xuất sắc bài tập lớn"
            });

            KetQuaHocTaps.Add(new KetQuaHocTapDto
            {
                Id = ++_ketQuaId,
                DangKyLopHocId = dk2.Id,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 2,
                MaHocVien = "HV002",
                HoTenHocVien = "Trần Thị Bình",
                MonHocId = 1,
                MaMonHoc = "MH01",
                TenMonHoc = "Lập trình C# cơ bản",
                DiemTongKet = 4.50m,
                NguoiCapNhatTaiKhoanId = 1,
                NguoiCapNhatTen = "admin",
                NgayCapNhat = DateTime.Now,
                GhiChu = "Cần thi lại phần OOP"
            });

            // Seed 3 PhieuThu
            PhieuThus.Add(new PhieuThuDto
            {
                Id = ++_phieuThuId,
                MaPhieuThu = "PT-20260215-001",
                DangKyLopHocId = dk1.Id,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 1,
                MaHocVien = "HV001",
                HoTenHocVien = "Nguyễn Văn An",
                NgayThu = new DateTime(2026, 2, 15, 9, 30, 0),
                SoTien = 5000000.00m,
                HinhThuc = HinhThucThanhToan.CHUYEN_KHOAN,
                NguoiThuTaiKhoanId = 3,
                NguoiThuTen = "thungan",
                GhiChu = "Nộp đợt 1 chuyển khoản qua Vietcombank",
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });

            PhieuThus.Add(new PhieuThuDto
            {
                Id = ++_phieuThuId,
                MaPhieuThu = "PT-20260301-002",
                DangKyLopHocId = dk1.Id,
                LopHocId = lop1.Id,
                MaLopHoc = lop1.MaLopHoc,
                TenLopHoc = lop1.TenLopHoc,
                KhoaHocId = lop1.KhoaHocId,
                TenKhoaHoc = lop1.TenKhoaHoc,
                HocVienId = 1,
                MaHocVien = "HV001",
                HoTenHocVien = "Nguyễn Văn An",
                NgayThu = new DateTime(2026, 3, 1, 14, 0, 0),
                SoTien = 3500000.00m,
                HinhThuc = HinhThucThanhToan.TIEN_MAT,
                NguoiThuTaiKhoanId = 3,
                NguoiThuTen = "thungan",
                GhiChu = "Nộp đợt 2 đủ học phí bằng tiền mặt",
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });

            PhieuThus.Add(new PhieuThuDto
            {
                Id = ++_phieuThuId,
                MaPhieuThu = "PT-20260220-003",
                DangKyLopHocId = dk3.Id,
                LopHocId = lop2.Id,
                MaLopHoc = lop2.MaLopHoc,
                TenLopHoc = lop2.TenLopHoc,
                KhoaHocId = lop2.KhoaHocId,
                TenKhoaHoc = lop2.TenKhoaHoc,
                HocVienId = 3,
                MaHocVien = "HV003",
                HoTenHocVien = "Lê Hoàng Cường",
                NgayThu = new DateTime(2026, 2, 20, 10, 15, 0),
                SoTien = 5500000.00m,
                HinhThuc = HinhThucThanhToan.CHUYEN_KHOAN,
                NguoiThuTaiKhoanId = 3,
                NguoiThuTen = "thungan",
                GhiChu = "Thanh toán toàn bộ học phí khóa WinForms",
                TrangThai = TrangThaiPhieuThu.HieuLuc
            });
        }
    }

    public int NextLopHocId() { lock (_lock) { return ++_lopHocId; } }
    public int NextDangKyId() { lock (_lock) { return ++_dangKyId; } }
    public int NextLichHocId() { lock (_lock) { return ++_lichHocId; } }
    public int NextKetQuaId() { lock (_lock) { return ++_ketQuaId; } }
    public int NextPhieuThuId() { lock (_lock) { return ++_phieuThuId; } }
    public object SyncLock => _lock;
}
