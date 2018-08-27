using Microsoft.EntityFrameworkCore;
using NetControlApp.Data;
using NetControlApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            // Fill in the fields which do not depend on the user input.
            analysisModel.StartTime = DateTime.Now;
            analysisModel.AlgorithmCurrentIteration = 0;
            analysisModel.AlgorithmCurrentIterationNoImprovement = 0;
            analysisModel.ScheduledToStop = false;

            // The separators, which might be also provided as a parameter to the function.
            var toReturn = false;
            var separators = new List<String>() { ";", "\t", "\n", "\r" };

            // Checks if the network is provided as seed nodes.
            if (analysisModel.UserIsNetworkSeed)
            {
                //analysisModel.Status = "The seed nodes part of the program is not yet implemented.";
                //toReturn = false;
                // Load the seed nodes from the given text.
                var seedText = analysisModel.UserGivenDrugTarget == null ? analysisModel.UserGivenNodes + ";" + analysisModel.UserGivenTarget : 
                    analysisModel.UserGivenNodes + ";" + analysisModel.UserGivenTarget + ";" + analysisModel.UserGivenDrugTarget;
                var seedNodes = AlgorithmFunctions.GetNodes(seedText, separators);
                // If there are no seed nodes, return an error.
                if (seedNodes.Count == 0)
                {
                    analysisModel.Status = "No seed nodes have been given.";
                    toReturn = false;
                }
                else
                {
                    // Reads the edges from the file.
                    var fullNetworkText = File.ReadAllText(@"Algorithms\OmnipathNetwork.txt");
                    var fullNetworkEdges = AlgorithmFunctions.GetEdges(fullNetworkText, separators);
                    // Or stream the full network from the file.
                    //Stream openFileStream = File.OpenRead(@"Algorithms\OmnipathNetwork.serialized");
                    //BinaryFormatter deserializer = new BinaryFormatter();
                    //var fullNetworkEdges = (List<(String, String)>)deserializer.Deserialize(openFileStream);
                    //openFileStream.Close();
                    // Identify the specified build algorithm.
                    var buildAlgorithm = analysisModel.UserGivenNetworkGeneration == "neighbors" ? -1 :
                        analysisModel.UserGivenNetworkGeneration == "gap0" ? 0 :
                        analysisModel.UserGivenNetworkGeneration == "gap1" ? 1 :
                        analysisModel.UserGivenNetworkGeneration == "gap2" ? 2 :
                        analysisModel.UserGivenNetworkGeneration == "gap2" ? 2 : -1;
                    // Generate the edges from the seed nodes based on the provided algorithm.
                    var edges = AlgorithmFunctions.GetEdgesFromSeed(fullNetworkEdges, seedNodes, buildAlgorithm);
                    // Get the network nodes from the list of edges.
                    var nodes = edges.Select((edge) => edge.Item1).Concat(edges.Select((edge) => edge.Item2)).Distinct().ToList();
                    // Get the target nodes that appear in the graph.
                    var targets = AlgorithmFunctions.GetTargetNodes(analysisModel.UserGivenTarget, nodes, separators);
                    if (targets.Count == 0)
                    {
                        analysisModel.Status = "None of the given target nodes could be found in the network.";
                        toReturn = false;
                    }
                    else
                    {
                        analysisModel.NetworkEdgeCount = edges.Count;
                        analysisModel.NetworkEdges = "";
                        foreach (var edge in edges)
                        {
                            analysisModel.NetworkEdges += edge.Item1 + ";" + edge.Item2 + ";";
                        }
                        analysisModel.NetworkNodeCount = nodes.Count;
                        analysisModel.NetworkNodes = "";
                        foreach (var node in nodes)
                        {
                            analysisModel.NetworkNodes += node + ";";
                        }
                        analysisModel.NetworkTargetCount = targets.Count;
                        analysisModel.NetworkTargets = "";
                        foreach (var node in targets)
                        {
                            analysisModel.NetworkTargets += node + ";";
                        }
                        if (analysisModel.UserGivenDrugTarget != null)
                        {
                            var drugTargets = AlgorithmFunctions.GetTargetNodes(analysisModel.UserGivenDrugTarget, nodes, separators);
                            analysisModel.NetworkDrugTargetCount = drugTargets.Count;
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
                        UpdateAlgorithmParameters(analysisModel);
                        analysisModel.Status = "Networks generated and saved into the database.";
                        toReturn = true;
                    }
                }
            }
            else
            {
                // If the network is given as edges - pairs of nodes.
                var edges = AlgorithmFunctions.GetEdges(analysisModel.UserGivenNodes, separators);
                // If there is an uneven number of nodes, return an error.
                if (edges.Count == 0)
                {
                    analysisModel.Status = "No edges have been given.";
                    toReturn = false;
                }
                else
                {
                    var nodes = AlgorithmFunctions.GetNodes(analysisModel.UserGivenNodes, separators);
                    var targets = AlgorithmFunctions.GetTargetNodes(analysisModel.UserGivenTarget, nodes, separators);
                    // If none of the target nodes could be found in the network, return an error.
                    if (targets.Count == 0)
                    {
                        analysisModel.Status = "None of the given target nodes could be found in the network.";
                        toReturn = false;
                    }
                    else
                    {
                        analysisModel.NetworkEdgeCount = edges.Count;
                        analysisModel.NetworkEdges = "";
                        foreach (var edge in edges)
                        {
                            analysisModel.NetworkEdges += edge.Item1 + ";" + edge.Item2 + ";";
                        }
                        analysisModel.NetworkNodeCount = nodes.Count;
                        analysisModel.NetworkNodes = "";
                        foreach (var node in nodes)
                        {
                            analysisModel.NetworkNodes += node + ";";
                        }
                        analysisModel.NetworkTargetCount = targets.Count;
                        analysisModel.NetworkTargets = "";
                        foreach (var node in targets)
                        {
                            analysisModel.NetworkTargets += node + ";";
                        }
                        if (analysisModel.UserGivenDrugTarget != null)
                        {
                            var drugTargets = AlgorithmFunctions.GetTargetNodes(analysisModel.UserGivenDrugTarget, nodes, separators);
                            analysisModel.NetworkDrugTargetCount = drugTargets.Count;
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
                        UpdateAlgorithmParameters(analysisModel);
                        analysisModel.Status = "Networks generated and saved into the database.";
                        toReturn = true;
                    }
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Updates the parameters in the database, based on the chosen type of algorithm.
        /// </summary>
        /// <param name="analysisModel"></param>
        public static void UpdateAlgorithmParameters(AnalysisModel analysisModel)
        {
            // Set the empty parameters of the used algorithm to the default values and remove the parameters of the unused algorithm.
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
                    analysisModel.GreedyMaxPathLength = analysisModel.NetworkNodeCount - 1;
                }
                if (analysisModel.GreedyRepeats == null)
                {
                    analysisModel.GreedyRepeats = 1;
                }
                if (analysisModel.GreedyHeuristics == null)
                {
                    analysisModel.GreedyHeuristics = "A,B,C,D,E,F,G;Z";
                }
            }
        }

        /// <summary>
        /// Parses the edges in the given text, separated by the provided separators.
        /// </summary>
        /// <param name="text">The text to parse. It can be read from a file or as a database entry.</param>
        /// <param name="separators">A collection of strings that separate the nodes.</param>
        /// <returns>The list of edges in the graph, as a string array. An edge goes from a node in an even positions in the array, to the following node.</returns>
        public static List<(String, String)> GetEdges(String text, List<String> separators)
        {
            var nodes = text.Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var edges = new List<(String, String)>();
            // If there is an odd number of nodes given in the edges, we will simply ignore the last one.
            for (int i = 0; i < nodes.Length - nodes.Length % 2; i = i + 2)
            {
                edges.Add((nodes[i], nodes[i + 1]));
            }
            return edges.Distinct().ToList();
        }

        /// <summary>
        /// Parses the nodes in the graph, from the given list of edges.
        /// </summary>
        /// <param name="edges">The current list of edges, previously computed.</param>
        /// <returns>The list of nodes in the graph.</returns>
        public static List<String> GetNodes(String text, List<String> separators)
        {
            return text.Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        /// <summary>
        /// Parses the target nodes in the given text, based on the given list of nodes. If any target nodes are not in the given list, they are ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="nodes">The current list of nodes. The target nodes must be in this list.</param>
        /// <param name="separators">A collection of strings that separates the nodes.</param>
        /// <returns>The list of target nodes.</returns>
        public static List<String> GetTargetNodes(String text, List<String> nodes, List<String> separators)
        {
            return nodes.Intersect(text.Split(separators.ToArray(), StringSplitOptions.RemoveEmptyEntries).Distinct()).ToList();
        }

        /// <summary>
        /// Bulds the network around the seed nodes, taking the edges from the given full network.
        /// </summary>
        /// <param name="fullNetworkText"></param>
        /// <param name="seedNodes"></param>
        /// <returns></returns>
        public static List<(String, String)> GetEdgesFromSeed(List<(String, String)> fullNetworkEdges, List<String> seedNodes, Int32 buildAlgorithm)
        {
            var edges = new List<(String, String)>();
            // Build algorithm "gap" with the value of buildAlgorithm. For example, for buildAlgorithm = 1, we will build the network with gap 1.
            if (buildAlgorithm >= 0)
            {
                var list = new List<List<(String, String)>>();
                // For "buildAlgorithm" times, for all terminal nodes, add all possible edges.
                for (int index = 0; index < buildAlgorithm + 1; index++)
                {
                    var temporaryList = new List<(String, String)>();
                    var terminalNodes = new List<String>();
                    // If it is the first iteration, then use the seed nodes as terminal nodes.
                    if (index == 0)
                    {
                        terminalNodes = seedNodes;
                    }
                    // If it is not the first iteration, compute the terminal nodes.
                    else
                    {
                        foreach (var edge in list[list.Count - 1])
                        {
                            terminalNodes.Add(edge.Item2);
                        }
                        terminalNodes = terminalNodes.Distinct().ToList();
                    }
                    // For all terminal nodes, add all possible edges starting in them.
                    foreach (var fullNetworkEdge in fullNetworkEdges)
                    {
                        foreach (var node in terminalNodes)
                        {
                            if (node.Equals(fullNetworkEdge.Item1))
                            {
                                temporaryList.Add(fullNetworkEdge);
                            }
                        }
                    }
                    list.Add(temporaryList);
                }
                // Starting from the right, mark all terminal nodes that are not seed nodes for removal.
                var initialNodes = new List<String>(seedNodes);
                for (int index = buildAlgorithm; index >= 0; index--)
                {
                    var temporaryList = new List<String>();
                    for (int i = 0; i < list[index].Count; i++)
                    {
                        if (!initialNodes.Contains(list[index][i].Item2))
                        {
                            list[index].RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            temporaryList.Add(list[index][i].Item1);
                        }
                    }
                    temporaryList.AddRange(seedNodes);
                    initialNodes = temporaryList.Distinct().ToList();
                }
                // And we finally add all the edges to the list.
                foreach (var edgeList in list)
                {
                    foreach (var edge in edgeList)
                    {
                        edges.Add(edge);
                    }
                }
            }
            // Build algorithm "neighbours".
            else if (buildAlgorithm == -1)
            {
                // For every edge in the full network,
                foreach (var edge in fullNetworkEdges)
                {
                    // For every seed node,
                    foreach (var item in seedNodes)
                    {
                        // If the edge starts from the node or ends in the node,
                        if (item.Equals(edge.Item1) || item.Equals(edge.Item2))
                        {
                            // Add the edge to the new list.
                            edges.Add(edge);
                        }
                    }
                }
            }
            else
            {
                // Wrong build index.
            }
            return edges.Distinct().ToList();
        }
        /// <summary>
        /// Parses the edges in the given graphml text.
        /// </summary>
        /// <param name="graphml">The graphml text to parse in xml format.</param>
        /// <returns>The list of edges in the graph, as a string array. An edge goes from a node in an even positions in the array, to the following node.</returns>
        public static List<(String, String)> GetGraphmlEdges(string graphml)
        {
            XDocument doc = XDocument.Load(graphml);
            XNamespace ns = doc.Root.Name.Namespace;
            XElement graph = doc.Element(ns + "graphml").Element(ns + "graph");
            var edges = (from edge in graph.Descendants(ns + "edge")
                         select (edge.Attribute("source").Value, edge.Attribute("target").Value)
                         ).ToList<(String, String)>();

            return edges;
        }
        /// <summary>
        /// Parses the nodes in the given graphml text.
        /// </summary>
        /// <param name="graphml">The graphml text to parse in xml format.</param>
        /// <returns>The list of nodes in the graph.</returns>
        public static List<String> GetGraphmlNodes(string graphml)
        {
            XDocument doc = XDocument.Load(graphml);
            XNamespace ns = doc.Root.Name.Namespace;
            XElement graph = doc.Element(ns + "graphml").Element(ns + "graph");
            var nodes = (from node in graph.Descendants(ns + "node")
                         select node.Attribute("id").Value
                         ).ToList<String>();

            return nodes; //this may include nodes that are not associated with any edges.
        }

        /// <summary>
        /// Checks if the matrix associated with the network has a rank equal to the number of targets.
        /// </summary>
        /// <param name="edges">List of the edges of the network.</param>
        /// <param name="inputs">List of the inputs of the network.</param>
        /// <param name="targets">List of the targets of the network.</param>
        /// <returns>A boolean indicating whether the matrix associated with the network has rank equal to the number of the targets in the network or not</returns>
        public static bool MatrixCheckRank(List<(String, String)> edges, List<String> inputs, List<String> targets)
        {
            //Extract the nodes from the edges
            List<String> nodes = new List<String>();
            foreach (var edge in edges)
            {
                nodes.Add(edge.Item1);
                nodes.Add(edge.Item2);
            }
            nodes = nodes.Distinct().ToList();


            /*In case some driver nodes are not in 'nodes' (This happens if a driver node doesn't have any
                edges associated with it)
                Remove it from the set of driver nodes.*/
            inputs = inputs.Intersect(nodes).ToList();

            /*In case some target nodes are not in 'nodes' (This happens if a target node doesn't have any
              edges associated with it)
                Remove it from the set of target nodes.*/
            targets = targets.Intersect(nodes).ToList();

            int n = nodes.Count;
            int p = targets.Count;
            int m = inputs.Count;

            BigInteger[,] A = new BigInteger[n, n];

            foreach (var edge in edges)
            {
                A[nodes.IndexOf(edge.Item2), nodes.IndexOf(edge.Item1)] = 1;
            }

            BigInteger[,] B = new BigInteger[n, m];
            foreach (var input in inputs)
            {
                B[nodes.IndexOf(input), inputs.IndexOf(input)] = 1;
            }

            BigInteger[,] C = new BigInteger[p, n];
            foreach (var target in targets)
            {
                C[targets.IndexOf(target), nodes.IndexOf(target)] = 1;
            }

            var M = MatrixMultiply(C, B);
            if (MatrixRank(M) == p)
                return true;

            BigInteger[,] ithPower = new BigInteger[n, n];
            for (int i = 0; i < n; i++)
                ithPower[i, i] = 1;

            for (int i = 1; i < n; i++)
            {
                ithPower = MatrixMultiply(ithPower, A);
                var toAppend = MatrixMultiply(MatrixMultiply(C, ithPower), B);
                M = MatrixAppend(M, toAppend);
                //MatrixPrint(M);
                var rank = MatrixRank(M);
                if (rank == p)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Mutiplies Matrix A by B.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <param name="B">Matrix B.</param>
        /// <returns>A*B as Matrix</returns>
        public static BigInteger[,] MatrixMultiply(BigInteger[,] A, BigInteger[,] B)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int q = B.GetLength(0);
            int p = B.GetLength(1);
            if (n != q)
            {
                Console.WriteLine("Dimensions of Matrices don't match. Cannot Multiply.");
                return null;
            }
            BigInteger[,] C = new BigInteger[m, p];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    BigInteger val = 0;
                    for (int k = 0; k < n; k++)
                    {
                        val += A[i, k] * B[k, j];
                    }
                    C[i, j] = val;
                }
            }
            return C;
        }


        /// <summary>
        /// Appends matrix B to the right of matrix A.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <param name="B">Matrix B to be appended.</param>
        /// <returns>A|B as a Matrix</returns>
        public static BigInteger[,] MatrixAppend(BigInteger[,] A, BigInteger[,] B)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int q = B.GetLength(0);
            int p = B.GetLength(1);
            if (m != q)
            {
                Console.WriteLine("Matrix Dimensions Don't Match. Cannot Append");
                return null;
            }
            BigInteger[,] C = new BigInteger[m, n + p];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C[i, j] = A[i, j];
                }
                for (int j = n; j < n + p; j++)
                {
                    C[i, j] = B[i, j - n];
                }
            }
            return C;
        }

        /// <summary>
        /// Applies Gaussian Elimination to the matrix A.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <returns>row echelon form of matrix A</returns>
        public static BigInteger[,] MatrixGaussianElimination(BigInteger[,] A)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            BigInteger[,] C = new BigInteger[m, n];
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    C[i, j] = A[i, j];

            int h = 0;
            int k = 0;

            while (h < m && k < n)
            {
                int pivot = -1;
                for (int i = h; i < m; i++)
                    if (C[i, k] != 0)
                    {
                        pivot = i;
                        break;
                    }
                if (pivot == -1)
                    k++;
                else
                {
                    MatrixSwapRows(C, h, pivot);
                    for (int i = h + 1; i < m; i++)
                    {
                        BigInteger f = C[i, k];
                        C[i, k] = 0;
                        for (int j = k + 1; j < n; j++)
                        {
                            C[i, j] = (C[i, j] * C[h, k] - f * C[h, j]);
                        }
                    }
                    h++;
                    k++;
                }
            }
            return C;
        }


        /// <summary>
        /// Swaps rows of the input matrix A.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <param name="row1">row to be swapped.</param>
        /// <param name="row2">row to be swapped.</param>
        public static void MatrixSwapRows(BigInteger[,] A, int row1, int row2)
        {
            if (row1 == row2)
                return;
            int n = A.GetLength(1);
            BigInteger[] temp = new BigInteger[n];
            for (int i = 0; i < n; i++)
                temp[i] = A[row1, i];
            for (int i = 0; i < n; i++)
                A[row1, i] = A[row2, i];
            for (int i = 0; i < n; i++)
                A[row2, i] = temp[i];

        }


        /// <summary>
        /// Calculates rank of the matrix A using Gaussian Elimination.
        /// </summary>
        /// <param name="A">Matrix A.</param>
        /// <returns>rank of matrix A</returns>
        public static int MatrixRank(BigInteger[,] A)
        {
            BigInteger[,] C = MatrixGaussianElimination(A);
            int m = C.GetLength(0);
            int n = C.GetLength(1);
            int rank = 0;
            int j = 0;
            for (int i = 0; i < m; i++)
            {
                for (j = 0; j < n; j++)
                {
                    if (C[i, j] != 0)
                    {
                        rank++;
                        break;
                    }
                }
            }
            return rank;
        }
    }
}
