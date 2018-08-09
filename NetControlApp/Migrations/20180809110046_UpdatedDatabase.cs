using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class UpdatedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlgorithmCurrentIterationNoImprovement",
                table: "AnalysisModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmCurrentIterationNoImprovement",
                table: "AnalysisModel");
        }
    }
}
