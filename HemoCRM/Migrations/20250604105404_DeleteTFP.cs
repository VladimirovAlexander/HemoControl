using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTFP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoagulogramTests_Tests_Id",
                table: "CoagulogramTests");

            migrationBuilder.DropForeignKey(
                name: "FK_CompleteBloodCountTests_Tests_Id",
                table: "CompleteBloodCountTests");

            migrationBuilder.DropForeignKey(
                name: "FK_FactorAndVWFTests_Tests_Id",
                table: "FactorAndVWFTests");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "FactorAndVWFTests",
                newName: "TestId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CompleteBloodCountTests",
                newName: "TestId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CoagulogramTests",
                newName: "TestId");

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CoagulogramTests_Tests_TestId",
                table: "CoagulogramTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompleteBloodCountTests_Tests_TestId",
                table: "CompleteBloodCountTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FactorAndVWFTests_Tests_TestId",
                table: "FactorAndVWFTests",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoagulogramTests_Tests_TestId",
                table: "CoagulogramTests");

            migrationBuilder.DropForeignKey(
                name: "FK_CompleteBloodCountTests_Tests_TestId",
                table: "CompleteBloodCountTests");

            migrationBuilder.DropForeignKey(
                name: "FK_FactorAndVWFTests_Tests_TestId",
                table: "FactorAndVWFTests");

            migrationBuilder.DropColumn(
                name: "TestType",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "FactorAndVWFTests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "CompleteBloodCountTests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "CoagulogramTests",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoagulogramTests_Tests_Id",
                table: "CoagulogramTests",
                column: "Id",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompleteBloodCountTests_Tests_Id",
                table: "CompleteBloodCountTests",
                column: "Id",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FactorAndVWFTests_Tests_Id",
                table: "FactorAndVWFTests",
                column: "Id",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
