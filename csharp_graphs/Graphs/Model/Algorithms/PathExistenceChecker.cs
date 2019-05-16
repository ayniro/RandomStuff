using System;

namespace Graphs.Model.Algorithms
{
    public static class PathExistenceChecker
    {
        public static bool CheckPathExistence(int[,] adjacencyMatrix, int first, int second)
        {
            first--;
            second--;
            var resultMatrix = new int[
                adjacencyMatrix.GetLength(0),
                adjacencyMatrix.GetLength(0)];
            var currMatrix = new int[
                adjacencyMatrix.GetLength(0),
                adjacencyMatrix.GetLength(0)];
            
            Array.Copy(
                adjacencyMatrix,
                currMatrix,
                adjacencyMatrix.Length);

            for (int i = 0; i < adjacencyMatrix.GetLength(0) - 1; ++i)
            {
                SumMatrices(resultMatrix, currMatrix);
                currMatrix = MultiplyMatrices(currMatrix, adjacencyMatrix);
            }

            return resultMatrix[first, second] > 0;
        }

        private static void SumMatrices(int[,] first, int[,] second)
        {
            for (int i = 0; i < first.GetLength(0); ++i)
            {
                for (int j = 0; j < first.GetLength(0); ++j)
                {
                    first[i, j] += second[i, j];
                }
            }
        }

        private static int[,] MultiplyMatrices(int[,] first, int[,] second)
        {
            var result = new int[first.GetLength(0), second.GetLength(1)];
            
            for (int i = 0; i < first.GetLength(0); ++i)
            {
                for (int j = 0; j < second.GetLength(1); ++j)
                {
                    int sum = 0;
                    for (int k = 0; k < first.GetLength(1); ++k)
                    {
                        sum += first[i, k] * second[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }
    }
}