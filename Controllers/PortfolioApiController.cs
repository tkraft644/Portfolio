using Microsoft.AspNetCore.Mvc;
using Portfolio.Services;

namespace Portfolio.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioApiController : ControllerBase
{
    private readonly IPortfolioContentService _contentService;
    private readonly ICvEmailSender _cvEmailSender;

    public PortfolioApiController(IPortfolioContentService contentService, ICvEmailSender cvEmailSender)
    {
        _contentService = contentService;
        _cvEmailSender = cvEmailSender;
    }

    [HttpGet("summary")]
    public IActionResult GetSummary() => Ok(_contentService.GetHomeIndexModel());

    [HttpGet("about")]
    public IActionResult GetAbout() => Ok(_contentService.GetAboutModel());

    [HttpGet("projects")]
    public IActionResult GetProjects() => Ok(_contentService.GetProjectsModel());

    [HttpGet("contact")]
    public IActionResult GetContact() =>
        Ok(new
        {
            info = _contentService.GetContactInfo(),
            cvEmailEnabled = _cvEmailSender.IsEnabled
        });
}
