using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class AdminProfileEditViewModel
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime ExperienceStartDate { get; set; }

    [Required]
    [StringLength(1024)]
    public string HeroBackgroundUrl { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string ContactEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string ContactPhone { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string AboutBioTemplatePl { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string AboutBioTemplateEn { get; set; } = string.Empty;
}

