namespace Portfolio.Models;

using System.Globalization;

public class EducationItem
{
    public string School { get; set; } = default!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }

    public string PeriodText => GetPeriodText();

    public string GetPeriodText()
    {
        var from = From.ToString("dd/MM/yyyy");
        var to = To.HasValue
            ? To.Value.ToString("dd/MM/yyyy")
            : (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? "present" : "obecnie");
        return $"{from} – {to}";
    }
}
