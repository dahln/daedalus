using Microsoft.EntityFrameworkCore.Migrations;

namespace climatepi.Server.Migrations
{
    public partial class RemovingAltitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltitudeCentimeters",
                table: "Conditions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AltitudeCentimeters",
                table: "Conditions",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
