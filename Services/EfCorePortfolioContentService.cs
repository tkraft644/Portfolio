using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Models;

namespace Portfolio.Services;

public sealed class EfCorePortfolioContentService : IPortfolioContentService
{
    private readonly PortfolioDbContext _db;
    private readonly IExperienceCalculator _experienceCalculator;

    public EfCorePortfolioContentService(PortfolioDbContext db, IExperienceCalculator experienceCalculator)
    {
        _db = db;
        _experienceCalculator = experienceCalculator;
    }

    public HomeIndexViewModel GetHomeIndexModel()
    {
        var profile = _db.Profiles
            .AsNoTracking()
            .Include(x => x.Technologies)
            .Single();

        var yearsOfExperience = _experienceCalculator.GetYearsOfExperience(profile.ExperienceStartDate);

        return new HomeIndexViewModel
        {
            Name = profile.Name,
            Role = profile.Role,
            YearsOfExperience = yearsOfExperience,
            HeroBackgroundUrl = profile.HeroBackgroundUrl,
            MainTechnologies = profile.Technologies
                .OrderBy(x => x.SortOrder)
                .Select(x => x.Name)
                .ToList()
        };
    }

    public AboutViewModel GetAboutModel()
    {
        var profile = _db.Profiles
            .AsNoTracking()
            .Include(x => x.Hobbies)
            .Include(x => x.Education)
            .Include(x => x.Experience)
            .ThenInclude(x => x.Responsibilities)
            .Single();

        var isEnglish = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";
        var yearsOfExperience = _experienceCalculator.GetYearsOfExperience(profile.ExperienceStartDate);

        var bioTemplate = isEnglish ? profile.AboutBioTemplateEn : profile.AboutBioTemplatePl;
        var bio = string.Format(System.Globalization.CultureInfo.CurrentUICulture, bioTemplate, yearsOfExperience);

        return new AboutViewModel
        {
            Bio = bio,
            Hobbies = profile.Hobbies
                .OrderBy(x => x.SortOrder)
                .Select(x => isEnglish ? x.TextEn : x.TextPl)
                .ToList(),
            Education = profile.Education
                .OrderBy(x => x.SortOrder)
                .Select(x => new EducationItem
                {
                    School = isEnglish ? x.SchoolEn : x.SchoolPl,
                    From = x.From,
                    To = x.To
                })
                .ToList(),
            Experience = profile.Experience
                .OrderByDescending(x => x.From)
                .Select(x => new ExperienceItem
                {
                    Position = x.Position,
                    Company = x.Company,
                    From = x.From,
                    To = x.To,
                    Responsibilities = x.Responsibilities
                        .OrderBy(r => r.SortOrder)
                        .Select(r => isEnglish ? r.TextEn : r.TextPl)
                        .ToList()
                })
                .ToList()
        };
    }

    public ProjectsViewModel GetProjectsModel()
    {
        var profile = _db.Profiles
            .AsNoTracking()
            .Include(x => x.Projects)
            .ThenInclude(x => x.Technologies)
            .Include(x => x.Challenges)
            .Single();

        return new ProjectsViewModel
        {
            Projects = profile.Projects
                .OrderBy(x => x.SortOrder)
                .Select(x => new ProjectItem
                {
                    Company = x.Company,
                    TitlePl = x.TitlePl,
                    TitleEn = x.TitleEn,
                    DescriptionPl = x.DescriptionPl,
                    DescriptionEn = x.DescriptionEn,
                    Technologies = x.Technologies
                        .OrderBy(t => t.SortOrder)
                        .Select(t => t.Name)
                        .ToList()
                })
                .ToList(),
            Challenges = profile.Challenges
                .OrderBy(x => x.SortOrder)
                .Select(x => new ChallengeItem
                {
                    TitlePl = x.TitlePl,
                    TitleEn = x.TitleEn,
                    DescriptionPl = x.DescriptionPl,
                    DescriptionEn = x.DescriptionEn
                })
                .ToList()
        };
    }

    public ContactViewModel GetContactInfo()
    {
        var profile = _db.Profiles
            .AsNoTracking()
            .Include(x => x.SocialLinks)
            .Single();

        return new ContactViewModel
        {
            Email = profile.ContactEmail,
            Phone = profile.ContactPhone,
            SocialLinks = profile.SocialLinks
                .OrderBy(x => x.SortOrder)
                .Select(x => new SocialLink
                {
                    Name = x.Name,
                    Url = x.Url
                })
                .ToList()
        };
    }
}

