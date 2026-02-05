namespace Portfolio.Data.Entities;

public class PortfolioHobbyEntity
{
    public int Id { get; set; }

    public int PortfolioProfileId { get; set; }
    public PortfolioProfileEntity Profile { get; set; } = default!;

    public int SortOrder { get; set; }

    public string TextPl { get; set; } = default!;
    public string TextEn { get; set; } = default!;
}

