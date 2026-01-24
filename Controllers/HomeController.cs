using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Portfolio.Models;
using Microsoft.Extensions.Options;
using Portfolio.Models;
using System.Globalization;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly EmailSettings _emailSettings;

    public HomeController(
        ILogger<HomeController> logger,
        IWebHostEnvironment env,
        IOptions<EmailSettings> emailOptions)
    {
        _logger = logger;
        _env = env;
        _emailSettings = emailOptions.Value;
    }
    public IActionResult Index()
    {
       
        var start = new DateTime(2022, 11, 1);
        var today = DateTime.Today;

        
        int months = ((today.Year - start.Year) * 12) + today.Month - start.Month;
        if (today.Day < start.Day)
            months--; 


        double years = months / 12.0;
        
        years = Math.Round(years * 2, MidpointRounding.AwayFromZero) / 2.0;

        var model = new HomeIndexViewModel
        {
            Name = "Tomasz Kraft",
            Role = ".NET Developer",
            YearsOfExperience = years,          
            MainTechnologies = new List<string>
            {
                "C#",
                ".NET",
                "ASP.NET Core",
                "Entity Framework Core",
                "WPF + MVVM",
                "SQL Server"
            }
        };

        return View(model);
    }
    public IActionResult About()
{
    var isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

    var model = new AboutViewModel
    {
        Bio = isEnglish
            ? "I am a .NET developer with 3 years of experience. I build web and desktop applications " +
              "using .NET, C#, ASP.NET Core, WPF and EF Core. I focus on high code quality, architecture " +
              "and good engineering practices. In my free time I am interested in photography and off-road automotive."
            : "Jestem .NET developerem z 3-letnim doświadczeniem. Tworzę aplikacje webowe i desktopowe " +
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

    return View(model);
}

    public IActionResult Projects()
{
    var model = new ProjectsViewModel
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

    return View(model);
}

    // GET: Contact
    [HttpGet]
    public IActionResult Contact()
    {
        var model = new ContactPageViewModel
        {
            Info = BuildContactInfo()
        };

        return View(model);
    }

    // POST: Contact – wysyłka CV
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactPageViewModel model)
    {
        model.Info = BuildContactInfo();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await SendCvAsync(model.Request.Email);
            model.CvSent = true;
            ModelState.Clear();
            model.Request.Email = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania CV");

            var isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

            ModelState.AddModelError(
                string.Empty,
                isEnglish
                    ? "An error occurred while sending the CV. Please try again later."
                    : "Wystąpił błąd podczas wysyłania CV. Spróbuj ponownie później.");
        }

        return View(model);
    }

    private ContactViewModel BuildContactInfo() =>
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

    private async Task SendCvAsync(string recipientEmail)
    {
        if (string.IsNullOrWhiteSpace(_emailSettings.Host) ||
            string.IsNullOrWhiteSpace(_emailSettings.User) ||
            string.IsNullOrWhiteSpace(_emailSettings.Password))
        {
            throw new InvalidOperationException(
                "Konfiguracja EmailSettings jest niepełna. Uzupełnij Host/User/Password w konfiguracji (user-secrets/env).");
        }

        var isEnglish = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en";

        var subject = isEnglish
            ? "CV – Tomasz Kraft"
            : "CV – Tomasz Kraft";

        var body = isEnglish
            ? "Hi,\n\nPlease find attached my CV in PDF format.\n\nBest regards,\nTomasz Kraft"
            : "Cześć,\n\nW załączeniu przesyłam moje CV w formacie PDF.\n\nPozdrawiam,\nTomasz Kraft";

        using var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromDisplayName),
            Subject = subject,
            Body = body
        };

        message.To.Add(recipientEmail);

        var cvPath = Path.Combine(_env.WebRootPath, "files", "TomaszKraftCV.pdf");
        if (!System.IO.File.Exists(cvPath))
        {
            throw new FileNotFoundException("Nie znaleziono pliku CV.", cvPath);
        }

        message.Attachments.Add(new Attachment(cvPath));

        using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.User, _emailSettings.Password)
        };

        await client.SendMailAsync(message);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}