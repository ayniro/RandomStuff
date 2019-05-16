using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Graphs.Model;
using Graphs.Model.Algorithms;
using Graphs.Utility;
using Graphs.View;

namespace Graphs.Presenter
{
    public interface IGraphPresenter
    {
        void OnGraphPropertiesSet(int vertices, double density, int minWeight, int maxWeight);
        void OnShimbelAlgorithmCall(IPresenterConnectedDialog dialog, int edgesAmount, bool shortestPaths);
        void OnCheckPathAlgorithmCall(IPresenterConnectedDialog dialog, int first, int second);

        void OnFindPathAlgorithmCall(
            IPresenterConnectedDialog dialog,
            int first,
            int second,
            bool dijkstra,
            bool bellmanFord,
            bool floyd);

        void OnPrimAlgorithmCall(IPresenterConnectedDialog dialog);
        void OnKruskalAlgorithmCall(IPresenterConnectedDialog dialog);
        void OnTotalSstCall(IPresenterConnectedDialog dialog);
        string OnGetPruferCall();
        void OnDecodePruferCall(List<int> code);

        void OnCreateFlowNetworkCall();
        int OnMaxFlowCall();
        int OnMinCostFlowCall();

        void SaveGraph();
        void LoadSavedGraph();
        
        bool IsGraphGenerated();
        bool IsSstGenerated();
        bool IsFlowNetworkGenerated();
        void ResetGraph();
        int MaxVertices { get; }
        int Vertices { get; }
    }

    public interface IPresenterConnectedDialog
    {
        void SetData(string data);
    }

    public class GraphPresenter : IGraphPresenter
    {
        private IGraphView _view;
        private GraphMatrix _graphMatrix;
        private GraphMatrix _reservedGraph;
        private bool _graphGenerated;
        private bool _sstGenerated;
        private bool _flowNetworkGenerated;
        private int _logEntryNumber = 1;

        public GraphPresenter(IGraphView view, int maxVertices = 25)
        {
            if (maxVertices < 2) throw new ArgumentOutOfRangeException();
            _view = view;
            _reservedGraph = null;
            MaxVertices = maxVertices;
        }

        public void OnGraphPropertiesSet(int vertices, double density, int minWeight, int maxWeight)
        {
            _graphMatrix = new GraphMatrix(vertices, density, minWeight, maxWeight);
            _graphMatrix.GenerateGraph();
            _graphGenerated = true;
            _sstGenerated = false;
            _flowNetworkGenerated = false;
            _graphMatrix.OutputToFile();
            _view.AppendToLog(
                $"{_logEntryNumber++}: Generated Graph's Adjacency Matrix:" +
                Environment.NewLine +
                _graphMatrix.GetStringAdjacencyMatrix() +
                $"Generated Graph's Weight Matrix:" + Environment.NewLine +
                _graphMatrix.GetStringWeightMatrix() + Environment.NewLine);
            SaveGraph();
            UpdateViewGraphImage();
        }

        public void OnShimbelAlgorithmCall(IPresenterConnectedDialog dialog, int edgesAmount, bool shortestPaths)
        {
            var matrix = ShimbelAlgorithm.FindPaths(_graphMatrix, edgesAmount, shortestPaths);

            string matrixStr = MatrixPrinter.GetMatrix(matrix);

            _view.AppendToLog($"{_logEntryNumber++}: Shimbel's Algorithm for " +
                              (shortestPaths ? "shortest" : "longest") +
                              $" paths ({edgesAmount} edges):" +
                              Environment.NewLine +
                              matrixStr + Environment.NewLine);

            dialog.SetData(matrixStr);
        }

        public void OnCheckPathAlgorithmCall(IPresenterConnectedDialog dialog, int first, int second)
        {
            bool result = PathExistenceChecker.CheckPathExistence(_graphMatrix.GetAdjacencyMatrix(), first, second);
            string data = $"Path between {first} and {second} " + (result ? "exists" : "doesn't exist");
            _view.AppendToLog($"{_logEntryNumber++}: Check path:" +
                              Environment.NewLine +
                              data +
                              Environment.NewLine + Environment.NewLine);
            dialog.SetData(data);
        }

        public void OnFindPathAlgorithmCall(
            IPresenterConnectedDialog dialog,
            int first,
            int second,
            bool dijkstra,
            bool bellmanFord,
            bool floyd)
        {
            string strPath;
            if (dijkstra)
            {
                strPath = EvaluateDijkstra(first, second);
            }
            else if (bellmanFord)
            {
                strPath = EvaluateBellmanFord(first, second);
            }
            else if (floyd)
            {
                strPath = EvaluateFloyd(first, second);
            }
            else
            {
                strPath = "Algorithm not specified error";
            }

            dialog.SetData(strPath);
        }

        public void OnPrimAlgorithmCall(IPresenterConnectedDialog dialog)
        {
            (var tree, int iter) = MinimumSpanningTree.Prim(_graphMatrix);
            var builder = new StringBuilder();
            _sstGenerated = true;
            int weight = 0;
            if (tree.Count > 0)
            {
                for (int i = 0; i < tree.Count; ++i)
                {
                    builder.AppendLine((tree[i].Item1 + 1) + " <-> " + (tree[i].Item2 + 1));
                    weight += _graphMatrix.
                        GetWeightMatrix()[
                            Math.Min(tree[i].Item1, tree[i].Item2),
                            Math.Max(tree[i].Item1, tree[i].Item2)];
                }
            }
            else
            {
                builder.Append("SST not found");
                _sstGenerated = false;
            }

            _view.AppendToLog($"{_logEntryNumber++}: Prim's Algorithm:" +
                              Environment.NewLine + 
                              builder.ToString() + 
                              $"Weight: {weight}" +
                              Environment.NewLine +
                              $"Iterations: {iter}" + 
                              Environment.NewLine + Environment.NewLine);
            
            dialog.SetData("Built SST");
            _graphMatrix.OutputToFileWithPrim();
            UpdateViewGraphImage();
        }

        public void OnKruskalAlgorithmCall(IPresenterConnectedDialog dialog)
        {
            (var tree, int iter) = MinimumSpanningTree.Kruskal(_graphMatrix);
            var builder = new StringBuilder();
            _sstGenerated = true;
            int weight = 0;
            if (tree.Count > 0)
            {
                for (int i = 0; i < tree.Count; ++i)
                {
                    builder.AppendLine((tree[i].Item1 + 1) + " <-> " + (tree[i].Item2 + 1));
                    weight += _graphMatrix.
                        GetWeightMatrix()[
                            Math.Min(tree[i].Item1, tree[i].Item2),
                            Math.Max(tree[i].Item1, tree[i].Item2)];
                }
            }
            else
            {
                builder.Append("SST not found");
                _sstGenerated = false;
            }

            _view.AppendToLog($"{_logEntryNumber++}: Kruskal's Algorithm:" +
                              Environment.NewLine + 
                              builder.ToString() + 
                              $"Weight: {weight}" + 
                              Environment.NewLine +
                              $"Iterations: {iter}" + 
                              Environment.NewLine + Environment.NewLine);
            
            dialog.SetData("Built SST");
            _graphMatrix.OutputToFileWithKruskal();
            UpdateViewGraphImage();
        }

        public void OnTotalSstCall(IPresenterConnectedDialog dialog)
        {
            int total = MinimumSpanningTree.TotalStAmount(_graphMatrix);
            string totalStr = total > 0 ? total.ToString() : $"More than {Int32.MaxValue}";
            _view.AppendToLog($"{_logEntryNumber++}: Kirchhoff's total amount of spanning trees:" +
                              Environment.NewLine +
                              $"Total amount: {totalStr}" +
                              Environment.NewLine + Environment.NewLine);
        }

        public string OnGetPruferCall()
        {
            var (tree, vertices) = MinimumSpanningTree.GetPruferCode(_graphMatrix);
            for (int i = 0; i < vertices.Count; ++i)
            {
                vertices[i] += 1;
            }
            string code = String.Join(" ", vertices);
            if (code.Equals("")) code = "Empty";
            
            _view.AppendToLog($"{_logEntryNumber++}: Prufer's code:" +
                              Environment.NewLine +
                              "Matrix: " +
                              Environment.NewLine +
                              MatrixPrinter.GetMatrix(tree) +
                              $"Code: {code}" +
                              Environment.NewLine + Environment.NewLine);
            
            return code;
        }

        public void OnDecodePruferCall(List<int> code)
        {
            var graphMatrix = MinimumSpanningTree.DecodePrufer(code);
            if (_graphMatrix == null)
            {
                _graphMatrix = new GraphMatrix(code.Count + 2, 1, -10, 10);
                _graphMatrix.SetAdjacencyMatrix(graphMatrix);
                _graphMatrix.SetWeightMatrix(graphMatrix);
                _graphMatrix.SetSstMatrix(graphMatrix);
                _graphGenerated = true;
                _sstGenerated = true;
                _flowNetworkGenerated = false;
            }
            else
            {
                _graphMatrix.SetSstMatrix(graphMatrix);
            }

            string pruferCode = String.Join(" ", code);
            
            _view.AppendToLog($"{_logEntryNumber++}: Prufer's Decode:" +
                              Environment.NewLine +
                              "Matrix: " +
                              Environment.NewLine +
                              MatrixPrinter.GetMatrix(graphMatrix) +
                              $"Code: {pruferCode}" +
                              Environment.NewLine + Environment.NewLine);
            
            _graphMatrix.OutputSst();
            UpdateViewGraphImage();
        }

        public void OnCreateFlowNetworkCall()
        {
            FlowAlgorithms.TurnIntoFlowNetwork(_graphMatrix);
            _flowNetworkGenerated = true;
            
            _view.AppendToLog($"{_logEntryNumber++}: Flow Network Generated:" +
                              Environment.NewLine +
                              "Adjacency Matrix: " +
                              Environment.NewLine +
                              MatrixPrinter.GetMatrix(_graphMatrix.GetAdjacencyMatrix()) +
                              "Weight Matrix: " +
                              Environment.NewLine + 
                              MatrixPrinter.GetMatrix(_graphMatrix.GetWeightMatrix()) +
                              "Capacities Matrix: " +
                              Environment.NewLine +
                              MatrixPrinter.GetMatrix(_graphMatrix.GetCapacitiesMatrix()) +
                              Environment.NewLine);
            
            _graphMatrix.OutputToFile();
            UpdateViewGraphImage();
        }

        public int OnMaxFlowCall()
        {
            int maxFlow = FlowAlgorithms.MaxFlow(_graphMatrix, 0, Vertices - 1);
            
            _view.AppendToLog($"{_logEntryNumber++}: Max Flow found:" +
                              Environment.NewLine +
                              $"Max Flow: {maxFlow}" +
                              Environment.NewLine + Environment.NewLine);
            return maxFlow;
        }

        public int OnMinCostFlowCall()
        {
            int maxFlow = OnMaxFlowCall();
            (int minCostFlow, int cost) = FlowAlgorithms.MinCostFlow(
                _graphMatrix,
                0,
                Vertices - 1,
                maxFlow * 2 / 3);
            
            _view.AppendToLog($"{_logEntryNumber++}: Max Flow found:" +
                              Environment.NewLine +
                              $"Max Flow: {maxFlow}" +
                              Environment.NewLine +
                              $"2 * Max Flow / 3: {2 * maxFlow / 3}" +
                              Environment.NewLine +
                              $"Found Flow: {minCostFlow}" +
                              Environment.NewLine +
                              $"Found Flow cost: {cost}" +
                              Environment.NewLine + Environment.NewLine);

            return minCostFlow;
        }
        
        private void UpdateViewGraphImage()
        {
            string inputFile = Path.Combine(
                System.Environment.CurrentDirectory, "Graphs/graph.txt");
            string outputFile = Path.Combine(
                System.Environment.CurrentDirectory, "Graphs/image.png");
            string output = ImageCreator.CreateImage(inputFile, outputFile, 1024);
            if (output.Equals(""))
            {
                _view.SetGraphImage(outputFile);
            }
            else
            {
                _view.SetGraphImage("notGenerated.png");
            }
        }

        public bool IsGraphGenerated()
        {
            return _graphGenerated;
        }

        public bool IsSstGenerated()
        {
            return _sstGenerated;
        }

        public bool IsFlowNetworkGenerated()
        {
            return _flowNetworkGenerated;
        }

        public void ResetGraph()
        {
            LoadSavedGraph();
            _graphMatrix.OutputToFile();
            _sstGenerated = false;
            _flowNetworkGenerated = false;
            UpdateViewGraphImage();
        }

        public void SaveGraph()
        {
            if (_graphGenerated)
            {
                _reservedGraph = _graphMatrix.GetCopy();
//                _sstGenerated = false;
//                _flowNetworkGenerated = false;
            }
        }

        public void LoadSavedGraph()
        {
            if (_reservedGraph != null)
            {
                _graphMatrix = _reservedGraph;
                UpdateViewGraphImage();
            }
        }

        public int MaxVertices { get; }

        public int Vertices => _graphMatrix.GetVerticesAmount();

        private string EvaluateDijkstra(int first, int second)
        {
            List<int> path;
            int[] dijkstraDistances = new int[0];
            int[] ancestorNodes;
            int iterations = 0;
            string distance = "";
            string strPath = "";
            bool hasNegativeWeights = _graphMatrix.HasNegativeWeights();
            
            if (!_graphMatrix.HasNegativeWeights())
            {
                (path, dijkstraDistances, ancestorNodes, iterations) =
                    ShortestPathFinder.Dijkstra(_graphMatrix.GetWeightMatrix(), first, second);

                distance = dijkstraDistances[second - 1] != Int32.MaxValue
                    ? dijkstraDistances[second - 1].ToString()
                    : "inf";
                strPath = !distance.Equals("inf")
                    ? String.Join(" -> ", path.ToArray())
                    : "Path doesn't exist";
            }

            LogDijkstra(
                first,
                second,
                distance,
                strPath,
                iterations,
                String.Join(
                    " ", dijkstraDistances).Replace(
                    Int32.MaxValue.ToString(), "inf"),
                hasNegativeWeights);

            if (strPath == "") strPath = "Graph has negative weights";
            
            return strPath;
        }

        private string EvaluateBellmanFord(int first, int second)
        {
            List<int> path;
            int[] distances;
            int iterations;
            bool hasNegativeCycles;

            (path, distances, iterations, hasNegativeCycles) =
                ShortestPathFinder.BellmanFord(_graphMatrix.GetWeightMatrix(), first, second);
            
            string distance = distances[second - 1] != Int32.MaxValue
                ? distances[second - 1].ToString()
                : "inf";
            string strPath = !distance.Equals("inf")
                ? String.Join(" -> ", path.ToArray())
                : "Path doesn't exist";
            if (hasNegativeCycles) strPath = "Graph contains negative cycles";

            LogBellmanFord(
                first,
                second,
                distance,
                strPath,
                iterations,
                String.Join(
                    " ", distances).Replace(
                    Int32.MaxValue.ToString(), "inf"),
                hasNegativeCycles);
            
            return strPath;
        }

        private string EvaluateFloyd(int first, int second)
        {
            List<int> path;
            int iterations;
            int[,] distances;
            bool hasNegativeCycle;

            (path, distances, iterations, hasNegativeCycle) =
                ShortestPathFinder.Floyd(_graphMatrix.GetWeightMatrix(), first, second);
            
            string distance = distances[first - 1, second - 1] != Int32.MaxValue
                ? distances[first - 1, second - 1].ToString()
                : "inf";
            string strPath = !distance.Equals("inf")
                ? String.Join(" -> ", path.ToArray())
                : "Path doesn't exist";

            
            var builder = new StringBuilder();
            for (int i = 0; i < distances.GetLength(0); ++i)
            {
                for (int j = 0; j < distances.GetLength(0); ++j)
                {
                    if (distances[i, j] != Int32.MaxValue)
                    {
                        builder.AppendFormat("{0, -4}", distances[i, j] + " ");
                    }
                    else
                    {
                        builder.Append("inf ");
                    }
                }

                builder.AppendLine();
            }
            
            
            LogFloyd(
                first,
                second,
                distance,
                strPath,
                iterations,
                builder.ToString(),
                hasNegativeCycle);
            
            return strPath;
        }

        private void LogDijkstra(int first,
            int second,
            string distance,
            string strPath,
            int iterations,
            string distances,
            bool hasNegativeWeights)
        {
            if (hasNegativeWeights)
            {
                _view.AppendToLog(
                    $"{_logEntryNumber++}: Dijkstra Algorithm:" +
                    Environment.NewLine +
                    "Graph has negative weights" +
                    Environment.NewLine + Environment.NewLine);
                return;
            }
            _view.AppendToLog($"{_logEntryNumber++}: Dijkstra Algorithm:" +
                              Environment.NewLine +
                              $"Path from {first} to {second}:" +
                              Environment.NewLine +
                              strPath +
                              Environment.NewLine +
                              $"Distance: {distance}" +
                              Environment.NewLine +
                              $"Iterations: {iterations}" +
                              Environment.NewLine +
                              $"Shortest distances from {first} to other nodes:" +
                              Environment.NewLine +
                              distances +
                              Environment.NewLine + Environment.NewLine);
        }

        private void LogBellmanFord(int first,
            int second,
            string distance,
            string strPath,
            int iterations,
            string distances,
            bool hasNegativeCycles)
        {
            if (hasNegativeCycles)
            {
                _view.AppendToLog(
                    $"{_logEntryNumber++}: Bellman-Ford Algorithm:" +
                    Environment.NewLine +
                    "Graph has negative cycle" +
                    Environment.NewLine + Environment.NewLine);
                return;
            }
            _view.AppendToLog($"{_logEntryNumber++}: Bellman-Ford Algorithm:" +
                              Environment.NewLine +
                              $"Path from {first} to {second}:" +
                              Environment.NewLine +
                              strPath +
                              Environment.NewLine +
                              $"Distance: {distance}" +
                              Environment.NewLine +
                              $"Iterations: {iterations}" +
                              Environment.NewLine +
                              $"Shortest distances from {first} to other nodes:" +
                              Environment.NewLine +
                              distances +
                              Environment.NewLine + Environment.NewLine);
        }

        private void LogFloyd(int first,
            int second,
            string distance,
            string strPath,
            int iterations,
            string distances,
            bool hasNegativeCycles)
        {
            if (hasNegativeCycles)
            {
                _view.AppendToLog(
                    $"{_logEntryNumber++}: Floyd Algorithm:" +
                    Environment.NewLine +
                    "Graph has negative cycle");
                return;
            }
            _view.AppendToLog($"{_logEntryNumber++}: Floyd Algorithm:" +
                              Environment.NewLine +
                              $"Path from {first} to {second}:" +
                              Environment.NewLine +
                              strPath +
                              Environment.NewLine +
                              $"Distance: {distance}" +
                              Environment.NewLine +
                              $"Iterations: {iterations}" +
                              Environment.NewLine +
                              $"Shortest distances:" +
                              Environment.NewLine +
                              distances +
                              Environment.NewLine);
        }
    }
}