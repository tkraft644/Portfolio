namespace Portfolio.Models;

public class FunLeaderboardEntryViewModel
{
    public int Rank { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
