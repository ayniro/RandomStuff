using System;
using System.Collections.Generic;

namespace Graphs.Model.Algorithms
{
    public static class FlowAlgorithms
    {
        public static void TurnIntoFlowNetwork(GraphMatrix g)
        {
            g.GenerateFlowNetworkFromThis(100);
        }

        public static int MaxFlow(GraphMatrix g, int s, int t)
        {
            var graph = g.GetCapacitiesMatrix();
            var resGraph = new int[graph.GetLength(0), graph.GetLength(0)];
            Array.Copy(graph, resGraph, graph.Length);

            var parents = new int[graph.GetLength(0)];

            int maxFlow = 0;
            while (FlowBreadthFirstSearch(resGraph, s, t, parents))
            {
                int pathFlow = Int32.MaxValue;
                for (int v = t; v != s; v = parents[v])
                {
                    int u = parents[v];
                    pathFlow = Math.Min(pathFlow, resGraph[u, v]);
                }

                for (int v = t; v != s; v = parents[v])
                {
                    int u = parents[v];
                    resGraph[u, v] -= pathFlow;
                    resGraph[v, u] += pathFlow;
                }

                maxFlow += pathFlow;
            }
            
            return maxFlow;
        }

        public static (int, int) MinCostFlow(GraphMatrix g, int s, int t, int flow)
        {
            var graph = g.GetCapacitiesMatrix();
            var weights = g.GetWeightMatrix();
            var adjMatrix = g.GetAdjacencyMatrix();
            var resGraph = new int[graph.GetLength(0), graph.GetLength(0)];
            Array.Copy(graph, resGraph, graph.Length);
            int maxFlow = 0;
            int flowCost = 0;

            var (shortestPath, distances) =
                FlowBellmanFord(weights, adjMatrix, resGraph, s, t);

            while (shortestPath.Count != 0)
            {
                int pathFlow = flow - maxFlow;
                for (int i = shortestPath.Count - 1; i > 0; --i)
                {
                    pathFlow = Math.Min(pathFlow, resGraph[
                        shortestPath[i - 1],
                        shortestPath[i]]);
                }

                maxFlow += pathFlow;
                flowCost += (maxFlow >= flow ? pathFlow - (maxFlow - flow) : pathFlow) *
                            distances[t];
                if (maxFlow >= flow) break;

                for (int i = shortestPath.Count - 1; i > 0; --i)
                {
                    resGraph[shortestPath[i - 1], shortestPath[i]] -= pathFlow;
                    resGraph[shortestPath[i], shortestPath[i - 1]] += pathFlow;
                }

                (shortestPath, distances) =
                    FlowBellmanFord(weights, adjMatrix, resGraph, s, t);
            }

            return (maxFlow > flow ? flow : maxFlow, flowCost);
        }

        private static (List<int>, int[]) FlowBellmanFord(int[,] weightMatrix, int[,] adjMatrix, int[,] resGraph, int first, int second)
        {
            var returnPath = new List<int>();
            var distances = GetDistances(weightMatrix.GetLength(0), first);
            var ancestorNodes = new int[weightMatrix.GetLength(0)];
            Array.Fill(ancestorNodes, -1);

            for (int i = 0; i < weightMatrix.GetLength(0) - 1; ++i)
            {
                bool relaxed = false;
                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    for (int k = 0; k < weightMatrix.GetLength(0); ++k)
                    {
                        if (adjMatrix[j, k] != 0 && resGraph[j, k] > 0)
                        {
                            if (distances[j] != Int32.MaxValue && 
                                distances[j] + weightMatrix[j, k] < distances[k])
                            {
                                distances[k] = distances[j] + weightMatrix[j, k];
                                ancestorNodes[k] = j;
                                relaxed = true;
                            }
                        }
                    }
                }
                
                if (!relaxed) break;
            }

            if (distances[second] != Int32.MaxValue)
            {
                for (int j = second; j != first; j = ancestorNodes[j])
                {
                    if (j == ancestorNodes[j]) break;
                    returnPath.Add(j);
                }

                returnPath.Add(first);
                returnPath.Reverse();
            }
            
            return (returnPath, distances);
        }
        
        private static int[] GetDistances(int length, int node)
        {
            var distances = new int[length];
            for (int i = 0; i < distances.Length; ++i)
            {
                if (i != node)
                    distances[i] = Int32.MaxValue;
                else
                    distances[i] = 0;
            }

            return distances;
        }
        
        private static bool FlowBreadthFirstSearch(int[,] resMatrix, int s, int t, int[] parent)
        {
            var visited = new bool[resMatrix.GetLength(0)];
            
            var queue = new Queue<int>();
            queue.Enqueue(s);
            visited[s] = true;
            parent[s] = -1;

            while (queue.Count != 0)
            {
                int u = queue.Dequeue();

                for (int v = 0; v < resMatrix.GetLength(0); ++v)
                {
                    if (!visited[v] && resMatrix[u, v] != 0)
                    {
                        queue.Enqueue(v);
                        parent[v] = u;
                        visited[v] = true;
                    }
                }
            }

            return visited[t];
        }
    }
}