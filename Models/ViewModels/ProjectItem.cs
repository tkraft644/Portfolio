using System.Globalization;

namespace Portfolio.Models;

public class ProjectItem
{
    public string TitlePl { get; set; } = default!;
    public string TitleEn { get; set; } = default!;
    public string Company { get; set; } = default!;

    public string DescriptionPl { get; set; } = default!;
    public string DescriptionEn { get; set; } = default!;

    public IList<string> Technologies { get; set; } = new List<string>();

    public string Title =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? TitleEn : TitlePl;

    public string Description =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? DescriptionEn : DescriptionPl;
}
