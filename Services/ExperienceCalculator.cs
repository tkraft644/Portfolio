namespace Portfolio.Services;

public class ExperienceCalculator : IExperienceCalculator
{
    public double GetYearsOfExperience(DateTime startDate, DateTime? today = null)
    {
        var currentDate = (today ?? DateTime.Today).Date;
        startDate = startDate.Date;

        var months = ((currentDate.Year - startDate.Year) * 12) + currentDate.Month - startDate.Month;
        if (currentDate.Day < startDate.Day)
        {
            months--;
        }

        var years = months / 12.0;
        return Math.Round(years * 2, MidpointRounding.AwayFromZero) / 2.0;
    }
}
