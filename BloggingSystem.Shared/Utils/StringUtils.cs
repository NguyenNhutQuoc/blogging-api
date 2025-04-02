using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BloggingSystem.Shared.Utils;

public class StringUtils
{
    public static string ToSlug(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";

        // Chuyển đổi dấu tiếng Việt thành không dấu
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        string str = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        
        // Chuyển sang chữ thường
        str = str.ToLower();
        
        // Loại bỏ các ký tự không mong muốn
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        
        // Loại bỏ khoảng trắng thừa
        str = Regex.Replace(str, @"\s+", " ").Trim();
        
        // Cắt chuỗi nếu dài hơn 45 ký tự
        if (str.Length > 45)
            str = str.Substring(0, 45).Trim();
        
        // Thay thế khoảng trắng bằng dấu gạch ngang
        str = Regex.Replace(str, @"\s", "-");
        
        // Loại bỏ nhiều dấu gạch ngang liên tiếp
        str = Regex.Replace(str, @"-+", "-");
        
        // Loại bỏ dấu gạch ngang ở đầu hoặc cuối nếu có
        str = str.Trim('-');
        
        return str;
    }
}