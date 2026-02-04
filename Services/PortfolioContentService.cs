using System.Globalization;
using Portfolio.Models;

namespace Portfolio.Services;

public class PortfolioContentService : IPortfolioContentService
{
    private readonly IExperienceCalculator _experienceCalculator;
    private static readonly DateTime ExperienceStartDate = new(2022, 11, 1);

    public PortfolioContentService(IExperienceCalculator experienceCalculator)
    {
        _experienceCalculator = experienceCalculator;
    }

    public HomeIndexViewModel GetHomeIndexModel()
    {
        var yearsOfExperience = _experienceCalculator.GetYearsOfExperience(ExperienceStartDate);

        return new HomeIndexViewModel
        {
            Name = "Tomasz Kraft",
            Role = ".NET Developer",
            YearsOfExperience = yearsOfExperience,
            HeroBackgroundUrl = "/img/hero-dotnet-code.svg",
            MainTechnologies = new List<string>
            {
                "C#",
                ".NET",
                "LINQ",
                "ASP.NET Core",
                "REST",
                "Entity Framework Core",
                "Autofac",
                "xUnit",
                "Angular (SPA)",
                "TypeScript",
                "WPF + MVVM",
                "DevExpress",
                "Docker",
                "SQL Server"
            }
        };
    }

    public AboutViewModel GetAboutModel()
    {
        var yearsOfExperience = _experienceCalculator.GetYearsOfExperience(ExperienceStartDate);
        var isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

        return new AboutViewModel
        {
            Bio = isEnglish
                ? $"I am a .NET developer with {yearsOfExperience} years of experience. I build web and desktop applications " +
                  "using .NET, C#, ASP.NET Core, WPF and EF Core. I focus on high code quality, architecture " +
                  "and good engineering practices. In my free time I am interested in photography and off-road automotive."
                : $"Jestem .NET developerem z {yearsOfExperience}-letnim doświadczeniem. Tworzę aplikacje webowe i desktopowe " +
                  "w technologii .NET, C#, ASP.NET Core, WPF oraz EF Core. Interesuje mnie wysoka jakość kodu, " +
                  "architektura i dobre praktyki. Hobbystycznie interesuję się fotografią oraz motoryzacją off-roadową.",

            Hobbies = isEnglish
                ? new List<string>
                {
                    "Photography",
                    "Off-road automotive",
                    "New technologies"
                }
                : new List<string>
                {
                    "Fotografia",
                    "Motoryzacja Off-Road",
                    "Nowe technologie"
                },

            Education = isEnglish
                ? new List<EducationItem>
                {
                    new()
                    {
                        School = "West Pomeranian University of Technology, Szczecin",
                        From = new DateTime(2018, 9, 30),
                        To = new DateTime(2020, 9, 22)
                    },
                    new()
                    {
                        School = "Adam Mickiewicz University, Poznań",
                        From = new DateTime(2020, 9, 24),
                        To = new DateTime(2023, 4, 17)
                    }
                }
                : new List<EducationItem>
                {
                    new()
                    {
                        School = "Zachodniopomorski Uniwersytet Technologiczny",
                        From = new DateTime(2018, 9, 30),
                        To = new DateTime(2020, 9, 22)
                    },
                    new()
                    {
                        School = "Uniwersytet im. Adama Mickiewicza",
                        From = new DateTime(2020, 9, 24),
                        To = new DateTime(2023, 4, 17)
                    }
                },

            Experience = new List<ExperienceItem>
            {
                new()
                {
                    Position = ".NET Developer",
                    Company = "Vectio sp. z o.o.",
                    From = new DateTime(2024, 2, 1),
                    To = null,
                    Responsibilities = isEnglish
                        ? new List<string>
                        {
                            "Building ASP.NET Core MVC and Web API applications",
                            "Working with Entity Framework Core and SQL Server",
                            "Developing WPF desktop applications with MVVM and DevExpress",
                            "System integrations and code refactoring"
                        }
                        : new List<string>
                        {
                            "Tworzenie aplikacji webowych ASP.NET Core MVC i Web API",
                            "Praca z Entity Framework Core oraz SQL Server",
                            "Aplikacje desktopowe WPF + MVVM + DevExpress",
                            "Integracje systemów, refaktoryzacja kodu"
                        }
                },
                new()
                {
                    Position = ".NET Developer",
                    Company = "Volvo Polska",
                    From = new DateTime(2022, 11, 1),
                    To = new DateTime(2023, 9, 30),
                    Responsibilities = isEnglish
                        ? new List<string>
                        {
                            "Developing internal applications in ASP.NET Core",
                            "Implementing new features and Web APIs",
                            "Working with WPF + MVVM + DevExpress",
                            "Optimizing queries and working with SQL Server"
                        }
                        : new List<string>
                        {
                            "Rozwój aplikacji wewnętrznych w ASP.NET Core",
                            "Implementacja nowych funkcji oraz API",
                            "WPF + MVVM + DevExpress",
                            "Optymalizacja zapytań, praca z SQL Server"
                        }
                }
            }
        };
    }

    public ProjectsViewModel GetProjectsModel()
    {
        return new ProjectsViewModel
        {
            Projects = new List<ProjectItem>
            {
                new()
                {
                    TitlePl = "System wspierający procesy w fabryce",
                    TitleEn = "System supporting factory processes",
                    Company = "Volvo Polska",
                    DescriptionPl =
                        "Oprogramowanie wspierające procesy w fabryce – m.in. obsługa danych produkcyjnych, " +
                        "integracje z istniejącymi systemami i raportowanie.",
                    DescriptionEn =
                        "Software supporting factory processes – including production data handling, " +
                        "systems integration and reporting.",
                    Technologies = new List<string>
                    {
                        "C#",
                        ".NET",
                        "ASP.NET Core MVC",
                        "Entity Framework",
                        "SQL Server"
                    }
                },
                new()
                {
                    TitlePl = "System do obsługi firmy logistycznej",
                    TitleEn = "Logistics management system",
                    Company = "Vectio sp. z o.o.",
                    DescriptionPl =
                        "Rozbudowany system do obsługi firmy logistycznej – zlecenia transportowe, kierowcy, flota, " +
                        "rozliczenia, raporty oraz integracje z systemami zewnętrznymi.",
                    DescriptionEn =
                        "A comprehensive system for logistics companies – transport orders, drivers, fleet, " +
                        "settlements, reporting and external systems integrations.",
                    Technologies = new List<string>
                    {
                        "C#",
                        ".NET 8",
                        "ASP.NET Core",
                        "WPF + MVVM + DevExpress",
                        "Entity Framework Core",
                        "SQL Server"
                    }
                }
            },

            Challenges = new List<ChallengeItem>
            {
                new()
                {
                    TitlePl = "Migracja z .NET Framework 4.8 do .NET 8",
                    TitleEn = "Migration from .NET Framework 4.8 to .NET 8",
                    DescriptionPl =
                        "Migracja ok. 90% systemu z .NET Framework 4.8 do .NET 8 praktycznie w pojedynkę – " +
                        "w tym dostosowanie architektury, aktualizacja bibliotek i rozwiązanie problemów zgodności.",
                    DescriptionEn =
                        "Migration of ~90% of the system from .NET Framework 4.8 to .NET 8 almost independently – " +
                        "including architecture adjustments, library updates and compatibility fixes."
                },
                new()
                {
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
                new()
                {
                    TitlePl = "End-to-end funkcjonalności",
                    TitleEn = "End-to-end features",
                    DescriptionPl =
                        "Prowadzenie funkcjonalności od rozmów z biznesem, przez projekt techniczny, " +
                        "implementację w .NET i EF Core, po wdrożenie i utrzymanie.",
                    DescriptionEn =
                        "Driving features end-to-end — from business discussions, through technical design, " +
                        "implementation in .NET and EF Core, to deployment and maintenance."
                }
            }
        };
    }

    public ContactViewModel GetContactInfo() =>
        new()
        {
            Email = "tomasz.kraft.solutions@gmail.com",
            Phone = "+48 668 005 812",
            SocialLinks = new List<SocialLink>
            {
                new() { Name = "LinkedIn", Url = "https://www.linkedin.com/in/tomek-kraft-760337252/" },
                new() { Name = "GitHub", Url = "https://github.com/tkraft644" }
            }
        };
}
