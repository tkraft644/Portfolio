namespace Portfolio.Data.Entities;

public class PortfolioExperienceEntity
{
    public int Id { get; set; }

    public int PortfolioProfileId { get; set; }
    public PortfolioProfileEntity Profile { get; set; } = default!;

    public int SortOrder { get; set; }

    public string Position { get; set; } = default!;
    public string Company { get; set; } = default!;

    public DateTime From { get; set; }
    public DateTime? To { get; set; }

    public List<PortfolioExperienceResponsibilityEntity> Responsibilities { get; set; } = new();
}

