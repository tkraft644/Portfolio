using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class FunLeaderboardSubmitRequest
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string PlayerName { get; set; } = string.Empty;

    [Range(1, 2000000000)]
    public int Score { get; set; }
}
