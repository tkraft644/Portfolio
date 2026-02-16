using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Portfolio.Data;
using Portfolio.Data.Entities;
using Portfolio.Models;
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

    [HttpGet("/api/fun/leaderboard")]
    public async Task<ActionResult<List<FunLeaderboardEntryViewModel>>> GetFunLeaderboard(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var db = TryGetDb();
        if (db is null)
        {
            return Ok(new List<FunLeaderboardEntryViewModel>());
        }

        limit = Math.Clamp(limit, 1, 50);

        var top = await db.GameScores
            .AsNoTracking()
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);

        var result = top
            .Select((entry, index) => new FunLeaderboardEntryViewModel
            {
                Rank = index + 1,
                PlayerName = entry.PlayerName,
                Score = entry.Score,
                CreatedAtUtc = entry.CreatedAtUtc
            })
            .ToList();

        return Ok(result);
    }

    [HttpPost("/api/fun/leaderboard")]
    [EnableRateLimiting("fun-leaderboard-submit")]
    public async Task<IActionResult> SubmitFunScore(
        [FromBody] FunLeaderboardSubmitRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var db = TryGetDb();
        if (db is null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = "Leaderboard database is not configured." });
        }

        var playerName = NormalizePlayerName(request.PlayerName);
        if (playerName.Length < 2)
        {
            ModelState.AddModelError(nameof(request.PlayerName), "Player name is too short.");
            return ValidationProblem(ModelState);
        }

        var score = Math.Max(1, request.Score);
        var entity = new PortfolioGameScoreEntity
        {
            PlayerName = playerName,
            Score = score,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.GameScores.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            id = entity.Id,
            score = entity.Score,
            playerName = entity.PlayerName,
            createdAtUtc = entity.CreatedAtUtc
        });
    }

    private PortfolioDbContext? TryGetDb() =>
        HttpContext.RequestServices.GetService<PortfolioDbContext>();

    private static string NormalizePlayerName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var filtered = string.Concat(value.Trim().Where(ch => !char.IsControl(ch)));
        if (filtered.Length > 80)
        {
            filtered = filtered[..80];
        }

        return filtered;
    }
}
