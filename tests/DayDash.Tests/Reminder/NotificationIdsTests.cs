using DayDash.Modules.Reminder.Application.Services;
using Xunit;

namespace DayDash.Tests.Reminder;

public class NotificationIdsTests
{
    [Fact]
    public void Same_guid_maps_to_the_same_id_every_call()
    {
        var id = Guid.NewGuid();

        Assert.Equal(NotificationIds.ForExam(id), NotificationIds.ForExam(id));
        Assert.Equal(NotificationIds.ForEvent(id), NotificationIds.ForEvent(id));
    }

    [Fact]
    public void Exam_and_event_namespaces_do_not_collide_for_the_same_guid()
    {
        var id = Guid.NewGuid();

        Assert.NotEqual(NotificationIds.ForExam(id), NotificationIds.ForEvent(id));
    }

    [Fact]
    public void Ids_are_positive_and_clear_of_the_fixed_daily_id()
    {
        for (var i = 0; i < 5_000; i++)
        {
            Assert.True(NotificationIds.ForExam(Guid.NewGuid()) > NotificationIds.DailyStudyReminder);
            Assert.True(NotificationIds.ForEvent(Guid.NewGuid()) >= 100);
        }
    }

    [Fact]
    public void Collisions_are_rare_over_ten_thousand_samples()
    {
        var seen = new HashSet<int>();
        var collisions = 0;

        for (var i = 0; i < 10_000; i++)
        {
            if (!seen.Add(NotificationIds.ForEvent(Guid.NewGuid())))
            {
                collisions++;
            }
        }

        Assert.True(collisions < 20, $"{collisions} collisions in 10k samples");
    }
}
