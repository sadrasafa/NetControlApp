using Microsoft.EntityFrameworkCore;
using NetControlApp.Data;
using NetControlApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetControlApp.Algorithms
{
    /// <summary>
    /// Usage example for running the genetic algorithm with a given graph.
    /// </summary>
    public class AlgorithmFunctions
    {
        /// <summary>
        /// Reads the user provided fields, and generates the network based on them.
        /// </summary>
        /// <param name="analysisModel">The analysis model to parse.</param>
        /// <returns>True if the network could be generate with no issues, false otherwise.</returns>
        public static bool GenerateNetwork(AnalysisModel analysisModel)
        {
            // The separators, which might be also provided as a parameter to the function.
            var toReturn = false;
            var separators = new string[] { ";", "\t", "\n", "\r" };
            // Checks if the network is provided as seed nodes.
            if (analysisModel.UserIsNetworkSeed)
            {
                analysisModel.Status = "The seed nodes part of the program is not yet implemented.";
            }
            else
            {
                // If the network is given as edges - pairs of nodes.
                var edges = Functions.GetEdges(analysisModel.UserGivenNodes, separators);
                // If there is an uneven number of nodes, return an error.
                if (edges.Count() % 2 != 0)
                {
                    analysisModel.Status = "An odd number of nodes is given as the network edges.";
                }
                else
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
                    var nodes = Functions.GetNodes(uniqueEdges.ToArray());
                    var targets = Functions.GetTargets(analysisModel.UserGivenTarget, nodes, separators);
                    // If none of the target nodes could be found in the network, return an error.
                    if (targets.Count() == 0)
                    {
                        analysisModel.Status = "None of the given target nodes could be found in the network.";
                        toReturn = false;
                    }
                    else
                    {
                        analysisModel.NetworkEdgeCount = uniqueEdges.Count() / 2;
                        analysisModel.NetworkEdges = "";
                        foreach (var node in uniqueEdges)
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
                            analysisModel.NetworkDrugTargetCount = 0;
                            analysisModel.NetworkDrugTargets = null;
                        }
                        UpdateParameters(analysisModel);
                        analysisModel.Status = "Networks generated and saved into the database.";
                        toReturn = true;
                    }
                }
            }
            return toReturn;
        }

        public static void UpdateParameters(AnalysisModel analysisModel)
        {
            // Remove the parameters of the unused algorithm, and set the empty
            // parameters of the used algorithm to the default values.
            if (analysisModel.UserIsNetworkSeed == false)
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
    }
}
