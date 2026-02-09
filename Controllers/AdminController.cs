using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Data;
using Portfolio.Data.Entities;
using Portfolio.Models;
using System.Globalization;

namespace Portfolio.Controllers;

[Authorize(Policy = "AdminOnly")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly AdminSettings _settings;

    public AdminController(IOptions<AdminSettings> settings)
    {
        _settings = settings.Value;
    }

    private bool IsEnabled => _settings.Enabled && !string.IsNullOrWhiteSpace(_settings.Password);

    private PortfolioDbContext? TryGetDb() =>
        HttpContext.RequestServices.GetService<PortfolioDbContext>();

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles.AsNoTracking().SingleAsync(cancellationToken);
        ViewData["ProfileName"] = profile.Name;
        ViewData["ProfileRole"] = profile.Role;
        return View();
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles.AsNoTracking().SingleAsync(cancellationToken);
        return View(new AdminProfileEditViewModel
        {
            Name = profile.Name,
            Role = profile.Role,
            ExperienceStartDate = profile.ExperienceStartDate,
            HeroBackgroundUrl = profile.HeroBackgroundUrl,
            ContactEmail = profile.ContactEmail,
            ContactPhone = profile.ContactPhone,
            AboutBioTemplatePl = profile.AboutBioTemplatePl,
            AboutBioTemplateEn = profile.AboutBioTemplateEn
        });
    }

    [HttpPost("profile")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(AdminProfileEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (!model.AboutBioTemplatePl.Contains("{0}", StringComparison.Ordinal) ||
            !model.AboutBioTemplateEn.Contains("{0}", StringComparison.Ordinal))
        {
            ModelState.AddModelError(
                nameof(model.AboutBioTemplatePl),
                "Szablon bio musi zawierać {0} (liczba lat doświadczenia).");
            ModelState.AddModelError(
                nameof(model.AboutBioTemplateEn),
                "Szablon bio musi zawierać {0} (years of experience).");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles.SingleAsync(cancellationToken);
        profile.Name = model.Name.Trim();
        profile.Role = model.Role.Trim();
        profile.ExperienceStartDate = model.ExperienceStartDate.Date;
        profile.HeroBackgroundUrl = model.HeroBackgroundUrl.Trim();
        profile.ContactEmail = model.ContactEmail.Trim();
        profile.ContactPhone = model.ContactPhone.Trim();
        profile.AboutBioTemplatePl = model.AboutBioTemplatePl.Trim();
        profile.AboutBioTemplateEn = model.AboutBioTemplateEn.Trim();

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet("technologies")]
    public async Task<IActionResult> Technologies(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .AsNoTracking()
            .Include(x => x.Technologies)
            .SingleAsync(cancellationToken);

        var items = string.Join(
            Environment.NewLine,
            profile.Technologies
                .OrderBy(x => x.SortOrder)
                .Select(x => x.Name));

        ViewData["Title"] = "Technologie";
        ViewData["Hint"] = "Jedna technologia na linię. Zapis nadpisze listę w bazie.";
        return View("TextAreaEdit", new AdminTextAreaEditViewModel { Items = items });
    }

    [HttpPost("technologies")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Technologies(AdminTextAreaEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Technologie";
            ViewData["Hint"] = "Jedna technologia na linię. Zapis nadpisze listę w bazie.";
            return View("TextAreaEdit", model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .Include(x => x.Technologies)
            .SingleAsync(cancellationToken);

        var parsed = model.Items
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        profile.Technologies.Clear();
        var order = 10;
        foreach (var name in parsed)
        {
            profile.Technologies.Add(new PortfolioTechnologyEntity
            {
                Profile = profile,
                SortOrder = order,
                Name = name
            });
            order += 10;
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Technologies));
    }

    [HttpGet("social-links")]
    public async Task<IActionResult> SocialLinks(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .AsNoTracking()
            .Include(x => x.SocialLinks)
            .SingleAsync(cancellationToken);

        var items = string.Join(
            Environment.NewLine,
            profile.SocialLinks
                .OrderBy(x => x.SortOrder)
                .Select(x => $"{x.Name} | {x.Url}"));

        ViewData["Title"] = "Social links";
        ViewData["Hint"] = "Format: Name | Url (jedna pozycja na linię). Zapis nadpisze listę w bazie.";
        return View("TextAreaEdit", new AdminTextAreaEditViewModel { Items = items });
    }

    [HttpPost("social-links")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SocialLinks(AdminTextAreaEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Social links";
            ViewData["Hint"] = "Format: Name | Url (jedna pozycja na linię). Zapis nadpisze listę w bazie.";
            return View("TextAreaEdit", model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .Include(x => x.SocialLinks)
            .SingleAsync(cancellationToken);

        var parsed = model.Items
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseNameUrl)
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        profile.SocialLinks.Clear();
        var order = 10;
        foreach (var item in parsed)
        {
            profile.SocialLinks.Add(new PortfolioSocialLinkEntity
            {
                Profile = profile,
                SortOrder = order,
                Name = item.Name,
                Url = item.Url
            });
            order += 10;
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(SocialLinks));
    }

    private static NameUrl? ParseNameUrl(string line)
    {
        var parts = line.Split('|', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return null;
        }

        var name = parts[0].Trim();
        var url = parts[1].Trim();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return new NameUrl(name, url);
    }

    private sealed record NameUrl(string Name, string Url);

    [HttpGet("hobbies")]
    public async Task<IActionResult> Hobbies(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .AsNoTracking()
            .Include(x => x.Hobbies)
            .SingleAsync(cancellationToken);

        var items = string.Join(
            Environment.NewLine,
            profile.Hobbies
                .OrderBy(x => x.SortOrder)
                .Select(x => $"{x.TextPl} | {x.TextEn}"));

        ViewData["Title"] = "Zainteresowania";
        ViewData["Hint"] = "Format: PL | EN (jedna pozycja na linię). Zapis nadpisze listę w bazie.";
        return View("TextAreaEdit", new AdminTextAreaEditViewModel { Items = items });
    }

    [HttpPost("hobbies")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Hobbies(AdminTextAreaEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Zainteresowania";
            ViewData["Hint"] = "Format: PL | EN (jedna pozycja na linię). Zapis nadpisze listę w bazie.";
            return View("TextAreaEdit", model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var parsed = model.Items
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseBilingualText)
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        if (parsed.Count == 0)
        {
            ModelState.AddModelError(nameof(model.Items), "Dodaj przynajmniej jedną pozycję.");
            ViewData["Title"] = "Zainteresowania";
            ViewData["Hint"] = "Format: PL | EN (jedna pozycja na linię). Zapis nadpisze listę w bazie.";
            return View("TextAreaEdit", model);
        }

        var profile = await db.Profiles
            .Include(x => x.Hobbies)
            .SingleAsync(cancellationToken);

        profile.Hobbies.Clear();
        var order = 10;
        foreach (var item in parsed)
        {
            profile.Hobbies.Add(new PortfolioHobbyEntity
            {
                Profile = profile,
                SortOrder = order,
                TextPl = item.Pl,
                TextEn = item.En
            });
            order += 10;
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Hobbies));
    }

    [HttpGet("education")]
    public async Task<IActionResult> Education(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .AsNoTracking()
            .Include(x => x.Education)
            .SingleAsync(cancellationToken);

        var model = new AdminEducationListViewModel
        {
            Items = profile.Education
                .OrderBy(x => x.SortOrder)
                .Select(x => new AdminEducationListItem
                {
                    Id = x.Id,
                    SortOrder = x.SortOrder,
                    SchoolPl = x.SchoolPl,
                    SchoolEn = x.SchoolEn,
                    From = x.From,
                    To = x.To
                })
                .ToList()
        };

        return View("EducationList", model);
    }

    [HttpGet("education/new")]
    public IActionResult EducationCreate()
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        return View("EducationEdit", new AdminEducationEditViewModel
        {
            From = DateTime.Today
        });
    }

    [HttpGet("education/edit/{id:int}")]
    public async Task<IActionResult> EducationEdit(int id, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var edu = await db.Education
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (edu is null)
        {
            return NotFound("Not found");
        }

        return View("EducationEdit", new AdminEducationEditViewModel
        {
            Id = edu.Id,
            SchoolPl = edu.SchoolPl,
            SchoolEn = edu.SchoolEn,
            From = edu.From,
            To = edu.To
        });
    }

    [HttpPost("education/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EducationSave(AdminEducationEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (model.To is not null && model.To.Value.Date < model.From.Date)
        {
            ModelState.AddModelError(nameof(model.To), "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
        }

        if (!ModelState.IsValid)
        {
            return View("EducationEdit", model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        if (model.Id == 0)
        {
            var profile = await db.Profiles.SingleAsync(cancellationToken);
            var nextSortOrder = await db.Education
                .Where(x => x.PortfolioProfileId == profile.Id)
                .MaxAsync(x => (int?)x.SortOrder, cancellationToken) ?? 0;

            db.Education.Add(new PortfolioEducationEntity
            {
                Profile = profile,
                SortOrder = nextSortOrder + 10,
                SchoolPl = model.SchoolPl.Trim(),
                SchoolEn = model.SchoolEn.Trim(),
                From = model.From.Date,
                To = model.To?.Date
            });
        }
        else
        {
            var edu = await db.Education.SingleOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
            if (edu is null)
            {
                return NotFound("Not found");
            }

            edu.SchoolPl = model.SchoolPl.Trim();
            edu.SchoolEn = model.SchoolEn.Trim();
            edu.From = model.From.Date;
            edu.To = model.To?.Date;
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Education));
    }

    [HttpPost("education/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EducationDelete(int id, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var edu = await db.Education.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (edu is null)
        {
            return NotFound("Not found");
        }

        db.Education.Remove(edu);
        await db.SaveChangesAsync(cancellationToken);

        TempData["Admin.Success"] = "Usunięto pozycję.";
        return RedirectToAction(nameof(Education));
    }

    [HttpGet("experience")]
    public async Task<IActionResult> Experience(CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var profile = await db.Profiles
            .AsNoTracking()
            .Include(x => x.Experience)
            .ThenInclude(x => x.Responsibilities)
            .SingleAsync(cancellationToken);

        var model = new AdminExperienceListViewModel
        {
            Items = profile.Experience
                .OrderByDescending(x => x.From)
                .Select(x => new AdminExperienceListItem
                {
                    Id = x.Id,
                    Company = x.Company,
                    Position = x.Position,
                    From = x.From,
                    To = x.To,
                    ResponsibilitiesCount = x.Responsibilities.Count
                })
                .ToList()
        };

        return View("ExperienceList", model);
    }

    [HttpGet("experience/new")]
    public IActionResult ExperienceCreate()
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        return View("ExperienceEdit", new AdminExperienceEditViewModel
        {
            From = DateTime.Today
        });
    }

    [HttpGet("experience/edit/{id:int}")]
    public async Task<IActionResult> ExperienceEdit(int id, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var exp = await db.Experience
            .AsNoTracking()
            .Include(x => x.Responsibilities)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (exp is null)
        {
            return NotFound("Not found");
        }

        var responsibilities = string.Join(
            Environment.NewLine,
            exp.Responsibilities
                .OrderBy(x => x.SortOrder)
                .Select(x => $"{x.TextPl} | {x.TextEn}"));

        return View("ExperienceEdit", new AdminExperienceEditViewModel
        {
            Id = exp.Id,
            Company = exp.Company,
            Position = exp.Position,
            From = exp.From,
            To = exp.To,
            Responsibilities = responsibilities
        });
    }

    [HttpPost("experience/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExperienceSave(AdminExperienceEditViewModel model, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        if (model.To is not null && model.To.Value.Date < model.From.Date)
        {
            ModelState.AddModelError(nameof(model.To), "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");
        }

        if (!ModelState.IsValid)
        {
            return View("ExperienceEdit", model);
        }

        var parsedResponsibilities = model.Responsibilities
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseBilingualText)
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        if (parsedResponsibilities.Count == 0)
        {
            ModelState.AddModelError(nameof(model.Responsibilities), "Dodaj przynajmniej jedną odpowiedzialność.");
            return View("ExperienceEdit", model);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        PortfolioExperienceEntity exp;
        if (model.Id == 0)
        {
            var profile = await db.Profiles.SingleAsync(cancellationToken);
            var nextSortOrder = await db.Experience
                .Where(x => x.PortfolioProfileId == profile.Id)
                .MaxAsync(x => (int?)x.SortOrder, cancellationToken) ?? 0;

            exp = new PortfolioExperienceEntity
            {
                Profile = profile,
                SortOrder = nextSortOrder + 10
            };
            db.Experience.Add(exp);
        }
        else
        {
            exp = await db.Experience
                .Include(x => x.Responsibilities)
                .SingleOrDefaultAsync(x => x.Id == model.Id, cancellationToken)
                ?? throw new InvalidOperationException("Experience not found.");
        }

        exp.Company = model.Company.Trim();
        exp.Position = model.Position.Trim();
        exp.From = model.From.Date;
        exp.To = model.To?.Date;

        exp.Responsibilities.Clear();
        var order = 10;
        foreach (var r in parsedResponsibilities)
        {
            exp.Responsibilities.Add(new PortfolioExperienceResponsibilityEntity
            {
                Experience = exp,
                SortOrder = order,
                TextPl = r.Pl,
                TextEn = r.En
            });
            order += 10;
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Admin.Success"] = "Zapisano zmiany.";
        return RedirectToAction(nameof(Experience));
    }

    [HttpPost("experience/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExperienceDelete(int id, CancellationToken cancellationToken)
    {
        if (!IsEnabled)
        {
            return NotFound("Not found");
        }

        var db = TryGetDb();
        if (db is null)
        {
            return View("NotConfigured");
        }

        var exp = await db.Experience.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (exp is null)
        {
            return NotFound("Not found");
        }

        db.Experience.Remove(exp);
        await db.SaveChangesAsync(cancellationToken);

        TempData["Admin.Success"] = "Usunięto pozycję.";
        return RedirectToAction(nameof(Experience));
    }

    private static BilingualText? ParseBilingualText(string line)
    {
        var parts = line.Split('|', 2, StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return null;
        }

        var pl = parts[0].Trim();
        if (string.IsNullOrWhiteSpace(pl))
        {
            return null;
        }

        var en = parts.Length == 2 ? parts[1].Trim() : pl;
        if (string.IsNullOrWhiteSpace(en))
        {
            en = pl;
        }

        return new BilingualText(pl, en);
    }

    private sealed record BilingualText(string Pl, string En);
}
