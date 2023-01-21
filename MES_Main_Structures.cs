using System;
using System.IO;

namespace ConsoleApp1
{
    internal class MES_Main_Structures
    {
        public static int numberOfNodes = 0;

        public static int numberOfElements = 0;
        public static int getNumberOfNodes()
        {
            return numberOfNodes;
        }
        public static int getNumberOfElements()
        {
            return numberOfElements;
        }

        public class Node
        {
            public Node(int id, double x = 0.0, double y = 0.0, double temp = 0.0, int bc = 0)
            {
                ID = id;
                X = x;
                Y = y;
                Temp = temp;
                BC = bc;
            }

            public void Show()
            {
                Console.WriteLine($"ID: {this.ID}\nX: {this.X}\nY: {this.Y}\nTemp: {this.Temp}\nBC: {this.BC}\n");
            }

            public int ID { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Temp { get; set; }
            public int BC { get; set; }
        }
        
        public class Element
        {
            public Element(int id)
            {
                ID = id;
                Nodes = new int[4] { 0, 0, 0, 0 };
            }

            public void Show()
            {
                Console.WriteLine($"ID: {this.ID}\nElement1: {this.Nodes[0]}\nElement2: {this.Nodes[1]}\nElement3: {this.Nodes[2]}\nElement4 : {this.Nodes[3]}\n");
            }

            public int[] getNodes() { return Nodes; }

            public int ID { get; set; }
            public int[] Nodes { get; set; }
        }

        public class Grid
        {
            public Grid(Element[] ElementsData, Node[] NodesData, GlobalData globalData)
            {
                numberOfElements = ElementsData.Length;
                numberOfNodes = NodesData.Length;

                arrayOfElements = ElementsData;
                arrayOfNodes = NodesData;

                NinEStructure = createNodesInElementStructure(globalData);
            }
            public int numberOfElements { get; set; }
            public int numberOfNodes { get; set; }
            public Node[][] NinEStructure { get; set; }
            public Node[] arrayOfNodes { get; set; }
            public Element[] arrayOfElements { get; set; }

            public Node[][] getNineStructure()
            {
                return NinEStructure;
            }

            public Node[] getNodeArray()
            {
                return arrayOfNodes;
            }

            public Element[] getElementsArray()
            {
                return arrayOfElements;
            }

            // fukcja zwracająca referencję do elementu siatki
            public Node[] getElement(int index)
            {
                return NinEStructure[index];
            }

            // funkcja testowa modyfikująca pierwszy element w siatce - dla celów testowych
            public void set_testNodes(Node[] testNodes)
            {
                testNodes[0].X = 0.0;
                testNodes[0].Y = 0.0;
                testNodes[1].X = 0.025;
                testNodes[1].Y = 0.0;
                testNodes[2].X = 0.025;
                testNodes[2].Y = 0.025;
                testNodes[3].X = 0.0;
                testNodes[3].Y = 0.025;
            }

            // funkcje pomocnicze przy tworzeniu struktury NodesInElement //

            // funkcja prywatna wykorzystywana przy tworzeniu struktury NodesInElement 
            private Node[][] createNodesInElementStructure(GlobalData globalData)
            {
                Node[][] nodesForGridElements = new Node[numberOfElements][];

                for (int i = 0; i < numberOfElements; i++)
                {
                    int[] nodesListForElement = new int[4];
                    nodesListForElement = deductNodesForEachIndividualElement(arrayOfElements[i]);

                    nodesForGridElements[i] = new Node[4];
                    initializeNodeArrayForIndividualElement(nodesForGridElements[i], arrayOfNodes, nodesListForElement);
                }
                return nodesForGridElements;
            }

            // funkcja dedukująca poszczególne Node -> przekonwertowanie infomracji -> ID node'a w strukturze Element na poszczególne Node'y 
            private int[] deductNodesForEachIndividualElement(Element element)
            {
                int[] nodesResult = new int[4];
                int i = 0;
                foreach (var value in element.Nodes)
                {
                    nodesResult[i] = value;
                    i++;
                }
                return nodesResult;
            }

            // inicjalizacja tablicy Node[] dla każdego elementu
            private void initializeNodeArrayForIndividualElement(Node[] nodes, Node[] allGridNodes, int[] nodeList)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i] = new Node(allGridNodes[nodeList[i] - 1].ID, allGridNodes[nodeList[i] - 1].X, allGridNodes[nodeList[i] - 1].Y, allGridNodes[nodeList[i] - 1].Temp, allGridNodes[nodeList[i] - 1].BC);
                }
            }
        }

        public class GlobalData
        {
            public GlobalData(params int[] gdParams)
            {
                for (int i = 0; i < gdParams.GetLength(0); i++)
                {
                    SimulationTime = gdParams[0];
                    SimulationStepTime = gdParams[1];
                    Conductivity = gdParams[2];
                    Alfa = gdParams[3];
                    Tot = gdParams[4];
                    InitialTemp = gdParams[5];
                    Density = gdParams[6];
                    SpecificHeat = gdParams[7];
                    Nodes_number = gdParams[8];
                    Elements_number = gdParams[9];
                }
            }

            public void ShowGlobalData()
            {
                Console.WriteLine($"SimulationTime: {this.SimulationTime}\nSimulationStepTime: {this.SimulationStepTime}\nConductivity: {this.Conductivity}\nAlfa: {this.Alfa}\n" +
                    $"Tot: {this.Tot}\nInitialTemp: {this.InitialTemp}\nDensity: {this.Density}\nSpecificHeat: {this.SpecificHeat}\nNodes_number: {this.Nodes_number}\n" +
                    $"Elements_number: {this.Elements_number}");
            }

            public int SimulationTime { get; set; }
            public int SimulationStepTime { get; set; }
            public int Conductivity { get; set; }
            public int Alfa { get; set; }
            public int Tot { get; set; }
            public int InitialTemp { get; set; }
            public int Density { get; set; }
            public int SpecificHeat { get; set; }
            public int Nodes_number { get; set; }
            public int Elements_number { get; set; }
        }

        public static void get_GlobalData_From_File(string filePath, ref int numberOfNodes, ref int numberOfElements, GlobalData GD)
        {
            try
            {
                using FileStream fs = File.OpenRead(filePath);
                int[] globalDataElements = new int[10];
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < 10; i++)
                {
                    int separator = lines[i].LastIndexOf(" ");
                    string substring1 = lines[i].Substring(separator + 1);
                    int value = int.Parse(substring1);
                    globalDataElements[i] = value;
                }
                numberOfNodes = globalDataElements[8];
                numberOfElements = globalDataElements[9];

                create_Global_Data(globalDataElements, GD);
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
            }
        }

        public static void get_Nodes_and_Elements_From_File(string filePath, int noOfNodes, int noOfElements, Node[] nodes, Element[] elements)
        {
            try
            {
                using FileStream fs = File.OpenRead(filePath);
                string[] lines = File.ReadAllLines(filePath);

                int nodeIndex = 0;
                for (int i = 11; i < 11 + noOfNodes; i++)
                {
                    string delimiter = ",";
                    string[] result = lines[i].Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    int id = int.Parse(result[0]);
                    double valueX = double.Parse(result[1], System.Globalization.CultureInfo.InvariantCulture);
                    double valueY = double.Parse(result[2], System.Globalization.CultureInfo.InvariantCulture);

                    nodes[nodeIndex].ID = id;
                    nodes[nodeIndex].X = valueX;
                    nodes[nodeIndex].Y = valueY;
                    nodeIndex++;
                }

                int elementIndex = 0;
                for (int i = 12 + noOfNodes; i < 12 + noOfNodes + noOfElements; i++)
                {
                    string delimiter = ",";
                    string[] result = lines[i].Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    int id = int.Parse(result[0]);
                    int element1 = int.Parse(result[1]);
                    int element2 = int.Parse(result[2]);
                    int element3 = int.Parse(result[3]);
                    int element4 = int.Parse(result[4]);

                    elements[elementIndex].ID = id;
                    elements[elementIndex].Nodes[0] = element1;
                    elements[elementIndex].Nodes[1] = element2;
                    elements[elementIndex].Nodes[2] = element3;
                    elements[elementIndex].Nodes[3] = element4;
                    elementIndex++;
                }

                //
                //Aktualizacja wartości BC dla poszczególnych nodów
                //

                int BC_Row_Position = 13 + noOfNodes + noOfElements;
                string delimiter2 = ",";
                string[] result2 = lines[BC_Row_Position].Split(delimiter2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var x in result2)
                {
                    int value = int.Parse(x);
                    nodes[value - 1].BC = 1;
                }

                //Console.WriteLine("Press any key to exit.");
                //Console.ReadKey();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught: " + ex.Message);
            }
        }

        public static void create_Global_Data(int[] values, GlobalData GD)
        {
            GD.SimulationTime = values[0];
            GD.SimulationStepTime = values[1];
            GD.Conductivity = values[2];
            GD.Alfa = values[3];
            GD.Tot = values[4];
            GD.InitialTemp = values[5];
            GD.Density = values[6];
            GD.SpecificHeat = values[7];
            GD.Nodes_number = values[8];
            GD.Elements_number = values[9];
        }

        public static void initalize_Nodes_And_Elements_Array(Node[] nodes, Element[] elements)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new Node(i + 1);
            }

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new Element(i + 1);
            }
        }
    }
}
