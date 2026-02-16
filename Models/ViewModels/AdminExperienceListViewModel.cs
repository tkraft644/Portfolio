namespace Portfolio.Models;

public class AdminExperienceListViewModel
{
    public IList<AdminExperienceListItem> Items { get; set; } = new List<AdminExperienceListItem>();
}

public class AdminExperienceListItem
{
    public int Id { get; set; }

    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;

    public DateTime From { get; set; }
    public DateTime? To { get; set; }

    public int ResponsibilitiesCount { get; set; }
}

