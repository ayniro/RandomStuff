using System;
using System.Collections.Generic;
using Graphs.Utility;

namespace Graphs.Model.Algorithms
{
    public static class MinimumSpanningTree
    {
        public static (List<(int, int)>, int) Prim(GraphMatrix g)
        {
            var returnTree = new List<(int, int)>();
            var used = new bool[g.GetVerticesAmount()];
            var minEdgeWeight = new int[g.GetVerticesAmount()];
            var endOfMinEdge = new int[g.GetVerticesAmount()];
            var matrix = GetPrimMatrix(GetOrientedMatrix(g));
            int iter = 0;
            bool hasMST = true;
            Array.Fill(minEdgeWeight, Int32.MaxValue);
            Array.Fill(endOfMinEdge, -1);

            minEdgeWeight[0] = 0;
            
            for (int i = 0; i < g.GetVerticesAmount(); ++i)
            {
                int minVertex = -1;
                for (int j = 0; j < g.GetVerticesAmount(); ++j)
                {
                    if (!used[j] && (minVertex == -1 || minEdgeWeight[j] < minEdgeWeight[minVertex]))
                    {
                        minVertex = j;
                    }
                }

                if (minEdgeWeight[minVertex] == Int32.MaxValue)
                {
                    hasMST = false;
                    break;
                }

                used[minVertex] = true;

                if (endOfMinEdge[minVertex] != -1)
                {
                    returnTree.Add((minVertex, endOfMinEdge[minVertex]));
                }

                for (int j = 0; j < g.GetVerticesAmount(); ++j)
                {
                    if (matrix[minVertex, j] < minEdgeWeight[j])
                    {
                        minEdgeWeight[j] = matrix[minVertex, j];
                        endOfMinEdge[j] = minVertex;
                    }

                    iter++;
                }
            }

            g.SetSstMatrix(GetSstMatrix(g, returnTree));
            
            return (returnTree, iter);
        }

        public static (List<(int, int)>, int) Kruskal(GraphMatrix g)
        {
            var edges = GetKruskalEdges(g);
            var spanningTree = new List<(int, int)>();
            var vertexTree = new int[g.GetVerticesAmount()];
            int iter = 0;
            
            for (int i = 0; i < vertexTree.Length; ++i)
            {
                vertexTree[i] = i;
            }
            
            for (int i = 0; i < edges.Count; ++i)
            {
                int first = edges[i].Item1;
                int second = edges[i].Item2;
                int weight = edges[i].Item3;

                if (vertexTree[first] != vertexTree[second])
                {
                    spanningTree.Add((first, second));
                    int oldVertexTree = vertexTree[second];
                    int newVertexTree = vertexTree[first];

                    for (int j = 0; j < g.GetVerticesAmount(); ++j)
                    {
                        if (vertexTree[j] == oldVertexTree)
                            vertexTree[j] = newVertexTree;
                        iter++;
                    }
                }
            }
            
            g.SetSstMatrix(GetSstMatrix(g, spanningTree));

            return (spanningTree, iter);
        }

        public static int TotalStAmount(GraphMatrix g)
        {
//            var kirchhoffMatrix = new int[g.GetVerticesAmount(), g.GetVerticesAmount()];
//            var orientedMatrix = GetOrientedMatrix(g);
//
//            for (int i = 0; i < g.GetVerticesAmount(); ++i)
//            {
//                int degree = 0;
//                for (int j = 0; j < g.GetVerticesAmount(); ++j)
//                {
//                    if (orientedMatrix[i, j] > 0 || orientedMatrix[j, i] > 0)
//                    {
//                        degree++;
//                        kirchhoffMatrix[i, j] = -1;
//                    }
//                }
//
//                kirchhoffMatrix[i, i] = degree;
//            }
//
//            for (int i = 0; i < kirchhoffMatrix.GetLength(0); ++i)
//            {
//                for (int j = 0; j < kirchhoffMatrix.GetLength(0); ++j)
//                {
//                    Console.Out.Write(kirchhoffMatrix[i, j] + " ");
//                }
//
//                Console.Out.WriteLine("");
//            }
//
//            return CalculateKirchhoffDeterminant(kirchhoffMatrix);

            var kirchhoffMatrix = new double[g.GetVerticesAmount(), g.GetVerticesAmount()];
            var orientedMatrix = GetOrientedMatrix(g);

            for (int i = 0; i < g.GetVerticesAmount(); ++i)
            {
                int degree = 0;
                for (int j = 0; j < g.GetVerticesAmount(); ++j)
                {
                    if (orientedMatrix[i, j] != 0 || orientedMatrix[j, i] != 0)
                    {
                        degree++;
                        kirchhoffMatrix[i, j] = -1;
                    }
                }

                kirchhoffMatrix[i, i] = degree;
            }
            
            for (int i = 0; i < kirchhoffMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < kirchhoffMatrix.GetLength(0); ++j)
                {
                    Console.Out.Write(kirchhoffMatrix[i, j] + " ");
                }

                Console.Out.WriteLine("");
            }

            return (int)MatrixOperations.MatrixDeterminant(
                MatrixOperations.GetMinor(kirchhoffMatrix, 0, 0));
        }

        public static (int[,], List<int>) GetPruferCode(GraphMatrix g)
        {
            var tree = g.GetSstMatrix();
            var verticesTaken = new bool[g.GetVerticesAmount()];
            int verticesTakenAmount = 0;
            var vertices = new List<int>();

            while (verticesTakenAmount != verticesTaken.Length - 2)
            {
                int minVertex = ChooseMinLeaf(tree, verticesTaken);
                int parentVertex = -1;
                for (int i = 0; i < tree.GetLength(0); ++i)
                {
                    if (tree[i, minVertex] == 1 && !verticesTaken[i])
                    {
                        parentVertex = i;
                        break;
                    }
                }
                verticesTaken[minVertex] = true;
                vertices.Add(parentVertex);
                verticesTakenAmount++;
            }

            return (tree, vertices);
        }

        public static int[,] DecodePrufer(List<int> code)
        {
            var matrix = new int[code.Count + 2, code.Count + 2];
            var vertices = new bool[code.Count + 2];

            for (int i = 0; i < code.Count; ++i)
            {
                int minVertex = -1;

                for (int j = 0; j < vertices.Length; ++j)
                {
                    bool notInCode = true;
                    if (vertices[j]) continue;
                    
                    for (int k = i; k < code.Count; ++k)
                    {
                        if (code[k] - 1 == j)
                        {
                            notInCode = false;
                            break;
                        }
                    }

                    if (notInCode)
                    {
                        minVertex = j;
                        vertices[minVertex] = true;
                        break;
                    }
                }

                if (minVertex != -1)
                {
                    matrix[code[i] - 1, minVertex] = 1;
                    matrix[minVertex, code[i] - 1] = 1;
                }
            }

            int first = -1;
            int second = -1;
            for (int i = 0; i < vertices.Length; ++i)
            {
                if (!vertices[i])
                {
                    first = i;
                    break;
                }
            }
            
            for (int i = first + 1; i < vertices.Length; ++i)
            {
                if (!vertices[i])
                {
                    second = i;
                }
            }

            matrix[first, second] = 1;
            matrix[second, first] = 1;
            
            return matrix;
        }

        private static int ChooseMinLeaf(int[,] matrix, bool[] verticesTaken)
        {
            var leafs = new List<int>();

            //Console.Out.WriteLine(MatrixPrinter.GetMatrix(matrix));
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                int parents = 0;
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[i, j] == 1)
                    {
                        parents++;
                    }
                }

                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (matrix[i, j] == 1 && verticesTaken[j])
                    {
                        parents--;
                    }
                }
                
                if (parents == 1 && !verticesTaken[i]) leafs.Add(i);
            }
            
            leafs.Sort();

            Console.Out.WriteLine("Leafs: " + String.Join(" ", leafs));
            
            return leafs[0];
        }
        
        private static int[,] GetPrimMatrix(int[,] weightMatrix)
        {
            var matrix = new int[weightMatrix.GetLength(0), weightMatrix.GetLength(0)];
            
            for (int i = 0; i < weightMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < weightMatrix.GetLength(0); ++j)
                {
                    if (weightMatrix[i, j] == 0)
                    {
                        matrix[i, j] = Int32.MaxValue;
                    }
                    else
                    {
                        matrix[i, j] = weightMatrix[i, j];
                    }
                }
            }

            return matrix;
        }
        
        private static List<(int, int, int)> GetKruskalEdges(GraphMatrix g)
        {
            var edges = new List<(int, int, int)>();
            var matrix = g.GetWeightMatrix();
            
            for (int i = 0; i < g.GetVerticesAmount(); ++i)
            {
                for (int j = 0; j < g.GetVerticesAmount(); j++)
                {
                    if (matrix[i, j] != 0)
                    {
                        edges.Add((i, j, matrix[i, j]));
                    }
                }
            }
            
            edges.Sort((first, second) => first.Item3.CompareTo(second.Item3));

            return edges;
        }

        private static int[,] GetOrientedMatrix(GraphMatrix g)
        {
            var matrix = g.GetWeightMatrix();
            var orientedMatrix = new int[matrix.GetLength(0), matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (matrix[i, j] != 0)
                    {
                        orientedMatrix[i, j] = matrix[i, j];
                        orientedMatrix[j, i] = matrix[i, j];
                    }
                }
            }

            return orientedMatrix;
        }

        private static int CalculateKirchhoffDeterminant(int[,] matrix)
        {
            return MatrixOperations.CalculateDeterminant(
                MatrixOperations.GetMinor(matrix, 0, 0));
        }

        private static int[,] GetSstMatrix(GraphMatrix m, List<(int, int)> edges)
        {
            var matrix = new int[m.GetVerticesAmount(), m.GetVerticesAmount()];

            foreach (var edge in edges)
            {
                matrix[edge.Item1, edge.Item2] = 1;
                matrix[edge.Item2, edge.Item1] = 1;
            }
            
            return matrix;
        }
    }
}