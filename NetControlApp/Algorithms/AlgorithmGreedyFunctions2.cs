using NetControlApp.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetControlApp.Greedy
{
    class AlgorithmGreedyFunctions
    {
        /// <summary>
        /// The initial testing program of the greedy algorithm.
        /// </summary>
        public static void InitialProgram()
        {
            var randomSeed = new Random().Next();
            var separators = new List<String>() { ";", "\t", "\n", "\r" };
            var rand = new Random(randomSeed);

            var graphType = "BreastDEF";
            var graphText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Graph.txt");
            var targetText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Target.txt");
            var drugTargetText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\DrugTarget.txt");

            var greedyRepeats = 2;
            var greedyHeuristic = "A;B;C;D;E;F;G;Z";
            var greedyMaxPathLength = 5;
            var greedyMaxIterations = 1000;
            var greedyMaxIterationsNoImprovement = 100;

            var nodes = AlgorithmFunctions.GetNodes(graphText, separators);
            var edges = AlgorithmFunctions.GetEdges(graphText, separators);
            var targets = AlgorithmFunctions.GetTargetNodes(targetText, nodes, separators);
            var drugTargets = AlgorithmFunctions.GetTargetNodes(drugTargetText, nodes, separators);

            var fullSeparator = ";";
            var halfSeparator = ",";

            // Get the heuristic list from the string.
            var heuristics = AlgorithmGreedyFunctions.GetHeuristic(greedyHeuristic, fullSeparator, halfSeparator);
            // Get all heuristics.
            var allHeuristics = new List<string>();
            foreach (var item in heuristics)
            {
                foreach (var h in item)
                {
                    allHeuristics.Add(h);
                }
            }

            Console.WriteLine($"{nodes.Count} nodes, {edges.Count} edges, {targets.Count} targets, {drugTargets.Count} drug targets.");
            heuristics.ForEach((item) => { item.ForEach((h) => Console.Write($"{h} ")); Console.Write("; "); });
            Console.WriteLine();

            // Set up for the initial iteration.
            var currentIteration = 0;
            var currentIterationNoImprovement = 0;
            var bestResult = targets.Count;
            // Run for as long as we haven't reached the final iteration or the final iteration without improvement.
            while (currentIteration < greedyMaxIterations && currentIterationNoImprovement < greedyMaxIterationsNoImprovement)
            {
                // Display the current iteration.
                // Console.WriteLine($"Now at iteration {currentIteration} / {greedyMaxIterations}, {currentIterationNoImprovement} / {greedyMaxIterationsNoImprovement}");
                // Set up the control path to start from the target nodes.
                var controlPath = new Dictionary<String, List<String>>();
                targets.ForEach((node) => controlPath[node] = new List<String>() { node });
                var currentRepeat = 0;
                while (currentRepeat < greedyRepeats)
                {
                    var currentTargets = new List<String>(targets);
                    var currentPathLength = 0;
                    // If it is the first check of the current iteration, we have no kept nodes, so the current targets are simply the targets.
                    // The optimization part for the "repeats" starts here.
                    var keptNodes = AlgorithmGreedyFunctions.GetKeptTargetNodes(controlPath);
                    controlPath = AlgorithmGreedyFunctions.ResetControlPath(keptNodes, controlPath);
                    currentTargets = currentTargets.Except(keptNodes).ToList();
                    // Run until there are no current targets or we reached the maximum path length.
                    while (currentTargets.Any() && currentPathLength < greedyMaxPathLength)
                    {
                        //var (matchedEdges, unmatched) = ComputeMaximalMatching(nodes, edges, heuristics, controlPath, currentTargets, rand, drugTargets);
                        var unmatched = new List<string>();
                        unmatched.AddRange(currentTargets);
                        var free = new List<string>();
                        free.AddRange(nodes);
                        var matchedEdges = new List<(string, string)>();
                        // If it is the first check of the current iteration, we have no kept nodes, so the left nodes and edges remain unchanged.
                        // Otherwise, we remove from the left nodes the corresponding nodes in the current step in the control paths for the kept nodes.
                        // The optimization part for the "repeat" begins here.
                        foreach (var item in keptNodes)
                        {
                            if (currentPathLength + 1 < controlPath[item].Count)
                            {
                                var leftNode = controlPath[item][currentPathLength + 1];
                                free.Remove(leftNode);
                            }
                        }
                        foreach (var heuristic in heuristics)
                        {
                            var matchings = new List<(string, string)>();
                            var left = new List<string>(); left.AddRange(free);
                            var right = new List<string>(); right.AddRange(unmatched);
                            var currentEdges = new List<(string, string)>();
                            currentEdges = GetSingleHeuristicEdges(left, right, edges, heuristic, controlPath, drugTargets);
                            var mEdges = GetMaximumMatching(left, right, currentEdges, rand);
                            foreach (var mEdge in mEdges)
                            {
                                matchedEdges.Add(mEdge);
                                free.Remove(mEdge.Item1);
                                unmatched.Remove(mEdge.Item2);
                            }
                        }
                        currentTargets = AlgorithmGreedyFunctions.GetMatchedNodes(matchedEdges);
                        // And update the control path.
                        controlPath = AlgorithmGreedyFunctions.UpdateControlPath(matchedEdges, controlPath);
                        
                        currentPathLength++;
                    }
                    currentRepeat++;
                }
                // Display the control path for the current iteration.
                //foreach (var item in controlPath)
                //{
                //    Console.Write($"{item.Key}:\t");
                //    foreach (var node in item.Value)
                //    {
                //        Console.Write($"{node}\t");
                //    }
                //    Console.WriteLine();
                //}
                //Console.WriteLine("----");
                // The optimization part for the "cut to driven" parameter begins here.
                var stop = false;
                while (!stop)
                {
                    stop = true;
                    foreach (var item1 in controlPath)
                    {
                        var controllingNode = item1.Value.Last();
                        foreach (var item2 in controlPath)
                        {
                            var firstIndex = item2.Value.IndexOf(controllingNode);
                            if (firstIndex != -1 && firstIndex != item2.Value.Count - 1)
                            {
                                item2.Value.RemoveRange(firstIndex + 1, item2.Value.Count - firstIndex - 1);
                                stop = false;
                            }
                        }
                    }
                }
                // We compute the result.
                var controllingNodes = AlgorithmGreedyFunctions.GetControllingNodes(controlPath).Keys.ToList();
                var result = controllingNodes.Count;
                // If the current solution is better than the previously obtained best solution.
                if (result < bestResult)
                {
                    bestResult = result;
                    Console.WriteLine($"{result} nodes in best solution so far, out of which {controllingNodes.Intersect(drugTargets).Count()} drug targets.");
                    controllingNodes.ForEach((node) => Console.Write($"{node} "));
                    Console.WriteLine();
                    currentIterationNoImprovement = 0;
                }
                else
                {
                    currentIterationNoImprovement++;
                }
                currentIteration++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="heuristic"></param>
        /// <param name="fullSeparator"></param>
        /// <param name="halfSeparator"></param>
        /// <returns></returns>
        private static List<List<string>> GetHeuristic(string heuristic, string fullSeparator, string halfSeparator)
        {
            var heuristics = new List<List<string>>();
            foreach (var fullItem in heuristic.Split(fullSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                heuristics.Add(new List<string>());
                foreach (var halfItem in fullItem.Split(halfSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    heuristics[heuristics.Count - 1].Add(halfItem);
                }
            }
            return heuristics;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftNodes"></param>
        /// <param name="rightNodes"></param>
        /// <param name="edges"></param>
        /// <param name="heuristic"></param>
        /// <param name="controlPath"></param>
        /// <param name="drugTargetNodes"></param>
        /// <returns></returns>
        public static List<(string, string)> GetSingleHeuristicEdges(List<string> leftNodes, List<string> rightNodes, List<(string, string)> edges, List<string> heuristic, Dictionary<string, List<string>> controlPath, List<string> drugTargetNodes)
        {
            var heuristicEdges = new List<(string, string)>();
            // Get all edges in the current control path, if needed.
            var currentEdges = new List<(string, string)>();
            if (heuristic.Contains("A") || heuristic.Contains("C") || heuristic.Contains("E"))
            {
                foreach (var item in controlPath)
                {
                    for (int index = 0; index < item.Value.Count - 1; index++)
                    {
                        currentEdges.Add((item.Value[index + 1], item.Value[index]));
                    }
                }
                currentEdges = currentEdges.Distinct().ToList();
            }
            // Get all existing driven nodes, if needed.
            var drivenNodes = new List<string>();
            var currentLength = controlPath.Max((item) => item.Value.Count);
            if (heuristic.Contains("C") || heuristic.Contains("D"))
            {
                foreach (var item in controlPath)
                {
                    if (item.Value.Count < currentLength)
                    {
                        drivenNodes.Add(item.Value.Last());
                    }
                }
            }
            // Get all previously seen nodes in the control paths, if needed.
            var currentNodes = new List<string>();
            if (heuristic.Contains("E") || heuristic.Contains("F"))
            {
                foreach (var item in controlPath)
                {
                    foreach (var node in item.Value)
                    {
                        currentNodes.Add(node);
                    }
                }
                currentNodes = currentNodes.Distinct().ToList();
            }
            // Get all edges starting from a drug target node and ending in target nodes, if needed.
            var temporaryDrugEdges = new List<(string, string)>();
            if (drugTargetNodes != null && (heuristic.Contains("A") || heuristic.Contains("B")))
            {
                temporaryDrugEdges = edges.Where((edge) => rightNodes.Contains(edge.Item2) && drugTargetNodes.Contains(edge.Item1)).ToList();
            }
            // Get all edges starting from an already driven node and ending in target nodes, if needed.
            var temporaryDrivenEdges = new List<(string, string)>();
            if (heuristic.Contains("C") || heuristic.Contains("D"))
            {
                temporaryDrivenEdges = edges.Where((edge) => rightNodes.Contains(edge.Item2) && drivenNodes.Contains(edge.Item1)).ToList();
            }
            // Get all edges starting from a previously seen node in the control paths, if needed.
            var temporarySeenEdges = new List<(string, string)>();
            if (heuristic.Contains("E") || heuristic.Contains("F"))
            {
                temporarySeenEdges = edges.Where((edge) => rightNodes.Contains(edge.Item2) && currentNodes.Contains(edge.Item1)).ToList();
            }
            // Get all edges not starting from a node in the current control path for a node, if needed (to avoid loops).
            var temporaryControlEdges = new List<(string, string)>();
            if (heuristic.Contains("G"))
            {
                foreach (var target in rightNodes)
                {
                    // Get the nodes in the current control path.
                    var currentPathNodes = controlPath.First((item) => item.Value.Last() == target).Value.Distinct();
                    // Get all edges from nodes not in the current control path to the current target and add them to list.
                    temporaryControlEdges.AddRange(edges.Where((edge) => edge.Item2 == target && !currentPathNodes.Contains(edge.Item1)));
                }
            }
            // Get all possible edges.
            var allEdges = edges.Where((edge) => rightNodes.Contains(edge.Item2)).ToList();
            // For every target node.
            foreach (var target in rightNodes)
            {
                // We define a new list of edges which will end in the target node.
                var temporaryHeuristicEdges = new List<(string, string)>();
                // For all heuristic strings, separated by ";"
                foreach (var h in heuristic)
                {
                    // Add previously seen edges from drug-target nodes.
                    if (drugTargetNodes != null && h == "A")
                    {
                        temporaryHeuristicEdges.AddRange(currentEdges.Where((edge) => edge.Item2 == target).Intersect(temporaryDrugEdges.Where((edge) => edge.Item2 == target)));
                    }
                    // Add all edges from drug-target nodes.
                    else if (drugTargetNodes != null && h == "B")
                    {
                        temporaryHeuristicEdges.AddRange(temporaryDrugEdges.Where((edge) => edge.Item2 == target));
                    }
                    // Add previously seen edges from already driven nodes.
                    else if (h == "C")
                    {
                        temporaryHeuristicEdges.AddRange(currentEdges.Where((edge) => edge.Item2 == target).Intersect(temporaryDrivenEdges.Where((edge) => edge.Item2 == target)));
                    }
                    // Add all edges from already driven nodes.
                    else if (h == "D")
                    {
                        temporaryHeuristicEdges.AddRange(temporaryDrivenEdges.Where((edge) => edge.Item2 == target));
                    }
                    // Add previously seen edges from anywhere in the control path.
                    else if (h == "E")
                    {
                        temporaryHeuristicEdges.AddRange(currentEdges.Where((edge) => edge.Item2 == target).Intersect(temporarySeenEdges.Where((edge) => edge.Item2 == target)));
                    }
                    // Add all edges from previously seen nodes anywhere in the control path.
                    else if (h == "F")
                    {
                        temporaryHeuristicEdges.AddRange(temporarySeenEdges.Where((edge) => edge.Item2 == target));
                    }
                    // Add all edges from a node that has not appeared in the current control path (to preferably avoid loops).
                    else if (h == "G")
                    {
                        temporaryHeuristicEdges.AddRange(temporaryControlEdges.Where((edge) => edge.Item2 == target));
                    }
                    // Add all possible edges.
                    else if (h == "Z")
                    {
                        temporaryHeuristicEdges.AddRange(allEdges.Where((edge) => edge.Item2 == target));
                    }
                }
                // We add the heuristic edges of the current node to the list of edges.
                heuristicEdges.AddRange(temporaryHeuristicEdges);
            }
            // Remove all edges which don't start from one of the left nodes.
            heuristicEdges = heuristicEdges.Where((edge) => leftNodes.Contains(edge.Item1)).ToList();
            // We return all of the obtained edges.
            return heuristicEdges;
        }

        /// <summary>
        /// The Hopcroft-Karp algorithm for maximum matching in a bipartite graph. The implementation is a slightly modified version
        /// from the one found on https://en.wikipedia.org/wiki/Hopcroft–Karp_algorithm.
        /// </summary>
        /// <param name="leftNodes">The left nodes of the bipartite graph.</param>
        /// <param name="rightNodes">The right nodes of the bipartite graph.</param>
        /// <param name="edges">The edges of the bipartite graph.</param>
        /// <param name="rand">The random variable for choosing randomly a maximum matching.</param>
        /// <returns></returns>
        private static List<(String, String)> GetMaximumMatching(List<String> leftNodes, List<String> rightNodes, List<(String, String)> edges, Random rand)
        {
            // The Wikipedia algorithm uses considers the left nodes as U, and the right ones as V. But, as the unmatched nodes are considered, in order,
            // from the left side of the bipartite graph, the obtained matching would not be truly random, especially on the first step.
            // That is why I perform here a simple switch, by inter-changing the lists U and V (left and right side nodes), and using the opposite
            // direction edges, in order to obtained a random maximum matching.
            var U = rightNodes;
            var V = leftNodes;
            var oppositeEdges = edges.Select((item) => (item.Item2, item.Item1)).ToList();
            // The actual algorithm starts from here.
            var PairU = new Dictionary<String, String>();
            var PairV = new Dictionary<String, String>();
            var dist = new Dictionary<String, int>();
            foreach (var u in U)
            {
                PairU[u] = "null";
            }
            foreach (var v in V)
            {
                PairV[v] = "null";
            }
            var matching = 0;
            while (BreadthFirstSearch(U, PairU, PairV, dist, oppositeEdges, rand))
            {
                foreach (var u in U)
                {
                    if (PairU[u] == "null")
                    {
                        if (DepthFirstSearch(u, PairU, PairV, dist, oppositeEdges, rand))
                        {
                            matching++;
                        }
                    }
                }
            }
            // Instead of the number of performed matchings, we will return the actual edges corresponding to these matchings.
            // Because in the beginning of the function we switched the direction of the edges, now we switch back the direction of
            // the matched edges, to fit in with the rest of the program.
            var matchedEdges = new List<(String, String)>();
            foreach (var item in PairU)
            {
                // We will return only edges corresponding to the matching, that is edges with both nodes not null.
                if (item.Value != "null")
                {
                    // Here we perform the switch back to the original direction. Otherwise we would return the pair (key, value).
                    matchedEdges.Add((item.Value, item.Key));
                }
            }
            return matchedEdges;
        }

        /// <summary>
        /// The breadth-first search of the Hopcroft-Karp maximum matching algorithm.
        /// </summary>
        /// <param name="U">The left nodes of the bipartite graph.</param>
        /// <param name="PairU">Dictionary containing a matching from the nodes on the left to nodes on the right.</param>
        /// <param name="PairV">Dictionary containing a matching from the nodes on the right to nodes on the left.</param>
        /// <param name="dist"></param>
        /// <param name="edges">List of edges in the bipartite graph.</param>
        /// <param name="rand">The random variable for choosing randomly a maximum matching.</param>
        /// <returns></returns>
        private static bool BreadthFirstSearch(List<String> U, Dictionary<String, String> PairU, Dictionary<String, String> PairV, Dictionary<String, int> dist, List<(String, String)> edges, Random rand)
        {
            var queue = new Queue<String>();
            foreach (var u in U)
            {
                if (PairU[u] == "null")
                {
                    dist[u] = 0;
                    queue.Enqueue(u);
                }
                else
                {
                    dist[u] = Int32.MaxValue;
                }
            }
            dist["null"] = Int32.MaxValue;
            while (queue.Any())
            {
                var u = queue.Dequeue();
                if (dist[u] < dist["null"])
                {
                    var adj = edges.Where((edge) => edge.Item1 == u).Select((edge) => edge.Item2).OrderBy((item) => rand.Next());
                    foreach (var v in adj)
                    {
                        if (dist[PairV[v]] == Int32.MaxValue)
                        {
                            dist[PairV[v]] = dist[u] + 1;
                            queue.Enqueue(PairV[v]);
                        }
                    }
                }
            }
            return dist["null"] != Int32.MaxValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u">A node from the left part of the bipartite graph.</param>
        /// <param name="PairU">Dictionary containing a matching from the nodes on the left to nodes on the right.</param>
        /// <param name="PairV">Dictionary containing a matching from the nodes on the right to nodes on the left.</param>
        /// <param name="dist"></param>
        /// <param name="edges">List of edges in the bipartite graph.</param>
        /// <param name="rand">The random variable for choosing randomly a maximum matching.</param>
        /// <returns></returns>
        private static bool DepthFirstSearch(String u, Dictionary<String, String> PairU, Dictionary<String, String> PairV, Dictionary<String, int> dist, List<(String, String)> edges, Random rand)
        {
            if (u != "null")
            {
                var adj = edges.Where((edge) => edge.Item1 == u).Select((edge) => edge.Item2).OrderBy((item) => rand.Next());
                foreach (var v in adj)
                {
                    if (dist[PairV[v]] == dist[u] + 1)
                    {
                        if (DepthFirstSearch(PairV[v], PairU, PairV, dist, edges, rand))
                        {
                            PairV[v] = u;
                            PairU[u] = v;
                            return true;
                        }
                    }
                }
                dist[u] = Int32.MaxValue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Computes the left-side matched nodes corresponding to the matched edges.
        /// </summary>
        /// <param name="matchedEdges">The list of edges corresponding to the maximum matching.</param>
        /// <returns>List of left-side matched nodes.</returns>
        private static List<String> GetMatchedNodes(List<(string, string)> matchedEdges)
        {
            return matchedEdges.Select((edge) => edge.Item1).Distinct().ToList();
        }

        /// <summary>
        /// Updates the control paths for all targets, based on the provided matched edges.
        /// </summary>
        /// <param name="matchedEdges">The list of edges corresponding to the maximum matching.</param>
        /// <param name="controlPath">The current control path.</param>
        /// <returns>The new control paths.</returns>
        private static Dictionary<string, List<string>> UpdateControlPath(List<(string, string)> matchedEdges, Dictionary<string, List<string>> controlPath)
        {
            var newControlPath = new Dictionary<string, List<string>>();
            foreach (var item in controlPath)
            {
                newControlPath[item.Key] = new List<string>();
                newControlPath[item.Key].AddRange(item.Value);
            }
            foreach (var item in newControlPath)
            {
                var matchedEdge = matchedEdges.FirstOrDefault((edge) => edge.Item2 == item.Value.Last());
                if (!matchedEdge.Equals((null, null)))
                {
                    item.Value.Add(matchedEdge.Item1);
                }
            }
            //foreach (var edge in matchedEdges)
            //{
            //    foreach (var item in newControlPath)
            //    {
            //        if (item.Value.Count == currentPathLength + 1 && item.Value.Last() == edge.Item2)
            //        {
            //            item.Value.Add(edge.Item1);
            //            break;
            //        }
            //    }
            //}
            return newControlPath;
        }

        /// <summary>
        /// For each controlling node, it computes the target nodes that it controls.
        /// </summary>
        /// <param name="controlPath">The control path of the current iteration.</param>
        /// <returns></returns>
        private static Dictionary<String, List<String>> GetControllingNodes(Dictionary<String, List<String>> controlPath)
        {
            var controlNodes = new Dictionary<String, List<String>>();
            foreach (var item in controlPath)
            {
                if (controlNodes.ContainsKey(item.Value.Last()))
                {
                    controlNodes[item.Value.Last()].Add(item.Key);
                }
                else
                {
                    controlNodes[item.Value.Last()] = new List<String>() { item.Key };
                }
            }
            return controlNodes;
        }

        /// <summary>
        /// Cuts the control paths of all target nodes, other than the specified nodes to be kept.
        /// </summary>
        /// <param name="keptNodes">The nodes for which to keep the control path intact.</param>
        /// <param name="controlPath">The current control path.</param>
        /// <returns>The new control paths.</returns>
        private static Dictionary<string, List<string>> ResetControlPath(List<string> keptNodes, Dictionary<string, List<string>> controlPath)
        {
            var newControlPath = new Dictionary<string, List<string>>(controlPath);
            var matchingNodes = newControlPath.Keys.Except(keptNodes).ToList();
            foreach (var item in matchingNodes)
            {
                newControlPath[item] = new List<string>() { item };
            }
            return newControlPath;
        }

        /// <summary>
        /// Computes the target nodes which are controlled by a node which controls only one target node.
        /// </summary>
        /// <param name="controlPath">The current control path.</param>
        /// <returns>List of target nodes controlled by a node which controls only one target node.</returns>
        private static List<String> GetKeptTargetNodes(Dictionary<String, List<String>> controlPath)
        {
            var keptTargetNodes = new List<String>();
            var controllingNodes = AlgorithmGreedyFunctions.GetControllingNodes(controlPath);
            foreach (var item in controllingNodes)
            {
                if (item.Value.Count > 1)
                {
                    foreach (var target in item.Value)
                    {
                        keptTargetNodes.Add(target);
                    }
                }
            }
            return keptTargetNodes;
        }
    }
}
