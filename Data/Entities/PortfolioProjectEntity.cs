namespace Portfolio.Data.Entities;

public class PortfolioProjectEntity
{
    public int Id { get; set; }

    public int PortfolioProfileId { get; set; }
    public PortfolioProfileEntity Profile { get; set; } = default!;

    public int SortOrder { get; set; }

    public string Company { get; set; } = default!;

    public string TitlePl { get; set; } = default!;
    public string TitleEn { get; set; } = default!;

    public string DescriptionPl { get; set; } = default!;
    public string DescriptionEn { get; set; } = default!;

    public List<PortfolioProjectTechnologyEntity> Technologies { get; set; } = new();
}

