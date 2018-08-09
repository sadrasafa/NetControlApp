using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class CompletedGreedyParams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GreedyCutNonBranching",
                table: "AnalysisModel");

            migrationBuilder.DropColumn(
                name: "GreedyCutToDriven",
                table: "AnalysisModel");

            migrationBuilder.AddColumn<int>(
                name: "GreedyRepeats",
                table: "AnalysisModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GreedyRepeats",
                table: "AnalysisModel");

            migrationBuilder.AddColumn<bool>(
                name: "GreedyCutNonBranching",
                table: "AnalysisModel",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GreedyCutToDriven",
                table: "AnalysisModel",
                nullable: true);
        }
    }
}
