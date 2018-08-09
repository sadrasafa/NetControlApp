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
            var separators = new String[] { ";", "\t", "\r", "\n" };
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
        private async Task RunGeneticAlgorithm(AnalysisModel analysisModel, String[] separators)
        {
            // Get the initial list of nodes and edges from the generated network.
            var oldEdges = Functions.GetEdges(analysisModel.NetworkEdges, separators);
            var oldNodes = Functions.GetNodes(oldEdges);
            var oldTargets = Functions.GetTargets(analysisModel.NetworkTargets, oldNodes, separators);
            var oldTargetIndices = Functions.GetTargetIndices(oldNodes, oldTargets);
            var oldA = Functions.GetAdjacencyMatrix(oldNodes, oldEdges);
            var oldPowersA = Functions.GetAdjacencyMatrixPowers(oldA, analysisModel.GeneticMaxPathLength.Value);
            var oldList = Functions.GetList(oldPowersA, oldTargetIndices);

            // Get the trimmed list of nodes and edges.
            var edges = Functions.GetNewEdges(oldNodes, oldEdges, oldList);
            var nodes = Functions.GetNodes(edges);
            var singleNodes = Functions.GetSingleNodes(oldNodes, nodes, oldList);
            var A = Functions.GetAdjacencyMatrix(nodes, edges);
            var targets = Functions.GetNewTargets(oldTargets, singleNodes);
            var drugTargets = analysisModel.NetworkDrugTargetCount.Value != 0 ? Functions.GetDrugTargets(analysisModel.NetworkDrugTargets, nodes, separators) : null;
            var targetIndices = Functions.GetTargetIndices(nodes, targets);
            var C = Functions.GetTargetMatrix(nodes, targetIndices);
            var powersA = Functions.GetAdjacencyMatrixPowers(A, analysisModel.GeneticMaxPathLength.Value);
            var powers = Functions.GetTargetMatrixPowers(C, powersA);
            var list = Functions.GetList(powersA, targetIndices);

            // Initialization of the first population.
            var rand = new Random(analysisModel.GeneticRandomSeed.Value);
            var bestFitness = 0.0;
            var generationsSinceLastImprovement = 0;
            var currentGeneration = 0;
            var p = new Population(analysisModel.GeneticPopulationSize.Value);
            p.Initialize(powers, list, analysisModel.GeneticElementsRandom.Value, rand);

            // Running for the given number of generations.
            for (int i = 0; i < analysisModel.GeneticMaxIteration; i++)
            {
                currentGeneration = i;
                p = p.nextPopulation(analysisModel.GeneticPercentageElite.Value, analysisModel.GeneticPercentageRandom.Value,
                    analysisModel.GeneticProbabilityMutation.Value, powers, list, analysisModel.GeneticElementsRandom.Value, rand, nodes);
                var fitness = p.getBestFitness();
                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    generationsSinceLastImprovement = 0;
                    // Get the better results.
                    var bestChromosomes = p.GetBestChromosomes();
                    var results = new List<List<String>>(bestChromosomes.Count);
                    var resultString = "";
                    foreach (var item in bestChromosomes)
                    {
                        var temporaryList = new List<String>();
                        foreach (var gene in item.Genes.Distinct())
                        {
                            temporaryList.Add(nodes[gene]);
                        }
                        foreach (var gene in singleNodes)
                        {
                            temporaryList.Add(gene);
                        }
                        results.Add(temporaryList);
                    }

                    foreach (var result in results)
                    {
                        foreach (var node in result)
                        {
                            resultString += node + ";";
                        }
                        resultString += "\n";
                    }

                    analysisModel.NetworkBestResultCount = bestChromosomes.First().Genes.Distinct().Count();
                    analysisModel.NetworkBestResultNodes = resultString;
                    _context.Update(analysisModel);
                }
                else
                {
                    generationsSinceLastImprovement++;
                }
                analysisModel.Status = $"Completed generation {i} / {analysisModel.GeneticMaxIteration}. {generationsSinceLastImprovement} / {analysisModel.GeneticMaxIterationNoImprovement} since last improvement.";
                await _context.SaveChangesAsync();
                if (generationsSinceLastImprovement == analysisModel.GeneticMaxIterationNoImprovement)
                {
                    break;
                }
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
        private async Task RunGreedyAlgorithm(AnalysisModel analysisModel, String[] separators)
        {
            throw new NotImplementedException();
        }
    }
}
