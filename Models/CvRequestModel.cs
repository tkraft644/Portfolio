using System.ComponentModel.DataAnnotations;

namespace Portfolio.Models;

public class CvRequestModel
{
    [Required(ErrorMessage = "Podaj adres e-mail.")]
    [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail.")]
    public string Email { get; set; } = string.Empty;
}