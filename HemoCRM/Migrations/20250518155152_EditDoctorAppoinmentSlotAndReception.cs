using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class EditDoctorAppoinmentSlotAndReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentDate",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "DoctorAppointmentSlots");

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "Receptions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_SlotId",
                table: "Receptions",
                column: "SlotId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_DoctorAppointmentSlots_SlotId",
                table: "Receptions",
                column: "SlotId",
                principalTable: "DoctorAppointmentSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_DoctorAppointmentSlots_SlotId",
                table: "Receptions");

            migrationBuilder.DropIndex(
                name: "IX_Receptions_SlotId",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "Receptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentDate",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Receptions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "DoctorAppointmentSlots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
