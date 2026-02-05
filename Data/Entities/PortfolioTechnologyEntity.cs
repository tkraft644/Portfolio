namespace Portfolio.Data.Entities;

public class PortfolioTechnologyEntity
{
    public int Id { get; set; }

    public int PortfolioProfileId { get; set; }
    public PortfolioProfileEntity Profile { get; set; } = default!;

    public int SortOrder { get; set; }

    public string Name { get; set; } = default!;
}

