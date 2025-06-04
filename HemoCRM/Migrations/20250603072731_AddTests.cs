using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "APTT",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FactorIX",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FactorVIII",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Fibrinogen",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Hematocrit",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Hemoglobin",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "INR",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MCH",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MCV",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PT",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Platelets",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceptionId",
                table: "Tests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "RedBloodCells",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestType",
                table: "Tests",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "VWFActivity",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WhiteBloodCells",
                table: "Tests",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_ReceptionId",
                table: "Tests",
                column: "ReceptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Receptions_ReceptionId",
                table: "Tests",
                column: "ReceptionId",
                principalTable: "Receptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Receptions_ReceptionId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_ReceptionId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "APTT",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "FactorIX",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "FactorVIII",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Fibrinogen",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Hematocrit",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Hemoglobin",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "INR",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "MCH",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "MCV",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "PT",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Platelets",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "ReceptionId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "RedBloodCells",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "TestType",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "VWFActivity",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "WhiteBloodCells",
                table: "Tests");
        }
    }
}
