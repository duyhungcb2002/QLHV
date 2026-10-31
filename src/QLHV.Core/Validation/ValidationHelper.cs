using System.Text.RegularExpressions;

namespace QLHV.Core.Validation;

/// <summary>
/// Các hàm tiện ích kiểm tra tính hợp lệ của dữ liệu đầu vào
/// </summary>
public static class ValidationHelper
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^[0-9\+\-\s\(\)]{8,15}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Kiểm tra định dạng email hợp lệ
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return true; // Cho phép rỗng nếu không bắt buộc
        return EmailRegex.IsMatch(email.Trim());
    }

    /// <summary>
    /// Kiểm tra định dạng số điện thoại hợp lệ
    /// </summary>
    public static bool IsValidPhoneNumber(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return true; // Cho phép rỗng nếu không bắt buộc
        return PhoneRegex.IsMatch(phone.Trim());
    }

    /// <summary>
    /// Chuẩn hóa chuỗi (cắt khoảng trắng hai đầu, null thành rỗng)
    /// </summary>
    public static string NormalizeString(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// So sánh hai chuỗi mã không phân biệt hoa thường và bỏ khoảng trắng
    /// </summary>
    public static bool IsCodeMatch(string? code1, string? code2)
    {
        return string.Equals(code1?.Trim(), code2?.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Chuyển đổi chuỗi tiếng Việt có dấu thành không dấu để hỗ trợ tìm kiếm linh hoạt
    /// </summary>
    public static string RemoveDiacritics(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder
            .ToString()
            .Normalize(System.Text.NormalizationForm.FormC)
            .Replace('đ', 'd')
            .Replace('Đ', 'D');
    }

    /// <summary>
    /// Kiểm tra source có chứa keyword hay không (hỗ trợ cả có dấu và không dấu, không phân biệt hoa thường)
    /// </summary>
    public static bool ContainsSearch(string? source, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return true;
        if (string.IsNullOrWhiteSpace(source)) return false;

        var kw = keyword.Trim();
        if (source.Contains(kw, StringComparison.OrdinalIgnoreCase)) return true;

        var sourceNoDiacritics = RemoveDiacritics(source);
        var kwNoDiacritics = RemoveDiacritics(kw);
        return sourceNoDiacritics.Contains(kwNoDiacritics, StringComparison.OrdinalIgnoreCase);
    }
}
