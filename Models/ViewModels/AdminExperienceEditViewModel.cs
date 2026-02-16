using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class AdminExperienceEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime From { get; set; }

    [DataType(DataType.Date)]
    public DateTime? To { get; set; }

    [Required]
    public string Responsibilities { get; set; } = string.Empty;
}

