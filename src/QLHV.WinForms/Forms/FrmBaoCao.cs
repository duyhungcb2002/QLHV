using QLHV.Contracts.Dtos;
using QLHV.Contracts.Enums;
using QLHV.Core.Security;
using QLHV.Core.Services;
using QLHV.WinForms.Configuration;

namespace QLHV.WinForms.Forms;

public partial class FrmBaoCao : Form
{
    private readonly IBaoCaoService _baoCaoService;
    private readonly IMonHocService _monHocService;
    private readonly IGiangVienService _giangVienService;
    private readonly ICurrentUserService _currentUserService;

    public FrmBaoCao(
        IBaoCaoService baoCaoService,
        ILopHocService _,
        IMonHocService monHocService,
        IGiangVienService giangVienService,
        ICurrentUserService currentUserService)
    {
        _baoCaoService = baoCaoService;
        _monHocService = monHocService;
        _giangVienService = giangVienService;
        _currentUserService = currentUserService;

        InitializeComponent();
        dgvTab1.ApplyStandardGridStyle();
        dgvTab2.ApplyStandardGridStyle();
        dgvTab3.ApplyStandardGridStyle();
        dgvTab4_ChiTiet.ApplyStandardGridStyle();
        dgvTab4_TyLe.ApplyStandardGridStyle();
        dgvTab5.ApplyStandardGridStyle();

        Load += async (_, _) =>
        {
            await LoadCombosAsync();
            await LoadTab1Async();
        };

        tabControlBaoCao.SelectedIndexChanged += async (_, _) =>
        {
            if (tabControlBaoCao.SelectedTab == tabHocVienLop) await LoadTab1Async();
            else if (tabControlBaoCao.SelectedTab == tabLichHocLop) await LoadTab2Async();
            else if (tabControlBaoCao.SelectedTab == tabPhanCongGV) await LoadTab3Async();
            else if (tabControlBaoCao.SelectedTab == tabKetQuaHocTap) await LoadTab4Async();
            else if (tabControlBaoCao.SelectedTab == tabCongNoHocPhi) await LoadTab5Async();
        };
    }

    private async Task LoadCombosAsync()
    {
        var lops = await _baoCaoService.GetLopHocFilterOptionsAsync();
        if (lops.IsSuccess && lops.Data != null)
        {
            var list1 = lops.Data.ToList();
            var list2 = lops.Data.ToList();
            var list4 = lops.Data.ToList();
            list4.Insert(0, new LopHocDto { Id = 0, TenLopHoc = "-- Tất cả các lớp --" });

            cboTab1_Lop.DataSource = list1;
            cboTab1_Lop.DisplayMember = "TenLopHoc";
            cboTab1_Lop.ValueMember = "Id";

            cboTab2_Lop.DataSource = list2;
            cboTab2_Lop.DisplayMember = "TenLopHoc";
            cboTab2_Lop.ValueMember = "Id";

            cboTab4_Lop.DataSource = list4;
            cboTab4_Lop.DisplayMember = "TenLopHoc";
            cboTab4_Lop.ValueMember = "Id";
        }
        else
        {
            cboTab1_Lop.DataSource = null;
            cboTab2_Lop.DataSource = null;
            cboTab4_Lop.DataSource = null;
        }

        var gvs = await _giangVienService.GetAllAsync(new GiangVienFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (gvs.IsSuccess && gvs.Data != null)
        {
            cboTab3_GV.DataSource = gvs.Data.ToList();
            cboTab3_GV.DisplayMember = "HoTen";
            cboTab3_GV.ValueMember = "Id";
        }
        else
        {
            cboTab3_GV.DataSource = null;
        }

        var mons = await _monHocService.GetAllAsync(new MonHocFilter { TrangThai = TrangThaiHoatDong.DangHoatDong });
        if (mons.IsSuccess && mons.Data != null)
        {
            var listM = mons.Data.ToList();
            listM.Insert(0, new MonHocDto { Id = 0, TenMonHoc = "-- Tất cả môn học --" });

            cboTab4_Mon.DataSource = listM;
            cboTab4_Mon.DisplayMember = "TenMonHoc";
            cboTab4_Mon.ValueMember = "Id";
        }
        else
        {
            cboTab4_Mon.DataSource = null;
        }
    }

    private async Task LoadTab1Async()
    {
        if (cboTab1_Lop.SelectedValue is not int lopId || lopId <= 0)
        {
            dgvTab1.DataSource = null;
            return;
        }

        var res = await _baoCaoService.GetBaoCaoHocVienTheoLopAsync(lopId);
        if (res.IsSuccess && res.Data != null)
        {
            dgvTab1.DataSource = res.Data.Select(x => new
            {
                x.MaHocVien,
                x.HoTenHocVien,
                NgaySinh = x.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                x.DienThoai,
                x.Email,
                NgayDangKy = x.NgayDangKy.ToString("dd/MM/yyyy"),
                HocPhiPhaiThu = x.HocPhiPhaiThu.ToString("N0") + " đ",
                DaThu = x.DaThu.ToString("N0") + " đ",
                ConNo = x.ConNo.ToString("N0") + " đ"
            }).ToList();
        }
        else
        {
            dgvTab1.DataSource = null;
            MessageBox.Show(res.Message ?? "Không thể tải báo cáo danh sách học viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task LoadTab2Async()
    {
        if (cboTab2_Lop.SelectedValue is not int lopId || lopId <= 0)
        {
            dgvTab2.DataSource = null;
            return;
        }

        var res = await _baoCaoService.GetBaoCaoLichHocTheoLopAsync(lopId);
        if (res.IsSuccess && res.Data != null)
        {
            dgvTab2.DataSource = res.Data.Select(x => new
            {
                NgayHoc = x.NgayHoc.ToString("dd/MM/yyyy"),
                ThoiGian = $"{x.GioBatDau:hh\\:mm} - {x.GioKetThuc:hh\\:mm}",
                x.TenMonHoc,
                x.HoTenGiangVien,
                x.TenPhong,
                x.NoiDung
            }).ToList();
        }
        else
        {
            dgvTab2.DataSource = null;
            MessageBox.Show(res.Message ?? "Không thể tải báo cáo thời khóa biểu lớp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task LoadTab3Async()
    {
        if (cboTab3_GV.SelectedValue is not int gvId || gvId <= 0)
        {
            dgvTab3.DataSource = null;
            return;
        }

        DateTime? tuNgay = null;
        DateTime? denNgay = null;

        var res = await _baoCaoService.GetBaoCaoGiangDayTheoGiangVienAsync(gvId, tuNgay, denNgay);
        if (res.IsSuccess && res.Data != null)
        {
            dgvTab3.DataSource = res.Data.Select(x => new
            {
                NgayHoc = x.NgayHoc.ToString("dd/MM/yyyy"),
                ThoiGian = $"{x.GioBatDau:hh\\:mm} - {x.GioKetThuc:hh\\:mm}",
                x.TenLopHoc,
                x.TenMonHoc,
                x.TenPhong
            }).ToList();
        }
        else
        {
            dgvTab3.DataSource = null;
            MessageBox.Show(res.Message ?? "Không thể tải báo cáo lịch giảng dạy của giảng viên.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task LoadTab4Async()
    {
        int? lopId = cboTab4_Lop.SelectedValue is int l && l > 0 ? l : null;
        int? monId = cboTab4_Mon.SelectedValue is int m && m > 0 ? m : null;

        var resDiem = await _baoCaoService.GetBaoCaoKetQuaHocTapAsync(lopId, monId);
        if (resDiem.IsSuccess && resDiem.Data != null)
        {
            dgvTab4_ChiTiet.DataSource = resDiem.Data.Select(x => new
            {
                x.TenLopHoc,
                x.TenMonHoc,
                x.MaHocVien,
                x.HoTenHocVien,
                DiemTongKet = x.DiemTongKet.HasValue ? x.DiemTongKet.Value.ToString("0.00") : "Chưa nhập",
                x.KetQua,
                x.GhiChu
            }).ToList();
        }
        else
        {
            dgvTab4_ChiTiet.DataSource = null;
            MessageBox.Show(resDiem.Message ?? "Không thể tải báo cáo kết quả học tập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        var resTyLe = await _baoCaoService.GetThongKeTyLeMonHocAsync(lopId);
        if (resTyLe.IsSuccess && resTyLe.Data != null)
        {
            dgvTab4_TyLe.DataSource = resTyLe.Data.Select(x => new
            {
                x.TenMonHoc,
                x.TongSoHocVien,
                x.SoLuongDaCham,
                x.SoLuongChuaNhap,
                x.SoLuongDat,
                x.SoLuongKhongDat,
                TyLeDatTrenDaCham = x.TyLeDatTrenDaCham.ToString("0.00") + " %",
                TyLeDatTrenTongSo = x.TyLeDatTrenTongSo.ToString("0.00") + " %"
            }).ToList();
        }
        else
        {
            dgvTab4_TyLe.DataSource = null;
        }
    }

    private async Task LoadTab5Async()
    {
        var res = await _baoCaoService.GetBaoCaoCongNoHocPhiAsync();
        if (res.IsSuccess && res.Data != null)
        {
            dgvTab5.DataSource = res.Data.Select(x => new
            {
                x.MaLopHoc,
                x.TenLopHoc,
                x.TongHocVien,
                TongHocPhi = x.TongHocPhiPhaiThu.ToString("N0") + " đ",
                TongDaThu = x.TongDaThu.ToString("N0") + " đ",
                TongConNo = x.TongConNo.ToString("N0") + " đ",
                TyLeThu = x.TyLeThu.ToString("0.00") + " %"
            }).ToList();
        }
        else
        {
            dgvTab5.DataSource = null;
            MessageBox.Show(res.Message ?? "Không thể tải báo cáo công nợ học phí.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void btnTab1_Xem_Click(object? sender, EventArgs e) => await LoadTab1Async();
    private async void btnTab2_Xem_Click(object? sender, EventArgs e) => await LoadTab2Async();
    private async void btnTab3_Xem_Click(object? sender, EventArgs e) => await LoadTab3Async();
    private async void btnTab4_Xem_Click(object? sender, EventArgs e) => await LoadTab4Async();
    private async void btnTab5_TaiLai_Click(object? sender, EventArgs e) => await LoadTab5Async();
}
