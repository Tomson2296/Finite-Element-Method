using System;
using static ConsoleApp1.MES_Main_Structures;

namespace ConsoleApp1
{
    internal class MES_Aggregation_Matrix
    {
        public class H_Aggregation_Matrix
        {
            public H_Aggregation_Matrix(MES_Main_Structures.Grid grid, MES_H_Matrix.H_Matrix h2_Matrix)
            {
                H2_Aggregation_Matrix_Values = calculateAggregation_H_Matrix(grid, h2_Matrix);
            }
            double[,] H2_Aggregation_Matrix_Values { get; set; }
            
            // funkcja zwracająca macierz zaagregowaną Aggregation H Matrix
            public double[,] getAggregation_H_Matrix()
            {
                return H2_Aggregation_Matrix_Values;
            }
              
            // funkcja wyświetlająca macierz zaagregowaną Aggregation H Matrix
            public void showAggregation_H_Matrix()
            {
                Console.WriteLine("Macierz H zaagregowana");
                for (int i = 0; i < H2_Aggregation_Matrix_Values.GetLength(0); i++)
                {
                    for (int j = 0; j < H2_Aggregation_Matrix_Values.GetLength(1); j++)
                    {
                        if (j == H2_Aggregation_Matrix_Values.GetLength(1) - 1)
                        {
                            Console.Write(Math.Round(H2_Aggregation_Matrix_Values[i, j], 3, MidpointRounding.AwayFromZero) + "\n");
                        }
                        else
                        {
                            Console.Write(Math.Round(H2_Aggregation_Matrix_Values[i, j], 3, MidpointRounding.AwayFromZero) + " ");
                        }
                    }
                }
                Console.WriteLine();
            }

            // prywatna funkcja wywoływana w konstruktorze odpowiedzialna za tworzenie macierzy agregacji 
            private double[,] calculateAggregation_H_Matrix(MES_Main_Structures.Grid grid, MES_H_Matrix.H_Matrix h2_Matrix)
            {
                int numberOfGridElements = grid.numberOfElements;
                int numberOfGridNodes = grid.numberOfNodes;
                double[,,] H2GlovalMatrix = h2_Matrix.get_H_Global_Matrix();

                double[,] aggregationMatrix = new double[numberOfGridNodes, numberOfGridNodes];

                for (int i = 0; i < numberOfGridElements; i++)
                {
                    appendValuesToAggregation_H_Matrix(grid.getElement(i), H2GlovalMatrix, i, aggregationMatrix);
                }
                return aggregationMatrix;
            }

            // funkcja dodająca odpowiednie wartości z macierzy H + Hbc do macierzy agregacji
            private void appendValuesToAggregation_H_Matrix(Node[] nodesList, double[,,] H2GlovalMatrix, int elementIndex, double[,] aggregationMatrix)
            {
                int i = 0;
                foreach (Node node in nodesList)
                {
                    aggregationMatrix[node.ID - 1, nodesList[0].ID - 1] += H2GlovalMatrix[elementIndex, i, 0];
                    aggregationMatrix[node.ID - 1, nodesList[1].ID - 1] += H2GlovalMatrix[elementIndex, i, 1];
                    aggregationMatrix[node.ID - 1, nodesList[2].ID - 1] += H2GlovalMatrix[elementIndex, i, 2];
                    aggregationMatrix[node.ID - 1, nodesList[3].ID - 1] += H2GlovalMatrix[elementIndex, i, 3];
                    i++;
                }
            }
        }

        public class C_Aggregation_Matrix
        {
            public C_Aggregation_Matrix(MES_Main_Structures.Grid grid, MES_C_Matrix.C_Matrix c2_Matrix)
            {
                C2_Aggregation_Matrix_Values = calculateAggregation_C_Matrix(grid, c2_Matrix);
            }
            double[,] C2_Aggregation_Matrix_Values { get; set; }

            // funkcja zwracająca macierz zaagregowaną Aggregation C Matrix
            public double[,] getAggregation_C_Matrix()
            {
                return C2_Aggregation_Matrix_Values;
            }

            // funkcja wyświetlająca macierz zaagregowaną Aggregation C Matrix
            public void showAggregation_C_Matrix()
            {
                Console.WriteLine("Macierz C zaagregowana");
                for (int i = 0; i < C2_Aggregation_Matrix_Values.GetLength(0); i++)
                {
                    for (int j = 0; j < C2_Aggregation_Matrix_Values.GetLength(1); j++)
                    {
                        if (j == C2_Aggregation_Matrix_Values.GetLength(1) - 1)
                        {
                            Console.Write(Math.Round(C2_Aggregation_Matrix_Values[i, j], 3, MidpointRounding.AwayFromZero) + "\n");
                        }
                        else
                        {
                            Console.Write(Math.Round(C2_Aggregation_Matrix_Values[i, j], 3, MidpointRounding.AwayFromZero) + " ");
                        }
                    }
                }
                Console.WriteLine();
            }

            private double[,] calculateAggregation_C_Matrix(MES_Main_Structures.Grid grid, MES_C_Matrix.C_Matrix c2_Matrix)
            {
                int numberOfGridElements = grid.numberOfElements;
                int numberOfGridNodes = grid.numberOfNodes;
                double[,,] C2GlovalMatrix = c2_Matrix.get_C_Global_Matrix();

                double[,] aggregationMatrix = new double[numberOfGridNodes, numberOfGridNodes];
                for (int i = 0; i < numberOfGridElements; i++)
                {
                    appendValuesToAggregation_C_Matrix(grid.getElement(i), C2GlovalMatrix, i, aggregationMatrix);
                }
                return aggregationMatrix;
            }

            private void appendValuesToAggregation_C_Matrix(Node[] nodesList, double[,,] C2GlovalMatrix, int elementIndex, double[,] aggregationMatrix)
            {
                int i = 0;
                foreach (Node node in nodesList)
                {
                    aggregationMatrix[node.ID - 1, nodesList[0].ID - 1] += C2GlovalMatrix[elementIndex, i, 0];
                    aggregationMatrix[node.ID - 1, nodesList[1].ID - 1] += C2GlovalMatrix[elementIndex, i, 1];
                    aggregationMatrix[node.ID - 1, nodesList[2].ID - 1] += C2GlovalMatrix[elementIndex, i, 2];
                    aggregationMatrix[node.ID - 1, nodesList[3].ID - 1] += C2GlovalMatrix[elementIndex, i, 3];
                    i++;
                }
            }
        }
    }
}
