using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class AddedParamsToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisModel",
                columns: table => new
                {
                    RunId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    AnalysisName = table.Column<string>(maxLength: 100, nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    NetType = table.Column<bool>(nullable: false),
                    NetNodes = table.Column<string>(nullable: false),
                    Target = table.Column<string>(nullable: false),
                    DrugTarget = table.Column<string>(nullable: true),
                    AlgorithmType = table.Column<string>(nullable: false),
                    AlgorithmParams = table.Column<string>(nullable: false),
                    DoContact = table.Column<bool>(nullable: false),
                    Network = table.Column<string>(nullable: true),
                    Progress = table.Column<double>(nullable: true),
                    BestResult = table.Column<string>(nullable: true),
                    IsCompleted = table.Column<bool>(nullable: true),
                    ScheduledToStop = table.Column<bool>(nullable: true),
                    RandomSeed = table.Column<int>(nullable: true),
                    MaxIteration = table.Column<int>(nullable: true),
                    MaxIterationNoImprovement = table.Column<int>(nullable: true),
                    MaxPathLength = table.Column<int>(nullable: true),
                    GeneticPopulationSize = table.Column<int>(nullable: true),
                    GeneticElementsRandom = table.Column<int>(nullable: true),
                    GeneticPercentageRandom = table.Column<double>(nullable: true),
                    GeneticPercentageElite = table.Column<double>(nullable: true),
                    GeneticProbabilityMutation = table.Column<double>(nullable: true),
                    GreedyHeuristics = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisModel", x => x.RunId);
                    table.ForeignKey(
                        name: "FK_AnalysisModel_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisModel_UserId",
                table: "AnalysisModel",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisModel");
        }
    }
}
