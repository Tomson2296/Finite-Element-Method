using System;

namespace ConsoleApp1
{
    internal class MES_Universal_Element
    {
        public abstract class UniElement
        {
            public abstract double[,] derivativeValues_N_Ksi { get; set; }
            public abstract double[,] derivativeValues_N_Eta { get; set; }
            public abstract double[,] shapeValues_N { get; set; }

            public abstract void showDerivativeValues_N_Ksi();
            public abstract void showDerivativeValues_N_Eta();
            public abstract void showShapeValues_N();
            public abstract double[,] getDerivativeValues_N_Ksi();
            public abstract double[,] getDerivativeValues_N_Eta();
            public abstract double[,] getShapeValues_N();

            protected abstract void deduct_Derivative_N_Ksi_Values(double[,] derivativeValues_N_Ksi);
            protected abstract void deduct_Derivative_N_Eta_Values(double[,] derivativeValues_N_Eta);
            protected abstract void deduct_Shape_Values(double[,] shapeValues_N);
        }

        public class UniElement2x2 : UniElement
        {
            public UniElement2x2()
            {
                derivativeValues_N_Ksi = new double[4, 4];
                derivativeValues_N_Eta = new double[4, 4];
                shapeValues_N = new double[4, 4];

                deduct_Derivative_N_Ksi_Values(derivativeValues_N_Ksi);
                deduct_Derivative_N_Eta_Values(derivativeValues_N_Eta);
                deduct_Shape_Values(shapeValues_N);
            }
            public override double[,] derivativeValues_N_Ksi { get; set; }
            public override double[,] derivativeValues_N_Eta { get; set; }
            public override double[,] shapeValues_N { get; set; }

            // funkcja wyświetlająca macierz z wartościami dN / dKsi
            public override void showDerivativeValues_N_Ksi()
            {
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Ksi.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Ksi.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz z wartościami dN / dEta
            public override void showDerivativeValues_N_Eta()
            {
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Eta.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Eta.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz funkcji kształu dla elementu uniwersalnego
            public override void showShapeValues_N()
            {
                for (int i = 0; i < shapeValues_N.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeValues_N.GetLength(1); j++)
                    {
                        if (j == shapeValues_N.GetLength(1) - 1)
                        {
                            Console.Write(shapeValues_N[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(shapeValues_N[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja pobierająca macierz z wartościami dN / dKsi
            public override double[,] getDerivativeValues_N_Ksi()
            {
                return derivativeValues_N_Ksi;
            }

            // funkcja pobierająca macierz z wartościami dN / dEta
            public override double[,] getDerivativeValues_N_Eta()
            {
                return derivativeValues_N_Eta;
            }

            // funkcja pibierająca macierz funkcji kształu dla elementu uniwersalnego
            public override double[,] getShapeValues_N()
            {
                return shapeValues_N;
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dKsi
            protected override void deduct_Derivative_N_Ksi_Values(double[,] derivativeValues_N_Ksi)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    if (j == 0 || j == 1)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._2interpolationPoints[0]);
                    }
                    else if (j == 2 || j == 3)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._2interpolationPoints[1]);
                    }
                    j++;
                }
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dEta
            protected override void deduct_Derivative_N_Eta_Values(double[,] derivativeValues_N_Eta)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    if (j == 0 || j == 2)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._2interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._2interpolationPoints[0]);
                    }
                    else if (j == 1 || j == 3)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._2interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._2interpolationPoints[1]);
                    }
                    j++;
                }
            }

            // prywanta funkcja uzupełniająca macierz funkcji ksztattu dla elementu uniwersalnego
            protected override void deduct_Shape_Values(double[,] shapeValues_N)
            {
                shapeValues_N[0, 0] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[0]) * (1.0 - MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[0, 1] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[0]) * (1.0 - MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[0, 2] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[0]) * (1.0 + MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[0, 3] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[0]) * (1.0 + MES_Math_Constants._2interpolationPoints[0]);

                shapeValues_N[1, 0] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[1]) * (1.0 - MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[1, 1] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[1]) * (1.0 - MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[1, 2] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[1]) * (1.0 + MES_Math_Constants._2interpolationPoints[0]);
                shapeValues_N[1, 3] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[1]) * (1.0 + MES_Math_Constants._2interpolationPoints[0]);

                shapeValues_N[2, 0] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[0]) * (1.0 - MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[2, 1] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[0]) * (1.0 - MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[2, 2] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[0]) * (1.0 + MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[2, 3] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[0]) * (1.0 + MES_Math_Constants._2interpolationPoints[1]);

                shapeValues_N[3, 0] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[1]) * (1.0 - MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[3, 1] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[1]) * (1.0 - MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[3, 2] = 0.25 * (1.0 + MES_Math_Constants._2interpolationPoints[1]) * (1.0 + MES_Math_Constants._2interpolationPoints[1]);
                shapeValues_N[3, 3] = 0.25 * (1.0 - MES_Math_Constants._2interpolationPoints[1]) * (1.0 + MES_Math_Constants._2interpolationPoints[1]);
            }
        }

        public class UniElement3x3 : UniElement
        {
            public UniElement3x3()
            {
                derivativeValues_N_Ksi = new double[9, 4];
                derivativeValues_N_Eta = new double[9, 4];
                shapeValues_N = new double[9, 4];

                deduct_Derivative_N_Ksi_Values(derivativeValues_N_Ksi);
                deduct_Derivative_N_Eta_Values(derivativeValues_N_Eta);
                deduct_Shape_Values(shapeValues_N);
            }
            public override double[,] derivativeValues_N_Ksi { get; set; }

            public override double[,] derivativeValues_N_Eta { get; set; }

            public override double[,] shapeValues_N { get; set; }

            // funkcja wyświetlająca macierz z wartościami dN / dKsi
            public override void showDerivativeValues_N_Ksi()
            {
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Ksi.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Ksi.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz z wartościami dN / dEta
            public override void showDerivativeValues_N_Eta()
            {
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Eta.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Eta.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz funkcji kształu dla elementu uniwersalnego
            public override void showShapeValues_N()
            {
                for (int i = 0; i < shapeValues_N.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeValues_N.GetLength(1); j++)
                    {
                        if (j == shapeValues_N.GetLength(1) - 1)
                        {
                            Console.Write(shapeValues_N[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(shapeValues_N[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja pobierająca macierz z wartościami dN / dKsi
            public override double[,] getDerivativeValues_N_Ksi()
            {
                return derivativeValues_N_Ksi;
            }

            // funkcja pobierająca macierz z wartościami dN / dEta
            public override double[,] getDerivativeValues_N_Eta()
            {
                return derivativeValues_N_Eta;
            }

            // funkcja pibierająca macierz funkcji kształu dla elementu uniwersalnego 
            public override double[,] getShapeValues_N()
            {
                return shapeValues_N;
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dKsi
            protected override void deduct_Derivative_N_Ksi_Values(double[,] derivativeValues_N_Ksi)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    if (j < 3)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[0]);
                    }
                    else if (j >= 3 && j < 6)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[1]);
                    }
                    else
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[2]);
                    }
                    j++;
                }
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dEta
            protected override void deduct_Derivative_N_Eta_Values(double[,] derivativeValues_N_Eta)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    if (j % 3 == 0)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[0]);
                    }
                    else if (j % 3 == 1)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[1]);
                    }
                    else
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._3interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._3interpolationPoints[2]);
                    }
                    j++;
                }
            }

            // prywanta funkcja uzupełniająca macierz funkcji kształtu dla elementu uniwersalnego
            protected override void deduct_Shape_Values(double[,] shapeValues_N)
            {
                shapeValues_N[0, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[0, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[0, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[0, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);

                shapeValues_N[1, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[1, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[1, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[1, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);

                shapeValues_N[2, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[2, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[2, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);
                shapeValues_N[2, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[0]);

                shapeValues_N[3, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[3, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[3, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[3, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);

                shapeValues_N[4, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[4, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[4, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[4, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);

                shapeValues_N[5, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[5, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[5, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);
                shapeValues_N[5, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[1]);

                shapeValues_N[6, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[6, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[6, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[6, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[0]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);

                shapeValues_N[7, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[7, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[7, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[7, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[1]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);

                shapeValues_N[8, 0] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[8, 1] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 - MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[8, 2] = 0.25 * (1.0 + MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);
                shapeValues_N[8, 3] = 0.25 * (1.0 - MES_Math_Constants._3interpolationPoints[2]) * (1.0 + MES_Math_Constants._3interpolationPoints[2]);
            }
        };

        public class UniElement4x4 : UniElement
        {
            public UniElement4x4()
            {
                derivativeValues_N_Ksi = new double[16, 4];
                derivativeValues_N_Eta = new double[16, 4];
                shapeValues_N = new double[16, 4];

                deduct_Derivative_N_Ksi_Values(derivativeValues_N_Ksi);
                deduct_Derivative_N_Eta_Values(derivativeValues_N_Eta);
                deduct_Shape_Values(shapeValues_N);
            }
            public override double[,] derivativeValues_N_Ksi { get; set; }

            public override double[,] derivativeValues_N_Eta { get; set; }

            public override double[,] shapeValues_N { get; set; }

            // funkcja wyświetlająca macierz z wartościami dN / dKsi
            public override void showDerivativeValues_N_Ksi()
            {
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Ksi.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Ksi.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Ksi[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz z wartościami dN / dEta
            public override void showDerivativeValues_N_Eta()
            {
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    for (int j = 0; j < derivativeValues_N_Eta.GetLength(1); j++)
                    {
                        if (j == derivativeValues_N_Eta.GetLength(1) - 1)
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(derivativeValues_N_Eta[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja wyświetlająca macierz funkcji kształu dla elementu uniwersalnego
            public override void showShapeValues_N()
            {
                for (int i = 0; i < shapeValues_N.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeValues_N.GetLength(1); j++)
                    {
                        if (j == shapeValues_N.GetLength(1) - 1)
                        {
                            Console.Write(shapeValues_N[i, j] + "\n");
                        }
                        else
                        {
                            Console.Write(shapeValues_N[i, j] + " ");
                        }
                    }
                }
            }

            // funkcja pobierająca macierz z wartościami dN / dKsi
            public override double[,] getDerivativeValues_N_Ksi()
            {
                return derivativeValues_N_Ksi;
            }

            // funkcja pobierająca macierz z wartościami dN / dEta
            public override double[,] getDerivativeValues_N_Eta()
            {
                return derivativeValues_N_Eta;
            }

            // funkcja pibierająca macierz funkcji kształu dla elementu uniwersalnego 
            public override double[,] getShapeValues_N()
            {
                return shapeValues_N;
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dKsi
            protected override void deduct_Derivative_N_Ksi_Values(double[,] derivativeValues_N_Ksi)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Ksi.GetLength(0); i++)
                {
                    if (j < 4)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[0]);
                    }
                    else if (j >= 4 && j < 8)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[1]);
                    }
                    else if (j >= 8 & j < 12)
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[2]);
                    }
                    else
                    {
                        derivativeValues_N_Ksi[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Ksi[i, 1] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Ksi[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Ksi[i, 3] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[3]);
                    }
                    j++;
                }
            }

            // prywatna funkcja uzupełniająca macierz derivatives_N_Eta => macierz dN / dEta
            protected override void deduct_Derivative_N_Eta_Values(double[,] derivativeValues_N_Eta)
            {
                int j = 0;
                for (int i = 0; i < derivativeValues_N_Eta.GetLength(0); i++)
                {
                    if (j % 4 == 0)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[0]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[0]);
                    }
                    else if (j % 4 == 1)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[1]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[1]);
                    }
                    else if (j % 4 == 2)
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[2]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[2]);
                    }
                    else
                    {
                        derivativeValues_N_Eta[i, 0] = -0.25 * (1 - MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Eta[i, 1] = -0.25 * (1 + MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Eta[i, 2] = 0.25 * (1 + MES_Math_Constants._4interpolationPoints[3]);
                        derivativeValues_N_Eta[i, 3] = 0.25 * (1 - MES_Math_Constants._4interpolationPoints[3]);
                    }
                    j++;
                }
            }

            // prywanta funkcja uzupełniająca macierz funkcji kształtu dla elementu uniwersalnego
            protected override void deduct_Shape_Values(double[,] shapeValues_N)
            {
                shapeValues_N[0, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[0, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[0, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[0, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);

                shapeValues_N[1, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[1, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[1, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[1, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);

                shapeValues_N[2, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[2, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[2, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[2, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);

                shapeValues_N[3, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[3, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[3, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);
                shapeValues_N[3, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[0]);

                shapeValues_N[4, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[4, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[4, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[4, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);

                shapeValues_N[5, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[5, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[5, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[5, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);

                shapeValues_N[6, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[6, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[6, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[6, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);

                shapeValues_N[7, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[7, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[7, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);
                shapeValues_N[7, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[1]);

                shapeValues_N[8, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[8, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[8, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[8, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);

                shapeValues_N[9, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[9, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[9, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[9, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);

                shapeValues_N[10, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[10, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[10, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[10, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);

                shapeValues_N[11, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[11, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[11, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);
                shapeValues_N[11, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[2]);

                shapeValues_N[12, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[12, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[12, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[12, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[0]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);

                shapeValues_N[13, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[13, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[13, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[13, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[1]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);

                shapeValues_N[14, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[14, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[14, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[14, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[2]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);

                shapeValues_N[15, 0] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[15, 1] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 - MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[15, 2] = 0.25 * (1.0 + MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);
                shapeValues_N[15, 3] = 0.25 * (1.0 - MES_Math_Constants._4interpolationPoints[3]) * (1.0 + MES_Math_Constants._4interpolationPoints[3]);
            }
        }



        public abstract class UniElement_Sides
        {
            public abstract double[,,] shapeFunctionValues { get; set; }
            public abstract double[,] ksiValues { get; set; }
            public abstract double[,] etaValues { get; set; }

            public abstract void show_Shape_Function_Values();

            protected abstract void initiate_Ksi_Values(double[,] ksiValues);

            protected abstract void initiate_Eta_Values(double[,] etaValues);

            protected abstract void deduct_Shape_Function_Values(double[,,] shapeFunctionValues, double[,] ksiValues, double[,] etaValues);

            public abstract double[,] get_ksi_Values();

            public abstract double[,] get_eta_Values();

            public abstract double[,,] get_Shape_Function_Values();
        }

        public class UniElement2x2_Sides : UniElement_Sides
        {
            public UniElement2x2_Sides()
            {
                shapeFunctionValues = new double[4, 2, 4];
                ksiValues = new double[4, 2];
                etaValues = new double[4, 2];

                initiate_Ksi_Values(ksiValues);
                initiate_Eta_Values(etaValues);
                deduct_Shape_Function_Values(shapeFunctionValues, ksiValues, etaValues);
            }
            public override double[,,] shapeFunctionValues { get; set; }
            public override double[,] ksiValues { get; set; }
            public override double[,] etaValues { get; set; }

            public override void show_Shape_Function_Values()
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        for (int k = 0; k < shapeFunctionValues.GetLength(2); k++)
                        {
                            if (k == shapeFunctionValues.GetLength(2) - 1)
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + "\n");

                            }
                            else
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + " ");
                            }
                        }
                    }
                }
            }

            protected override void initiate_Ksi_Values(double[,] ksiValues)
            {
                ksiValues[0, 0] = -1.0 / Math.Sqrt(3.0);
                ksiValues[0, 1] = 1.0 / Math.Sqrt(3.0);
                ksiValues[1, 0] = 1.0;
                ksiValues[1, 1] = 1.0;
                ksiValues[2, 0] = 1.0 / Math.Sqrt(3.0);
                ksiValues[2, 1] = -1.0 / Math.Sqrt(3.0);
                ksiValues[3, 0] = -1.0;
                ksiValues[3, 1] = -1.0;
            }

            protected override void initiate_Eta_Values(double[,] etaValues)
            {
                etaValues[0, 0] = -1.0;
                etaValues[0, 1] = -1.0;
                etaValues[1, 0] = -1.0 / Math.Sqrt(3.0);
                etaValues[1, 1] = 1.0 / Math.Sqrt(3.0);
                etaValues[2, 0] = 1.0;
                etaValues[2, 1] = 1.0;
                etaValues[3, 0] = 1.0 / Math.Sqrt(3.0);
                etaValues[3, 1] = -1.0 / Math.Sqrt(3.0);
            }

            protected override void deduct_Shape_Function_Values(double[,,] shapeFunctionValues, double[,] ksiValues, double[,] etaValues)
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        shapeFunctionValues[i, j, 0] = 0.25 * (1 - ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 1] = 0.25 * (1 + ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 2] = 0.25 * (1 + ksiValues[i, j]) * (1 + etaValues[i, j]);
                        shapeFunctionValues[i, j, 3] = 0.25 * (1 - ksiValues[i, j]) * (1 + etaValues[i, j]);
                    }
                }
            }

            public override double[,] get_ksi_Values()
            {
                return ksiValues;
            }

            public override double[,] get_eta_Values()
            {
                return etaValues;
            }

            public override double[,,] get_Shape_Function_Values()
            {
                return shapeFunctionValues;
            }
        }
        
        public class UniElement3x3_Sides : UniElement_Sides
        {
            public UniElement3x3_Sides()
            {
                shapeFunctionValues = new double[4, 3, 4];
                ksiValues = new double[4, 3];
                etaValues = new double[4, 3];

                initiate_Ksi_Values(ksiValues);
                initiate_Eta_Values(etaValues);
                deduct_Shape_Function_Values(shapeFunctionValues, ksiValues, etaValues);
            }
            public override double[,,] shapeFunctionValues { get; set; }
            public override double[,] ksiValues { get; set; }
            public override double[,] etaValues { get; set; }

            public override void show_Shape_Function_Values()
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        for (int k = 0; k < shapeFunctionValues.GetLength(2); k++)
                        {
                            if (k == shapeFunctionValues.GetLength(2) - 1)
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + "\n");

                            }
                            else
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + " ");
                            }
                        }
                    }
                }
            }

            protected override void initiate_Ksi_Values(double[,] ksiValues)
            {
                ksiValues[0, 0] = -Math.Sqrt(3.0 / 5.0);
                ksiValues[0, 1] = 0.0;
                ksiValues[0, 2] = Math.Sqrt(3.0 / 5.0);

                ksiValues[1, 0] = 1.0;
                ksiValues[1, 1] = 1.0;
                ksiValues[1, 2] = 1.0;

                ksiValues[2, 0] = Math.Sqrt(3.0 / 5.0);
                ksiValues[2, 1] = 0.0;
                ksiValues[2, 2] = -Math.Sqrt(3.0 / 5.0);

                ksiValues[3, 0] = -1.0;
                ksiValues[3, 1] = -1.0;
                ksiValues[3, 2] = -1.0;
            }

            protected override void initiate_Eta_Values(double[,] etaValues)
            {
                etaValues[0, 0] = -1.0;
                etaValues[0, 1] = -1.0;
                etaValues[0, 2] = -1.0;

                etaValues[1, 0] = -Math.Sqrt(3.0 / 5.0);
                etaValues[1, 1] = 0.0;
                etaValues[1, 2] = Math.Sqrt(3.0 / 5.0);

                etaValues[2, 0] = 1.0;
                etaValues[2, 1] = 1.0;
                etaValues[2, 2] = 1.0;

                etaValues[3, 0] = Math.Sqrt(3.0 / 5.0);
                etaValues[3, 1] = 0.0;
                etaValues[3, 2] = -Math.Sqrt(3.0 / 5.0);
            }

            protected override void deduct_Shape_Function_Values(double[,,] shapeFunctionValues, double[,] ksiValues, double[,] etaValues)
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        shapeFunctionValues[i, j, 0] = 0.25 * (1 - ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 1] = 0.25 * (1 + ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 2] = 0.25 * (1 + ksiValues[i, j]) * (1 + etaValues[i, j]);
                        shapeFunctionValues[i, j, 3] = 0.25 * (1 - ksiValues[i, j]) * (1 + etaValues[i, j]);
                    }
                }
            }

            public override double[,] get_ksi_Values()
            {
                return ksiValues;
            }

            public override double[,] get_eta_Values()
            {
                return etaValues;
            }

            public override double[,,] get_Shape_Function_Values()
            {
                return shapeFunctionValues;
            }

        }

        public class UniElement4x4_Sides : UniElement_Sides
        {
            public UniElement4x4_Sides()
            {
                shapeFunctionValues = new double[4, 4, 4];
                ksiValues = new double[4, 4];
                etaValues = new double[4, 4];

                initiate_Ksi_Values(ksiValues);
                initiate_Eta_Values(etaValues);
                deduct_Shape_Function_Values(shapeFunctionValues, ksiValues, etaValues);
            }
            public override double[,,] shapeFunctionValues { get; set; }
            public override double[,] ksiValues { get; set; }
            public override double[,] etaValues { get; set; }

            public override void show_Shape_Function_Values()
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        for (int k = 0; k < shapeFunctionValues.GetLength(2); k++)
                        {
                            if (k == shapeFunctionValues.GetLength(2) - 1)
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + "\n");

                            }
                            else
                            {
                                Console.Write(shapeFunctionValues[i, j, k] + " ");
                            }
                        }
                    }
                }
            }

            protected override void initiate_Ksi_Values(double[,] ksiValues)
            {
                ksiValues[0, 0] = -Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[0, 1] = -Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[0, 2] = Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[0, 3] = Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));

                ksiValues[1, 0] = 1.0;
                ksiValues[1, 1] = 1.0;
                ksiValues[1, 2] = 1.0;
                ksiValues[1, 3] = 1.0;

                ksiValues[2, 0] = Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[2, 1] = Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[2, 2] = -Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                ksiValues[2, 3] = -Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));

                ksiValues[3, 0] = -1.0;
                ksiValues[3, 1] = -1.0;
                ksiValues[3, 2] = -1.0;
                ksiValues[3, 3] = -1.0;
            }
            
            protected override void initiate_Eta_Values(double[,] etaValues)
            {
                etaValues[0, 0] = -1.0;
                etaValues[0, 1] = -1.0;
                etaValues[0, 2] = -1.0;
                etaValues[0, 3] = -1.0;

                etaValues[1, 0] = -Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[1, 1] = -Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[1, 2] = Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[1, 3] = Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));

                etaValues[2, 0] = 1.0;
                etaValues[2, 1] = 1.0;
                etaValues[2, 2] = 1.0;
                etaValues[2, 3] = 1.0;

                etaValues[3, 0] = Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[3, 1] = Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[3, 2] = -Math.Sqrt(3.0 / 7.0 - 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
                etaValues[3, 3] = -Math.Sqrt(3.0 / 7.0 + 2.0 / 7.0 * Math.Sqrt(6.0 / 5.0));
            }

            protected override void deduct_Shape_Function_Values(double[,,] shapeFunctionValues, double[,] ksiValues, double[,] etaValues)
            {
                for (int i = 0; i < shapeFunctionValues.GetLength(0); i++)
                {
                    for (int j = 0; j < shapeFunctionValues.GetLength(1); j++)
                    {
                        shapeFunctionValues[i, j, 0] = 0.25 * (1 - ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 1] = 0.25 * (1 + ksiValues[i, j]) * (1 - etaValues[i, j]);
                        shapeFunctionValues[i, j, 2] = 0.25 * (1 + ksiValues[i, j]) * (1 + etaValues[i, j]);
                        shapeFunctionValues[i, j, 3] = 0.25 * (1 - ksiValues[i, j]) * (1 + etaValues[i, j]);
                    }
                }
            }

            public override double[,] get_ksi_Values()
            {
                return ksiValues;
            }

            public override double[,] get_eta_Values()
            {
                return etaValues;
            }

            public override double[,,] get_Shape_Function_Values()
            {
                return shapeFunctionValues;
            }
        }
    }
}
