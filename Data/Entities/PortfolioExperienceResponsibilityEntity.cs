namespace Portfolio.Data.Entities;

public class PortfolioExperienceResponsibilityEntity
{
    public int Id { get; set; }

    public int PortfolioExperienceId { get; set; }
    public PortfolioExperienceEntity Experience { get; set; } = default!;

    public int SortOrder { get; set; }

    public string TextPl { get; set; } = default!;
    public string TextEn { get; set; } = default!;
}

