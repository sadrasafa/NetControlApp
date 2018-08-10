using NetControlApp.Algorithms;
using NetControlApp.Data;
using NetControlApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Services
{
    public class AnalysisRun : IAnalysisRun
    {
        private ApplicationDbContext _context;

        public AnalysisRun(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Runs the selected algorithm on the analysis with the given ID.
        /// </summary>
        /// <param name="analysisId">The unique database ID of the analysis on which to run the algorithm.</param>
        public async Task RunAnalysis(int analysisId)
        {
            // The given separators for the texts.
            var separators = new List<String>() { ";", "\t", "\r", "\n" };
            var analysisModel = _context.AnalysisModel.First(a => a.AnalysisId == analysisId);

            // Checks the type of the algorithm and selects the corresponding function.
            if (analysisModel.AlgorithmType == "greedy")
            {
                await RunGreedyAlgorithm(analysisModel, separators);
            }
            else
            {
                await RunGeneticAlgorithm(analysisModel, separators);
            }
        }

        /// <summary>
        /// Runs the genetic algorithm on the given analysis.
        /// </summary>
        /// <param name="analysisModel">The analysis upon which to run the algorithm.</param>
        /// <param name="separators">The separators which are used to split the entries in the database strings.</param>
        /// <returns></returns>
        private async Task RunGeneticAlgorithm(AnalysisModel analysisModel, List<String> separators)
        {
            // Get the initial list of nodes and edges from the generated network.
            var oldNodes = AlgorithmFunctions.GetNodes(analysisModel.NetworkEdges, separators);
            var oldEdges = AlgorithmFunctions.GetEdges(analysisModel.NetworkEdges, separators);
            var oldTargets = AlgorithmFunctions.GetTargetNodes(analysisModel.NetworkTargets, oldNodes, separators);
            var oldTargetIndices = AlgorithmGeneticFunctions.GetTargetIndices(oldNodes, oldTargets);
            var oldA = AlgorithmGeneticFunctions.GetAdjacencyMatrix(oldNodes, oldEdges);
            var oldPowersA = AlgorithmGeneticFunctions.GetAdjacencyMatrixPowers(oldA, analysisModel.GeneticMaxPathLength.Value);
            var oldList = AlgorithmGeneticFunctions.GetList(oldPowersA, oldTargetIndices);

            // Get the trimmed list of nodes and edges.
            var nodes = AlgorithmGeneticFunctions.GetNewNodes(oldNodes, oldList);
            var edges = AlgorithmGeneticFunctions.GetNewEdges(oldEdges, nodes);
            var singleNodes = AlgorithmGeneticFunctions.GetSingleTargets(nodes, oldTargets);
            var A = AlgorithmGeneticFunctions.GetAdjacencyMatrix(nodes, edges);
            var targets = AlgorithmGeneticFunctions.GetNewTargets(nodes, oldTargets);
            var drugTargets = analysisModel.NetworkDrugTargetCount.Value != 0 ? AlgorithmFunctions.GetTargetNodes(analysisModel.NetworkDrugTargets, nodes, separators) : null;
            var targetIndices = AlgorithmGeneticFunctions.GetTargetIndices(nodes, targets);
            var C = AlgorithmGeneticFunctions.GetTargetMatrix(nodes, targetIndices);
            var powersA = AlgorithmGeneticFunctions.GetAdjacencyMatrixPowers(A, analysisModel.GeneticMaxPathLength.Value);
            var powers = AlgorithmGeneticFunctions.GetTargetMatrixPowers(C, powersA);
            var list = AlgorithmGeneticFunctions.GetList(powersA, targetIndices);

            // Initialization of the first population.
            var rand = new Random(analysisModel.GeneticRandomSeed.Value);
            var bestFitness = 0.0;
            var p = new Population(analysisModel.GeneticPopulationSize.Value);
            p.Initialize(powers, list, analysisModel.GeneticElementsRandom.Value, rand);

            // Running for the given number of iterations.
            while (analysisModel.AlgorithmCurrentIteration < analysisModel.GeneticMaxIteration && analysisModel.AlgorithmCurrentIterationNoImprovement < analysisModel.GeneticMaxIterationNoImprovement && !analysisModel.ScheduledToStop.Value)
            {
                // Move on to the next population.
                p = p.nextPopulation(analysisModel.GeneticPercentageElite.Value, analysisModel.GeneticPercentageRandom.Value,
                    analysisModel.GeneticProbabilityMutation.Value, powers, list, analysisModel.GeneticElementsRandom.Value, rand);
                // Get the best fitness of the current population.
                var fitness = p.getBestFitness();
                // If the fitness is better than the current best one.
                if (bestFitness <= fitness)
                {
                    if (bestFitness < fitness)
                    {
                        // Update the current best fitness.
                        bestFitness = fitness;
                        analysisModel.AlgorithmCurrentIterationNoImprovement = -1;
                    }
                    // Get the best results.
                    var bestChromosomes = p.GetBestChromosomes();
                    var result = "";
                    foreach (var chromosome in bestChromosomes)
                    {
                        foreach (var gene in chromosome.Genes.Distinct())
                        {
                            result += nodes[gene] + ";";
                        }
                        result += "\n";
                    }
                    analysisModel.NetworkBestResultCount = bestChromosomes.First().Genes.Distinct().Count();
                    analysisModel.NetworkBestResultNodes = result;
                    analysisModel.AlgorithmCurrentIterationNoImprovement++;
                }
                else
                {
                    analysisModel.AlgorithmCurrentIterationNoImprovement++;
                }
                analysisModel.AlgorithmCurrentIteration++;
                analysisModel.Status = $"Analysis ongoing ({analysisModel.AlgorithmCurrentIteration} / {analysisModel.GeneticMaxIteration}, " +
                    $"{analysisModel.AlgorithmCurrentIterationNoImprovement} / {analysisModel.GeneticMaxIterationNoImprovement}).";
                await _context.SaveChangesAsync();
            }
            analysisModel.Status = "Completed";
            analysisModel.EndTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Runs the greedy algorithm on the given analysis.
        /// </summary>
        /// <param name="analysisModel">The analysis upon which to run the algorithm.</param>
        /// <param name="separators">The separators which are used to split the entries in the database strings.</param>
        /// <returns></returns>
        private async Task RunGreedyAlgorithm(AnalysisModel analysisModel, List<String> separators)
        {
            throw new NotImplementedException();
        }
    }
}
