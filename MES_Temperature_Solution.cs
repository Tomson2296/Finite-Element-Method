using System;
using MESMain = ConsoleApp1.MES_Main_Structures;
using MESAggrH = ConsoleApp1.MES_Aggregation_Matrix;
using MESP = ConsoleApp1.MES_P_Vector;

namespace ConsoleApp1
{
    internal class MES_Temperature_Solution
    {
        public class MES_Temp_Solution
        {
            public MES_Temp_Solution(MESMain.GlobalData globalData, MESAggrH.H_Aggregation_Matrix h_Aggregation_Matrix,
                MESAggrH.C_Aggregation_Matrix c_Aggregation_Matrix, MESP.P_Vector p_Vector)
            {
                Temp_Values = calculate_temp_for_Nodes(globalData, h_Aggregation_Matrix, c_Aggregation_Matrix, p_Vector);
            }
            public double[,] Temp_Values { get; set; }

            // wyświetlenie macierzy temperatur dla każdej iteracji //
            public void show_Temp_Values(MESMain.GlobalData globalData)
            {
                for (int i = 0; i < Temp_Values.GetLength(0); i++)
                {
                    Console.WriteLine("Wartości temperatury dla czasu: " + (i * globalData.SimulationStepTime) + "s");
                    for (int j = 0; j < Temp_Values.GetLength(1); j++)
                    {
                        Console.WriteLine("Node nr: " + (j + 1) + "  -  "  + Math.Round(Temp_Values[i, j], 3, MidpointRounding.AwayFromZero));
                    }
                }
                Console.WriteLine();
            }

            // wyliczanie wartości temperatury dla każdego z węzłów siatki korzystając z algorytmu eliminacji Gaussa - otrzymanie rozwiązania problemu nieustalonego przepływu ciepła //
            private double[,] calculate_temp_for_Nodes(MESMain.GlobalData globalData, MESAggrH.H_Aggregation_Matrix h2_AggregationMatrix, MESAggrH.C_Aggregation_Matrix c2_AggregationMatrix, MESP.P_Vector p_Vector)
            {
                int numberOfIterations = globalData.SimulationTime / globalData.SimulationStepTime;
                // result temperature matrix //
                double[,] temp_results = new double[numberOfIterations + 1, globalData.Nodes_number];

                double[,] aggregated_h_global = h2_AggregationMatrix.getAggregation_H_Matrix();
                double[,] aggregated_c_global = c2_AggregationMatrix.getAggregation_C_Matrix();
                double[] aggregated_p_vector = p_Vector.get_P_Vector();

                double[] T0 = new double[globalData.Nodes_number];
                double[] T1 = new double[globalData.Nodes_number];
                set_initial_temp_Values(T0, globalData);
                add_To_Temp_Results(temp_results, T0, 0);

                double[,] h_modified = new double[globalData.Nodes_number, globalData.Nodes_number];
                modify_H_Matrix(h_modified, aggregated_h_global, aggregated_c_global, globalData);

                double[] p_modified = new double[globalData.Nodes_number];

                for (int i = 1; i <= numberOfIterations; i++)
                {
                    modify_P_Vector(globalData, aggregated_c_global, aggregated_p_vector, p_modified, T0);

                    gaussian_elimination_method(h_modified, p_modified, T1, globalData);

                    //show_Temp_Values(T1);

                    show_min_max_temp(i, T1, globalData);

                    update_Temp_Values(T0, T1);

                    add_To_Temp_Results(temp_results, T0, i);
                }
                return temp_results;
            }

            private void add_To_Temp_Results(double[,] Temp_result, double[] Temp, int iteration)
            {
                for (int i = 0; i < Temp.Length; i++)
                {
                    Temp_result[iteration, i] = Temp[i];
                }
            }

            // aktualizacja wektora temperatur - podstawienie nnowych temperatur dla następnej iteracji // 
            private void update_Temp_Values(double[] T0, double[] T1)
            {
                for (int i = 0; i < T0.Length; i++)
                {
                    T0[i] = T1[i];
                }
            }

            // ustawienie początkowej temperatury dla każdego z węzłów na wartość początkową zczytaną z globalData: initalTemp //
            private void set_initial_temp_Values(double[] temp_Vector, MESMain.GlobalData globalData)
            {
                for (int i = 0; i < temp_Vector.Length; i++)
                {
                    temp_Vector[i] = globalData.InitialTemp;
                }
            }

            // funkcja modyfikująca macierz H na potrzeby wyznaczania rozwiązania //
            private void modify_H_Matrix(double[,] h_modified, double[,] aggregated_h_global, double[,] aggregated_c_global, MESMain.GlobalData globalData)
            {
                for (int i = 0; i < aggregated_h_global.GetLength(0); ++i)
                {
                    for (int j = 0; j < aggregated_h_global.GetLength(1); ++j)
                    {
                        h_modified[i, j] = aggregated_h_global[i, j] + aggregated_c_global[i, j] / globalData.SimulationStepTime;
                    }
                }
            }

            // funkcja modyfująca co każdą iteracje wektor P uzupełniając go nowymi wartościami // 
            private void modify_P_Vector(MESMain.GlobalData globalData, double[,] aggregated_c_global, double[] aggregated_p_vector, double[] p_modified, double[] temp0)
            {
                for (int i = 0; i < aggregated_c_global.GetLength(0); i++)
                {
                    p_modified[i] = aggregated_p_vector[i];
                    for (int j = 0; j < aggregated_c_global.GetLength(1); j++)
                    {
                        p_modified[i] += aggregated_c_global[i, j] / globalData.SimulationStepTime * temp0[j];
                    }
                }
            }

            // metoda eliminacji gaussa do wyznaczanie niewiadomych równania nieustalonego przepływu ciepła //
            private void gaussian_elimination_method(double[,] h_modified, double[] p_modified, double[] temp1, MESMain.GlobalData globalData)
            {
                double multiplicator, calculated_Value;
                double[,] h_combined = new double[globalData.Nodes_number, globalData.Nodes_number + 1];

                for (int i = 0; i < globalData.Nodes_number; i++)
                {
                    temp1[i] = 0.0;
                    for (int j = 0; j < (globalData.Nodes_number + 1); j++)
                    {
                        if (j == globalData.Nodes_number) h_combined[i, j] = p_modified[i];
                        else h_combined[i, j] = h_modified[i, j];
                    }
                }

                for (int i = 0; i < globalData.Nodes_number; i++)
                {
                    for (int j = i + 1; j < globalData.Nodes_number; j++)
                    {
                        if (Math.Abs(h_modified[i, i]) < Math.Abs(h_modified[j, i]))
                        {
                            for (int k = 0; k < globalData.Nodes_number + 1; k++)
                            {
                                /* swapping mat[i][k] and mat[j][k] */
                                h_combined[i, k] = h_combined[i, k] + h_combined[j, k];
                                h_combined[j, k] = h_combined[i, k] - h_combined[j, k];
                                h_combined[i, k] = h_combined[i, k] - h_combined[j, k];
                            }
                        }
                    }
                }



                // eliminacja współczynników //
                for (int i = 0; i < temp1.Length - 1; i++)
                {
                    for (int j = i + 1; j < temp1.Length; j++)
                    {
                        multiplicator = -h_combined[j, i] / h_combined[i, i];
                        for (int k = i + 1; k <= temp1.Length; k++)
                        {
                            h_combined[j, k] += multiplicator * h_combined[i, k];
                        }
                    }
                }

                // wyliczanie niewiadomych //
                for (int i = temp1.Length - 1; i >= 0; i--)
                {
                    calculated_Value = h_combined[i, temp1.Length];
                    for (int j = temp1.Length - 1; j >= i + 1; j--)
                    {
                        calculated_Value -= h_combined[i, j] * temp1[j];
                    }
                    temp1[i] = calculated_Value / h_combined[i, i];
                }
            }

            // wyświetlenie wartości temperatury najniższej oraz najwyższej dla każdej iteracji //
            private void show_min_max_temp(int iteration, double[] temp_Values, MESMain.GlobalData globalData)
            {
                double minTemp = double.MaxValue;
                double maxTemp = double.MinValue;

                Console.Write("Time:" + iteration * globalData.SimulationStepTime + "s");
                for (int i = 0; i < temp_Values.Length; i++)
                {
                    if (temp_Values[i] < minTemp)
                    {
                        minTemp = Math.Round(temp_Values[i], 3, MidpointRounding.AwayFromZero);
                    }

                    if (temp_Values[i] > maxTemp)
                    {
                        maxTemp = Math.Round(temp_Values[i], 3, MidpointRounding.AwayFromZero);
                    }
                }
                Console.Write("\t" + minTemp + " " + maxTemp + "\n");
            }
        }
    }
}
