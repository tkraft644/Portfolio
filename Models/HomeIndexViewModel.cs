namespace Portfolio.Models;
 
 public class HomeIndexViewModel
 {
     public string Name { get; set; } = default!;
     public string Role { get; set; } = default!;
     public double YearsOfExperience { get; set; }
 
     public IList<string> MainTechnologies { get; set; } = new List<string>();
 
     public string HeroBackgroundUrl { get; set; } =
         "https://images.unsplash.com/photo-1519389950473-47ba0277781c?q=80&w=2070";
 }