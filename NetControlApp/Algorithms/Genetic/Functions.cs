using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetControlApp.Algorithms
{
    public class Functions
    {
        /// <summary>
        /// Parses the edges in the given text, separated by the provided separators.
        /// </summary>
        /// <param name="text">The text to parse. It can be read from a file or as a database entry.</param>
        /// <param name="separators">A collection of strings that separate the nodes.</param>
        /// <returns>The list of edges in the graph, as a string array. An edge goes from a node in an even positions in the array, to the following node.</returns>
        public static String[] GetEdges(String text, String[] separators)
        {
            return text.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        /// <summary>
        /// Parses the nodes in the graph, from the given list of edges.
        /// </summary>
        /// <param name="edges">The current list of edges, previously computed.</param>
        /// <returns>The list of nodes in the graph.</returns>
        public static String[] GetNodes(String[] edges)
        {
            return edges.Distinct().ToArray();
        }

        /// <summary>
        /// Parses the list of seed nodes in the given text, separated by the provided separators.
        /// </summary>
        /// <param name="text">The text to parse. It can be read from a file or as a database entry.</param>
        /// <param name="separators">A collection of strings that separate the nodes.</param>
        /// <returns>The list of edges in the graph, as a string array. An edge goes from a node in an even positions in the array, to the following node.</returns>
        public static String[] GetSeedNodes(String text, String[] separators)
        {
            return text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
        }

        /// <summary>
        /// Bulds the network around the seed nodes, taking the edges from the given full network.
        /// </summary>
        /// <param name="fullNetworkText"></param>
        /// <param name="seedNodes"></param>
        /// <returns></returns>
        public static String[] GetEdgesFromSeed(String fullNetwork, String[] seedNodes, String[] separators, Int32 buildAlgorithm)
        {
            var edges = new List<String>();
            // Read the entire network from the file.
            var fullNetworkEdges = Functions.GetEdges(fullNetwork, separators);
            // Build algorithm "gap"
            if (buildAlgorithm >= 0)
            {
                var list = new List<List<String>>();
                // Add all edges starting from the seed nodes.
                var temporaryList = new List<String>();
                for (int i = 0; i < fullNetworkEdges.Length; i = i + 2)
                {
                    foreach (var item in seedNodes)
                    {
                        if (item.Equals(fullNetworkEdges[i]))
                        {
                            var isInList = false;
                            for (int index = 0; index < temporaryList.Count; index = index + 2)
                            {
                                if (temporaryList[index] == fullNetworkEdges[i] && temporaryList[index + 1] == fullNetworkEdges[i + 1])
                                {
                                    isInList = true;
                                    break;
                                }
                            }
                            if (!isInList)
                            {
                                temporaryList.Add(fullNetworkEdges[i]);
                                temporaryList.Add(fullNetworkEdges[i + 1]);
                            }
                        }
                    }
                }
                list.Add(temporaryList);
                Console.WriteLine($"{temporaryList.Count / 2} edges in step 0.");
                // For all terminal nodes add all possible edges, for "gap" times.
                for (int index = 0; index < buildAlgorithm; index++)
                {
                    temporaryList = new List<string>();
                    for (int index1 = 0; index1 < list[list.Count - 1].Count; index1 = index1 + 2)
                    {
                        for (int index2 = 0; index2 < fullNetworkEdges.Length; index2 = index2 + 2)
                        {
                            if (list[list.Count - 1][index1 + 1] == fullNetworkEdges[index2])
                            {
                                var isInList = false;
                                for (int i = 0; i < temporaryList.Count; i = i + 2)
                                {
                                    if (temporaryList[i] == fullNetworkEdges[index2] && temporaryList[i + 1] == fullNetworkEdges[index2 + 1])
                                    {
                                        isInList = true;
                                        break;
                                    }
                                }
                                if (!isInList)
                                {
                                    temporaryList.Add(fullNetworkEdges[index2]);
                                    temporaryList.Add(fullNetworkEdges[index2 + 1]);
                                }
                            }
                        }
                    }
                    list.Add(temporaryList);
                    Console.WriteLine($"{temporaryList.Count / 2} edges in step {index + 1}.");
                }
                // Starting from the right, mark all terminal nodes that are not seed nodes for removal.
                list.Add(new List<String>());
                for (int index = buildAlgorithm; index >=0 ; index--)
                {
                    temporaryList = new List<String>(list[index]);
                    for (int index1 = 0; index1 < temporaryList.Count; index1 = index1 + 2)
                    {
                        Boolean isEnding = true;
                        for (int index2 = 0; index2 < list[index + 1].Count; index2 = index2 + 2)
                        {
                            if (temporaryList[index1 + 1] == list[index + 1][index2])
                            {
                                isEnding = false;
                                break;
                            }
                        }
                        if (isEnding)
                        {
                            if (!seedNodes.Contains(temporaryList[index1 + 1]))
                            {
                                list[index][index1] = "ToRemove";
                                list[index][index1 + 1] = "ToRemove";
                            }
                        }
                    }
                }
                // Mow remove all items marked for removal.
                foreach (var item in list)
                {
                    item.RemoveAll(n => n == "ToRemove");
                }
                // And we add all the edges to the list.
                foreach (var item in list)
                {
                    for (int i = 0; i < item.Count; i = i + 2)
                    {
                        edges.Add(item[i]);
                        edges.Add(item[i + 1]);
                    }
                }
            }
            // Build algorithm "neighbours".
            else if (buildAlgorithm == -1)
            {
                // For every edges in the full network,
                for (int i = 0; i < fullNetworkEdges.Length; i = i + 2)
                {
                    // For every seed node,
                    foreach (var item in seedNodes)
                    {
                        // If the edge starts from the node,
                        if (item.Equals(fullNetworkEdges[i]))
                        {
                            // Add the edge to the new list.
                            edges.Add(fullNetworkEdges[i]);
                            edges.Add(fullNetworkEdges[i + 1]);
                        }
                        // If the ends in the node,
                        else if (item.Equals(fullNetworkEdges[i + 1]))
                        {
                            // Add the edge to the liet.
                            edges.Add(fullNetworkEdges[i - 1]);
                            edges.Add(fullNetworkEdges[i]);
                        }
                    }
                }
            }
            else
            {
                // Wrong build index.
            }
            return edges.ToArray();
        }

        /// <summary>
        /// Computes the adjacency matrix from the given text (graph), based on the list of nodes provided.
        /// </summary>
        /// <param name="nodes">The array containing the current list of nodes, previously computed.</param>
        /// <param name="edges">The array containing the current list of edges, previously computed.</param>
        /// <returns>The adjacency matrix of the given graph.</returns>
        public static Matrix<Double> GetAdjacencyMatrix(String[] nodes, String[] edges)
        {
            var A = Matrix<Double>.Build.DenseDiagonal(nodes.Length, nodes.Length, 0.0);
            for (int i = 0; i < edges.Length; i = i + 2)
            {
                var index1 = Array.FindIndex(nodes, node => node.Equals(edges[i]));
                var index2 = Array.FindIndex(nodes, node => node.Equals(edges[i + 1]));
                A[index2, index1] = 1.0;
            }
            return A;
        }

        /// <summary>
        /// Parses the target nodes in the given text, based on the given list of nodes. If any target nodes are not in the given list, they are ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="nodes">The current list of nodes. The target nodes must be in this list.</param>
        /// <param name="separators">A collection of strings that separates the nodes.</param>
        /// <returns>The list of target nodes.</returns>
        public static String[] GetTargets(String text, String[] nodes, String[] separators)
        {
            return nodes.Intersect(text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Distinct()).ToArray();
        }

        /// <summary>
        /// Parses the drug target nodes in the given text, based on the given list of nodes. If any target nodes are not in the given list, they are ignored.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="nodes">The current list of nodes. The target nodes must be in this list.</param>
        /// <param name="separators">A collection of strings that separates the nodes.</param>
        /// <returns>The list of target nodes.</returns>
        public static String[] GetDrugTargets(String text, String[] nodes, String[] separators)
        {
            return nodes.Intersect(text.Split(separators, StringSplitOptions.RemoveEmptyEntries).Distinct()).ToArray();
        }

        /// <summary>
        /// Computes the indices of the target nodes in the list of nodes.
        /// </summary>
        /// <param name="nodes">The complete list of nodes.</param>
        /// <param name="targets">The list of target nodes.</param>
        /// <returns>The array containing, in order, the indices of the target nodes.</returns>
        public static Int32[] GetTargetIndices(String[] nodes, String[] targets)
        {
            var targetIndices = new Int32[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                targetIndices[i] = Array.FindIndex(nodes, node => node.Equals(targets[i]));
            }
            // By default, the indices should be in increasing order. If the functionality might somehow change, uncomment the next line.
            // targetIndices = targetIndices.OrderBy(i => i).ToArray();
            return targetIndices;
        }

        /// <summary>
        /// Forms the target-corresponding matrix C. It will be a submatrix of the identity matrix.
        /// </summary>
        /// <param name="nodes">The complete list of nodes.</param>
        /// <param name="targetIndices">The indices of the target nodes.</param>
        /// <returns>The target matrix C.</returns>
        public static Matrix<Double> GetTargetMatrix(String[] nodes, Int32[] targetIndices)
        {
            // We begin from an all-zero matrix.
            var C = Matrix<Double>.Build.Dense(targetIndices.Length, nodes.Length);
            // For every target node,
            for (int i = 0; i < targetIndices.Length; i++)
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
        public static Matrix<Double>[] GetAdjacencyMatrixPowers(Matrix<Double> A, Int32 maximumPower)
        {
            var powers = new Matrix<Double>[maximumPower + 1];
            powers[0] = Matrix<Double>.Build.DenseIdentity(A.RowCount);
            for (int i = 1; i < maximumPower + 1; i++)
            {
                Console.WriteLine($"Computing the powers ({i}/{maximumPower})");
                powers[i] = A.Multiply(powers[i - 1]);
            }
            return powers;
        }

        /// <summary>
        /// Computes the powers of the combination between the target matrix C and the adjacency matrix A, to be used in validating the chromosomes.
        /// </summary>
        /// <param name="C">The target-corresponding matrix C.</param>
        /// <param name="AdjacencyPowers">The array containing the powers of the adjacency matrix A.</param>
        /// <returns>An array containing the powers of the matrices. The array goes as C, CA, CA^2, ... .</returns>
        public static Matrix<Double>[] GetTargetMatrixPowers(Matrix<Double> C, Matrix<Double>[] AdjacencyPowers)
        {
            var powers = new Matrix<Double>[AdjacencyPowers.Length];
            for (int i = 0; i < AdjacencyPowers.Length; i++)
            {
                powers[i] = C.Multiply(AdjacencyPowers[i]);
            }
            return powers;
        }

        /// <summary>
        /// Computes, for every target node, the list of nodes from which it can be reached.
        /// </summary>
        /// <param name="AdjacencyPowers">The powers of the adjacency matrix A.</param>
        /// <param name="targetIndices">The indices of the target nodes.</param>
        /// <returns>The list of nodes from which the target nodes can be reached.</returns>
        public static Int32[][] GetList(Matrix<Double>[] AdjacencyPowers, Int32[] targetIndices)
        {
            // Initialize the main list.
            var list = new List<List<Int32>>(targetIndices.Length);
            // Initialize the arrays 
            for (int i = 0; i < targetIndices.Length; i++)
            {
                list.Add(new List<Int32>());
            }
            // For every power of the adjacency matrix,
            for (int index1 = 0; index1 < AdjacencyPowers.Length; index1++)
            {
                // and for every target node,
                for (int index2 = 0; index2 < targetIndices.Length; index2++)
                {
                    // get corresponding row,
                    var row = AdjacencyPowers[index1].Row(targetIndices[index2]);
                    // get all of the non-zero entries in the row,
                    var nz = row.Select((value, index) => value != 0 ? index : -1).Where(i => i != -1);
                    // And add them to the corresponding list.
                    list[index2].AddRange(nz);
                }
            }
            // Initialize the list array.
            var array = new Int32[targetIndices.Length][];
            // For every target node,
            for (int i = 0; i < list.Count; i++)
            {
                // remove the duplicate nodes and add them to the array.
                array[i] = list[i].Distinct().ToArray();
            }
            return array;
        }

        /// <summary>
        /// Computes the list of target nodes which have no edges in the trimmed graph.
        /// </summary>
        /// <param name="oldNodes">The current list of nodes.</param>
        /// <param name="oldList">The existing path list.</param>
        /// <returns></returns>
        public static String[] GetSingleNodes(String[] oldNodes, String[] nodes, Int32[][] oldList)
        {
            var singleNodes = new List<String>();
            for (int i = 0; i < oldList.Length; i++)
            {
                if (oldList[i].Length == 1 && !nodes.Contains(oldNodes[oldList[i][0]]))
                {
                    singleNodes.Add(oldNodes[oldList[i][0]]);
                }
            }
            return singleNodes.ToArray();
        }

        /// <summary>
        /// Computes the new target nodes of the trimmed graph.
        /// </summary>
        /// <param name="targetNodes"></param>
        /// <param name="singleNodes"></param>
        /// <returns></returns>
        public static String[] GetNewTargets(String[] targetNodes, String[] singleNodes)
        {
            var newTargets = new List<String>();
            foreach (var item in targetNodes)
            {
                if (!singleNodes.Contains(item))
                {
                    newTargets.Add(item);
                }
            }
            return newTargets.ToArray();
        }

        /// <summary>
        /// The main function for trimming the graph by removing all of the edges corresponding to nodes from which the target nodes can't be reached.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="edges"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static String[] GetNewEdges(String[] nodes, String[] edges, Int32[][] list)
        {
            var newNodeList = new List<Int32>();
            for (int index1 = 0; index1 < list.Length; index1++)
            {
                if (list[index1].Length > 1)
                {
                    for (int index2 = 0; index2 < list[index1].Length; index2++)
                    {
                        newNodeList.Add(list[index1][index2]);
                    }
                }
            }
            var newNodeIndices = newNodeList.Distinct().ToArray();
            var newNodes = new String[newNodeIndices.Length];
            for (int i = 0; i < newNodeIndices.Length; i++)
            {
                newNodes[i] = nodes[newNodeIndices[i]];
            }
            var newEdges = new List<String>();
            for (int i = 0; i < edges.Length; i = i + 2)
            {
                if (newNodes.Contains(edges[i]) && newNodes.Contains(edges[i + 1]))
                {
                    newEdges.Add(edges[i]);
                    newEdges.Add(edges[i + 1]);
                }
            }
            return newEdges.ToArray();
        }

        /// <summary>
        /// Checkes if a given sequence of genes can control the target nodes in the current graph.
        /// </summary>
        /// <param name="matrixPowers">The list of powers of the target matrix.</param>
        /// <param name="nodes">The nodes in the graph.</param>
        /// <param name="singleNodes">The single nodes in the graph.</param>
        /// <param name="genes">The list of genes to check.</param>
        /// <returns>True if the given genes fully control the target nodes, false otherwise.</returns>
        public static bool IsItValid(Matrix<Double>[] matrixPowers, String[] nodes, String[] singleNodes, String[] genes)
        {
            bool isValid = false;
            var genesId = new List<int>(genes.Length);
            var singlesId = new List<string>(singleNodes.Length);
            for (int i = 0; i < genes.Length; i++)
            {
                if (singleNodes.Contains(genes[i]))
                {
                    singlesId.Add(genes[i]);
                }
                else
                {
                    var toAdd = Array.IndexOf(nodes, genes[i]);
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
                for (int i = 1; i < matrixPowers.Length; i++)
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
            if (isValid && singlesId.Count == singleNodes.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
