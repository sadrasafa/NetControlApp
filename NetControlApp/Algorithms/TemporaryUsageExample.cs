using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Algorithms
{
    /// <summary>
    /// Usage example for running the genetic algorithm with a given graph.
    /// </summary>
   public class TemporaryUsageExample
    {
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
