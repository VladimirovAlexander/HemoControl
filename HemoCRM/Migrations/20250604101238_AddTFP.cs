using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HemoCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddTFP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "CoagulogramTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PT = table.Column<double>(type: "double precision", nullable: false),
                    INR = table.Column<double>(type: "double precision", nullable: false),
                    APTT = table.Column<double>(type: "double precision", nullable: false),
                    Fibrinogen = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoagulogramTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoagulogramTests_Tests_Id",
                        column: x => x.Id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompleteBloodCountTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Hemoglobin = table.Column<double>(type: "double precision", nullable: false),
                    Hematocrit = table.Column<double>(type: "double precision", nullable: false),
                    WhiteBloodCells = table.Column<double>(type: "double precision", nullable: false),
                    RedBloodCells = table.Column<double>(type: "double precision", nullable: false),
                    Platelets = table.Column<double>(type: "double precision", nullable: false),
                    MCH = table.Column<double>(type: "double precision", nullable: false),
                    MCV = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompleteBloodCountTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompleteBloodCountTests_Tests_Id",
                        column: x => x.Id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactorAndVWFTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FactorVIII = table.Column<double>(type: "double precision", nullable: false),
                    FactorIX = table.Column<double>(type: "double precision", nullable: false),
                    VWFActivity = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorAndVWFTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactorAndVWFTests_Tests_Id",
                        column: x => x.Id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoagulogramTests");

            migrationBuilder.DropTable(
                name: "CompleteBloodCountTests");

            migrationBuilder.DropTable(
                name: "FactorAndVWFTests");

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
        }
    }
}
