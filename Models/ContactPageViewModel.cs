namespace Portfolio.Models;

public class ContactPageViewModel
{
    public ContactViewModel Info { get; set; } = new();
    public CvRequestModel Request { get; set; } = new();
    public bool CvSent { get; set; }
    
}