using System.Text;

namespace AltinKasap.Web.Helpers;

public static class SlugHelper
{
    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var s = input.Trim().ToLowerInvariant();
        s = s.Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
             .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u")
             .Replace("İ", "i").Replace("Ç", "c").Replace("Ğ", "g")
             .Replace("Ö", "o").Replace("Ş", "s").Replace("Ü", "u");

        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (char.IsWhiteSpace(c) || c == '-' || c == '_') sb.Append('-');
        }
        var result = sb.ToString();
        while (result.Contains("--")) result = result.Replace("--", "-");
        return result.Trim('-');
    }
}
