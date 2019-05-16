using System;
using System.Text;

namespace Graphs.Utility
{
    public static class MatrixPrinter
    {
        public static string GetMatrix(int[,] matrix)
        {
            int maxIntLength = 1;
            var builder = new StringBuilder();
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    if (matrix[i, j].ToString().Length > maxIntLength)
                        maxIntLength = matrix[i, j].ToString().Length;
                }
            }

            var stringFormat = "{0, " + $"{-(maxIntLength + 1)}" + "}";
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    builder.AppendFormat(stringFormat, matrix[i, j]);
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
        
        public static string GetMatrix(double[,] matrix)
        {
            int maxIntLength = 1;
            var builder = new StringBuilder();
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    if (Math.Round(matrix[i, j], 2).ToString().Length > maxIntLength)
                        maxIntLength = Math.Round(matrix[i, j], 2).ToString().Length;
                }
            }

            var stringFormat = "{0, " + $"{-(maxIntLength + 1)}" + "}";
            
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                {
                    builder.AppendFormat(stringFormat, Math.Round(matrix[i, j], 2));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}