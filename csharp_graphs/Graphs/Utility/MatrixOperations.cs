using System;

namespace Graphs.Utility
{
    public class MatrixOperations
    {
        public static int CalculateDeterminant(int[,] matrix)
        {
            if (matrix.GetLength(0) == 1) return matrix[0, 0];

            int result = 0;

            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                int mult = matrix[0, i];
                var minor = GetMinor(matrix, 0, i);
                result += mult * (i % 2 == 0 ? 1 : -1) * CalculateDeterminant(minor);
            }

            return result;
        }

        public static int[,] GetMinor(int[,] matrix, int row, int column)
        {
            var newMatrix = new int[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            int currRow = 0;
            int currColumn = 0;
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                if (i == row) continue;
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (j == column) continue;
                    newMatrix[currRow, currColumn++] = matrix[i, j];
                }

                currRow++;
                currColumn = 0;
            }

            return newMatrix;
        }
        
        public static double[,] GetMinor(double[,] matrix, int row, int column)
        {
            var newMatrix = new double[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            int currRow = 0;
            int currColumn = 0;
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                if (i == row) continue;
                for (int j = 0; j < matrix.GetLength(0); ++j)
                {
                    if (j == column) continue;
                    newMatrix[currRow, currColumn++] = matrix[i, j];
                }

                currRow++;
                currColumn = 0;
            }

            return newMatrix;
        }

        private static double[,] MatrixCreate(int rows, int cols)
        {
            // allocates/creates a matrix initialized to all 0.0. assume rows and cols > 0
            // do error checking here
            var result = new double[rows, cols];
            return result;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] MatrixDecompose(double[,] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // returns: result is L (with 1s on diagonal) and U; perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            //Check if matrix is square
            if (rows != cols)
                throw new Exception("Attempt to MatrixDecompose a non-square matrix");

            var result = MatrixDuplicate(matrix); // make a copy of the input matrix

            perm = new int[rows]; // set up row permutation result
            for (int i = 0; i < rows; ++i)
            {
                perm[i] = i;
            } // i are rows counter

            toggle = 1; // toggle tracks row swaps. +1 -> even, -1 -> odd. used by MatrixDeterminant

            for (int j = 0; j < rows - 1; ++j) // each column, j is counter for columns
            {
                double colMax = Math.Abs(result[j, j]); // find largest value in col j
                int pRow = j;
                for (int i = j + 1; i < rows; ++i)
                {
                    if (result[i, j] > colMax)
                    {
                        colMax = result[i, j];
                        pRow = i;
                    }
                }

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    var rowPtr = new double[result.GetLength(1)];

                    //in order to preserve value of j new variable k for counter is declared
                    //rowPtr[] is a 1D array that contains all the elements on a single row of the matrix
                    //there has to be a loop over the columns to transfer the values
                    //from the 2D array to the 1D rowPtr array.
                    //----transfer 2D array to 1D array BEGIN

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        rowPtr[k] = result[pRow, k];
                    }

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        result[pRow, k] = result[j, k];
                    }

                    for (int k = 0; k < result.GetLength(1); k++)
                    {
                        result[j, k] = rowPtr[k];
                    }

                    //----transfer 2D array to 1D array END

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

                if (Math.Abs(result[j, j]) < 1.0E-20) // if diagonal after swap is zero . . .
                    return null; // consider a throw

                for (int i = j + 1; i < rows; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < rows; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }
            } // main j column loop

            return result;
        } // MatrixDecompose

        // --------------------------------------------------------------------------------------------------------------
        public static double MatrixDeterminant(double[,] matrix)
        {
            int[] perm;
            int toggle;
            var lum = MatrixDecompose(matrix, out perm, out toggle);
            if (lum == null)
                throw new Exception("Unable to compute MatrixDeterminant");
            double result = toggle;
            for (int i = 0; i < lum.GetLength(0); ++i)
                result *= lum[i, i];

            return result;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] MatrixDuplicate(double[,] matrix)
        {
            // allocates/creates a duplicate of a matrix. assumes matrix is not null.
            var result = MatrixCreate(matrix.GetLength(0), matrix.GetLength(1));
            for (int i = 0; i < matrix.GetLength(0); ++i) // copy the values
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    result[i, j] = matrix[i, j];
            return result;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] ExtractLower(double[,] matrix)
        {
            // lower part of a Doolittle decomposition (1.0s on diagonal, 0.0s in upper)
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var result = MatrixCreate(rows, cols);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (i == j)
                        result[i, j] = 1.0f;
                    else if (i > j)
                        result[i, j] = matrix[i, j];
                }
            }

            return result;
        }

        // --------------------------------------------------------------------------------------------------------------
        private static double[,] ExtractUpper(double[,] matrix)
        {
            // upper part of a Doolittle decomposition (0.0s in the strictly lower part)
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            var result = MatrixCreate(rows, cols);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (i <= j)
                        result[i, j] = matrix[i, j];
                }
            }

            return result;
        }
    }
}