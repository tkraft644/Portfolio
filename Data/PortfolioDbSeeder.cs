using Microsoft.EntityFrameworkCore;
using Portfolio.Data.Entities;

namespace Portfolio.Data;

public sealed class PortfolioDbSeeder
{
    private readonly PortfolioDbContext _db;
    private readonly ILogger<PortfolioDbSeeder> _logger;

    public PortfolioDbSeeder(PortfolioDbContext db, ILogger<PortfolioDbSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (await _db.Profiles.AnyAsync(cancellationToken))
                {
                    return;
                }

                _db.Profiles.Add(CreateDefaultProfile());
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Seeded Portfolio database.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Database not ready yet (attempt {Attempt}/{Max}). Retrying...", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }

    private static PortfolioProfileEntity CreateDefaultProfile()
    {
        var profile = new PortfolioProfileEntity
        {
            Name = "Tomasz Kraft",
            Role = ".NET Developer",
            ExperienceStartDate = new DateTime(2022, 11, 1),
            HeroBackgroundUrl = "/img/hero-dotnet-code.svg",
            AboutBioTemplatePl =
                "Jestem .NET developerem z {0}-letnim doświadczeniem. Tworzę aplikacje webowe i desktopowe " +
                "w technologii .NET, C#, ASP.NET Core, WPF oraz EF Core. Interesuje mnie wysoka jakość kodu, " +
                "architektura i dobre praktyki. Hobbystycznie interesuję się fotografią oraz motoryzacją off-roadową.",
            AboutBioTemplateEn =
                "I am a .NET developer with {0} years of experience. I build web and desktop applications " +
                "using .NET, C#, ASP.NET Core, WPF and EF Core. I focus on high code quality, architecture " +
                "and good engineering practices. In my free time I am interested in photography and off-road automotive.",
            ContactEmail = "tomasz.kraft.solutions@gmail.com",
            ContactPhone = "+48 668 005 812"
        };

        profile.Technologies.AddRange(new[]
        {
            Tech(profile, 10, "C#"),
            Tech(profile, 20, ".NET"),
            Tech(profile, 30, "LINQ"),
            Tech(profile, 40, "ASP.NET Core"),
            Tech(profile, 50, "REST"),
            Tech(profile, 60, "Entity Framework Core"),
            Tech(profile, 70, "Autofac"),
            Tech(profile, 80, "xUnit"),
            Tech(profile, 90, "Angular (SPA)"),
            Tech(profile, 100, "TypeScript"),
            Tech(profile, 110, "WPF + MVVM"),
            Tech(profile, 120, "DevExpress"),
            Tech(profile, 130, "Docker"),
            Tech(profile, 140, "SQL Server")
        });

        profile.SocialLinks.AddRange(new[]
        {
            Social(profile, 10, "LinkedIn", "https://www.linkedin.com/in/tomasz-kraft-760337252/"),
            Social(profile, 20, "GitHub", "https://github.com/tkraft644")
        });

        profile.Hobbies.AddRange(new[]
        {
            Hobby(profile, 10, "Fotografia", "Photography"),
            Hobby(profile, 20, "Motoryzacja Off-Road", "Off-road automotive"),
            Hobby(profile, 30, "Nowe technologie", "New technologies")
        });

        profile.Education.AddRange(new[]
        {
            Education(
                profile,
                10,
                "Zachodniopomorski Uniwersytet Technologiczny",
                "West Pomeranian University of Technology, Szczecin",
                new DateTime(2018, 9, 30),
                new DateTime(2020, 9, 22)),
            Education(
                profile,
                20,
                "Uniwersytet im. Adama Mickiewicza",
                "Adam Mickiewicz University, Poznań",
                new DateTime(2020, 9, 24),
                new DateTime(2023, 4, 17))
        });

        var expVectio = new PortfolioExperienceEntity
        {
            Profile = profile,
            SortOrder = 20,
            Position = ".NET Developer",
            Company = "Vectio sp. z o.o.",
            From = new DateTime(2024, 2, 1),
            To = null
        };
        expVectio.Responsibilities.AddRange(new[]
        {
            Responsibility(expVectio, 10, "Tworzenie aplikacji webowych ASP.NET Core MVC i Web API", "Building ASP.NET Core MVC and Web API applications"),
            Responsibility(expVectio, 20, "Praca z Entity Framework Core oraz SQL Server", "Working with Entity Framework Core and SQL Server"),
            Responsibility(expVectio, 30, "Aplikacje desktopowe WPF + MVVM + DevExpress", "Developing WPF desktop applications with MVVM and DevExpress"),
            Responsibility(expVectio, 40, "Integracje systemów, refaktoryzacja kodu", "System integrations and code refactoring")
        });

        var expVolvo = new PortfolioExperienceEntity
        {
            Profile = profile,
            SortOrder = 10,
            Position = ".NET Developer",
            Company = "Volvo Polska",
            From = new DateTime(2022, 11, 1),
            To = new DateTime(2023, 9, 30)
        };
        expVolvo.Responsibilities.AddRange(new[]
        {
            Responsibility(expVolvo, 10, "Rozwój aplikacji wewnętrznych w ASP.NET Core", "Developing internal applications in ASP.NET Core"),
            Responsibility(expVolvo, 20, "Implementacja nowych funkcji oraz API", "Implementing new features and Web APIs"),
            Responsibility(expVolvo, 30, "WPF + MVVM + DevExpress", "Working with WPF + MVVM + DevExpress"),
            Responsibility(expVolvo, 40, "Optymalizacja zapytań, praca z SQL Server", "Optimizing queries and working with SQL Server")
        });

        profile.Experience.AddRange(new[] { expVolvo, expVectio });

        var projectVolvo = new PortfolioProjectEntity
        {
            Profile = profile,
            SortOrder = 10,
            Company = "Volvo Polska",
            TitlePl = "System wspierający procesy w fabryce",
            TitleEn = "System supporting factory processes",
            DescriptionPl =
                "Oprogramowanie wspierające procesy w fabryce – m.in. obsługa danych produkcyjnych, " +
                "integracje z istniejącymi systemami i raportowanie.",
            DescriptionEn =
                "Software supporting factory processes – including production data handling, " +
                "systems integration and reporting."
        };
        projectVolvo.Technologies.AddRange(new[]
        {
            ProjectTech(projectVolvo, 10, "C#"),
            ProjectTech(projectVolvo, 20, ".NET"),
            ProjectTech(projectVolvo, 30, "ASP.NET Core MVC"),
            ProjectTech(projectVolvo, 40, "Entity Framework"),
            ProjectTech(projectVolvo, 50, "SQL Server")
        });

        var projectVectio = new PortfolioProjectEntity
        {
            Profile = profile,
            SortOrder = 20,
            Company = "Vectio sp. z o.o.",
            TitlePl = "System do obsługi firmy logistycznej",
            TitleEn = "Logistics management system",
            DescriptionPl =
                "Rozbudowany system do obsługi firmy logistycznej – zlecenia transportowe, kierowcy, flota, " +
                "rozliczenia, raporty oraz integracje z systemami zewnętrznymi.",
            DescriptionEn =
                "A comprehensive system for logistics companies – transport orders, drivers, fleet, " +
                "settlements, reporting and external systems integrations."
        };
        projectVectio.Technologies.AddRange(new[]
        {
            ProjectTech(projectVectio, 10, "C#"),
            ProjectTech(projectVectio, 20, ".NET 8"),
            ProjectTech(projectVectio, 30, "ASP.NET Core"),
            ProjectTech(projectVectio, 40, "WPF + MVVM + DevExpress"),
            ProjectTech(projectVectio, 50, "Entity Framework Core"),
            ProjectTech(projectVectio, 60, "SQL Server")
        });

        profile.Projects.AddRange(new[] { projectVolvo, projectVectio });

        profile.Challenges.AddRange(new[]
        {
            new PortfolioChallengeEntity
            {
                Profile = profile,
                SortOrder = 10,
                TitlePl = "Migracja z .NET Framework 4.8 do .NET 8",
                TitleEn = "Migration from .NET Framework 4.8 to .NET 8",
                DescriptionPl =
                    "Migracja ok. 90% systemu z .NET Framework 4.8 do .NET 8 w małym zespole– " +
                    "w tym dostosowanie architektury, aktualizacja bibliotek i rozwiązanie problemów zgodności.",
                DescriptionEn =
                    "Migration of ~90% of the system from .NET Framework 4.8 to .NET 8 in small team – " +
                    "including architecture adjustments, library updates and compatibility fixes."
            },
            new PortfolioChallengeEntity
            {
                Profile = profile,
                SortOrder = 20,
                TitlePl = "Optymalizacja dużego systemu WPF + EF Core",
                TitleEn = "Optimization of a large WPF + EF Core system",
                DescriptionPl =
                    "Praca nad wydajnością i stabilnością rozbudowanej aplikacji WPF/DevExpress obsługującej " +
                    "duże ilości danych – optymalizacja zapytań EF Core, redukcja zużycia pamięci, " +
                    "poprawa responsywności UI.",
                DescriptionEn =
                    "Performance and stability improvements in a large WPF/DevExpress application processing " +
                    "large datasets — EF Core query optimization, memory reduction, UI responsiveness improvements."
            },
            new PortfolioChallengeEntity
            {
                Profile = profile,
                SortOrder = 30,
                TitlePl = "End-to-end funkcjonalności",
                TitleEn = "End-to-end features",
                DescriptionPl =
                    "Prowadzenie funkcjonalności od rozmów z biznesem, przez projekt techniczny, " +
                    "implementację w .NET i EF Core, po wdrożenie i utrzymanie.",
                DescriptionEn =
                    "Driving features end-to-end — from business discussions, through technical design, " +
                    "implementation in .NET and EF Core, to deployment and maintenance."
            }
        });

        return profile;
    }

    private static PortfolioTechnologyEntity Tech(PortfolioProfileEntity profile, int sortOrder, string name) =>
        new()
        {
            Profile = profile,
            SortOrder = sortOrder,
            Name = name
        };

    private static PortfolioSocialLinkEntity Social(PortfolioProfileEntity profile, int sortOrder, string name, string url) =>
        new()
        {
            Profile = profile,
            SortOrder = sortOrder,
            Name = name,
            Url = url
        };

    private static PortfolioHobbyEntity Hobby(PortfolioProfileEntity profile, int sortOrder, string pl, string en) =>
        new()
        {
            Profile = profile,
            SortOrder = sortOrder,
            TextPl = pl,
            TextEn = en
        };

    private static PortfolioEducationEntity Education(
        PortfolioProfileEntity profile,
        int sortOrder,
        string schoolPl,
        string schoolEn,
        DateTime from,
        DateTime to) =>
        new()
        {
            Profile = profile,
            SortOrder = sortOrder,
            SchoolPl = schoolPl,
            SchoolEn = schoolEn,
            From = from,
            To = to
        };

    private static PortfolioExperienceResponsibilityEntity Responsibility(
        PortfolioExperienceEntity exp,
        int sortOrder,
        string pl,
        string en) =>
        new()
        {
            Experience = exp,
            SortOrder = sortOrder,
            TextPl = pl,
            TextEn = en
        };

    private static PortfolioProjectTechnologyEntity ProjectTech(PortfolioProjectEntity project, int sortOrder, string name) =>
        new()
        {
            Project = project,
            SortOrder = sortOrder,
            Name = name
        };
}
