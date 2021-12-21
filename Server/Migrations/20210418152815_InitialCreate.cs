using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace climatepi.Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DegreesCelsius = table.Column<double>(type: "REAL", nullable: false),
                    PressureMillibars = table.Column<double>(type: "REAL", nullable: false),
                    HumidityPercentage = table.Column<double>(type: "REAL", nullable: false),
                    AltitudeCentimeters = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conditions");
        }
    }
}
