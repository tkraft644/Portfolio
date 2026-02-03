using Portfolio.Services;
using Xunit;

namespace Portfolio.Tests;

public class ExperienceCalculatorTests
{
    [Theory]
    [InlineData(2020, 1, 1, 2020, 7, 1, 0.5)]
    [InlineData(2020, 1, 1, 2020, 9, 1, 0.5)]
    [InlineData(2020, 1, 1, 2020, 10, 1, 1.0)]
    [InlineData(2022, 11, 1, 2023, 11, 1, 1.0)]
    public void GetYearsOfExperience_RoundsToNearestHalfYear(
        int startY, int startM, int startD,
        int todayY, int todayM, int todayD,
        double expectedYears)
    {
        var calculator = new ExperienceCalculator();

        var start = new DateTime(startY, startM, startD);
        var today = new DateTime(todayY, todayM, todayD);

        var actualYears = calculator.GetYearsOfExperience(start, today);

        Assert.Equal(expectedYears, actualYears);
    }

    [Fact]
    public void GetYearsOfExperience_AdjustsForIncompleteMonth()
    {
        var calculator = new ExperienceCalculator();

        var start = new DateTime(2020, 1, 31);
        var today = new DateTime(2020, 2, 1);

        var actualYears = calculator.GetYearsOfExperience(start, today);

        Assert.Equal(0.0, actualYears);
    }
}
