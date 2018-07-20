using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class RemovedAlgParams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmParams",
                table: "AnalysisModel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlgorithmParams",
                table: "AnalysisModel",
                nullable: false,
                defaultValue: "");
        }
    }
}
