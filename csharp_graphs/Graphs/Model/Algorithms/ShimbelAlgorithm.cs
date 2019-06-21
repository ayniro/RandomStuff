using System;

namespace Graphs.Model.Algorithms
{
    public static class ShimbelAlgorithm
    {
        public static int[,] FindPaths(GraphMatrix graphMatrix, int pathLength, bool shortest = true)
        {
            var shimbelMatrix = new int[
                graphMatrix.GetNumberOfVertices(),
                graphMatrix.GetNumberOfVertices()];
            var multiplyMatrix = new int[
                graphMatrix.GetNumberOfVertices(),
                graphMatrix.GetNumberOfVertices()];

            Array.Copy(
                graphMatrix.GetWeightMatrix(),
                shimbelMatrix,
                graphMatrix.GetWeightMatrix().Length);
            Array.Copy(
                graphMatrix.GetWeightMatrix(),
                multiplyMatrix,
                graphMatrix.GetWeightMatrix().Length);

            for (int i = 0; i < pathLength - 1; ++i)
            {
                shimbelMatrix = ShimbelMatrixMultiply(shimbelMatrix, multiplyMatrix, shortest);
            }

            return shimbelMatrix;
        }

        private static int[,] ShimbelMatrixMultiply(int[,] a, int[,] b, bool shortest)
        {
            var result = new int[a.GetLength(0), b.GetLength(1)];
            
            for (int i = 0; i < a.GetLength(0); ++i)
            {
                for (int j = 0; j < b.GetLength(1); ++j)
                {
                    int sum = 0;
                    for (int k = 0; k < a.GetLength(1); ++k)
                    {
                        sum = ShimbelSum(sum, ShimbelMultiply(a[i, k], b[k, j]), shortest);
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }
        
        private static int ShimbelSum(int a, int b, bool shortestPath = true)
        {
            if (shortestPath) return ShimbelMin(a, b);
            return ShimbelMax(a, b);
        }

        private static int ShimbelMultiply(int a, int b)
        {
            if (a == 0 || b == 0) return 0;
            return a + b;
        }

        private static int ShimbelMin(int a, int b)
        {
            if (a == 0) return b;
            if (b == 0) return a;
            return Math.Min(a, b);
        }
        
        private static int ShimbelMax(int a, int b)
        {
            if (a == 0) return b;
            if (b == 0) return a;
            return Math.Max(a, b);
        }
    }
}
