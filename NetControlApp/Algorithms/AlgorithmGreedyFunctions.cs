using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetControlApp.Algorithms
{
    class AlgorithmGreedyFunctions
    {
        public static void InitialProgram()
        {
            var randomSeed = new Random().Next();
            var separators = new List<String>() { ";", "\t", "\n", "\r" };
            var rand = new Random(randomSeed);

            var graphType = "Big";
            var graphText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Graph.txt");
            var targetText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\{graphType}Target.txt");

            var greedyRepeats = 1;
            var greedyHeuristic = "";
            var greedyMaxPathLength = 20;
            var greedyMaxIterations = 100;
            var greedyMaxIterationsNoImprovement = 100;

            var nodes = AlgorithmFunctions.GetNodes(graphText, separators);
            var edges = AlgorithmFunctions.GetEdges(graphText, separators);
            var targets = AlgorithmFunctions.GetTargetNodes(targetText, nodes, separators);

            Console.WriteLine($"{nodes.Count} nodes, {edges.Count} edges, {targets.Count} targets");

            var currentIteration = 0;
            var currentIterationNoImprovement = 0;
            var bestResult = targets.Count;
            while (currentIteration < greedyMaxIterations && currentIterationNoImprovement < greedyMaxIterationsNoImprovement)
            {
                var better = false;
                var controlPath = new Dictionary<String, List<String>>();
                targets.ForEach((node) => controlPath[node] = new List<String>() { node });
                var controlNodes = new List<String>();
                var currentRepeat = 0;
                while (currentRepeat < greedyRepeats)
                {
                    var currentTargets = new List<String>(targets);
                    var currentPathLength = 0;
                    var keptNodes = AlgorithmGreedyFunctions.GetKeptTargetNodes(controlPath);
                    currentTargets = currentTargets.Except(keptNodes).ToList();
                    //Console.WriteLine("Kept nodes:");
                    //keptNodes.ForEach((node) => Console.Write($"{node} "));
                    //Console.WriteLine();
                    do
                    {
                        var currentEdges = new List<(String, String)>();
                        foreach (var target in currentTargets)
                        {
                            currentEdges.AddRange(AlgorithmGreedyFunctions.GetHeuristicEdges(target, edges, greedyHeuristic));
                        }
                        var leftNodes = nodes;
                        var rightNodes = currentTargets;
                        var matchingEdges = currentEdges;
                        // Here begins the part for the "repeat" optimization.
                        foreach (var item in keptNodes)
                        {
                            if (currentPathLength < controlPath[item].Count)
                            {
                                rightNodes.Add(controlPath[item][currentPathLength]);
                            }
                            if (currentPathLength + 1 < controlPath[item].Count)
                            {
                                currentEdges.Add((controlPath[item][currentPathLength], controlPath[item][currentPathLength + 1]));
                            }
                        }
                        var matchedEdges = AlgorithmGreedyFunctions.GetMaximumMatching(leftNodes, rightNodes, matchingEdges, rand);
                        AlgorithmGreedyFunctions.UpdateControlPath(matchedEdges, controlPath);
                        var unmatchedRightNodes = AlgorithmGreedyFunctions.GetUnmatchedNodes(currentTargets, matchedEdges);
                        var matchedLeftNodes = AlgorithmGreedyFunctions.GetMatchedNodes(matchedEdges);
                        currentTargets = matchedLeftNodes;
                        currentPathLength++;
                    } while (currentTargets.Any() && currentPathLength < greedyMaxPathLength);
                    currentRepeat++;
                }
                var c = GetControllingNodes(controlPath).Values.Distinct().Count();
                // If the current solution is better than the previously obtained best solution.
                if (c < bestResult)
                {
                    Console.WriteLine($"{c} nodes in solution.");
                    //foreach (var item in controlPath)
                    //{
                    //    Console.Write($"{item.Key} : ");
                    //    foreach (var node in item.Value)
                    //    {
                    //        Console.Write($"{node} ");
                    //    }
                    //    Console.WriteLine();
                    //}
                    //Console.WriteLine();
                    bestResult = c;
                }
                else
                {
                    currentIterationNoImprovement++;
                }
                currentIteration++;
            }
        }

        private static List<String> GetMatchedNodes(List<(string, string)> matchingEdges)
        {
            return matchingEdges.Select((edge) => edge.Item1).Distinct().ToList();
        }

        private static List<String> GetUnmatchedNodes(List<string> currentTargets, List<(string, string)> matchingEdges)
        {
            return currentTargets.Except(matchingEdges.Select((edge) => edge.Item2).Distinct()).ToList();
        }

        private static void UpdateControlPath(List<(string, string)> matchingEdges, Dictionary<string, List<string>> controlPath)
        {
            foreach (var edge in matchingEdges)
            {
                foreach (var item in controlPath)
                {
                    if (item.Value.Last() == edge.Item2)
                    {
                        item.Value.Add(edge.Item1);
                    }
                }
            }
        }

        private static List<(string, string)> GetHeuristicEdges(String target, List<(String, String)> edges, String heuristic)
        {
            // We temporarily return all edges which start with the given target.
            return edges.Where((edge) => edge.Item2 == target).ToList();
        }

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
            //var controlNodes = AlgorithmGreedyFunctions.GetControlledNodes(controlPath);
            //foreach (var node in controlNodes.Values.Distinct())
            //{
            //    if (controlNodes.Values.Count((item) => item == node) != 1)
            //    {
            //        keptTargetNodes.Add(controlNodes.First((item) => item.Value == node).Key);
            //    }
            //}
            return keptTargetNodes;
        }

        private static Dictionary<String, String> GetControlledNodes(Dictionary<String, List<String>> controlPath)
        {
            var controlNodes = new Dictionary<String, String>();
            foreach (var item in controlPath)
            {
                controlNodes[item.Key] = item.Value.Last();
            }
            return controlNodes;
        }

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

        public static void Test()
        {
            //var separators = new List<String>() { ";", "\t", "\n", "\r" };
            //var graphText = System.IO.File.ReadAllText($@"C:\Users\vpopescu\source\repos\NetControl\NetControl\Networks\TestGraph.txt");
            //var edges = AlgorithmFunctions.GetEdges(graphText, separators);
            var left = new List<String>() { "8", "10", "11", "13" };
            var right = new List<String>() { "6", "7", "9", "5", "4", "11", "12"};
            var edges = new List<(String, String)>() { ("8", "6"), ("8", "7"), ("8", "9"), ("10", "5"), ("11", "4"), ("13", "11"), ("13", "12") };
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                var j = GetMaximumMatching(left, right, edges, rand);
            }
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

        private static List<(String, String)> GetOppositeEdges(List<(String, String)> edges)
        {
            return edges.Select((item) => (item.Item2, item.Item1)).ToList();
        }
    }
}
