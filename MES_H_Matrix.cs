using System;
using static ConsoleApp1.MES_Main_Structures;

namespace ConsoleApp1
{
    internal class MES_H_Matrix
    {
        public class H_Matrix
        {
            public H_Matrix(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, MES_Universal_Element.UniElement_Sides element_Sides, int interpolation_Points)
            {
                H2_MatrixValues = calculate_H_Matrix(globalData, grid, element, element_Sides, interpolation_Points);
            }
            public double[,,] H2_MatrixValues { get; set; }
            
            // metoda pobierająca utworzoną macierz globalną H dla wszystkich elementów siatki //
            public double[,,] get_H_Global_Matrix()
            {
                return H2_MatrixValues;
            }
            
            // metoda wyświetlająca utworzoną macierz H dla wszystkich elementów siatki // 
            public void show_H_Global_Matrix()
            {
                for (int i = 0; i < H2_MatrixValues.GetLength(0); i++)
                {
                    Console.WriteLine("Macierz H dla elementu: " + i);
                    for (int j = 0; j < H2_MatrixValues.GetLength(1); j++)
                    {
                        for (int k = 0; k < H2_MatrixValues.GetLength(2); k++)
                        {
                            if (k == H2_MatrixValues.GetLength(2) - 1)
                            {
                                Console.Write(Math.Round(H2_MatrixValues[i, j, k],3,MidpointRounding.AwayFromZero) + "\n");

                            }
                            else 
                            {
                                Console.Write(Math.Round(H2_MatrixValues[i, j, k], 3, MidpointRounding.AwayFromZero) + " ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }

            // metoda obliczająca macierz H dla wszystkich elementów //
            private double[,,] calculate_H_Matrix(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, MES_Universal_Element.UniElement_Sides element_Sides, int interpolation_Points)
            {
                int numberOfGridElements = grid.numberOfElements;
                double[,,] HArrayforElements = new double[numberOfGridElements, interpolation_Points, 4];

                for (int i = 0; i < numberOfGridElements; i++)
                {
                    double[,,] jacob = new double[interpolation_Points, 2, 2];
                    double[] detJacob = new double[interpolation_Points];
                    double[,,] integrationValuesX = new double[interpolation_Points, interpolation_Points, 4];
                    double[,,] integrationValuesY = new double[interpolation_Points, interpolation_Points, 4];
                    double[,,] HBCMatrix = new double[interpolation_Points, 4, 4];

                    calculateJacobianForPoints(grid.getElement(i), jacob, element, interpolation_Points);

                    calculateDetJacobian(jacob, detJacob, interpolation_Points);

                    multiplyJacobianByDet(jacob, detJacob);

                    calculateValuesforIntegrationPoints(integrationValuesX, integrationValuesY, jacob, element);

                    double[,] HArrayforElement = new double[interpolation_Points, 4];

                    HArrayforElement = calculateH(integrationValuesX, integrationValuesY, globalData, detJacob, interpolation_Points);

                    HBCMatrix = createHBCMatrix(element_Sides, grid.getElement(i), globalData, interpolation_Points);

                    combine_H_Hbc_Arrays(HArrayforElement, HBCMatrix, grid.getElement(i));

                    appendTo_Global_H_Hbc_Array(HArrayforElements, HArrayforElement, i);
                }
                return HArrayforElements;
            }

            // kalkulacja macierzy jakobiego dla poszczególnych nodów w elemencie // 
            private void calculateJacobianForPoints(Node[] nodes, double[,,] jacob, MES_Universal_Element.UniElement element, int interpolation_Points)
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

            // obliczenie wyznacznika dla macierzy jakobiego dla każdego punktu całkowania - odpowiednie przemnożenie po przekątnych macierzy jakobiego //
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

            // obliczanie macierzy dx/dEta oraz macierzy dy/dKsi dla każdego punktu całkowania //
            private void calculateValuesforIntegrationPoints(double[,,] integrationValuesX, double[,,] integrationValuesY, double[,,] jacob, MES_Universal_Element.UniElement element)
            {
                for (int i = 0; i < integrationValuesX.GetLength(0); i++)
                {
                    for (int j = 0; j < integrationValuesX.GetLength(1); j++)
                    {
                        double[] integrationXResults = new double[4];
                        calculateIntegrationX(integrationXResults, jacob, i, element);
                        
                        double[] integrationYResults = new double[4];
                        calculateIntegrationY(integrationYResults, jacob, i, element);

                        integrationValuesX[i, j, 0] = integrationXResults[0];
                        integrationValuesX[i, j, 1] = integrationXResults[1];
                        integrationValuesX[i, j, 2] = integrationXResults[2];
                        integrationValuesX[i, j, 3] = integrationXResults[3];

                        integrationValuesY[i, j, 0] = integrationYResults[0];
                        integrationValuesY[i, j, 1] = integrationYResults[1];
                        integrationValuesY[i, j, 2] = integrationYResults[2];
                        integrationValuesY[i, j, 3] = integrationYResults[3];
                    }
                }
            }

            // metoda pomocniczna wykorzystywana w funkcji powyżej - wyznaczanie macierzy dx / dEta
            private void calculateIntegrationX(double[] integrationXResults, double[,,] jacob, int index, MES_Universal_Element.UniElement element)
            {
                double[,] ksiValues = element.getDerivativeValues_N_Ksi();
                double[,] etaValues = element.getDerivativeValues_N_Eta();

                for (int i = 0; i < integrationXResults.Length; i++)
                {
                    integrationXResults[i] = jacob[index, 0, 0] * ksiValues[index, i] + jacob[index, 0, 1] * etaValues[index, i];
                }
            }

            // metoda pomocniczna wykorzystywana w funkcji powyżej - wyznaczanie macierzy dy / dKsi
            private void calculateIntegrationY(double[] integrationYResults, double[,,] jacob, int index, MES_Universal_Element.UniElement element)
            {
                double[,] ksiValues = element.getDerivativeValues_N_Ksi();
                double[,] etaValues = element.getDerivativeValues_N_Eta();

                for (int i = 0; i < integrationYResults.Length; i++)
                {
                    integrationYResults[i] = jacob[index, 1, 0] * ksiValues[index, i] + jacob[index, 1, 1] * etaValues[index, i];
                }
            }

            // metoda wyliczająca macierz H dla każdego punktu całkowania - przemnożenie odpowiednich macierzy przez ich transpozycje, mnożenie przez współczynniki //
            // przewodność cieplną oraz wyznacznik macierzy jakobiego //
            private double[,] calculateH(double[,,] integrationX, double[,,] integrationY, MES_Main_Structures.GlobalData global, double[] detJacob, int interpolation_Points)
            {
                for (int i = 0; i < integrationX.GetLength(1); i++)
                {
                    for (int j = 0; j < integrationX.GetLength(2); j++)
                    {
                        double[] integrationXForPC = { integrationX[i, j, 0], integrationX[i, j, 1], integrationX[i, j, 2], integrationX[i, j, 3] };
                        double[] integrationYForPC = { integrationY[i, j, 0], integrationY[i, j, 1], integrationY[i, j, 2], integrationY[i, j, 3] };
                       
                        integrationX[i, j, 0] = integrationXForPC[0] * integrationXForPC[j];
                        integrationX[i, j, 1] = integrationXForPC[1] * integrationXForPC[j];
                        integrationX[i, j, 2] = integrationXForPC[2] * integrationXForPC[j];
                        integrationX[i, j, 3] = integrationXForPC[3] * integrationXForPC[j];

                        integrationY[i, j, 0] = integrationYForPC[0] * integrationYForPC[j];
                        integrationY[i, j, 1] = integrationYForPC[1] * integrationYForPC[j];
                        integrationY[i, j, 2] = integrationYForPC[2] * integrationYForPC[j];
                        integrationY[i, j, 3] = integrationYForPC[3] * integrationYForPC[j];
                    }
                }
                
                double[,,] tempArray = new double[interpolation_Points, interpolation_Points, 4];
                for (int i = 0; i < tempArray.GetLength(0); i++)
                {
                    double additionalParameters = global.Conductivity * detJacob[i];
                    for (int j = 0; j < tempArray.GetLength(1); j++) 
                    {
                        tempArray[i, j, 0] = (integrationX[i, j, 0] + integrationY[i, j, 0]) * additionalParameters;
                        tempArray[i, j, 1] = (integrationX[i, j, 1] + integrationY[i, j, 1]) * additionalParameters;
                        tempArray[i, j, 2] = (integrationX[i, j, 2] + integrationY[i, j, 2]) * additionalParameters;
                        tempArray[i, j, 3] = (integrationX[i, j, 3] + integrationY[i, j, 3]) * additionalParameters;
                    }
                }

                double[,] HArray;
                switch (interpolation_Points)
                {
                    case 4:
                        HArray = new double[4, 4];
                        for (int i = 0; i < HArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < HArray.GetLength(1); j++)
                            {
                                HArray[i, j] = tempArray[0, i, j] + tempArray[1, i, j] + tempArray[2, i, j] + tempArray[3, i, j];
                            }
                        }
                        return HArray;

                    case 9:
                        HArray = new double[9, 4];
                        double[] _3interpolationRanges = MES_Math_Constants._3interpolationRanges;
                        for (int i = 0; i < HArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < HArray.GetLength(1); j++)
                            {
                                HArray[i, j] = _3interpolationRanges[0] * _3interpolationRanges[0] * tempArray[0, i, j] + _3interpolationRanges[1] * _3interpolationRanges[0] * tempArray[1, i, j] + _3interpolationRanges[2] * _3interpolationRanges[0] * tempArray[2, i, j] 
                                    + _3interpolationRanges[0] * _3interpolationRanges[1] * tempArray[3, i, j] + _3interpolationRanges[1] * _3interpolationRanges[1] * tempArray[4, i, j] + _3interpolationRanges[2] * _3interpolationRanges[1] * tempArray[5, i, j] 
                                    + _3interpolationRanges[0] * _3interpolationRanges[2] * tempArray[6, i, j] + _3interpolationRanges[1] * _3interpolationRanges[2] * tempArray[7, i, j] + _3interpolationRanges[2] * _3interpolationRanges[2] * tempArray[8, i, j];
                            }
                        }
                        return HArray;

                    case 16:
                        HArray = new double[16, 4];
                        double[] _4interpolationRanges = MES_Math_Constants._4interpolationRanges;
                        for (int i = 0; i < HArray.GetLength(0); i++)
                        {
                            for (int j = 0; j < HArray.GetLength(1); j++)
                            {
                                HArray[i, j] = _4interpolationRanges[0] * _4interpolationRanges[0] * tempArray[0, i, j] + _4interpolationRanges[1] * _4interpolationRanges[0] * tempArray[1, i, j] + _4interpolationRanges[2] * _4interpolationRanges[0] * tempArray[2, i, j]
                                   + _4interpolationRanges[3] * _4interpolationRanges[0] * tempArray[3, i, j] + _4interpolationRanges[0] * _4interpolationRanges[1] * tempArray[4, i, j] + _4interpolationRanges[1] * _4interpolationRanges[1] * tempArray[5, i, j]
                                   + _4interpolationRanges[2] * _4interpolationRanges[1] * tempArray[6, i, j] + _4interpolationRanges[3] * _4interpolationRanges[1] * tempArray[7, i, j] + _4interpolationRanges[0] * _4interpolationRanges[2] * tempArray[8, i, j]
                                   + _4interpolationRanges[1] * _4interpolationRanges[2] * tempArray[9, i, j] + _4interpolationRanges[2] * _4interpolationRanges[2] * tempArray[10, i, j] + _4interpolationRanges[3] * _4interpolationRanges[2] * tempArray[11, i, j]
                                   + _4interpolationRanges[0] * _4interpolationRanges[3] * tempArray[12, i, j] + _4interpolationRanges[1] * _4interpolationRanges[3] * tempArray[13, i, j] + _4interpolationRanges[2] * _4interpolationRanges[3] * tempArray[14, i, j] 
                                   + _4interpolationRanges[3] * _4interpolationRanges[3] * tempArray[15, i, j];
                            }
                        }
                        return HArray;

                    default:
                        return null;
                }
            }


            // funkcja tworząca macierz Hbc dla każdego elementu //
            private double[,,] createHBCMatrix(MES_Universal_Element.UniElement_Sides element_Sides, Node[] nodes, MES_Main_Structures.GlobalData global, int interpolation_Points)
            {
                int numberOfNodesInElement = nodes.Length;
                //testNodes(grid.getElement(0));
                double[,,] HbcMatrix = new double[4, 4, 4];

                double[] jacobDet = new double[4];

                calculateDetJacobian(jacobDet, nodes);

                calculateHbc(HbcMatrix, element_Sides, jacobDet, global, interpolation_Points);

                return HbcMatrix;
            }

            // wyznacznik jakobianu dla każdgo boku elementu : długość boku / 2 //
            private void calculateDetJacobian(double[] detJacob, Node[] nodes)
            {
                for (int i = 0; i < detJacob.Length; i++)
                {
                    if (i != detJacob.Length - 1)
                    {
                        detJacob[i] = Math.Sqrt(Math.Pow(nodes[i + 1].X - nodes[i].X, 2) + Math.Pow(nodes[i + 1].Y - nodes[i].Y, 2)) / 2;
                    }
                    else
                    {
                        detJacob[i] = Math.Sqrt(Math.Pow(nodes[i].X - nodes[0].X, 2) + Math.Pow(nodes[i].Y - nodes[0].Y, 2)) / 2;
                    }
                }
            }

            // wyznaczanie macierzy Hbc dla pojedynczego elementu - wyznaczenie lokalnych macierzy Hbc dla każdej ściany elementu //
            private void calculateHbc(double[,,] HbcMatrix, MES_Universal_Element.UniElement_Sides element_Sides, double[] jacobDet, MES_Main_Structures.GlobalData global, int interpolation_Points)
            {
                double[,,] shapeFunctionValues = element_Sides.get_Shape_Function_Values();

                double[,,] Hbc_pc1 = new double[4, 4, 4];
                double[,,] Hbc_pc2 = new double[4, 4, 4];
                double[,,] Hbc_pc3 = new double[4, 4, 4];
                double[,,] Hbc_pc4 = new double[4, 4, 4];

                switch (interpolation_Points)
                {
                    case 4:
                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                Hbc_pc1[i, j, 0] = MES_Math_Constants._2interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 0];
                                Hbc_pc1[i, j, 1] = MES_Math_Constants._2interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 1];
                                Hbc_pc1[i, j, 2] = MES_Math_Constants._2interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 2];
                                Hbc_pc1[i, j, 3] = MES_Math_Constants._2interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 3];

                                Hbc_pc2[i, j, 0] = MES_Math_Constants._2interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 0];
                                Hbc_pc2[i, j, 1] = MES_Math_Constants._2interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 1];
                                Hbc_pc2[i, j, 2] = MES_Math_Constants._2interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 2];
                                Hbc_pc2[i, j, 3] = MES_Math_Constants._2interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 3];
                            }
                        }

                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                HbcMatrix[i, j, 0] = (Hbc_pc1[i, j, 0] + Hbc_pc2[i, j, 0]) * jacobDet[i];
                                HbcMatrix[i, j, 1] = (Hbc_pc1[i, j, 1] + Hbc_pc2[i, j, 1]) * jacobDet[i];
                                HbcMatrix[i, j, 2] = (Hbc_pc1[i, j, 2] + Hbc_pc2[i, j, 2]) * jacobDet[i];
                                HbcMatrix[i, j, 3] = (Hbc_pc1[i, j, 3] + Hbc_pc2[i, j, 3]) * jacobDet[i];
                            }
                        }
                        break;

                    case 9:
                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                Hbc_pc1[i, j, 0] = MES_Math_Constants._3interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 0];
                                Hbc_pc1[i, j, 1] = MES_Math_Constants._3interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 1];
                                Hbc_pc1[i, j, 2] = MES_Math_Constants._3interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 2];
                                Hbc_pc1[i, j, 3] = MES_Math_Constants._3interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 3];
                                                                       
                                Hbc_pc2[i, j, 0] = MES_Math_Constants._3interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 0];
                                Hbc_pc2[i, j, 1] = MES_Math_Constants._3interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 1];
                                Hbc_pc2[i, j, 2] = MES_Math_Constants._3interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 2];
                                Hbc_pc2[i, j, 3] = MES_Math_Constants._3interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 3];
                                                                       
                                Hbc_pc3[i, j, 0] = MES_Math_Constants._3interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 0];
                                Hbc_pc3[i, j, 1] = MES_Math_Constants._3interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 1];
                                Hbc_pc3[i, j, 2] = MES_Math_Constants._3interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 2];
                                Hbc_pc3[i, j, 3] = MES_Math_Constants._3interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 3];
                            }
                        }

                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                HbcMatrix[i, j, 0] = (Hbc_pc1[i, j, 0] + Hbc_pc2[i, j, 0] + Hbc_pc3[i, j, 0]) * jacobDet[i];
                                HbcMatrix[i, j, 1] = (Hbc_pc1[i, j, 1] + Hbc_pc2[i, j, 1] + Hbc_pc3[i, j, 1]) * jacobDet[i];
                                HbcMatrix[i, j, 2] = (Hbc_pc1[i, j, 2] + Hbc_pc2[i, j, 2] + Hbc_pc3[i, j, 2]) * jacobDet[i];
                                HbcMatrix[i, j, 3] = (Hbc_pc1[i, j, 3] + Hbc_pc2[i, j, 3] + Hbc_pc3[i, j, 3]) * jacobDet[i];
                            }
                        }
                        break;

                    case 16:
                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                Hbc_pc1[i, j, 0] = MES_Math_Constants._4interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 0];
                                Hbc_pc1[i, j, 1] = MES_Math_Constants._4interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 1];
                                Hbc_pc1[i, j, 2] = MES_Math_Constants._4interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 2];
                                Hbc_pc1[i, j, 3] = MES_Math_Constants._4interpolationRanges[0] * global.Alfa * shapeFunctionValues[i, 0, j] * shapeFunctionValues[i, 0, 3];
                                                                       
                                Hbc_pc2[i, j, 0] = MES_Math_Constants._4interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 0];
                                Hbc_pc2[i, j, 1] = MES_Math_Constants._4interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 1];
                                Hbc_pc2[i, j, 2] = MES_Math_Constants._4interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 2];
                                Hbc_pc2[i, j, 3] = MES_Math_Constants._4interpolationRanges[1] * global.Alfa * shapeFunctionValues[i, 1, j] * shapeFunctionValues[i, 1, 3];
                                                                       
                                Hbc_pc3[i, j, 0] = MES_Math_Constants._4interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 0];
                                Hbc_pc3[i, j, 1] = MES_Math_Constants._4interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 1];
                                Hbc_pc3[i, j, 2] = MES_Math_Constants._4interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 2];
                                Hbc_pc3[i, j, 3] = MES_Math_Constants._4interpolationRanges[2] * global.Alfa * shapeFunctionValues[i, 2, j] * shapeFunctionValues[i, 2, 3];
                                                                       
                                Hbc_pc4[i, j, 0] = MES_Math_Constants._4interpolationRanges[3] * global.Alfa * shapeFunctionValues[i, 3, j] * shapeFunctionValues[i, 3, 0];
                                Hbc_pc4[i, j, 1] = MES_Math_Constants._4interpolationRanges[3] * global.Alfa * shapeFunctionValues[i, 3, j] * shapeFunctionValues[i, 3, 1];
                                Hbc_pc4[i, j, 2] = MES_Math_Constants._4interpolationRanges[3] * global.Alfa * shapeFunctionValues[i, 3, j] * shapeFunctionValues[i, 3, 2];
                                Hbc_pc4[i, j, 3] = MES_Math_Constants._4interpolationRanges[3] * global.Alfa * shapeFunctionValues[i, 3, j] * shapeFunctionValues[i, 3, 3];
                            }
                        }
                        
                        for (int i = 0; i < Hbc_pc1.GetLength(0); i++)
                        {
                            for (int j = 0; j < Hbc_pc1.GetLength(1); j++)
                            {
                                HbcMatrix[i, j, 0] = (Hbc_pc1[i, j, 0] + Hbc_pc2[i, j, 0] + Hbc_pc3[i, j, 0] + Hbc_pc4[i, j, 0]) * jacobDet[i];
                                HbcMatrix[i, j, 1] = (Hbc_pc1[i, j, 1] + Hbc_pc2[i, j, 1] + Hbc_pc3[i, j, 1] + Hbc_pc4[i, j, 1]) * jacobDet[i];
                                HbcMatrix[i, j, 2] = (Hbc_pc1[i, j, 2] + Hbc_pc2[i, j, 2] + Hbc_pc3[i, j, 2] + Hbc_pc4[i, j, 2]) * jacobDet[i];
                                HbcMatrix[i, j, 3] = (Hbc_pc1[i, j, 3] + Hbc_pc2[i, j, 3] + Hbc_pc3[i, j, 3] + Hbc_pc4[i, j, 3]) * jacobDet[i];
                            }
                        }
                        break;

                    default:
                        break;
                }
            }

            // funkcja łącząca wartości z macierzy H oraz macierzy Hbc - ze sprawdzeniem warunku brzegowego //
            private void combine_H_Hbc_Arrays(double[,] H_Local, double[,,] Hbc_Local, Node[] nodes)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (i == nodes.Length - 1)
                    {
                        if (nodes[i].BC == 1 && nodes[0].BC == 1)
                        {
                            add_Hbc_to_H_Array(H_Local, Hbc_Local, i);
                        }
                    }
                    else
                    {
                        if (nodes[i].BC == 1 && nodes[i + 1].BC == 1)
                        {
                            add_Hbc_to_H_Array(H_Local, Hbc_Local, i);
                        }
                    }
                }
            }

            // funkcja dodająca odpowiednie wartości z macierzy Hbc do macierzy H - dla boków gdzie jest spełniony warunek brzegowy //
            private void add_Hbc_to_H_Array(double[,] H_Local, double[,,] Hbc_Local, int index1)
            {
                for (int i = 0; i < Hbc_Local.GetLength(0); i++)
                {
                    for (int j = 0; j < Hbc_Local.GetLength(1); j++)
                    {
                        H_Local[i, j] = H_Local[i, j] + Hbc_Local[index1, i, j];
                    }
                }
            }

            // funkcja dodająca wyznaczoną macierz H + Hbc dla danego elementu macierzy H + Hbc globalnej 
            private void appendTo_Global_H_Hbc_Array(double[,,] globalArray, double[,] localArray, int index)
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