namespace Portfolio.Models;

public class ExperienceItem
{
    public string Position { get; set; } = default!;
    public string Company { get; set; } = default!;
    public DateTime From { get; set; }
    public DateTime? To { get; set; }   // null = obecnie

    public IList<string> Responsibilities { get; set; } = new List<string>();

    public string GetPeriodText()
    {
        var from = From.ToString("MMMM yyyy");
        var to = To.HasValue ? To.Value.ToString("MMMM yyyy") : "obecnie";
        return $"{from} – {to}";
    }
}