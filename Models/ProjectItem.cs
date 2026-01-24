public class ProjectItem
{
    public string TitlePl { get; set; } = default!;
    public string TitleEn { get; set; } = default!;
    public string Company { get; set; } = default!;

    public string DescriptionPl { get; set; } = default!;
    public string DescriptionEn { get; set; } = default!;

    public IList<string> Technologies { get; set; } = new List<string>();
}