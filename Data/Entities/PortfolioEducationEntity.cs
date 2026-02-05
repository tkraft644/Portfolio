namespace Portfolio.Data.Entities;

public class PortfolioEducationEntity
{
    public int Id { get; set; }

    public int PortfolioProfileId { get; set; }
    public PortfolioProfileEntity Profile { get; set; } = default!;

    public int SortOrder { get; set; }

    public string SchoolPl { get; set; } = default!;
    public string SchoolEn { get; set; } = default!;

    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}

