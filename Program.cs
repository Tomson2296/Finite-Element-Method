using System;
using System.IO;

using static ConsoleApp1.MES_Main_Structures;
using MESMain = ConsoleApp1.MES_Main_Structures;
using MESUni = ConsoleApp1.MES_Universal_Element;
using MESH = ConsoleApp1.MES_H_Matrix;
using MESC = ConsoleApp1.MES_C_Matrix;
using MESAggrH = ConsoleApp1.MES_Aggregation_Matrix;
using MESP = ConsoleApp1.MES_P_Vector;
using MESTemp = ConsoleApp1.MES_Temperature_Solution;


class Program
{
    public static String model_Selection()
    {
        string result;
        string input;
        int value;
        do
        {
            Console.WriteLine("Wybierz jeden z dostepnych modeli: (1) 4x4 Simple, (2) 4x4 MixGrid, (3) 31x31 Kwadrat");
            input = Console.ReadLine();
            value = int.Parse(input);
        }
        while (value != 1 && value != 2 && value != 3);
        
        Console.WriteLine();
        switch (value)
        {
            case 1:
                result = "..\\..\\..\\..\\Mesh\\Test1_4_4.txt";
                return result;
            case 2:
                result = "..\\..\\..\\..\\Mesh\\Test2_4_4_MixGrid.txt";
                return result;
            case 3:
                result = "..\\..\\..\\..\\Mesh\\Test3_31_31_kwadrat.txt";
                return result;
            default:
                return String.Empty;
        }
    }
    public static int interpolation_Schema_Selection()
    {
        string input;
        int value;
        do
        {
            Console.WriteLine("Wybierz jeden z dostepnych schematow calkowania: 2, 3 lub 4-punktowy");
            input = Console.ReadLine();
            value = int.Parse(input);
        }
        while (value != 2 && value != 3 && value != 4);
        Console.WriteLine();
        return value * value;
    }
    public static void process_Program_Logic(int interpolation_Points, MESMain.GlobalData globalData, MESMain.Grid grid)
    {
        switch (interpolation_Points)
        {
            case 4:
                MESUni.UniElement2x2 element2X2 = new();
                MESUni.UniElement2x2_Sides element2X2_Sides = new();
                MESH.H_Matrix _h2GlobalMatrix = new(globalData, grid, element2X2, element2X2_Sides, interpolation_Points);
                //_h2GlobalMatrix.show_H_Global_Matrix();
                MESC.C_Matrix _c2GLobalMatrix = new(globalData, grid, element2X2, interpolation_Points);
                //_c2GLobalMatrix.show_C_Global_Matrix();
                MESP.P_Vector _2P_Vector = new(globalData, grid, element2X2, element2X2_Sides, interpolation_Points);
                //_2P_Vector.show_P_GlobalVector();
                MESAggrH.H_Aggregation_Matrix h2_AggregationMatrix = new(grid, _h2GlobalMatrix);
                //h2_AggregationMatrix.showAggregation_H_Matrix();
                MESAggrH.C_Aggregation_Matrix c2_AggregationMatrix = new(grid, _c2GLobalMatrix);
                //c2_AggregationMatrix.showAggregation_C_Matrix();
                MESTemp.MES_Temp_Solution _2temp_Solution = new(globalData, h2_AggregationMatrix, c2_AggregationMatrix, _2P_Vector);
                //_2temp_Solution.show_Temp_Values(globalData);
                break;

            case 9:
                MESUni.UniElement3x3 element3x3 = new();
                MESUni.UniElement3x3_Sides element3x3_Sides = new();
                MESH.H_Matrix _h3GlobalMatrix = new(globalData, grid, element3x3, element3x3_Sides, interpolation_Points);
                MESC.C_Matrix _c3GLobalMatrix = new(globalData, grid, element3x3, interpolation_Points);
                MESP.P_Vector _3P_Vector = new(globalData, grid, element3x3, element3x3_Sides, interpolation_Points);
                //_3P_Vector.show_P_GlobalVector();
                MESAggrH.H_Aggregation_Matrix _h3_AggregationMatrix = new(grid, _h3GlobalMatrix);
                //_h3_AggregationMatrix.showAggregation_H_Matrix();
                MESAggrH.C_Aggregation_Matrix _c3_AggregationMatrix = new(grid, _c3GLobalMatrix);
                //_c3_AggregationMatrix.showAggregation_C_Matrix();
                MESTemp.MES_Temp_Solution _3temp_Solution = new(globalData, _h3_AggregationMatrix, _c3_AggregationMatrix, _3P_Vector);
                break;

            case 16:
                MESUni.UniElement4x4 element4x4 = new();
                MESUni.UniElement4x4_Sides element4x4_Sides = new();
                MESH.H_Matrix _h4GlobalMatrix = new(globalData, grid, element4x4, element4x4_Sides, interpolation_Points);
                MESC.C_Matrix _c4GLobalMatrix = new(globalData, grid, element4x4, interpolation_Points);
                MESP.P_Vector _4P_Vector = new(globalData, grid, element4x4, element4x4_Sides, interpolation_Points);
                MESAggrH.H_Aggregation_Matrix _h4_AggregationMatrix = new(grid, _h4GlobalMatrix);
                MESAggrH.C_Aggregation_Matrix _c4_AggregationMatrix = new(grid, _c4GLobalMatrix);
                MESTemp.MES_Temp_Solution _4temp_Solution = new(globalData, _h4_AggregationMatrix, _c4_AggregationMatrix, _4P_Vector);
                break;

            default:
                break;
        }
    }

    static void Main()
    {
        String model_Identification_String = String.Empty;
        model_Identification_String = model_Selection();

        // Utworzenie struktury globalData -> informacje ilośc elementów, ilość węzłów, temperatura początkowa, przewodność cieplna itd. //
        MESMain.GlobalData globalData = new();
        MESMain.get_GlobalData_From_File(model_Identification_String, ref MESMain.numberOfNodes, ref MESMain.numberOfElements, globalData);

        // Wstępna inicjalizacja tablicy nodów i elementów
        MESMain.Node[] nodes = new Node[MESMain.numberOfNodes];
        MESMain.Element[] elements = new Element[MESMain.numberOfElements];
        MESMain.initalize_Nodes_And_Elements_Array(nodes, elements);
        MESMain.get_Nodes_and_Elements_From_File(model_Identification_String, MESMain.numberOfNodes, MESMain.numberOfElements, nodes, elements);

        // Utworzenie struktury grid -> przepisanie utworzonych struktur nodów i elementów do głownej struktury grid //
        MESMain.Grid grid = new(elements, nodes, globalData);
        //grid.set_testNodes(grid.getElement(0));
        
        // Wybor schematu całkowania oraz funkcja obsługująca dalszą logike programu - tworzenie odpowiednich struktur oraz wyznaczanie równania nieustalonego procesu przepływu ciepla w elemencie skończonym //
        int interpolation_Points;
        interpolation_Points = interpolation_Schema_Selection();
        process_Program_Logic(interpolation_Points, globalData, grid);
    }
}
