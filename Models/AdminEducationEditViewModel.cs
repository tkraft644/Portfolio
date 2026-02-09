using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class AdminEducationEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(300)]
    public string SchoolPl { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string SchoolEn { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime From { get; set; }

    [DataType(DataType.Date)]
    public DateTime? To { get; set; }
}

