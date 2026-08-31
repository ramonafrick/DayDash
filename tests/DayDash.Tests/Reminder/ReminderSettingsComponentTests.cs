using Bunit;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.Reminder.UI.Components;
using DayDash.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.Reminder;

public class ReminderSettingsComponentTests : CultureIsolatedTest
{
    private static DayDashTestContext NewContext(out FakeReminderService reminders)
    {
        reminders = new FakeReminderService();
        var ctx = new DayDashTestContext();
        ctx.Services.AddSingleton<IReminderService>(reminders);
        return ctx;
    }

    [Fact]
    public void Loads_the_persisted_config_into_the_form()
    {
        using var ctx = NewContext(out var reminders);
        reminders.Config = new ReminderConfig
        {
            DailyStudyReminderTime = new TimeOnly(7, 45),
            EventReminderDaysBefore = 4,
            IsEnabled = false,
        };

        var cut = ctx.Render<ReminderSettingsComponent>();

        Assert.Equal("07:45", cut.Find("#rm-time").GetAttribute("value"));
        Assert.Equal("4", cut.Find("#rm-days").GetAttribute("value"));
    }

    [Fact]
    public void Shows_a_hint_when_the_host_cannot_deliver_notifications()
    {
        using var ctx = NewContext(out var reminders);
        reminders.NotificationsSupported = false;

        var cut = ctx.Render<ReminderSettingsComponent>();

        Assert.Contains("Browser-Vorschau", cut.Markup);
    }

    [Fact]
    public void Saving_sends_the_edited_values_once_and_confirms()
    {
        using var ctx = NewContext(out var reminders);

        var cut = ctx.Render<ReminderSettingsComponent>();
        cut.Find("#rm-days").Change("5");
        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(reminders.LastSaved);
            Assert.Equal(5, reminders.LastSaved!.EventReminderDaysBefore);
            Assert.Contains("Gespeichert", cut.Markup);
        });
    }
}
