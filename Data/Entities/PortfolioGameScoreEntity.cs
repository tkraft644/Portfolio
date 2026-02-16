namespace Portfolio.Data.Entities;

public class PortfolioGameScoreEntity
{
    public int Id { get; set; }

    public string PlayerName { get; set; } = default!;
    public int Score { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
