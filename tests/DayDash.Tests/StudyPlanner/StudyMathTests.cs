using DayDash.Modules.StudyPlanner.Application.Services;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class StudyMathTests
{
    private static readonly DateOnly Today = new(2026, 3, 10);

    [Theory]
    [InlineData(0, 15, 0)]
    [InlineData(4, 20, 80)]
    [InlineData(3, 15, 45)]
    [InlineData(-2, 15, 0)]
    public void RecommendedMinutes_multiplies_goal_count_by_rate(int goals, int rate, int expected)
        => Assert.Equal(expected, StudyMath.RecommendedMinutes(goals, rate));

    [Fact]
    public void DailyMinutes_for_an_exam_today_uses_a_single_day_and_never_divides_by_zero()
        => Assert.Equal(120, StudyMath.DailyMinutes(120, Today, Today));

    [Fact]
    public void DailyMinutes_for_a_past_exam_is_also_clamped_to_one_day()
        => Assert.Equal(90, StudyMath.DailyMinutes(90, Today.AddDays(-5), Today));

    [Fact]
    public void DailyMinutes_uses_integer_division_over_the_remaining_days()
        => Assert.Equal(33, StudyMath.DailyMinutes(100, Today.AddDays(3), Today));

    [Fact]
    public void DailyMinutes_with_zero_total_is_zero()
        => Assert.Equal(0, StudyMath.DailyMinutes(0, Today.AddDays(5), Today));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(-3, -3)]
    public void DaysRemaining_is_the_signed_day_delta(int offset, int expected)
        => Assert.Equal(expected, StudyMath.DaysRemaining(Today.AddDays(offset), Today));
}
