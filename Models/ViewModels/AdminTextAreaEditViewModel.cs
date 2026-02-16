using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class AdminTextAreaEditViewModel
{
    [Required]
    public string Items { get; set; } = string.Empty;
}

