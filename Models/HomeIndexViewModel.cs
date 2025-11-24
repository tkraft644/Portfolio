namespace Portfolio.Models;
 
 public class HomeIndexViewModel
 {
     public string Name { get; set; } = default!;
     public string Role { get; set; } = default!;
     public int YearsOfExperience { get; set; }
 
     public IList<string> MainTechnologies { get; set; } = new List<string>();
 
     public string HeroBackgroundUrl { get; set; } =
         "https://images.unsplash.com/photo-1555949963-aa79dcee981c?q=80&w=2070";
 }