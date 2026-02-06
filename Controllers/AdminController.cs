using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Data;
using Portfolio.Data.Entities;
using Portfolio.Models;

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
}
