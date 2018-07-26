using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NetControlApp.Migrations
{
    public partial class AnalysesSecondForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmParams",
                table: "AnalysesModel");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "AnalysesModel",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Target",
                table: "AnalysesModel",
                newName: "UserGivenTarget");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "AnalysesModel",
                newName: "GeneticProbabilityMutation");

            migrationBuilder.RenameColumn(
                name: "Network",
                table: "AnalysesModel",
                newName: "UserGivenDrugTarget");

            migrationBuilder.RenameColumn(
                name: "NetType",
                table: "AnalysesModel",
                newName: "UserGivenNetworkType");

            migrationBuilder.RenameColumn(
                name: "NetNodes",
                table: "AnalysesModel",
                newName: "UserGivenNodes");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "AnalysesModel",
                newName: "GreedyCutToDriven");

            migrationBuilder.RenameColumn(
                name: "DrugTarget",
                table: "AnalysesModel",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "BestResult",
                table: "AnalysesModel",
                newName: "NetworkTargets");

            migrationBuilder.RenameColumn(
                name: "RunId",
                table: "AnalysesModel",
                newName: "AnalysisId");

            migrationBuilder.AlterColumn<string>(
                name: "AnalysisName",
                table: "AnalysesModel",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "AnalysesModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GeneticElementsRandom",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneticMaxIteration",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneticMaxIterationNoImprovement",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneticMaxPathLength",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeneticPercentageElite",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "GeneticPercentageRandom",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneticPopulationSize",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneticRandomSeed",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GreedyCutNonBranching",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GreedyHeuristics",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GreedyMaxIteration",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GreedyMaxIterationNoImprovement",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GreedyMaxPathLength",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GreedyRandomSeed",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkBestResultCount",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkBestResultNodes",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkDrugTargetCount",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkDrugTargets",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkEdgeCount",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkEdges",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkNodeCount",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NetworkNodes",
                table: "AnalysesModel",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NetworkTargetCount",
                table: "AnalysesModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticElementsRandom",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticMaxIteration",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticMaxIterationNoImprovement",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticMaxPathLength",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticPercentageElite",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticPercentageRandom",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticPopulationSize",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GeneticRandomSeed",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyCutNonBranching",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyHeuristics",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyMaxIteration",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyMaxIterationNoImprovement",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyMaxPathLength",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "GreedyRandomSeed",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkBestResultCount",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkBestResultNodes",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkDrugTargetCount",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkDrugTargets",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkEdgeCount",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkEdges",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkNodeCount",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkNodes",
                table: "AnalysesModel");

            migrationBuilder.DropColumn(
                name: "NetworkTargetCount",
                table: "AnalysesModel");

            migrationBuilder.RenameColumn(
                name: "UserGivenTarget",
                table: "AnalysesModel",
                newName: "Target");

            migrationBuilder.RenameColumn(
                name: "UserGivenNodes",
                table: "AnalysesModel",
                newName: "NetNodes");

            migrationBuilder.RenameColumn(
                name: "UserGivenNetworkType",
                table: "AnalysesModel",
                newName: "NetType");

            migrationBuilder.RenameColumn(
                name: "UserGivenDrugTarget",
                table: "AnalysesModel",
                newName: "Network");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AnalysesModel",
                newName: "DrugTarget");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "AnalysesModel",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "NetworkTargets",
                table: "AnalysesModel",
                newName: "BestResult");

            migrationBuilder.RenameColumn(
                name: "GreedyCutToDriven",
                table: "AnalysesModel",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "GeneticProbabilityMutation",
                table: "AnalysesModel",
                newName: "Progress");

            migrationBuilder.RenameColumn(
                name: "AnalysisId",
                table: "AnalysesModel",
                newName: "RunId");

            migrationBuilder.AlterColumn<string>(
                name: "AnalysisName",
                table: "AnalysesModel",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "AlgorithmParams",
                table: "AnalysesModel",
                nullable: false,
                defaultValue: "");
        }
    }
}
