using System;
using System.Collections.Generic;
using Graphs.Utility;

namespace Graphs.Model.Algorithms
{
    public static class ShortestPathFinder
    {
        public static (List<int>, int[], int[], int) Dijkstra(int[,] weightMatrix, int first, int second)
        {
            int iter = 0;
            first--;
            second--;
            var returnPath = new List<int>();
            var markedNodes = new bool[weightMatrix.GetLength(0)];
            // Ancestors of all nodes except source
            var ancestorNodes = new int[weightMatrix.GetLength(0)];
            ancestorNodes[first] = -1;
            // Distances of shortest paths to all vertices
            var distances = GetDistances(weightMatrix.GetLength(0), first);

            for (int i = 0; i < weightMatrix.GetLength(0); ++i)
            {
                int currVertex = -1;
                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    if (!markedNodes[j] && (currVertex == -1 || distances[j] < distances[currVertex]))
                    {
                        currVertex = j;
                    }
                }

                if (distances[currVertex] == Int32.MaxValue) break;
                markedNodes[currVertex] = true;

                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    if (weightMatrix[currVertex, j] > 0)
                    {
                        int nextVertex = j;
                        int weight = weightMatrix[currVertex, nextVertex];
                        if (distances[currVertex] + weight < distances[nextVertex])
                        {
                            distances[nextVertex] = distances[currVertex] + weight;
                            ancestorNodes[nextVertex] = currVertex;
                        }
                    }
                    iter++;
                }
            }

            // Getting shortest path
            if (distances[second] != Int32.MaxValue)
            {
                for (int j = second; j != first; j = ancestorNodes[j])
                {
                    if (j == ancestorNodes[j]) break;
                    returnPath.Add(j + 1);
                }

                returnPath.Add(first + 1);
                returnPath.Reverse();
            }

            return (returnPath, distances, ancestorNodes, iter);
        }

        public static (List<int>, int[], int, bool) BellmanFord(int[,] weightMatrix, int first, int second)
        {
            int iter = 0;
            first--;
            second--;
            var returnPath = new List<int>();
            var distances = GetDistances(weightMatrix.GetLength(0), first);
            var ancestorNodes = new int[weightMatrix.GetLength(0)];
            Array.Fill(ancestorNodes, -1);
            bool hasNegativeCycles = false;

            for (int i = 0; i < weightMatrix.GetLength(0) - 1; ++i)
            {
                bool relaxed = false;
                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    for (int k = 0; k < weightMatrix.GetLength(0); ++k)
                    {
                        if (weightMatrix[j, k] != 0)
                        {
                            if (distances[j] != Int32.MaxValue && 
                                distances[j] + weightMatrix[j, k] < distances[k])
                            {
                                distances[k] = distances[j] + weightMatrix[j, k];
                                ancestorNodes[k] = j;
                                relaxed = true;
                            }
                            iter++;
                        }
                    }
                }
                
                if (!relaxed) break;
            }
            
            for (int j = 0; j < weightMatrix.GetLength(0); ++j)
            {
                for (int k = 0; k < weightMatrix.GetLength(0); ++k)
                {
                    if (weightMatrix[j, k] > 0)
                    {
                        if (distances[j] != Int32.MaxValue && 
                            distances[j] + weightMatrix[j, k] < distances[k])
                        {
                            hasNegativeCycles = true;
                        }

                        //iter++;
                    }
                }
            }

            if (distances[second] != Int32.MaxValue)
            {
                for (int j = second; j != first; j = ancestorNodes[j])
                {
                    if (j == ancestorNodes[j]) break;
                    returnPath.Add(j + 1);
                }

                returnPath.Add(first + 1);
                returnPath.Reverse();
            }
            
            return (returnPath, distances, iter, hasNegativeCycles);
        }

        public static (List<int>, int[,], int, bool) Floyd(int[,] weightMatrix, int first, int second)
        {
            int iter = 0;
            first--;
            second--;
            var floydWeightMatrix = GetFloydMatrix(weightMatrix);
            var pathMatrix = new int[weightMatrix.GetLength(0), weightMatrix.GetLength(0)];
            bool hasNegativeCycle = false;

            for (int i = 0; i < pathMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < pathMatrix.GetLength(0); ++j)
                {
                    if (floydWeightMatrix[i, j] != Int32.MaxValue)
                    {
                        pathMatrix[i, j] = j;
                    }
                }
            }

            for (int i = 0; i < weightMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    for (int k = 0; k < weightMatrix.GetLength(0); ++k)
                    {
                        if (i != j &&
                            floydWeightMatrix[j, i] != Int32.MaxValue &&
                            i != k &&
                            floydWeightMatrix[i, k] != Int32.MaxValue &&
                            (floydWeightMatrix[j, k] == Int32.MaxValue ||
                             floydWeightMatrix[j, k] >
                             floydWeightMatrix[j, i] + floydWeightMatrix[i, k]))
                        {
                            pathMatrix[j, k] = pathMatrix[j, i];
                            floydWeightMatrix[j, k] =
                                floydWeightMatrix[j, i] + floydWeightMatrix[i, k];
                        }
                        iter++;
                    }
                }

                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    if (floydWeightMatrix[j, j] < 0)
                    {
                        hasNegativeCycle = true;
                    }
                }
            }

            var returnPath = new List<int>();
            if (!hasNegativeCycle)
                returnPath = GetFloydPath(pathMatrix, first, second);

            return (returnPath, floydWeightMatrix, iter, hasNegativeCycle);
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

        private static int[,] GetFloydMatrix(int[,] weightMatrix)
        {
            var matrix = new int[weightMatrix.GetLength(0), weightMatrix.GetLength(0)];
            Array.Copy(weightMatrix, matrix, weightMatrix.Length);

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (i != j && matrix[i, j] == 0)
                    {
                        matrix[i, j] = Int32.MaxValue;
                    }
                }
            }

            return matrix;
        }

        private static List<int> GetFloydPath(int[,] pathMatrix, int first, int second)
        {
            var path = new List<int>();
            
            path.Add(first + 1);
            while (first != second)
            {
                first = pathMatrix[first, second];
                path.Add(first + 1);
            }

            return path;
        }
        
    }
}