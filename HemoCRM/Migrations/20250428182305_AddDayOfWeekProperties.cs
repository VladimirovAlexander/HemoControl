using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDayOfWeekProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LunchEndTime",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "LunchStartTime",
                table: "DoctorSchedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AppointmentDuration",
                table: "DoctorSchedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakBetweenAppointments",
                table: "DoctorSchedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchEnd",
                table: "DoctorSchedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchStart",
                table: "DoctorSchedules",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDuration",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "BreakBetweenAppointments",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "LunchEnd",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "LunchStart",
                table: "DoctorSchedules");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchEndTime",
                table: "DoctorSchedules",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchStartTime",
                table: "DoctorSchedules",
                type: "interval",
                nullable: true);
        }
    }
}
