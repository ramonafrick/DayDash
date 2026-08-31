using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayDash.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class EventReminderLeadTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReminderDaysBefore",
                table: "CalendarEvents",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderDaysBefore",
                table: "CalendarEvents");
        }
    }
}
