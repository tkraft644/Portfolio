namespace Portfolio.Services;

public interface IExperienceCalculator
{
    double GetYearsOfExperience(DateTime startDate, DateTime? today = null);
}
