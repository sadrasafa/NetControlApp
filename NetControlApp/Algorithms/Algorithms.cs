using NetControlApp.Data;
using NetControlApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Algorithms
{
    /// <summary>
    /// Usage example for running the genetic algorithm with a given graph.
    /// </summary>
   public class Algorithms
    {
        /// <summary>
        /// Reads the user provided network file and writes it to the database.
        /// </summary>
        /// <param name="AnalysisId">The unique analysis id in the database.</param>
        /// <param name="_context">The database context.</param>
        /// <returns>True if the given fields have no conflicts, false otherwise.</returns>
        public static async Task WriteToDatabase(int AnalysisId, ApplicationDbContext  _context)
        {
            // The separators, which might be also provided as a parameter to the function.
            var separators = new string[] { ";", "\t", "\n", "\r"};
            // Read the model and updates the status.
            var analysisModel = await _context.AnalysisModel.FindAsync(AnalysisId);
            _context.Update(analysisModel);
            analysisModel.Status = "Generating and saving the networks to the database.";
            await _context.SaveChangesAsync();
            // Save the status change in the database.
            _context.Update(analysisModel);
            // Check if the network was given as seed nodes.
            if (analysisModel.UserGivenNetworkType)
            {
                analysisModel.NetworkEdges = "seed";
            }
            else
            {
                // Parse the edges from the text.
                var edges = analysisModel.UserGivenNodes.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                // If there is an odd number of nodes in the "edges" field, return an error.
                var limit = edges.Count();
                if (edges.Count() % 2 == 0)
                {
                    // Remove the duplicate edges from the list.
                    var uniqueEdges = new List<String>(edges.Count());
                    for (int i = 0; i < edges.Count(); i = i + 2)
                    {
                        bool exists = false;
                        for (int j = 0; j < uniqueEdges.Count(); j = j + 2)
                        {
                            if (edges[i] == uniqueEdges[j] && edges[i + 1] == uniqueEdges[j + 1])
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            uniqueEdges.Add(edges[i]);
                            uniqueEdges.Add(edges[i + 1]);
                        }
                    }
                    var nodes = edges.Distinct();
                    var targetNodes = analysisModel.UserGivenTarget.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    analysisModel.NetworkEdgeCount = uniqueEdges.Count() / 2;
                    analysisModel.NetworkNodeCount = nodes.Count();
                    analysisModel.NetworkTargetCount = targetNodes.Count();
                    analysisModel.NetworkEdges = "";
                    analysisModel.NetworkNodes = "";
                    analysisModel.NetworkTargets = "";
                    foreach (var edge in uniqueEdges)
                    {
                        analysisModel.NetworkEdges += edge + ";";
                    }
                    foreach (var node in nodes)
                    {
                        analysisModel.NetworkNodes += node + ";";
                    }
                    foreach (var target in targetNodes)
                    {
                        analysisModel.NetworkTargets += target + ";";
                    }
                    if (analysisModel.UserGivenDrugTarget != null)
                    {
                        var drugTargetNodes = analysisModel.UserGivenDrugTarget.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        analysisModel.NetworkDrugTargetCount = drugTargetNodes.Count();
                        analysisModel.NetworkDrugTargets = "";
                        foreach (var target in targetNodes)
                        {
                            analysisModel.NetworkTargets += target + ";";
                        }
                    }
                    else
                    {
                        analysisModel.NetworkDrugTargetCount = 0;
                        analysisModel.NetworkDrugTargets = "";
                    }
                    if (analysisModel.NetworkTargetCount == 0)
                    {
                        analysisModel.Status = "No target nodes were found in the database. Analysis stopped.";
                        analysisModel.ScheduledToStop = true;
                    }
                    else
                    {
                        analysisModel.Status = "Network generated and saved into the database.";
                    }
                }
                else
                {
                    analysisModel.Status = "The given network edges have an odd number of nodes. Analysis stopped.";
                    analysisModel.ScheduledToStop = true;
                }
                
            }
            await _context.SaveChangesAsync();
        }

        public static bool GenNetwork(AnalysisModel analysisModel)
        {
            // The separators, which might be also provided as a parameter to the function.
            var toReturn = false;
            var separators = new string[] { ";", "\t", "\n", "\r" };
            if (analysisModel.UserGivenNetworkType)
            {
                // Seed nodes.
            }
            else
            {
                var edges = Functions.GetEdges(analysisModel.UserGivenNodes, separators);
                var nodes = Functions.GetNodes(edges);
                var targets = Functions.GetTargets(analysisModel.UserGivenTarget, nodes, separators);
                if (targets.Count() == 0)
                {
                    analysisModel.Status = "No targets found.";
                    toReturn = false;
                }
                else
                {
                    if (analysisModel.UserGivenDrugTarget != null)
                    {
                        var drugTargets = Functions.GetDrugTargets(analysisModel.UserGivenDrugTarget, nodes, separators);
                        analysisModel.NetworkDrugTargetCount = drugTargets.Count();
                        analysisModel.NetworkDrugTargets = "";
                        foreach (var node in drugTargets)
                        {
                            analysisModel.NetworkDrugTargets += node + ";";
                        }
                    }
                    else
                    {
                        analysisModel.NetworkDrugTargets = null;
                        analysisModel.NetworkDrugTargetCount = 0;
                    }
                    analysisModel.NetworkEdgeCount = edges.Count() / 2;
                    analysisModel.NetworkEdges = "";
                    foreach (var node in edges)
                    {
                        analysisModel.NetworkEdges += node + ";";
                    }
                    analysisModel.NetworkNodeCount = nodes.Count();
                    analysisModel.NetworkNodes = "";
                    foreach (var node in nodes)
                    {
                        analysisModel.NetworkNodes += node + ";";
                    }
                    analysisModel.NetworkTargetCount = targets.Count();
                    analysisModel.NetworkTargets = "";
                    foreach (var node in targets)
                    {
                        analysisModel.NetworkTargets += node + ";";
                    }
                    analysisModel.Status = "Everything saved correctly.";
                    toReturn = true;
                }
            }
            return toReturn;
        }

        public static void GenerateNetwork(AnalysisModel analysisModel)
        {
            // The separators, which might be also provided as a parameter to the function.
            var separators = new string[] { ";", "\t", "\n", "\r" };
            // Check if the network was given as seed nodes.
            if (analysisModel.UserGivenNetworkType)
            {
                analysisModel.NetworkEdges = "seed";
            }
            else
            {
                // Parse the edges from the text.
                var edges = analysisModel.UserGivenNodes.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                // If there is an odd number of nodes in the "edges" field, return an error.
                if (edges.Count() % 2 == 0)
                {
                    // Remove the duplicate edges from the list.
                    var uniqueEdges = new List<String>(edges.Count());
                    for (int i = 0; i < edges.Count(); i = i + 2)
                    {
                        bool exists = false;
                        for (int j = 0; j < uniqueEdges.Count(); j = j + 2)
                        {
                            if (edges[i] == uniqueEdges[j] && edges[i + 1] == uniqueEdges[j + 1])
                            {
                                exists = true;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            uniqueEdges.Add(edges[i]);
                            uniqueEdges.Add(edges[i + 1]);
                        }
                    }
                    var nodes = edges.Distinct();
                    var targetNodes = nodes.Intersect(analysisModel.UserGivenTarget.Split(separators, StringSplitOptions.RemoveEmptyEntries)).Distinct();
                    analysisModel.NetworkEdgeCount = uniqueEdges.Count() / 2;
                    analysisModel.NetworkNodeCount = nodes.Count();
                    analysisModel.NetworkTargetCount = targetNodes.Count();
                    analysisModel.NetworkEdges = "";
                    analysisModel.NetworkNodes = "";
                    analysisModel.NetworkTargets = "";
                    foreach (var edge in uniqueEdges)
                    {
                        analysisModel.NetworkEdges += edge + ";";
                    }
                    foreach (var node in nodes)
                    {
                        analysisModel.NetworkNodes += node + ";";
                    }
                    foreach (var target in targetNodes)
                    {
                        analysisModel.NetworkTargets += target + ";";
                    }
                    if (analysisModel.UserGivenDrugTarget != null)
                    {
                        var drugTargetNodes = analysisModel.UserGivenDrugTarget.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        analysisModel.NetworkDrugTargetCount = drugTargetNodes.Count();
                        analysisModel.NetworkDrugTargets = "";
                        foreach (var target in targetNodes)
                        {
                            analysisModel.NetworkTargets += target + ";";
                        }
                    }
                    else
                    {
                        analysisModel.NetworkDrugTargetCount = 0;
                        analysisModel.NetworkDrugTargets = "";
                    }
                    if (analysisModel.NetworkTargetCount == 0)
                    {
                        analysisModel.Status = "No target nodes were found in the database. Can't create analysis.";
                        analysisModel.ScheduledToStop = true;
                    }
                    else
                    {
                        analysisModel.Status = "Network generated and saved into the database.";
                    }
                }
                else
                {
                    analysisModel.Status = "The given network edges have an odd number of nodes. Can't create analysis.";
                    analysisModel.ScheduledToStop = true;
                }

            }
        }

        public static void UpdateParameters(AnalysisModel analysisModel)
        {
            // Remove the parameters of the unused algorithm, and set the empty
            // parameters of the used algorithm to the default values.
            if (analysisModel.UserGivenNetworkType == false)
            {
                analysisModel.UserGivenNetworkGeneration = null;
            }
            if (analysisModel.AlgorithmType == "genetic")
            {
                if (analysisModel.GeneticRandomSeed == null)
                {
                    analysisModel.GeneticRandomSeed = (new Random()).Next();
                }
                if (analysisModel.GeneticMaxIteration == null)
                {
                    analysisModel.GeneticMaxIteration = 10000;
                }
                if (analysisModel.GeneticMaxIterationNoImprovement == null)
                {
                    analysisModel.GeneticMaxIterationNoImprovement = null;
                }
                if (analysisModel.GeneticMaxPathLength == null)
                {
                    analysisModel.GeneticMaxPathLength = 5;
                }
                if (analysisModel.GeneticPopulationSize == null)
                {
                    analysisModel.GeneticPopulationSize = 80;
                }
                if (analysisModel.GeneticElementsRandom == null)
                {
                    analysisModel.GeneticElementsRandom = 25;
                }
                if (analysisModel.GeneticPercentageRandom == null)
                {
                    analysisModel.GeneticPercentageRandom = 0.25;
                }
                if (analysisModel.GeneticPercentageElite == null)
                {
                    analysisModel.GeneticPercentageElite = 0.25;
                }
                if (analysisModel.GeneticProbabilityMutation == null)
                {
                    analysisModel.GeneticProbabilityMutation = 0.001;
                }
            }
            // Remove from the model the parameters of the unused algorithm type.
            else if (analysisModel.AlgorithmType == "greedy")
            {
                if (analysisModel.GreedyRandomSeed == null)
                {
                    analysisModel.GreedyRandomSeed = (new Random()).Next();
                }
                if (analysisModel.GreedyMaxIteration == null)
                {
                    analysisModel.GreedyMaxIteration = 10000;
                }
                if (analysisModel.GreedyMaxIterationNoImprovement == null)
                {
                    analysisModel.GreedyMaxIterationNoImprovement = null;
                }
                if (analysisModel.GreedyMaxPathLength == null)
                {
                    analysisModel.GreedyMaxPathLength = 0;
                }
                if (analysisModel.GreedyCutToDriven == null)
                {
                    analysisModel.GreedyCutToDriven = true;
                }
                if (analysisModel.GreedyCutNonBranching == null)
                {
                    analysisModel.GreedyCutNonBranching = false;
                }
                if (analysisModel.GreedyHeuristics == null)
                {
                    analysisModel.GreedyHeuristics = "(->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T)";
                }
            }
        }

        public static List<List<String>> UsageGeneticNoSeed()
        {
            var separators = new string[] { "\t", "\n", "\r", ";" };

            //
            // User input starts here.
            //

            // The graph text given in the JSON file.
            var isSeed = false;
            var graphText = "1;4\n1;5\n2;5\n2;6\n3;7\n4;11\n5;4\n5;9\n5;10\n6;8\n7;8\n9;8\n9;15\n10;14\n11;12\n11;13\n12;13\n13;16\n14;5\n17;16";

            // The target text given in the JSON file.
            var targetText = "11;8;10;13;17";

            // The drug target text given in the JSON file (optional)
            var drugTargetText = "";

            // The choice of algorithm.
            var algorithm = 0;

            // Parameters for the algorithm.
            var elitismPercentage = 0.25;
            var randomPercentage = 0.25;
            var mutationProbability = 0.001;
            var maximumPower = 5;
            var maximumRandom = 15;
            var populationSize = 80;
            var numberOfGenerations = 10000;
            var maximumGenerationsWithoutImprovement = 1000;
            var randomSeed = new Random().Next();

            //
            // User input ends here.
            //

            //
            // The algorithm outline starts here.
            //

            if (isSeed)
            {
                // Generate the network based on seed.
            }
            else
            {
                // Just get the network from the given text.
            }
            // Get the nodes and the targets, using the functions from "Functions".
            if (algorithm == 0)
            {
                // Compute other things needed for the genetic algorithm and run it.
            }
            else if (algorithm == 1) {
                // Compute other things needed for the greedy algrotihm and run it.
            }

            //
            // The algorithmoutline ends here.
            //
            
            //
            // The actual genetic algorithm run in our case starts here.
            //

            // Get the initial list of nodes and edges, based on the given text (user input).
            var oldEdges = Functions.GetEdges(graphText, separators);
            var oldNodes = Functions.GetNodes(oldEdges);
            var oldTargets = Functions.GetTargets(targetText, oldNodes, separators);
            var oldTargetIndices = Functions.GetTargetIndices(oldNodes, oldTargets);
            var oldA = Functions.GetAdjacencyMatrix(oldNodes, oldEdges);
            var oldPowersA = Functions.GetAdjacencyMatrixPowers(oldA, maximumPower);
            var oldList = Functions.GetList(oldPowersA, oldTargetIndices);

            // Get the optimized list of nodes and edges
            var edges = Functions.GetNewEdges(oldNodes, oldEdges, oldList);
            var nodes = Functions.GetNodes(edges);
            var singleNodes = Functions.GetSingleNodes(oldNodes, nodes, oldList);
            var A = Functions.GetAdjacencyMatrix(nodes, edges);
            var targets = Functions.GetNewTargets(oldTargets, singleNodes);
            var drugTargets = drugTargetText != "" ? Functions.GetDrugTargets(drugTargetText, nodes, separators) : null;
            var targetIndices = Functions.GetTargetIndices(nodes, targets);
            var C = Functions.GetTargetMatrix(nodes, targetIndices);
            var powersA = Functions.GetAdjacencyMatrixPowers(A, maximumPower);
            var powers = Functions.GetTargetMatrixPowers(C, powersA);
            var list = Functions.GetList(powersA, targetIndices);

            // Initialization of the first population.
            var rand = new Random(randomSeed);
            var bestFitness = 0.0;
            var generationsSinceLastImprovement = 0;
            var currentGeneration = 0;
            var p = new Population(populationSize);
            p.Initialize(powers, list, maximumRandom, rand);

            // Running for the given number of generations.
            for (int i = 0; i < numberOfGenerations; i++)
            {
                currentGeneration = i;
                p = p.nextPopulation(elitismPercentage, randomPercentage, mutationProbability, powers, list, maximumRandom, rand, nodes);
                var fitness = p.getBestFitness();
                if (fitness > bestFitness)
                {
                    bestFitness = fitness;
                    generationsSinceLastImprovement = 0;
                }
                else
                {
                    generationsSinceLastImprovement++;
                }
                if (generationsSinceLastImprovement == maximumGenerationsWithoutImprovement)
                {
                    break;
                }
            }

            // Getting the results.
            var bestChromosomes = p.GetBestChromosomes();

            //
            // The genetic algorithm ends here.
            //

            // Getting the results in a returnable form.
            var results = new List<List<String>>(bestChromosomes.Count);

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

            return results;
        }

        /// <summary>
        /// Usage example for running the genetic algorithm with given seed nodes.
        /// </summary>
        public static void UsageGeneticSeed()
        {

        }
    }
}
