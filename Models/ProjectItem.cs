namespace Portfolio.Models;

public class ProjectItem
{
    public string Title { get; set; } = default!;
    public string Company { get; set; } = default!;
    public string Description { get; set; } = default!;
    public IList<string> Technologies { get; set; } = new List<string>();
}