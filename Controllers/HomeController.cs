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
        var model = new HomeIndexViewModel
        {
            Name = "Tomasz Kraft",
            Role = ".NET Developer",
            YearsOfExperience = 3,
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
        var model = new AboutViewModel
        {
            Bio =
                "Jestem .NET developerem z 3-letnim doświadczeniem. Tworzę aplikacje webowe i desktopowe " +
                "w technologii .NET, C#, ASP.NET Core, WPF oraz EF Core. Interesuje mnie wysoka jakość kodu, " +
                "architektura i dobre praktyki. Hobbystycznie zajmuję się fotografią oraz motoryzacją off-roadową.",
            Hobbies = new List<string>
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
                    Responsibilities =
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
                    Responsibilities =
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
                    Title = "System wspierający procesy w fabryce",
                    Company = "Volvo Polska",
                    Description =
                        "Oprogramowanie wspierające procesy w fabryce – m.in. obsługa danych produkcyjnych, " +
                        "integracje z istniejącymi systemami i raportowanie.",
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
                    Title = "System do obsługi firmy logistycznej",
                    Company = "Vectio sp. z o.o.",
                    Description =
                        "Rozbudowany system do obsługi firmy logistycznej – zlecenia transportowe, kierowcy, flota, " +
                        "rozliczenia, raporty oraz integracje z systemami zewnętrznymi.",
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
                    Title = "Migracja z .NET Framework 4.8 do .NET 8",
                    Description =
                        "Migracja ok. 90% systemu z .NET Framework 4.8 do .NET 8 praktycznie w pojedynkę – " +
                        "w tym dostosowanie architektury, aktualizacja bibliotek i rozwiązanie problemów zgodności."
                },
                new()
                {
                    Title = "Optymalizacja dużego systemu WPF + EF Core",
                    Description =
                        "Praca nad wydajnością i stabilnością rozbudowanej aplikacji WPF/DevExpress obsługującej " +
                        "duże ilości danych – optymalizacja zapytań EF Core, redukcja zużycia pamięci, " +
                        "poprawa responsywności UI."
                },
                new()
                {
                    Title = "End-to-end funkcjonalności",
                    Description =
                        "Prowadzenie funkcjonalności od rozmów z biznesem, przez projekt techniczny, " +
                        "implementację w .NET i EF Core, po wdrożenie i utrzymanie."
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
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas wysyłania CV. Spróbuj ponownie później.");
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

        var subject = "CV – Tomasz Kraft";
        var body =
            "Cześć,\n\n" +
            "W załączeniu przesyłam moje CV w formacie PDF.\n\n" +
            "Pozdrawiam,\n" +
            "Tomasz Kraft";

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