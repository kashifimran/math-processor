using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathProcessorLib
{
    public static class Matrix
    {
        public static void CreateFunctions()
        {            
            Function.AddFunction("matrix",       CreateMatrix); //create matrix
            Function.AddFunction("trans",        Transpose);    //transpose of matrix
            Function.AddFunction("rows",         Rows);         //Number of rows in matrix
            Function.AddFunction("columns",      Columns);      //Number of columns in matrix
            Function.AddFunction("order",        Order);        //Order of the matrix (columns rows)
            Function.AddFunction("isdiag",       IsDiagonal);   //is the matrix a diagonal matrix ?
            Function.AddFunction("isiden",       IsIdentity);   //is the matrix an identity matrix ?
            Function.AddFunction("det",          Determinant);  //determinant of the matrix
            Function.AddFunction("rref",         Rref);         //reduced row echelon form
            Function.AddFunction("ref",          Ref);          //row echelon form
            Function.AddFunction("rank",         Rank);         //rank of the matrix
            Function.AddFunction("getrow",       GetRow);       //get the row at given index in matrix as array
            Function.AddFunction("getcol",       GetCol);       //get the column at given index in matrix as array
            Function.AddFunction("getcolmatrix", GetColMatrix); //get the column at given index in matrix as a column matrix
            Function.AddFunction("addrow",       AddRow);       //add a row at specified index to matrix 
            Function.AddFunction("addcol",       AddCol);       //add a column at specified index to matrix 
            Function.AddFunction("delrows",      DeleteRows);   //delete one or more rows at specified index from matrix 
            Function.AddFunction("delcols",      DeleteCols);   //delete one or more columns at specified index from matrix 
            Function.AddFunction("getiden",      GetIdentity);  //create an identity matrix of order given argument
            Function.AddFunction("comatrix",     GetCofactorMatrix); //Get Matrix of Cofactors
            Function.AddFunction("minormatrix",  GetMinorMatrix);    //Get Matrix of Minors
            Function.AddFunction("adjugate",     GetAdjugate);       //adjugate = classical adjoint
            Function.AddFunction("cofact",       GetCofactor);       //Get cofactor for given i, j   
            Function.AddFunction("minor",        GetMinor);          //Get minor for given i, j
            Function.AddFunction("inverse",      GetInverse);        //Get inverse of the matrix if exists
            Function.AddFunction("inversedet",   GetInverseUsingDeterminant);       //Get inverse of the matrix if exists
            //Function.AddFunction("mmul",         Multiply);          //optimized multiplication
        }

        
        /*This functions is meant to do optimized multiplication by using proper multiplication 
         * order. I think the solution is some dynamic algorithm. 
         * Kashif Imran...
        */
        public static Token Multiply (string operation, List<Token> arguments)
        {
            return Token.Error("Not yet implemented. Will you like to work on it?");
        }

        public static Token GetInverse (string operation, List<Token> arguments)
        {
            if (arguments[0].TokenType != TokenType.Matrix ||
                arguments[0].Extra != arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("Inverse only possible for invertible square matrix");
            }
            Token rrToken = Rref("rref", arguments);
            List<Token> args = new List<Token>();
            args.Add(rrToken);
            if (IsIdentity("isiden", args).FirstValue == 0)
            {
                return Token.Error("Matrix is not invertible");
            }                        
            List<double> data = new List<double>();
            int rows = (int)rrToken.Extra;            
            for (int i = 0; i <rows ; i++)
            {
                data.AddRange(arguments[0].GetRange(i * rows, rows));
                data.AddRange(rrToken.GetRange(i * rows, rows));
            }
            args.Clear();
            args.Add(new Token(TokenType.Matrix, rows, data));
            Token result = Rref("rref", args);
            args[0] = result;
            args.Add(new Token(TokenType.Vector, 1));
            args.Add ( new Token(TokenType.Vector, rows));
            return DeleteCols("delcols", args);
        }

        public static Token GetInverseUsingDeterminant(string operation, List<Token> arguments)
        {
            double det = Determinant("", arguments).FirstValue;
            if (det != 0)
            {
                Token temp = GetAdjugate("", arguments);
                for (int i = 0;i < temp.Count; i++)
                {
                    temp[i] = temp[i]/det;
                }
                return temp;
            }
            return new Token(TokenType.Matrix, 1, double.NaN);
        }
        
        public static Token GetAdjugate(string operation, List<Token> arguments)
        {
            List<Token> args = new List<Token>();
            args.Add(GetCofactorMatrix("", arguments));
            return Transpose("", args);
        }

        public static Token GetMinor (string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
            {
                return Token.Error("Funtion gexpects three parameters (matrix, i, j)");
            }
            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[2].TokenType != TokenType.Vector || arguments[0].Count < 1 ||
                arguments[1].Count != 1 || arguments[2].Count != 1 || arguments[0].Extra != arguments[0].Count / arguments[0].Extra
                )
            {
                return Token.Error("Invalid argument(s)");
            }

            Token newToken = arguments[0].Clone();
            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;

            newToken.RemoveRange(((int)arguments[1].FirstValue - 1)*cols, cols);
            newToken.Extra--;
            rows--;            
            for (int i = rows - 1; i >= 0; i--)
            {
                newToken.RemoveAt(i * cols + (int)arguments[2].FirstValue - 1);
            }
            List<Token> arg = new List<Token>();
            arg.Add(newToken);
            return Determinant("", arg);
        }

        public static Token GetMinorMatrix (string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix ||
                arguments[0].Extra != arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("Function expects a square matrix as argument.");
            }
            int rows = (int)arguments[0].Extra;
            double[] data = new double[rows * rows];
            List<Token> args = new List<Token>();
            Token first = new Token(TokenType.Vector, 0);
            Token second = new Token(TokenType.Vector, 0);
            args.Add(arguments[0]);
            args.Add(first);
            args.Add(second);
            for (int i = 0; i < rows; i++)
            {
                first[0] = i + 1;
                for (int j = 0; j < rows; j++)
                {
                    second[0] = j + 1;
                    data[i * rows + j] = GetMinor("", args).FirstValue;
                }
            }
            return new Token(TokenType.Matrix, rows, data);
        }


        public static Token GetCofactorMatrix(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix ||
                arguments[0].Extra != arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("Function expects a square matrix as argument.");
            }
            int rows = (int)arguments[0].Extra;
            double[] data = new double[rows * rows];
            List<Token> args = new List<Token>();
            Token first = new Token(TokenType.Vector,0);
            Token second = new Token(TokenType.Vector,0);
            args.Add(arguments[0]);
            args.Add(first);
            args.Add(second);
            for (int i = 0; i < rows; i++)
            {
                first[0] = i+1;
                for (int j = 0; j < rows; j++)
                {
                    second[0] = j+1;
                    data[i * rows + j] = GetCofactor("", args).FirstValue;
                }
            }
            return new Token(TokenType.Matrix, rows, data);
        }


        public static Token GetCofactor(string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
            {
                return Token.Error("Funtion gexpects three parameters (matrix, i, j)");
            }
            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[2].TokenType != TokenType.Vector || arguments[0].Count < 1 ||
                arguments[1].Count != 1 || arguments[2].Count != 1 || arguments[0].Extra != arguments[0].Count / arguments[0].Extra
                )
            {
                return Token.Error("Invalid argument(s)");
            }

            Token newToken = arguments[0].Clone();
            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;

            newToken.RemoveRange(((int)arguments[1].FirstValue - 1) * cols, cols);
            newToken.Extra--;
            rows--;
            for (int i = rows - 1; i >= 0; i--)
            {
                newToken.RemoveAt(i * cols + (int)arguments[2].FirstValue - 1);
            }
            List<Token> arg = new List<Token>();
            arg.Add(newToken);

            if ((arguments[1].FirstValue + arguments[2].FirstValue) % 2 == 0)
            {
                return Determinant("", arg);
            }
            else
            {
                Token minor = Determinant("", arg);
                minor[0] = -minor[0];
                return minor;
            }
        }
   
        public static Token GetIdentity (string operation, List<Token> arguments)
        {
            if (arguments.Count != 1)
            {
                return Token.Error("Funtion getiden() expect one argument as the order of the square matrix");
            }
            if (arguments[0].Count != 1 || arguments[0].FirstValue < 1 || arguments[0].FirstValue != (int)arguments[0].FirstValue)
            {
                return Token.Error("Argument not a valid positive number");
            }

            int order = (int)arguments[0].FirstValue;
            double [] data = new double[order * order];

            for (int i = 0; i < order; i++)
            {
                for (int j = 0; j < order; j++)
                {
                    if (i == j)
                    {
                        data[i * order + j] = 1;
                    }
                    else 
                    {
                        data[i * order + j] = 0;
                    }
                }
            }
            return new Token(TokenType.Matrix, order, data);
        }

        public static Token DeleteRows(string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
            {
                return Token.Error("Three arguments required by the funtion delrows()");
            }

            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[2].TokenType != TokenType.Vector || arguments[1].Count != 1 ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 ||
                arguments[1].FirstValue > arguments[0].Extra || arguments[2].FirstValue < 1 ||
                arguments[2].FirstValue - 1 > arguments[0].Extra - arguments[1].FirstValue
                )
            {
                return Token.Error("Invalid parameter(s) passed to function delrows()");
            }

            Token newToken = arguments[0].Clone();
            newToken.RemoveRange(((int)arguments[1].FirstValue - 1) * (arguments[0].Count / (int)arguments[0].Extra), (int)arguments[2].FirstValue * (arguments[0].Count / (int)arguments[0].Extra));
            newToken.Extra -= arguments[2].FirstValue;
            return newToken;
        }

        public static Token DeleteCols(string operation, List<Token> arguments)
        {
            if (arguments.Count != 3)
            {
                return Token.Error("Three arguments required by the funtion delcols()");
            }

            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[2].TokenType != TokenType.Vector || arguments[1].Count != 1 ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 ||
                arguments[1].FirstValue > arguments[0].Count / arguments[0].Extra || arguments[2].FirstValue < 1 ||
                arguments[2].FirstValue - 1 > (arguments[0].Count / arguments[0].Extra) - arguments[1].FirstValue
                )
            {
                return Token.Error("Invalid parameter(s) passed to function delcols()");
            }

            Token newToken = arguments[0].Clone();
            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;
            
            for (int i = rows -1; i >= 0; i--)
            {
                newToken.RemoveRange(i * cols + (int)arguments[1].FirstValue-1, (int)arguments[2].FirstValue);
            }
            return newToken;
        }

        public static Token AddRow(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2 || arguments.Count > 3)
            {
                return Token.Error("Two or three arguments required by the funtion addrow()");
            }
            if (arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Invalid parameter(s) passed to function addrow()");
            }
            if (arguments.Count > 1)
            {
                if (arguments[1].TokenType != TokenType.Vector ||
                    arguments[1].Count != 1 || arguments[1].FirstValue < 1 ||
                    arguments[1].FirstValue - 1 > arguments[0].Extra)
                {
                    return Token.Error("Invalid parameter(s) passed to function addrow()");
                }
            }
            if (arguments.Count > 2)
            {
                if (arguments[2].TokenType != TokenType.Vector ||
                arguments[2].Count != arguments[0].Count / (int)arguments[0].Extra)
                {
                    return Token.Error("Invalid parameter(s) passed to function addrow()");
                }
            }

            Token newToken = arguments[0].Clone();
            if (arguments.Count == 1)
            {
                double[] data = new double[arguments[0].Count / (int)arguments[0].Extra];
                newToken.InsertRange(arguments[0].Count, data);
            }
            else if (arguments.Count == 2)
            {
                double[] data = new double[arguments[0].Count / (int)arguments[0].Extra];
                newToken.InsertRange((int)((arguments[1].FirstValue - 1) * arguments[0].Count/arguments[0].Extra), data);
            }
            else
            {
                newToken.InsertRange((int)((arguments[1].FirstValue - 1) * arguments[0].Count / arguments[0].Extra), arguments[2].VectorArray);
            }
            newToken.Extra++;
            return newToken;
        }

        public static Token AddCol(string operation, List<Token> arguments)
        {
            if (arguments.Count < 2 || arguments.Count > 3)
            {
                return Token.Error("Two or three arguments required by the funtion addcol()");
            }
            if (arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Invalid parameter(s) passed to function addcol()");
            }
            if (arguments.Count >  1)
            {
                if (arguments[1].TokenType != TokenType.Vector ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 ||
                arguments[1].FirstValue - 1 > arguments[0].Count / (int)arguments[0].Extra)
                {
                    return Token.Error("Invalid parameter(s) passed to function addcol()");
                }
            }
            if (arguments.Count > 2)
            {
                if (arguments[2].TokenType != TokenType.Vector ||
                    arguments[2].Count != arguments[0].Extra)
                {
                    return Token.Error("Invalid parameter(s) passed to function addcol()");
                }
            }

            Token newToken = arguments[0].Clone();
            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;

            if (arguments.Count == 1)
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    newToken.Insert((i+1) * cols, 0);
                }
            }
            else if (arguments.Count == 2)
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    newToken.Insert(i * cols + (int)arguments[1].FirstValue - 1, 0);
                }
            }
            else
            {
                for (int i = rows - 1; i >= 0; i--)
                {
                    newToken.Insert(i * cols + (int)arguments[1].FirstValue - 1, arguments[2][i]);
                }
            }
            return newToken;
        }


        public static Token GetRow (string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
            {
                return Token.Error("Two arguments required by the funtion getrow()");
            }
            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 || 
                arguments[1].FirstValue > arguments[0].Extra)
            {
                return Token.Error("Invalid parameter(s) passed to function getrow()");
            }
            double[] rowData = new double[arguments[0].Count / (int)arguments[0].Extra];
            int i = ((int)arguments[1].FirstValue - 1) * (arguments[0].Count / (int)arguments[0].Extra);
            for (int j = 0; j < rowData.Count(); i++, j++)
            {
                rowData[j] = arguments[0][i];
            }
            return new Token(TokenType.Vector, rowData);
        }

        public static Token GetCol(string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
            {
                return Token.Error("Two arguments required by the funtion getcol()");
            }

            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 || 
                arguments[1].FirstValue > arguments[0].Count /arguments[0].Extra)
            {
                return Token.Error("Invalid parameter(s) passed to function getcol()");
            }

            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;
            double[] colData = new double[rows];

            for (int i = (int)arguments[1].FirstValue-1, j = 0; j < rows; i+= cols, j++)
            {
                colData[j] = arguments[0][i];
            }
            return new Token(TokenType.Vector, colData);
        }

        public static Token GetColMatrix (string operation, List<Token> arguments)
        {
            if (arguments.Count != 2)
            {
                return Token.Error("Two arguments required by the funtion getcolmat()");
            }

            if (arguments[0].TokenType != TokenType.Matrix || arguments[1].TokenType != TokenType.Vector ||
                arguments[1].Count != 1 || arguments[1].FirstValue < 1 ||
                arguments[1].FirstValue > arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("Invalid parameter(s) passed to function getcolmat()");
            }

            int rows = (int)arguments[0].Extra;
            int cols = arguments[0].Count / rows;
            double[] colData = new double[rows];

            for (int i = (int)arguments[1].FirstValue - 1, j = 0; j < rows; i += cols, j++)
            {
                colData[j] = arguments[0][i];
            }
            return new Token(TokenType.Matrix, rows, colData);
        }


        public static Token Rank (string operation, List<Token> arguments)
        {
            Token echelonMatrix = Ref(operation, arguments);
            if (echelonMatrix.TokenType == TokenType.Error)
                return echelonMatrix;

            int rowCount = (int)echelonMatrix.Extra;
            int columnCount = echelonMatrix.Count / rowCount;

            bool nonZero;
            int rank = 0;
            for (int i = 0; i < rowCount; i++)
            {
                nonZero = false;
                for (int j = 0; j < columnCount; j++)
                {
                    if (echelonMatrix[i * columnCount + j] != 0)
                    {
                        nonZero = true;
                        break;
                    }
                }
                if (nonZero)
                    rank++;
            }
            return new Token(TokenType.Vector, Math.Max(1, rank));
        }

        public static Token Determinant(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix ||
                arguments[0].Extra != arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("Function expects a square Matrix as parameter");
            }
            Token matrix = arguments[0].Clone();
            double product = RowReduce(matrix);            
            for (int i = 0; i < (int)matrix.Extra; i++)
            {
                for (int j = 0; j < matrix.Count / (int)matrix.Extra; j++)
                {
                    if (i == j)
                    {
                        product *= matrix[i * (matrix.Count / (int)matrix.Extra) + j];
                        if (product == 0)
                        {
                            return new Token(TokenType.Vector, 0);
                        }
                    }
                }
            }
            return new Token(TokenType.Vector, product);
        }

        public static Token Ref(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects a Matrix as parameter");
            }
            Token matrix = arguments[0].Clone();
            RowReduce(matrix);
            return matrix;
        }

        //RowReduce reduces the argument matrix to echelon from and returns the number that was used in dividing
        public static double RowReduce(Token matrix)
        {
            int lead = 0;
            int rowCount = (int)matrix.Extra;
            int columnCount = matrix.Count / rowCount;

            double factor = 1;
            for (int r = 0; r < rowCount; r++)
            {
                lead = PivotCol(matrix, r, rowCount, columnCount);
                if (lead >= columnCount)
                    break;

                if (AdjustPivotPosition(matrix, r, lead, columnCount) != 0)
                {
                    factor = -factor;
                }

                double div = matrix[r * columnCount + lead];
                factor *= div;
                for (int j = 0; j < columnCount; j++)
                    matrix[r * columnCount + j] /= div;

                for (int j = r + 1; j < rowCount; j++)
                {
                    double sub = matrix[j * columnCount + lead];
                    for (int k = 0; k < columnCount; k++)
                        matrix[j * columnCount + k] -= (sub * matrix[r * columnCount + k]);
                }
                lead++;
            }
            return factor;
        }

        public static Token Rref (string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects a Matrix as parameter");
            }
            Token matrix = arguments[0].Clone();
            int lead = 0;
            int rowCount = (int)matrix.Extra;
            int columnCount = matrix.Count / rowCount;            

            for (int r = 0; r < rowCount; r++)
            {
                lead = PivotCol(matrix, r, rowCount, columnCount);
                if (lead >= columnCount)
                    break;
                AdjustPivotPosition(matrix, r, lead, columnCount);               

                double div = matrix[r * columnCount + lead];

                for (int j = 0; j < columnCount; j++) 
                    matrix[r * columnCount + j] /= div;

                for (int j = 0; j < rowCount; j++)
                {
                    if (j != r)
                    {
                        double sub = matrix[j * columnCount + lead];
                        for (int k = 0; k < columnCount; k++)
                            matrix[j * columnCount + k] -= (sub * matrix[r * columnCount + k]);
                    }
                }
                lead++;
            }           
            return matrix;
        }

        static int AdjustPivotPosition (Token matrix, int firstRow, int pivotCol, int colCount)
        {
            if (matrix[firstRow * colCount + pivotCol] != 0)
                return 0;
            int secondRow = firstRow;            
            while (matrix[secondRow * colCount + pivotCol] == 0)
            {
                secondRow++;
            }
            if (firstRow == secondRow)
                return 0;
            for (int j = 0; j < colCount; j++)
            {
                double temp = matrix[firstRow * colCount + j];
                matrix[firstRow * colCount + j] = matrix[secondRow * colCount + j];
                matrix[secondRow * colCount + j] = temp;
            }
            return 1;
        }

        static int PivotCol(Token matrix, int rowStart, int rowCount, int colCount)
        {
            int j = 0;
            for (; j < colCount; j++)            
            {
                for (int i = rowStart; i < rowCount; i++)
                {
                    if (matrix[i * colCount + j] != 0)
                        return j;
                }
            }
            return j;
        }
             

        public static Token IsDiagonal(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects exactly one parameter of type Matrix");
            }

            if (arguments[0].Extra != arguments[0].Count / arguments[0].Extra)
            {
                return Token.Error("The matrix is not square");
            }
            
            for (int i = 0; i < arguments[0].Count ; i++)
            {
                if (i % ((int)arguments[0].Extra + 1) != 0)
                {
                    if (arguments[0][i] != 0)
                        return new Token(TokenType.Bool, 1, 0);
                }
            }
            return new Token(TokenType.Bool, 1, 1);
        }

        public static Token IsIdentity(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix ||
                arguments[0].Extra != arguments[0].Count / arguments[0].Extra
                )
            {
                return Token.Error("Exactly one square matrix expected by the function");
            }
            for (int i = 0; i < arguments[0].Count; i++)
            {
                if (i % ((int)arguments[0].Extra + 1) != 0)
                {
                    if (arguments[0][i] != 0)
                        return new Token(TokenType.Bool, 1, 0);
                }
                else
                {
                    if (arguments[0][i] != 1)
                        return new Token(TokenType.Bool, 1, 0);
                }
            }
            return new Token(TokenType.Bool, 1, 1);
        }

        public static Token Rows(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects exactly one parameter of type Matrix");
            }
            return new Token(TokenType.Vector, arguments[0].Extra);
        }

        public static Token Columns(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects exactly one parameter of type Matrix");
            }
            return new Token(TokenType.Vector, arguments[0].Count/arguments[0].Extra);
        }

        public static Token Order(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
            {
                return Token.Error("Function expects exactly one parameter of type Matrix");
            }
            return new Token(TokenType.Vector, new double[] {arguments[0].Extra, arguments[0].Count/arguments[0].Extra});
        }

        public static Token CreateMatrix(string operation, List<Token> arguments)
        {
            if (arguments.Count < 1)
                return Token.Error ( "No argument");
            int columns = arguments[0].Count;

            for (int i = 1; i < arguments.Count; i++)
            {
                if (arguments[i].TokenType != TokenType.Vector || arguments[i].Count != columns)
                    return Token.Error ( "Argument(s) not valid");
            }
            List<double> matrixList = new List<double>();
            foreach (Token t in arguments)
            {
                matrixList.AddRange(t.VectorArray);
            }            
            Token result = new Token(TokenType.Matrix, arguments.Count, matrixList);            
            return result;
        }


        public static Token Transpose(string operation, List<Token> arguments)
        {
            if (arguments.Count != 1 || arguments[0].TokenType != TokenType.Matrix)
                return Token.Error ( "Function expects exactly one argument of type Matrix");

            double [] matrixData = arguments[0].VectorArray;
            
            int rows = (int)arguments[0].Extra;
            int cols = (int)matrixData.Count() / rows;
            
            double[] newData = new double[arguments[0].Count];
            
            for (int i = 0; i < cols; i ++)
            {
                for (int j = 0; j < rows; j++)
                {
                    newData[i * rows + j] = matrixData[j * cols + i]; 
                }
            }
            Token result = new Token(TokenType.Matrix, cols, newData);
            return result;
        }        
    }
}
