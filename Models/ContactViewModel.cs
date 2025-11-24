namespace Portfolio.Models;

public class ContactViewModel
{
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public IList<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
}