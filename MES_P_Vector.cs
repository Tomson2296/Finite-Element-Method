using System;
using static ConsoleApp1.MES_Main_Structures;

namespace ConsoleApp1
{
    internal class MES_P_Vector
    {
        public class P_Vector
        {
            public P_Vector(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, MES_Universal_Element.UniElement_Sides element_Sides, int interpolation_Points)
            {
                P_VectorValues = calculate_P_Vector(globalData, grid, element, element_Sides, interpolation_Points);
            }
            public double[] P_VectorValues { get; set; }

            private double[] calculate_P_Vector(MES_Main_Structures.GlobalData globalData, MES_Main_Structures.Grid grid, MES_Universal_Element.UniElement element, MES_Universal_Element.UniElement_Sides element_Sides, int interpolation_Points)
            {
                int numberOfNodes = grid.numberOfNodes;
                double[] result = new double[numberOfNodes];
                
                int numberOfGridElements = grid.numberOfElements;
                for (int i = 0; i < numberOfGridElements; i++)
                {
                    double[] detJabob = new double[4];

                    calculateDetJacobian(detJabob, grid.getElement(i));

                    Vector_P_Local_Calculation(globalData, element_Sides, detJabob, grid.getElement(i), result, interpolation_Points);
                }
                return result;
            }

            // funkcja zwracająca globalny wektor P //
            public double[] get_P_Vector()
            {
                return P_VectorValues;
            }

            // funkcja wypisująca globalny wektor P // 
            public void show_P_GlobalVector()
            {
                Console.WriteLine("Wektor P: ");
                for (int i = 0; i < P_VectorValues.GetLength(0); i++)
                {
                    Console.WriteLine(Math.Round(P_VectorValues[i], 3, MidpointRounding.AwayFromZero));   
                }
                Console.WriteLine();
            }

            // funkcja obliczająca wyznacznik macierzy jakobianu //
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

            // funkcja sprawdzająca warunek brzegowy dla każdego wężła zawartego w elemencie -> jesli znaleziono -> obliczenie wartości wektora P -> agregacja do wektora P globalnego //
            private void Vector_P_Local_Calculation(MES_Main_Structures.GlobalData global, MES_Universal_Element.UniElement_Sides element_Sides, double[] jacobDet, Node[] nodes, double[] result, int interpolation_Points)
            {
                double[] P_LocalVector = new double[4];
                int[] nodesID = new int[nodes.Length];
                nodesID = deductIDForNodes(nodes);

                for (int i = 0; i < nodes.Length; i++)
                {
                    if (i == nodes.Length - 1)
                    {
                        if (nodes[i].BC == 1 && nodes[0].BC == 1)
                        {
                            // kalkulacja wektora P_Local w przypadku zainstnienia warunku brzegowego na danym boku elementu z równoczesną agregacją tego wektora do P_Global
                            P_LocalVector = calculate_P_Local_For_QSide(global, element_Sides, jacobDet, i, interpolation_Points);
                            aggregateTo_P_GlobalVector(P_LocalVector, result, nodesID);
                        }
                    }
                    else
                    {
                        if (nodes[i].BC == 1 && nodes[i + 1].BC == 1)
                        {
                            P_LocalVector = calculate_P_Local_For_QSide(global, element_Sides, jacobDet, i, interpolation_Points);
                            aggregateTo_P_GlobalVector(P_LocalVector, result, nodesID);
                        }
                    }
                }
            }

            // funkcja dedukująca ID węzłów danego elementu -> wykorzystywana później przy agregacji wektora obciążeń P //
            private int[] deductIDForNodes(Node[] nodes)
            {
                int[] nodesID = new int[nodes.Length];
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodesID[i] = nodes[i].ID;
                }
                return nodesID;
            }

            // funkcja odpowiadająca za liczenie wektora P dla danej ściany elementu, na którym stwierdzono występowanie warunku brzegowego //
            private double[] calculate_P_Local_For_QSide(MES_Main_Structures.GlobalData global, MES_Universal_Element.UniElement_Sides element_Sides, double[] jacobDet, int index1, int interpolation_Points)
            {
                double[] P_LocalVector = new double[4];
                double[,,] shapeFunctionValues = element_Sides.get_Shape_Function_Values();

                switch (interpolation_Points)
                {
                    case 4:
                        P_LocalVector[0] = global.Alfa * (MES_Math_Constants._2interpolationRanges[0] * shapeFunctionValues[index1, 0, 0] * global.Tot +
                                           MES_Math_Constants._2interpolationRanges[1] * shapeFunctionValues[index1, 1, 0] * global.Tot) * jacobDet[index1];

                        P_LocalVector[1] = global.Alfa * (MES_Math_Constants._2interpolationRanges[0] * shapeFunctionValues[index1, 0, 1] * global.Tot +
                                           MES_Math_Constants._2interpolationRanges[1] * shapeFunctionValues[index1, 1, 1] * global.Tot) * jacobDet[index1];

                        P_LocalVector[2] = global.Alfa * (MES_Math_Constants._2interpolationRanges[0] * shapeFunctionValues[index1, 0, 2] * global.Tot + 
                                           MES_Math_Constants._2interpolationRanges[1] * shapeFunctionValues[index1, 1, 2] * global.Tot) * jacobDet[index1];

                        P_LocalVector[3] = global.Alfa * (MES_Math_Constants._2interpolationRanges[0] * shapeFunctionValues[index1, 0, 3] * global.Tot +
                                           MES_Math_Constants._2interpolationRanges[1] * shapeFunctionValues[index1, 1, 3] * global.Tot) * jacobDet[index1];
                        break;

                    case 9:
                        P_LocalVector[0] = global.Alfa * (MES_Math_Constants._3interpolationRanges[0] * shapeFunctionValues[index1, 0, 0] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[1] * shapeFunctionValues[index1, 1, 0] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[2] * shapeFunctionValues[index1, 2, 0] * global.Tot) * jacobDet[index1];

                        P_LocalVector[1] = global.Alfa * (MES_Math_Constants._3interpolationRanges[0] * shapeFunctionValues[index1, 0, 1] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[1] * shapeFunctionValues[index1, 1, 1] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[2] * shapeFunctionValues[index1, 2, 1] * global.Tot) * jacobDet[index1];

                        P_LocalVector[2] = global.Alfa * (MES_Math_Constants._3interpolationRanges[0] * shapeFunctionValues[index1, 0, 2] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[1] * shapeFunctionValues[index1, 1, 2] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[2] * shapeFunctionValues[index1, 2, 2] * global.Tot) * jacobDet[index1];

                        P_LocalVector[3] = global.Alfa * (MES_Math_Constants._3interpolationRanges[0] * shapeFunctionValues[index1, 0, 3] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[1] * shapeFunctionValues[index1, 1, 3] * global.Tot + 
                                           MES_Math_Constants._3interpolationRanges[2] * shapeFunctionValues[index1, 2, 3] * global.Tot) * jacobDet[index1];
                        break;

                    case 16:
                        P_LocalVector[0] = global.Alfa * (MES_Math_Constants._4interpolationRanges[0] * shapeFunctionValues[index1, 0, 0] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[1] * shapeFunctionValues[index1, 1, 0] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[2] * shapeFunctionValues[index1, 2, 0] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[3] * shapeFunctionValues[index1, 3, 0] * global.Tot) * jacobDet[index1];
                        P_LocalVector[1] = global.Alfa * (MES_Math_Constants._4interpolationRanges[0] * shapeFunctionValues[index1, 0, 1] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[1] * shapeFunctionValues[index1, 1, 1] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[2] * shapeFunctionValues[index1, 2, 1] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[3] * shapeFunctionValues[index1, 3, 1] * global.Tot) * jacobDet[index1];
                        P_LocalVector[2] = global.Alfa * (MES_Math_Constants._4interpolationRanges[0] * shapeFunctionValues[index1, 0, 2] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[1] * shapeFunctionValues[index1, 1, 2] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[2] * shapeFunctionValues[index1, 2, 2] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[3] * shapeFunctionValues[index1, 3, 2] * global.Tot) * jacobDet[index1];
                        P_LocalVector[3] = global.Alfa * (MES_Math_Constants._4interpolationRanges[0] * shapeFunctionValues[index1, 0, 3] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[1] * shapeFunctionValues[index1, 1, 3] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[2] * shapeFunctionValues[index1, 2, 3] * global.Tot +
                                           MES_Math_Constants._4interpolationRanges[3] * shapeFunctionValues[index1, 3, 3] * global.Tot) * jacobDet[index1];
                        break;

                    default:
                        break;
                }
                return P_LocalVector;
            }

            // funkcja aggregująca wyznaczony wektor P dla danej ściany do wektora globalnego P_Global //
            private void aggregateTo_P_GlobalVector(double[] localVector, double[] globalVector, int[] nodesID)
            {
                for (int i = 0; i < localVector.Length; i++)
                {
                    if (localVector[i] != 0.0)
                    {
                        globalVector[nodesID[i] - 1] += Math.Round(localVector[i],3);
                    }
                }
            }
        }
    }
}
