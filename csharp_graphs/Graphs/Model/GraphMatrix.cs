using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Graphs.Utility;

namespace Graphs.Model
{
    public class GraphMatrix
    {
        private static readonly Random Rnd = new Random();
        private readonly int _minWeight;
        private readonly int _maxWeight;
        private int[,] _adjacencyMatrix;
        private int[,] _weightMatrix;
        private int[,] _sstWeightMatrix;
        private int[,] _flowCapacitiesMatrix;
        private readonly int _randomM;
        private readonly int _randomM2;
        private readonly int _randomN;

        public GraphMatrix(int verticesAmount, double density, int minWeight = -5, int maxWeight = 20)
        {
            if (density < 0 || density > 1) throw new ArgumentOutOfRangeException();
            if (minWeight > maxWeight) throw new Exception("minWeight > maxWeight");
            _minWeight = minWeight;
            _maxWeight = maxWeight;
            
            _adjacencyMatrix = new int[verticesAmount, verticesAmount];
            _weightMatrix = new int[verticesAmount, verticesAmount];
            _sstWeightMatrix = null;
            _flowCapacitiesMatrix = null;
            
            _randomM = 1;
            _randomM2 = Math.Max((int) Math.Ceiling(verticesAmount * (1 - density)), 2);
            _randomN = verticesAmount;
        }

        public void GenerateGraph()
        {
            var degrees = GenerateVertices();
            var distributedColumn = new bool[_adjacencyMatrix.GetLength(0)];
            var distributedRow = new bool[_adjacencyMatrix.GetLength(0)];

            _adjacencyMatrix = new int[
                _adjacencyMatrix.GetLength(0),
                _adjacencyMatrix.GetLength(0)];
            _weightMatrix = new int[
                _weightMatrix.GetLength(0),
                _weightMatrix.GetLength(0)];

            foreach (int vertex in degrees)
            {
                Console.Out.Write(vertex + " ");
            }

            Console.Out.WriteLine("");

            for (int i = 0; i < degrees.Count - 1; ++i)
            {
                int degree = degrees[i];
                int border = Math.Min(degree, degrees.Count - i - 1);
                for (int j = 0; j < border; ++j)
                {
                    int r = Rnd.Next(i + 1, _adjacencyMatrix.GetLength(0));
                    if (_adjacencyMatrix[i, r] == 0)
                    {
                        _adjacencyMatrix[i, r] = 1;
                        _weightMatrix[i, r] = Rnd.Next(_minWeight, _maxWeight);
                        distributedColumn[r] = true;
                        distributedRow[i] = true;
                    }
                    else
                    {
                        border++;
                    }
                }
            }

            for (int i = 1; i < distributedColumn.Length; ++i)
            {
                if (!distributedColumn[i])
                {
                    int r = i;
                    while (r == i) r = Rnd.Next(0, i + 1);
                    _adjacencyMatrix[r, i] = 1;
                    _weightMatrix[r, i] = Rnd.Next(_minWeight, _maxWeight);
                    distributedColumn[i] = true;
                }
            }
        }

        public void GenerateFlowNetworkFromThis(int maxCapacity)
        {
            var sinks = new List<int>();

            for (int i = 0; i < _weightMatrix.GetLength(0); ++i)
            {
                bool isSink = true;
                for (int j = 0; j < _weightMatrix.GetLength(0); ++j)
                {
                    if (_adjacencyMatrix[i, j] != 0)
                    {
                        isSink = false;
                        break;
                    }
                }
                if (isSink) sinks.Add(i);
            }
            
            if (sinks.Count > 1)
            {
                var newWeightMatrix = new int[
                    _weightMatrix.GetLength(0) + 1,
                    _weightMatrix.GetLength(0) + 1];
                var newAdjacencyMatrix = new int[
                    _adjacencyMatrix.GetLength(0) + 1,
                    _adjacencyMatrix.GetLength(0) + 1];
                
                for (int i = 0; i < _weightMatrix.GetLength(0); ++i)
                {
                    for (int j = 0; j < _weightMatrix.GetLength(0); ++j)
                    {
                        newWeightMatrix[i, j] = _weightMatrix[i, j];
                        newAdjacencyMatrix[i, j] = _adjacencyMatrix[i, j];
                    }
                }

                int lastSink = newWeightMatrix.GetLength(0) - 1;
                foreach (int sink in sinks)
                {
                    newAdjacencyMatrix[sink, lastSink] = 1;
                    newWeightMatrix[sink, lastSink] = 0;
                }
                SetWeightMatrix(newWeightMatrix);
                SetAdjacencyMatrix(newAdjacencyMatrix);
                InitFlowCapacities(maxCapacity, lastSink);
            }
            else
            {
                InitFlowCapacities(maxCapacity, -1);
            }
            
//            int lastSink = _weightMatrix.GetLength(0) - 1;
//            foreach (int sink in sinks)
//            {
//                _adjacencyMatrix[sink, lastSink] = 1;
//                _weightMatrix[sink, lastSink] = Rnd.Next(_minWeight, _maxWeight);
//            }
            
            _sstWeightMatrix = null;
        }

        public void SetSstMatrix(int[,] matrix)
        {
            _sstWeightMatrix = matrix;
        }

        public void SetAdjacencyMatrix(int[,] matrix)
        {
            _adjacencyMatrix = new int[matrix.GetLength(0), matrix.GetLength(0)];
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (matrix[i, j] != 0)
                    {
                        _adjacencyMatrix[i, j] = 1;
                    }
                }
            }
        }

        public void SetWeightMatrix(int[,] matrix)
        {
            _weightMatrix = new int[matrix.GetLength(0), matrix.GetLength(0)];
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (matrix[i, j] != 0)
                    {
                        _weightMatrix[i, j] = matrix[i, j];
                    }
                }
            }
        }

        public void MakeUndirected()
        {
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); ++i)
            {
                for (int j = i; j < _adjacencyMatrix.GetLength(0); ++j)
                {
                    _weightMatrix[j, i] = _weightMatrix[i, j];
                    _adjacencyMatrix[j, i] = _adjacencyMatrix[i, j];
                }
            }

            _sstWeightMatrix = null;
            _flowCapacitiesMatrix = null;
        }

        
        public int GetVerticesAmount()
        {
            return _adjacencyMatrix.GetLength(0);
        }

        public string GetStringAdjacencyMatrix()
        {
            return MatrixPrinter.GetMatrix(_adjacencyMatrix);
        }

        public string GetStringWeightMatrix()
        {
            return MatrixPrinter.GetMatrix(_weightMatrix);
        }

        public int[,] GetSstMatrix()
        {
            var matrix = new int[
                _sstWeightMatrix.GetLength(0),
                _sstWeightMatrix.GetLength(0)];
            Array.Copy(
                _sstWeightMatrix, 
                matrix, 
                _sstWeightMatrix.Length);
            return matrix;
        }
        
        public int[,] GetWeightMatrix()
        {
            var matrix = new int[
                _weightMatrix.GetLength(0),
                _weightMatrix.GetLength(0)];
            Array.Copy(_weightMatrix, matrix, _weightMatrix.Length);
            return matrix;
        }

        public int[,] GetAdjacencyMatrix()
        {
            var matrix = new int[
                _adjacencyMatrix.GetLength(0),
                _adjacencyMatrix.GetLength(0)];
            Array.Copy(_adjacencyMatrix, matrix, _weightMatrix.Length);
            return matrix;
        }

        public int[,] GetCapacitiesMatrix()
        {
            var matrix = new int[
                _flowCapacitiesMatrix.GetLength(0),
                _flowCapacitiesMatrix.GetLength(0)];
            Array.Copy(
                _flowCapacitiesMatrix, 
                matrix, 
                _flowCapacitiesMatrix.Length);
            return matrix;
        }

        public GraphMatrix GetCopy()
        {
            var g = new GraphMatrix(
                _adjacencyMatrix.GetLength(0), 
                1, 
                _minWeight, 
                _maxWeight);
            g.SetAdjacencyMatrix(GetAdjacencyMatrix());
            g.SetWeightMatrix(GetWeightMatrix());
            return g;
        }

        public bool HasNegativeWeights()
        {
            for (int i = 0; i < _weightMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < _weightMatrix.GetLength(0); ++j)
                {
                    if (_weightMatrix[i, j] < 0)
                        return true;
                }
            }

            return false;
        }

        public bool IsDirected()
        {
            throw new NotImplementedException();
        }

        public void MakeDirected()
        {
            throw new NotImplementedException();
        }
        
        public void OutputToFile(string fileName = "graph.txt")
        {
            var stringBuilder = new StringBuilder("digraph {" + Environment.NewLine);
            //stringBuilder.AppendLine("rankdir=LR");
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < _adjacencyMatrix.GetLength(0); ++j)
                {
                    if (_adjacencyMatrix[i, j] == 1)
                    {
                        int weight = _weightMatrix[i, j];

                        string capacity = "";
                        if (_flowCapacitiesMatrix != null)
                        capacity =
                            _flowCapacitiesMatrix[i, j] == 1000000
                                ? "inf"
                                : _flowCapacitiesMatrix[i, j].ToString();
                            
                        string label = _flowCapacitiesMatrix == null ?
                            $"\"{weight}\"]" : 
                            $"\"w:{weight} c:{capacity}\"]";
                            //$"\"w:{weight} f:{Math.Round(_flowCapacitiesMatrix[i, j], 2)}\"]"; 
                        stringBuilder.AppendLine(
                            $"{i + 1} -> {j + 1} [label=" + label);
                    }
                }
            }

            stringBuilder.AppendLine("}\n");

            File.WriteAllText(
                Path.Combine(
                    System.Environment.CurrentDirectory, "Graphs/", fileName),
                stringBuilder.ToString());
        }

        public void OutputSst(string fileName = "graph.txt")
        {
            var stringBuilder = new StringBuilder("graph {" + Environment.NewLine);
            for (int i = 0; i < _sstWeightMatrix.GetLength(0); ++i)
            {
                for (int j = i; j < _sstWeightMatrix.GetLength(0); ++j)
                {
                    if (_sstWeightMatrix[i, j] == 1)
                    {
                        stringBuilder.AppendLine(
                            $"{i + 1} -- {j + 1}");
                    }
                }
            }

            stringBuilder.AppendLine("}\n");

            File.WriteAllText(
                Path.Combine(
                    System.Environment.CurrentDirectory, "Graphs/", fileName),
                stringBuilder.ToString());
        }

        public void OutputToFileWithPrim(string fileName = "graph.txt")
        {
            OutputToFileWithPath(Algorithms.MinimumSpanningTree.Prim(this).Item1);
        }
        
        public void OutputToFileWithKruskal(string fileName = "graph.txt")
        {
            OutputToFileWithPath(Algorithms.MinimumSpanningTree.Kruskal(this).Item1);
        }
        
        public void OutputToFileWithPath(List<(int, int)> edges, string fileName = "graph.txt")
        {
            var stringBuilder = new StringBuilder("digraph {" + Environment.NewLine);
            
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < _adjacencyMatrix.GetLength(0); ++j)
                {
                    if (_adjacencyMatrix[i, j] == 1)
                    {
                        int weight = _weightMatrix[i, j];
                        stringBuilder.Append(
                            $"{i + 1} -> {j + 1} [label=\"{weight}\"");
                        
                        bool red = false;
                        foreach (var edge in edges)
                        {
                            if (i == edge.Item1 && j == edge.Item2 || i == edge.Item2 && j == edge.Item1)
                            {
                                red = true;
                                break;
                            }
                        }
                        
                        stringBuilder.Append(red ? ",color=red" : "");
                        stringBuilder.AppendLine("]");
                    }
                }
            }
            
            stringBuilder.AppendLine("}\n");

            File.WriteAllText(
                Path.Combine(
                    System.Environment.CurrentDirectory, "Graphs/", fileName),
                stringBuilder.ToString());
        }

        private void InitFlowCapacities(int maxCapacity, int lastSink)
        {
            if (maxCapacity < 0) throw new System.ArgumentException();
            
            var flowCapacitiesMatrix = new int[
                _weightMatrix.GetLength(0), 
                _weightMatrix.GetLength(0)];
            for (int i = 0; i < _weightMatrix.GetLength(0); ++i)
            {
                for (int j = 0; j < _weightMatrix.GetLength(0); ++j)
                {
                    if (_adjacencyMatrix[i, j] != 0)
                    {
                        if (j != lastSink)
                        {
                            if (_flowCapacitiesMatrix == null)
                            {
                                flowCapacitiesMatrix[i, j] =
                                    Rnd.Next(0, maxCapacity);
                            }
                            else
                            {
                                if (_flowCapacitiesMatrix[i, j] != 1000000)
                                {
                                    flowCapacitiesMatrix[i, j] =
                                        Rnd.Next(0, maxCapacity);
                                }
                                else
                                {
                                    flowCapacitiesMatrix[i, j] = 1000000;
                                }
                            }
                        }
                        else
                        {
                            flowCapacitiesMatrix[i, j] = 1000000;
                        }
                    }
                }
            }

            _flowCapacitiesMatrix = flowCapacitiesMatrix;
        }
        
        private List<int> GenerateVertices()
        {
            var l = new List<int>();
            for (int i = 0; i < _adjacencyMatrix.GetLength(0); ++i)
            {
                l.Add(GenerateRandomNumber(_randomM, _randomM2, _randomN));
            }

            l.Sort((i, i1) =>
            {
                if (i == i1) return 0;
                if (i < i1) return 1;
                return -1;
            });

            return l;
        }

        private static int GenerateRandomNumber(int m, int m2, int n)
        {
            int x = 0;
            int i = 0;

            while (i < m)
            {
                double p = (double) m2 / n;
                double r = Rnd.NextDouble();
                if (r < p)
                {
                    i++;
                    n--;
                    m2--;
                }
                else
                {
                    x++;
                    n--;
                }
            }

            return x;
        }
        
        
    }
}
