namespace Portfolio.Models;

public class ProjectsViewModel
{
    public IList<ProjectItem> Projects { get; set; } = new List<ProjectItem>();
    public IList<ChallengeItem> Challenges { get; set; } = new List<ChallengeItem>();
    
}