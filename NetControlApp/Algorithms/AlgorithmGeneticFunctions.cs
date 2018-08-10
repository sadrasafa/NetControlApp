using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetControlApp.Algorithms
{
    class AlgorithmGeneticFunctions
    {
        /// <summary>
        /// Computes the adjacency matrix from the given text (graph), based on the list of nodes provided.
        /// </summary>
        /// <param name="nodes">The array containing the current list of nodes, previously computed.</param>
        /// <param name="edges">The array containing the current list of edges, previously computed.</param>
        /// <returns>The adjacency matrix of the given graph.</returns>
        public static Matrix<Double> GetAdjacencyMatrix(List<String> nodes, List<(String, String)> edges)
        {
            var A = Matrix<Double>.Build.DenseDiagonal(nodes.Count, nodes.Count, 0.0);
            foreach (var edge in edges)
            {
                A[nodes.IndexOf(edge.Item2), nodes.IndexOf(edge.Item1)] = 1.0;
            }
            return A;
        }

        /// <summary>
        /// Computes the indices of the target nodes in the list of nodes.
        /// </summary>
        /// <param name="nodes">The complete list of nodes.</param>
        /// <param name="targets">The list of target nodes.</param>
        /// <returns>The array containing, in order, the indices of the target nodes.</returns>
        public static List<Int32> GetTargetIndices(List<String> nodes, List<String> targets)
        {
            var targetIndices = new List<Int32>(targets.Count);
            // For each node in the list of target nodes,
            foreach (var target in targets)
            {
                // We add to targetIndices the index of the target node in the list of nodes.
                targetIndices.Add(nodes.IndexOf(target));
            }
            return targetIndices;
        }

        /// <summary>
        /// Forms the target-corresponding matrix C. It will be a submatrix of the identity matrix.
        /// </summary>
        /// <param name="nodes">The complete list of nodes.</param>
        /// <param name="targetIndices">The indices of the target nodes.</param>
        /// <returns>The target matrix C.</returns>
        public static Matrix<Double> GetTargetMatrix(List<String> nodes, List<Int32> targetIndices)
        {
            // We begin from an all-zero matrix.
            var C = Matrix<Double>.Build.Dense(targetIndices.Count, nodes.Count);
            // For every target node,
            for (int i = 0; i < targetIndices.Count; i++)
            {
                // We set the corresponding entry in the matrix to 1.0.
                C[i, targetIndices[i]] = 1.0;
            }
            // And we return the matrix.
            return C;
        }

        /// <summary>
        /// Computes the powers of the adjacency matrix A, up to a given maximum power.
        /// </summary>
        /// <param name="A">The adjacency matrix of the graph.</param>
        /// <param name="maximumPower">The maximum power up to which the computation goes.</param>
        /// <returns>An array containing the powers of the matrix. The array goes as: I, A, A^2, ... .</returns>
        public static List<Matrix<Double>> GetAdjacencyMatrixPowers(Matrix<Double> A, Int32 maximumPower)
        {
            var powers = new List<Matrix<Double>>(maximumPower + 1);
            powers.Add(Matrix<Double>.Build.DenseIdentity(A.RowCount));
            for (int i = 1; i < maximumPower + 1; i++)
            {
                Console.WriteLine($"Computing the powers ({i}/{maximumPower})");
                powers.Add(A.Multiply(powers[i - 1]));
            }
            return powers;
        }

        /// <summary>
        /// Computes the powers of the combination between the target matrix C and the adjacency matrix A, to be used in validating the chromosomes.
        /// </summary>
        /// <param name="C">The target-corresponding matrix C.</param>
        /// <param name="AdjacencyPowers">The array containing the powers of the adjacency matrix A.</param>
        /// <returns>An array containing the powers of the matrices. The array goes as C, CA, CA^2, ... .</returns>
        public static List<Matrix<Double>> GetTargetMatrixPowers(Matrix<Double> C, List<Matrix<Double>> AdjacencyPowers)
        {
            var powers = new List<Matrix<Double>>(AdjacencyPowers.Count);
            foreach (var adjacencyPower in AdjacencyPowers)
            {
                powers.Add(C.Multiply(adjacencyPower));
            }
            return powers;
        }

        /// <summary>
        /// Computes the new list of nodes in the trimmed graph.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static List<String> GetNewNodes(List<String> nodes, List<List<Int32>> list)
        {
            var nodeIndices = new List<Int32>();
            list.ForEach((item) => nodeIndices.AddRange(item));
            nodeIndices = nodeIndices.Distinct().ToList();
            var newNodes = new List<String>();
            newNodes = nodeIndices.Select((item) => nodes[item]).ToList();
            return newNodes;
        }

        /// <summary>
        /// Computes the new list of edges in the trimmed graph.
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="newNodes"></param>
        /// <returns></returns>
        public static List<(String, String)> GetNewEdges(List<(String, String)> edges, List<String> newNodes)
        {
            return AlgorithmFunctions.GetEdgesFromSeed(edges, newNodes, 0);
        }

        /// <summary>
        /// Computes the new list of target nodes in the trimmed graph.
        /// </summary>
        /// <param name="newNodes"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static List<String> GetNewTargets(List<String> newNodes, List<String> targets)
        {
            return targets.Intersect(newNodes).ToList();
        }

        /// <summary>
        /// Computes the list of target nodes which will no longer appear in the trimmed graph.
        /// </summary>
        /// <param name="newNodes"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static List<String> GetSingleTargets(List<String> newNodes, List<String> targets)
        {
            return targets.Except(newNodes).ToList();
        }

        /// <summary>
        /// Checkes if a given sequence of genes can control the target nodes in the current graph.
        /// </summary>
        /// <param name="matrixPowers">The list of powers of the target matrix.</param>
        /// <param name="nodes">The nodes in the graph.</param>
        /// <param name="singleNodes">The single nodes in the graph.</param>
        /// <param name="genesToCheck">The list of genes to check.</param>
        /// <returns>True if the given genes fully control the target nodes, false otherwise.</returns>
        public static bool IsItValid(List<Matrix<Double>> matrixPowers, List<String> nodes, List<String> singleNodes, List<String> genesToCheck)
        {
            bool isValid = false;
            var genesId = new List<int>(genesToCheck.Count);
            var singlesId = new List<string>(singleNodes.Count);
            for (int i = 0; i < genesToCheck.Count; i++)
            {
                if (singleNodes.Contains(genesToCheck[i]))
                {
                    singlesId.Add(genesToCheck[i]);
                }
                else
                {
                    var toAdd = nodes.IndexOf(genesToCheck[i]);
                    if (toAdd == -1)
                    {
                        return false;
                    }
                    else
                    {
                        genesId.Add(toAdd);
                    }
                }
            }
            var form = genesId.Distinct().ToArray();
            var B = Matrix<Double>.Build.Dense(matrixPowers[0].ColumnCount, form.Length);
            for (int i = 0; i < form.Length; i++)
            {
                B[form[i], i] = 1.0;
            }
            var R = Matrix<Double>.Build.DenseOfMatrix(matrixPowers[0]).Multiply(B);
            int r = R.Rank();
            if (r == R.RowCount)
            {
                isValid = true;
            }
            else
            {
                for (int i = 1; i < matrixPowers.Count; i++)
                {
                    var M = Matrix<Double>.Build.DenseOfMatrix(matrixPowers[i]).Multiply(B);
                    R = R.Append(M);
                    r = R.Rank();
                    if (r == R.RowCount)
                    {
                        isValid = true;
                        break;
                    }
                }
            }
            if (isValid && singlesId.Count == singleNodes.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Computes, for every target node, the list of nodes from which it can be reached.
        /// </summary>
        /// <param name="AdjacencyPowers">The powers of the adjacency matrix A.</param>
        /// <param name="targetIndices">The indices of the target nodes.</param>
        /// <returns>The list of nodes from which the target nodes can be reached.</returns>
        public static List<List<Int32>> GetList(List<Matrix<Double>> AdjacencyPowers, List<Int32> targetIndices)
        {
            // Initialize the main list.
            var list = new List<List<Int32>>(targetIndices.Count);
            // Initialize the arrays 
            for (int i = 0; i < targetIndices.Count; i++)
            {
                list.Add(new List<Int32>());
            }
            // For every power of the adjacency matrix,
            for (int index1 = 0; index1 < AdjacencyPowers.Count; index1++)
            {
                // and for every target node,
                for (int index2 = 0; index2 < targetIndices.Count; index2++)
                {
                    // get corresponding row,
                    var row = AdjacencyPowers[index1].Row(targetIndices[index2]);
                    // get all of the non-zero entries in the row,
                    var nz = row.Select((value, index) => value != 0 ? index : -1).Where(i => i != -1);
                    // And add them to the corresponding list.
                    list[index2].AddRange(nz);
                }
            }
            // For every target node,
            for (int i = 0; i < list.Count; i++)
            {
                // remove the duplicate nodes and add them to the array.
                list[i] = list[i].Distinct().ToList();
            }
            return list;
        }

        /// <summary>
        /// The initial testing program of the genetic algorithm.
        /// </summary>
        public static void InitialProgram()
        {
            var haveDrugTarget = false;
            var elitismPercentage = 0.25;
            var randomPercentage = 0.25;
            var mutationProbability = 0.001;
            var maximumPower = 5;
            var maximumRandom = 15;
            var populationSize = 80;
            var numberOfGenerations = 10000;
            var maximumGenerationsWithoutImprovement = 1000;
            var randomSeed = new Random().Next();

            var haveSeedNodes = false;
            var fullNetworkPath = $@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\FullNetwork.txt";
            var buildAlgorithm = 2;

            //var isSeed = false;
            //var graphText = "1;4\n1;5\n2;5\n2;6\n3;7\n4;11\n5;4\n5;9\n5;10\n6;8\n7;8\n9;8\n9;15\n10;14\n11;12\n11;13\n12;13\n13;16\n14;5\n17;16";

            var separators = new List<String>() { ";", "\t", "\n", "\r" };
            var logFilepath = $@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Logs\Log{DateTime.Now.Month}--{DateTime.Now.Day}--{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.txt";
            var graphType = "Test";
            var rand = new Random(randomSeed);

            var graphText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Graph.txt");
            var targetText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Target.txt");
            var drugTargetText = haveDrugTarget ? System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\DrugTarget.txt") : null;
            var seedNodesText = haveSeedNodes ? System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Target.txt") : null;

            var oldEdges = new List<(String, String)>();
            if (haveSeedNodes)
            {
                var fullNetworkText = System.IO.File.ReadAllText(fullNetworkPath);
                var fullNetworkEdges = AlgorithmFunctions.GetEdges(fullNetworkText, separators);
                var seedNodes = AlgorithmFunctions.GetNodes(seedNodesText, separators);
                Console.WriteLine($"Seed nodes: {seedNodes.Count}");
                oldEdges = AlgorithmFunctions.GetEdgesFromSeed(fullNetworkEdges, seedNodes, buildAlgorithm);
            }
            else
            {
                Console.WriteLine($"No seed nodes.");
                oldEdges = AlgorithmFunctions.GetEdges(graphText, separators);
            }
            var oldNodes = AlgorithmFunctions.GetNodes(graphText, separators);
            var oldTargets = AlgorithmFunctions.GetTargetNodes(targetText, oldNodes, separators);
            var oldDrugTargets = haveDrugTarget ? AlgorithmFunctions.GetTargetNodes(drugTargetText, oldNodes, separators) : null;
            Console.WriteLine($"Old nodes: {oldNodes.Count}");
            Console.WriteLine($"Old edges: {oldEdges.Count}");
            var oldA = AlgorithmGeneticFunctions.GetAdjacencyMatrix(oldNodes, oldEdges);
            Console.WriteLine($"Old target nodes: {oldTargets.Count}");
            if (haveDrugTarget)
            {
                Console.WriteLine($"Old drug target nodes: {oldDrugTargets.Count}");
            }
            var oldTargetIndices = AlgorithmGeneticFunctions.GetTargetIndices(oldNodes, oldTargets);
            var oldC = AlgorithmGeneticFunctions.GetTargetMatrix(oldNodes, oldTargetIndices);

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var oldPowersA = AlgorithmGeneticFunctions.GetAdjacencyMatrixPowers(oldA, maximumPower);
            var oldPowers = AlgorithmGeneticFunctions.GetTargetMatrixPowers(oldC, oldPowersA);
            var oldList = AlgorithmGeneticFunctions.GetList(oldPowersA, oldTargetIndices);
            watch.Stop();
            Console.WriteLine($"Old time: {watch.ElapsedMilliseconds}");

            var nodes = AlgorithmGeneticFunctions.GetNewNodes(oldNodes, oldList);
            var edges = AlgorithmGeneticFunctions.GetNewEdges(oldEdges, nodes);
            var singleNodes = AlgorithmGeneticFunctions.GetSingleTargets(nodes, oldTargets);
            Console.WriteLine($"New nodes: {nodes.Count}");
            Console.WriteLine($"Single nodes: {singleNodes.Count}");
            Console.WriteLine($"New edges: {edges.Count}");
            var A = AlgorithmGeneticFunctions.GetAdjacencyMatrix(nodes, edges);
            var targets = AlgorithmGeneticFunctions.GetNewTargets(nodes, oldTargets);
            Console.WriteLine($"Target nodes: {targets.Count}");
            var drugTargets = haveDrugTarget ? AlgorithmGeneticFunctions.GetNewTargets(nodes, oldDrugTargets) : null;
            if (haveDrugTarget)
            {
                Console.WriteLine($"Drug target nodes: {drugTargets.Count}");
            }
            var targetIndices = AlgorithmGeneticFunctions.GetTargetIndices(nodes, targets);
            var C = AlgorithmGeneticFunctions.GetTargetMatrix(nodes, targetIndices);

            watch.Restart();
            var powersA = AlgorithmGeneticFunctions.GetAdjacencyMatrixPowers(A, maximumPower);
            var powers = AlgorithmGeneticFunctions.GetTargetMatrixPowers(C, powersA);
            var list = AlgorithmGeneticFunctions.GetList(powersA, targetIndices);
            watch.Stop();
            Console.WriteLine($"New time: {watch.ElapsedMilliseconds}");

            watch.Restart();
            var bestFitness = 0.0;
            var generationsSinceLastImprovement = 0;
            var p = new Population(populationSize);
            p.Initialize(powers, list, maximumRandom, rand);
            for (int i = 0; i < numberOfGenerations; i++)
            {
                Console.WriteLine($"Population {i + 1} / {numberOfGenerations} ({generationsSinceLastImprovement} since last improvement)");
                p = p.nextPopulation(elitismPercentage, randomPercentage, mutationProbability, powers, list, maximumRandom, rand);
                var fitness = p.getBestFitness();
                Console.WriteLine($"Best fitness: {fitness}");
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
            watch.Stop();
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}");
            if (generationsSinceLastImprovement == maximumGenerationsWithoutImprovement)
            {
                Console.WriteLine($"Algorithm stopped after {generationsSinceLastImprovement} generations with no improvement.");
            }
            else
            {
                Console.WriteLine($"Algorithm stopped after {numberOfGenerations}.");
            }

            p.DisplayBest(nodes, singleNodes, drugTargets);
            p.WriteBest(logFilepath, nodes, singleNodes);
        }
    }
}
