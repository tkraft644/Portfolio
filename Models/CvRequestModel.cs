using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class CvRequestModel
{
    [Required(ErrorMessage = "Validation.EmailRequired")]
    [EmailAddress(ErrorMessage = "Validation.EmailInvalid")]
    public string Email { get; set; } = string.Empty;
}
