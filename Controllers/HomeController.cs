using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Portfolio.Models;
using Portfolio.Services;

namespace Portfolio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IStringLocalizer<HomeController> _localizer;
    private readonly IPortfolioContentService _contentService;
    private readonly ICvEmailSender _cvEmailSender;

    public HomeController(
        ILogger<HomeController> logger,
        IStringLocalizer<HomeController> localizer,
        IPortfolioContentService contentService,
        ICvEmailSender cvEmailSender)
    {
        _logger = logger;
        _localizer = localizer;
        _contentService = contentService;
        _cvEmailSender = cvEmailSender;
    }
    public IActionResult Index()
    {
        return View(_contentService.GetHomeIndexModel());
    }
    public IActionResult About()
    {
        return View(_contentService.GetAboutModel());
    }

    public IActionResult Projects()
    {
        return View(_contentService.GetProjectsModel());
    }

    // GET: Contact
    [HttpGet]
    public IActionResult Contact()
    {
        var model = new ContactPageViewModel
        {
            Info = _contentService.GetContactInfo(),
            CvEmailEnabled = _cvEmailSender.IsEnabled
        };

        return View(model);
    }

    // POST: Contact – wysyłka CV
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactPageViewModel model, CancellationToken cancellationToken)
    {
        model.Info = _contentService.GetContactInfo();
        model.CvEmailEnabled = _cvEmailSender.IsEnabled;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_cvEmailSender.IsEnabled)
        {
            ModelState.AddModelError(string.Empty, _localizer["Contact.CvEmailDisabled"]);
            return View(model);
        }

        try
        {
            await _cvEmailSender.SendCvAsync(model.Request.Email, cancellationToken);
            model.CvSent = true;
            ModelState.Clear();
            model.Request.Email = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas wysyłania CV");
            ModelState.AddModelError(
                string.Empty,
                _localizer["Contact.CvSendError"]);
        }

        return View(model);
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
