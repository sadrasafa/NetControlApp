using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetControlApp.Data;
using NetControlApp.Models;
using Microsoft.AspNetCore.Identity;

namespace NetControlApp.Algorithms
{
    public class GenerateData
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly int _index;
        public GenerateData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, int index)
        {
            _index = index;
            _context = context;
            _userManager = userManager;
        }

        public void Generate()
        {
            var values = _context.AnalysisModel.Find(_index);


            var separators = new string[] { "\t", "\n", "\r", ";" };

            //
            // User input starts here.
            //

            // The graph text given in the JSON file.

            // The target text given in the JSON file.


            // The drug target text given in the JSON file (optional)

            // The choice of algorithm.
            var algorithm = values.AlgorithmType;


            // Parameters for the algorithm.
            var elitismPercentage = values.GeneticPercentageElite ?? 0.25;
            var randomPercentage = values.GeneticPercentageRandom ?? 0.25;
            var mutationProbability = values.GeneticProbabilityMutation ?? 0.001;
            var maximumPower = values.GeneticMaxPathLength ?? 5; //
            var maximumRandom = values.GeneticElementsRandom ?? 15; //
            var populationSize = values.GeneticPopulationSize ?? 80;
            var numberOfGenerations = values.GeneticMaxIteration ?? 10000;
            var maximumGenerationsWithoutImprovement = values.GeneticMaxIterationNoImprovement ?? 1000;
            var randomSeed = new Random().Next();

            // Get the initial list of nodes and edges, based on the given text (user input).
            var oldEdges = Functions.GetEdges(values.NetworkEdges, separators);
            var oldNodes = Functions.GetNodes(oldEdges);
            var oldTargets = Functions.GetTargets(values.NetworkTargets, oldNodes, separators);
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
            //var drugTargets = drugTargetText != "" ? Functions.GetDrugTargets(drugTargetText, nodes, separators) : null;
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

            foreach (var lista in results)
            {
                foreach (var node in lista)
                {
                    resultString += node + ";";
                }
                resultString += "\n";
            }
            values.NetworkBestResultNodes = resultString;
            _context.SaveChanges();
        }

    }
}
