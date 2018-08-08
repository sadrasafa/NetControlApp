using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class RemovedDoContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoContact",
                table: "AnalysisModel");

            migrationBuilder.RenameColumn(
                name: "UserGivenNetworkType",
                table: "AnalysisModel",
                newName: "UserIsNetworkSeed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserIsNetworkSeed",
                table: "AnalysisModel",
                newName: "UserGivenNetworkType");

            migrationBuilder.AddColumn<bool>(
                name: "DoContact",
                table: "AnalysisModel",
                nullable: false,
                defaultValue: false);
        }
    }
}
