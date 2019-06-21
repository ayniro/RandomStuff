using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graphs.Model.Algorithms
{
    public static class CyclesAlgorithms
    {
        public static GraphMatrix MakeEulerian(GraphMatrix g)
        {
            var graph = g.GetCopy();
            graph.MakeUndirected();

            if (graph.GetNumberOfVertices() > 2)
            {
                UnconnectedOddEliminate(graph);
                ConnectedHighOddEliminate(graph);
                ConnectedOddEliminate(graph);
                SingleOddEliminate(graph);
                UnconnectedOddEliminate(graph);
            }

            return graph;
        }

        public static GraphMatrix MakeHamiltonian(GraphMatrix g)
        {
            var graph = g.GetCopy();
            graph.MakeUndirected();

            if (graph.GetNumberOfVertices() > 2)
            {
                int vertices = graph.GetNumberOfVertices();
                for (int i = 0; i < vertices; ++i)
                {
                    int j = 0;
                    while (graph.GetDegree(i) < vertices / 2 + vertices % 2)
                    {
                        if (i != j && !graph.HasEdge(i, j))
                        {
                            graph.AddEdge(i, j);
                        }

                        j++;
                    }
                }
            }

            return graph;
        }

        public static List<int> FindEulerianCycle(GraphMatrix g)
        {
            var cycle = new List<int>();
            var graph = g.GetCopy();
            var stack = new Stack<int>();
            stack.Push(0);

            while (stack.Count != 0)
            {
                int v = stack.Peek();
                for (int i = 0; i < graph.GetNumberOfVertices(); ++i)
                {
                    if (graph.HasEdge(v, i))
                    {
                        stack.Push(i);
                        graph.DeleteEdge(v, i);
                        break;
                    }
                }

                if (v == stack.Peek())
                {
                    stack.Pop();
                    cycle.Add(v);
                }
            }
            
            return cycle;
        }

        public static List<List<int>> FindHamiltonianCycles(GraphMatrix g)
        {
            var hamiltonianPaths = new List<List<int>>();
            
            foreach (int vertex in g.GetAdjacentVertices(0))
            {
                var paths = 
                    GeneratePermutations(g.GetNumberOfVertices(), 0, vertex);
            
                foreach (var path in paths)
                {
                    if (!g.HasEdge(0, path[0]) || 
                        !g.HasEdge(path[path.Count - 1], vertex)) continue;
                    bool hamiltonianPath = true;
                    for (int i = 0; i < path.Count - 1; ++i)
                    {
                        if (!g.HasEdge(path[i], path[i + 1]))
                        {
                            hamiltonianPath = false;
                            break;
                        }
                    }

                    if (hamiltonianPath)
                    {
                        path.Insert(0, 0);
                        path.Insert(path.Count, vertex);
                        path.Insert(path.Count, 0);
                        hamiltonianPaths.Add(path);
                    }
                }
            }
            
            /*
            var builder = new StringBuilder();
            for (int i = 0; i < hamiltonianPaths.Count; ++i)
            {
                builder.AppendLine(
                    String.Join(
                        " -- ", 
                        hamiltonianPaths[i].Select(x => x += 1).ToArray()));
            }

            Console.Out.WriteLine(builder.ToString());
            */
            
            return hamiltonianPaths;
        }

        private static List<List<int>> GeneratePermutations(int n, int first, int last)
        {
            var permutations = new List<List<int>>();
            var list = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                if (i != first && i != last)
                    list.Add(i);
            }
            
            FillPermutations(permutations, list, list.Count);
            
            return permutations;
        }

        private static void FillPermutations(List<List<int>> perms, List<int> arr, int size)
        {
            if (size == 1)
            {
                perms.Add(arr.ToArray().ToList());
            }

            for (int i = 0; i < size; ++i)
            {
                FillPermutations(perms, arr, size - 1);

                if (size % 2 == 1)
                {
                    int tmp = arr[0];
                    arr[0] = arr[size - 1];
                    arr[size - 1] = tmp;
                }
                else
                {
                    int tmp = arr[i];
                    arr[i] = arr[size - 1];
                    arr[size - 1] = tmp;
                }
            }
        }
        
        private static void UnconnectedOddEliminate(GraphMatrix graph)
        {
            var oddVertices = graph.GetOddVertices();
            var used = new bool[graph.GetNumberOfVertices()];
            for (int i = 0; i < oddVertices.Count; ++i)
            {
                int first = oddVertices[i];
                for (int j = 0; j < oddVertices.Count; ++j)
                {
                    int second = oddVertices[j];
                    if (!used[first] && !used[second] && 
                        i != j && !graph.HasEdge(first, second))
                    {
                        graph.AddEdge(first, second);
                        used[first] = true;
                        used[second] = true;
                        break;
                    }
                }
            }
        }

        private static void ConnectedHighOddEliminate(GraphMatrix graph)
        {
            var oddVertices = graph.GetOddVertices();
            var used = new bool[graph.GetNumberOfVertices()];
            for (int i = 0; i < oddVertices.Count; ++i)
            {
                int first = oddVertices[i];
                for (int j = 0; j < oddVertices.Count; ++j)
                {
                    int second = oddVertices[j];
                    if (!used[first] && !used[second] && i != j &&
                        graph.GetDegree(first) > 1 &&
                        graph.GetDegree(second) > 1 &&
                        graph.HasEdge(first, second))
                    {
                        graph.DeleteEdge(first, second);
                        used[first] = true;
                        used[second] = true;
                    }
                }
            }
        }
        
        private static void ConnectedOddEliminate(GraphMatrix graph)
        {
            var oddVertices = graph.GetOddVertices();
            var used = new bool[graph.GetNumberOfVertices()];
            for (int i = 0; i < oddVertices.Count; ++i)
            {
                int first = oddVertices[i];
                for (int j = 0; j < oddVertices.Count; ++j)
                {
                    int second = oddVertices[j];
                    if (!used[first] && !used[second] && i != j)
                    {
                        for (int k = 0; k < graph.GetNumberOfVertices(); ++k)
                        {
                            if (k == first || k == second || graph.GetDegree(k) % 2 != 0)
                                continue;
                            if (!graph.HasEdge(first, k) && !graph.HasEdge(second, k))
                            {
                                graph.AddEdge(first, k);
                                graph.AddEdge(second, k);
                                used[first] = true;
                                used[second] = true;
                            }
                        }
                    }
                }
            }
        }

        private static void SingleOddEliminate(GraphMatrix graph)
        {
            var oddVertices = graph.GetOddVertices();
            int odd = -1;
            foreach (int v in oddVertices)
            {
                if (graph.GetDegree(v) > 1)
                {
                    odd = v;
                    break;
                }
            }
            
            if (odd == -1) return;

            for (int i = 0; i < graph.GetNumberOfVertices(); ++i)
            {
                if (graph.HasEdge(odd, i) && graph.GetDegree(i) != 1)
                {
                    graph.DeleteEdge(odd, i);
                    break;
                }
            }
        }
    }
}