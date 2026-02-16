namespace Portfolio.Models;

public class AdminEducationListViewModel
{
    public IList<AdminEducationListItem> Items { get; set; } = new List<AdminEducationListItem>();
}

public class AdminEducationListItem
{
    public int Id { get; set; }
    public int SortOrder { get; set; }

    public string SchoolPl { get; set; } = string.Empty;
    public string SchoolEn { get; set; } = string.Empty;

    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}

