using System;
using static ConsoleApp1.MES_Main_Structures;

namespace ConsoleApp1
{
    internal class MES_C_Matrix
    {
        public class C_Matrix
        {
            public C_Matrix(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, int interpolation_Points)
            {
                C2_MatrixValues = calculate_C_Array(globalData, grid, element, interpolation_Points);
            }
            public double[,,] C2_MatrixValues { get; set; }

            // pobranie macierzy C dla każdego elementu siatki // 
            public double[,,] get_C_Global_Matrix()
            {
                return C2_MatrixValues;
            }

            // wyświetlenie stworzonej macierzy C dla każdego elementu siatki // 
            public void show_C_Global_Matrix()
            {
                for (int i = 0; i < C2_MatrixValues.GetLength(0); i++)
                {
                    Console.WriteLine("Macierz C dla elementu: " + i);
                    for (int j = 0; j < C2_MatrixValues.GetLength(1); j++)
                    {
                        for (int k = 0; k < C2_MatrixValues.GetLength(2); k++)
                        {
                            if (k == C2_MatrixValues.GetLength(2) - 1)
                            {
                                Console.Write(Math.Round(C2_MatrixValues[i, j, k], 3, MidpointRounding.AwayFromZero) + "\n");

                            }
                            else
                            {
                                Console.Write(Math.Round(C2_MatrixValues[i, j, k], 3, MidpointRounding.AwayFromZero) + " ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }

            // główna funkcja obliczająca macierz C dla każdego elementu siatki //
            private double[,,] calculate_C_Array(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, int interpolation_Points)
            {
                int numberOfGridElements = grid.numberOfElements;
                double[,,] C_Array_For_Elements = new double[numberOfGridElements, interpolation_Points, 4];

                for (int i = 0; i < numberOfGridElements; i++)
                {
                    double[,,] jacob = new double[interpolation_Points, 2, 2];
                    double[] detJacob = new double[interpolation_Points];
                    double[,,] multiplication_Array = new double[interpolation_Points, interpolation_Points, 4];

                    calculateJacobian_For_Integration_Points(grid.getElement(i), jacob, element, interpolation_Points);

                    calculateDetJacobian(jacob, detJacob, interpolation_Points);

                    multiplyJacobianByDet(jacob, detJacob);

                    double[,] c_Array_For_Element = new double[interpolation_Points, 4];

                    c_Array_For_Element = calculate_C_Matrix(multiplication_Array, detJacob, globalData, element, interpolation_Points);

                    appendTo_Global_C_Array(C_Array_For_Elements, c_Array_For_Element, i);
                }
                return C_Array_For_Elements;
            }

            // kalkulacja jakobiana dla poszczególnych węzłów w elemencie //
            private void calculateJacobian_For_Integration_Points(Node[] nodes, double[,,] jacob, MES_Universal_Element.UniElement element, int interpolation_Points)
            {
                double[,] ksiValues = element.getDerivativeValues_N_Ksi();
                double[,] etaValues = element.getDerivativeValues_N_Eta();

                for (int i = 0; i < interpolation_Points; i++)
                {
                    double[] ksiResults = new double[2];
                    double[] etaResults = new double[2];

                    ksiResults[0] = nodes[0].X * ksiValues[i, 0] + nodes[1].X * ksiValues[i, 1] + nodes[2].X * ksiValues[i, 2] + nodes[3].X * ksiValues[i, 3];
                    ksiResults[1] = nodes[0].Y * ksiValues[i, 0] + nodes[1].Y * ksiValues[i, 1] + nodes[2].Y * ksiValues[i, 2] + nodes[3].Y * ksiValues[i, 3];

                    etaResults[0] = nodes[0].X * etaValues[i, 0] + nodes[1].X * etaValues[i, 1] + nodes[2].X * etaValues[i, 2] + nodes[3].X * etaValues[i, 3];
                    etaResults[1] = nodes[0].Y * etaValues[i, 0] + nodes[1].Y * etaValues[i, 1] + nodes[2].Y * etaValues[i, 2] + nodes[3].Y * etaValues[i, 3];

                    jacob[i, 0, 0] = ksiResults[0];
                    jacob[i, 0, 1] = ksiResults[1];
                    jacob[i, 1, 0] = etaResults[0];
                    jacob[i, 1, 1] = etaResults[1];
                }
            }

            // obliczenie wyznacznika macierzy jakobiego dla każdego węzła w elemencie //
            private void calculateDetJacobian(double[,,] jacob, double[] detJacob, int interpolation_Points)
            {
                for (int i = 0; i < interpolation_Points; i++)
                {
                    detJacob[i] = jacob[i, 0, 0] * jacob[i, 1, 1] - jacob[i, 0, 1] * jacob[i, 1, 0];
                }
            }

            // przemnożenie macierzy jakobiego - przez wartość wyznacznika podniesionego do potęgi -1 //
            private void multiplyJacobianByDet(double[,,] jacob, double[] detJacob)
            {
                for (int i = 0; i < jacob.GetLength(0); i++)
                {
                    jacob[i, 0, 0] *= Math.Pow(detJacob[i], -1);
                    jacob[i, 0, 1] *= Math.Pow(detJacob[i], -1) * (-1);
                    jacob[i, 1, 0] *= Math.Pow(detJacob[i], -1) * (-1);
                    jacob[i, 1, 1] *= Math.Pow(detJacob[i], -1);

                    double copy = jacob[i, 0, 0];
                    jacob[i, 0, 0] = jacob[i, 1, 1];
                    jacob[i, 1, 1] = copy;
                }
            }

            // funkcja obliczająca macierz funkcji kształtu dla każdego z punktów całkowania //
            private double[,] calculate_C_Matrix(double[,,] multiplicationArray, double[] detJacob, MES_Main_Structures.GlobalData globalData, MES_Universal_Element.UniElement element, int interpolation_Points)
            {
                for (int i = 0; i < multiplicationArray.GetLength(1); i++)
                {
                    double additionalParameters = globalData.Density * globalData.SpecificHeat * detJacob[i];
                    double[,] shapeValues = element.getShapeValues_N();
                    double[] shapeValues_Trans = {shapeValues[i, 0], shapeValues[i, 1], shapeValues[i, 2], shapeValues[i, 3]};
                    //Console.WriteLine(shapeValues[i, 0] + " " + shapeValues[i, 1] + " " + shapeValues[i, 2] + " " + shapeValues[i, 3]);
                        
                    for (int j = 0; j < multiplicationArray.GetLength(2); j++)
                    {
                            multiplicationArray[i, j, 0] = (shapeValues_Trans[j] * shapeValues_Trans[0]) * additionalParameters;
                            multiplicationArray[i, j, 1] = (shapeValues_Trans[j] * shapeValues_Trans[1]) * additionalParameters;
                            multiplicationArray[i, j, 2] = (shapeValues_Trans[j] * shapeValues_Trans[2]) * additionalParameters;
                            multiplicationArray[i, j, 3] = (shapeValues_Trans[j] * shapeValues_Trans[3]) * additionalParameters;
                    }
                }

                double[,] CArray;
                switch (interpolation_Points)
                {
                    case 4:
                        CArray = new double[4, 4];
                        for (int i = 0; i < CArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < CArray.GetLength(1); j++)
                            {
                                CArray[i, j] = multiplicationArray[0, i, j] + multiplicationArray[1, i, j] + multiplicationArray[2, i, j] + multiplicationArray[3, i, j];
                            }
                        }
                        return CArray;

                    case 9:
                        CArray = new double[9, 4];
                        double[] _3interpolationRanges = MES_Math_Constants._3interpolationRanges;
                        for (int i = 0; i < CArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < CArray.GetLength(1); j++)
                            {
                                CArray[i, j] = _3interpolationRanges[0] * _3interpolationRanges[0] * multiplicationArray[0, i, j] + _3interpolationRanges[1] * _3interpolationRanges[0] * multiplicationArray[1, i, j] + _3interpolationRanges[2] * _3interpolationRanges[0] * multiplicationArray[2, i, j]
                                    + _3interpolationRanges[0] * _3interpolationRanges[1] * multiplicationArray[3, i, j] + _3interpolationRanges[1] * _3interpolationRanges[1] * multiplicationArray[4, i, j] + _3interpolationRanges[2] * _3interpolationRanges[1] * multiplicationArray[5, i, j]
                                    + _3interpolationRanges[0] * _3interpolationRanges[2] * multiplicationArray[6, i, j] + _3interpolationRanges[1] * _3interpolationRanges[2] * multiplicationArray[7, i, j] + _3interpolationRanges[2] * _3interpolationRanges[2] * multiplicationArray[8, i, j];
                            }
                        }
                        return CArray;

                    case 16:
                        CArray = new double[16, 4];
                        double[] _4interpolationRanges = MES_Math_Constants._4interpolationRanges;
                        for (int i = 0; i < CArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < CArray.GetLength(1); j++)
                            {
                                CArray[i, j] = _4interpolationRanges[0] * _4interpolationRanges[0] * multiplicationArray[0, i, j] + _4interpolationRanges[1] * _4interpolationRanges[0] * multiplicationArray[1, i, j] + _4interpolationRanges[2] * _4interpolationRanges[0] * multiplicationArray[2, i, j]
                                   + _4interpolationRanges[3] * _4interpolationRanges[0] * multiplicationArray[3, i, j] + _4interpolationRanges[0] * _4interpolationRanges[1] * multiplicationArray[4, i, j] + _4interpolationRanges[1] * _4interpolationRanges[1] * multiplicationArray[5, i, j]
                                   + _4interpolationRanges[2] * _4interpolationRanges[1] * multiplicationArray[6, i, j] + _4interpolationRanges[3] * _4interpolationRanges[1] * multiplicationArray[7, i, j] + _4interpolationRanges[0] * _4interpolationRanges[2] * multiplicationArray[8, i, j]
                                   + _4interpolationRanges[1] * _4interpolationRanges[2] * multiplicationArray[9, i, j] + _4interpolationRanges[2] * _4interpolationRanges[2] * multiplicationArray[10, i, j] + _4interpolationRanges[3] * _4interpolationRanges[2] * multiplicationArray[11, i, j]
                                   + _4interpolationRanges[0] * _4interpolationRanges[3] * multiplicationArray[12, i, j] + _4interpolationRanges[1] * _4interpolationRanges[3] * multiplicationArray[13, i, j] + _4interpolationRanges[2] * _4interpolationRanges[3] * multiplicationArray[14, i, j]
                                   + _4interpolationRanges[3] * _4interpolationRanges[3] * multiplicationArray[15, i, j];
                            }
                        }
                        return CArray;

                    default:
                        return null;
                }
                
            }

            // funkcja dodająca wyliczona macierz C lokalną do macierzy C globalnej
            private void appendTo_Global_C_Array(double[,,] globalArray, double[,] localArray, int index)
            {
                for (int i = 0; i < localArray.GetLength(0); i++)
                {
                    for (int j = 0; j < localArray.GetLength(1); j++)
                    {
                        globalArray[index, i, j] = localArray[i, j];
                    }
                }
            }
        }
    }
}
