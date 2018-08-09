using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class AddedCurrentIteration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlgorithmCurrentIteration",
                table: "AnalysisModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmCurrentIteration",
                table: "AnalysisModel");
        }
    }
}
