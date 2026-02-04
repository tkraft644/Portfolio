namespace Portfolio.Models;
 
 public class HomeIndexViewModel
 {
     public string Name { get; set; } = default!;
     public string Role { get; set; } = default!;
     public double YearsOfExperience { get; set; }
 
    public IList<string> MainTechnologies { get; set; } = new List<string>();
 
    public string HeroBackgroundUrl { get; set; } = "/img/hero-dotnet-code.svg";
}
