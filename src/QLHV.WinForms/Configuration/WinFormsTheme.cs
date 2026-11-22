using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace QLHV.WinForms.Configuration;

/// <summary>
/// Cung cấp các thiết lập giao diện chuẩn mực, đồng bộ toàn hệ thống WinForms (WinForms Expert Standard).
/// Giúp tối ưu hóa trải nghiệm người dùng, bảng màu chuyên nghiệp, chống chớp nháy (DoubleBuffering)
/// và định dạng DataGridView nhất quán.
/// </summary>
public static class WinFormsTheme
{
    // Bảng màu hiện đại (Modern Palette)
    public static readonly Color Primary = Color.FromArgb(30, 64, 175);          // Blue 700 - Chính
    public static readonly Color PrimaryHover = Color.FromArgb(29, 78, 216);     // Blue 600
    public static readonly Color Success = Color.FromArgb(16, 185, 129);         // Emerald 500
    public static readonly Color SuccessHover = Color.FromArgb(5, 150, 105);     // Emerald 600
    public static readonly Color Warning = Color.FromArgb(245, 158, 11);         // Amber 500
    public static readonly Color Danger = Color.FromArgb(220, 38, 38);           // Red 600
    public static readonly Color Secondary = Color.FromArgb(156, 163, 175);      // Gray 400
    public static readonly Color LightBg = Color.FromArgb(249, 250, 251);        // Gray 50
    public static readonly Color DarkSidebar = Color.FromArgb(17, 24, 39);       // Gray 900
    public static readonly Color DarkSidebarHover = Color.FromArgb(31, 41, 55);  // Gray 800

    // Màu DataGridView
    public static readonly Color GridHeaderBg = Color.FromArgb(243, 244, 246);    // Gray 100
    public static readonly Color GridHeaderFg = Color.FromArgb(31, 41, 55);      // Gray 800
    public static readonly Color GridRowAlternating = Color.FromArgb(249, 250, 251); // Gray 50
    public static readonly Color GridSelectionBg = Color.FromArgb(219, 234, 254);  // Blue 100
    public static readonly Color GridSelectionFg = Color.FromArgb(30, 58, 138);   // Blue 900
    public static readonly Color GridBorder = Color.FromArgb(229, 231, 235);      // Gray 200

    /// <summary>
    /// Kích hoạt DoubleBuffering cho DataGridView qua Reflection để loại bỏ chớp giật khi cuộn hoặc resize.
    /// </summary>
    public static void EnableDoubleBuffering(this Control control)
    {
        try
        {
            var prop = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            prop?.SetValue(control, true, null);
        }
        catch
        {
            // Bỏ qua nếu môi trường không cho phép reflection
        }
    }

    /// <summary>
    /// Áp dụng style giao diện chuẩn công nghiệp cho DataGridView.
    /// </summary>
    public static void ApplyStandardGridStyle(this DataGridView dgv)
    {
        dgv.EnableDoubleBuffering();
        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.None;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.GridColor = GridBorder;
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.MultiSelect = false;
        dgv.AllowUserToResizeRows = false;
        dgv.EnableHeadersVisualStyles = false;

        // Font chữ
        dgv.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);

        // Header style
        dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBg;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderFg;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        dgv.ColumnHeadersHeight = 32;

        // Row default style
        dgv.DefaultCellStyle.BackColor = Color.White;
        dgv.DefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
        dgv.DefaultCellStyle.SelectionBackColor = GridSelectionBg;
        dgv.DefaultCellStyle.SelectionForeColor = GridSelectionFg;
        dgv.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);
        dgv.RowTemplate.Height = 30;

        // Alternating row style
        dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlternating;
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(17, 24, 39);
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridSelectionBg;
        dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = GridSelectionFg;
    }

    /// <summary>
    /// Cấu hình nút bấm theo phong cách phẳng hiện đại (Flat UI).
    /// </summary>
    public static void StyleButton(this Button btn, Color backColor, Color foreColor)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = backColor;
        btn.ForeColor = foreColor;
        btn.Cursor = Cursors.Hand;
        btn.UseVisualStyleBackColor = false;
    }
}
