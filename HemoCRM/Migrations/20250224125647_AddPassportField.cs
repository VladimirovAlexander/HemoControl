using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Migrations
{
    /// <inheritdoc />
    public partial class AddPassportField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Passport",
                table: "Patients",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passport",
                table: "Patients");
        }
    }
}
