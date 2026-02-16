namespace Portfolio.Models;

public class AboutViewModel
{
    public string Bio { get; set; } = default!;
    public IList<string> Hobbies { get; set; } = new List<string>();
    public IList<EducationItem> Education { get; set; } = new List<EducationItem>();
    public IList<ExperienceItem> Experience { get; set; } = new List<ExperienceItem>();
    
}
