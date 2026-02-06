using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class AdminLoginViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

