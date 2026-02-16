using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Portfolio.Models;

public class ContactPageViewModel
{
    [ValidateNever]
    public ContactViewModel Info { get; set; } = new();
    public CvRequestModel Request { get; set; } = new();
    public bool CvSent { get; set; }
    public bool CvEmailEnabled { get; set; }
    
}
