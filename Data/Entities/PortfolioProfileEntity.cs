namespace Portfolio.Data.Entities;

public class PortfolioProfileEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;

    public DateTime ExperienceStartDate { get; set; }
    public string HeroBackgroundUrl { get; set; } = default!;

    public string AboutBioTemplatePl { get; set; } = default!;
    public string AboutBioTemplateEn { get; set; } = default!;

    public string ContactEmail { get; set; } = default!;
    public string ContactPhone { get; set; } = default!;

    public List<PortfolioTechnologyEntity> Technologies { get; set; } = new();
    public List<PortfolioSocialLinkEntity> SocialLinks { get; set; } = new();
    public List<PortfolioHobbyEntity> Hobbies { get; set; } = new();
    public List<PortfolioEducationEntity> Education { get; set; } = new();
    public List<PortfolioExperienceEntity> Experience { get; set; } = new();
    public List<PortfolioProjectEntity> Projects { get; set; } = new();
    public List<PortfolioChallengeEntity> Challenges { get; set; } = new();
}

