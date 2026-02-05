namespace Portfolio.Data.Entities;

public class PortfolioProjectTechnologyEntity
{
    public int Id { get; set; }

    public int PortfolioProjectId { get; set; }
    public PortfolioProjectEntity Project { get; set; } = default!;

    public int SortOrder { get; set; }

    public string Name { get; set; } = default!;
}

