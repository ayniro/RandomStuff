using System;
using System.Collections.Generic;
using Graphs.Model;
using Graphs.Model.Algorithms;
using Graphs.Utility;
using Graphs.View;
using SpaceVIL.Common;

namespace Graphs
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(CommonService.GetSpaceVILInfo());
            CommonService.InitSpaceVILComponents();
            var view = new GraphView();
            view.Show();

//              var g = new GraphMatrix(2, 1, 2, 2);
//              g.GenerateGraph();
//              g.GenerateFlowNetworkFromThis(100);
//              Console.Out.WriteLine(MatrixPrinter.GetMatrix(g.GetWeightMatrix()));
//              Console.Out.WriteLine(MatrixPrinter.GetMatrix(g.GetCapacitiesMatrix()));
//              
//              (int flow, int cost) = FlowAlgorithms.MinCostFlow(g, 0, 1,
//                  FlowAlgorithms.MaxFlow(g, 0, 1) * 2 / 3);
//              Console.Out.WriteLine(flow);
//              Console.Out.WriteLine(cost);

//            Console.Out.WriteLine(
//                MatrixPrinter.GetMatrix(
//                    MinimumSpanningTree.DecodePrufer(new List<int> {1, 1, 3, 6, 4 ,1})));
//            GraphMatrix g = new GraphMatrix(8, 1, -5, 5);
//            g.SetSstMatrix(new int[,]
//            {
//                {0, 1, 0, 1, 1, 0, 0, 1},
//                {1, 0, 0, 0, 0, 0, 0, 0},
//                {0, 0, 0, 0, 0, 1, 1, 0},
//                {1, 0, 0, 0, 0, 1, 0, 0},
//                {1, 0, 0, 0, 0, 0, 0, 0},
//                {0, 0, 1, 1, 0, 0, 0, 0},
//                {0, 0, 1, 0, 0, 0, 0, 0},
//                {1, 0, 0, 0, 0, 0, 0, 0}
//            });
//            var a = MinimumSpanningTree.GetPruferCode(g).Item2;
//            Console.Out.WriteLine(String.Join(" ", a));
        }
    }
}