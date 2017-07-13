using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Program
    {
        private static Coordinate depot;
        private static Problem room;
        private static Parameters parameters;
        private static Picker picker;
        private static Solution solution;

        //Method to make a set of item pick list tryouts to tune parameters of Tabu Search algorithm
        public static void tuneParameters(Problem problem, Coordinate depot, Parameters parameters, String filePath)
        {
            StreamWriter wr = new StreamWriter(filePath, true);
            Picker picker;
            foreach (List<Item> itemList in parameters.ItemListSet)
            {
                problem.ItemList = itemList;
                picker = new Picker(depot);
                for (int j = 0; j < parameters.NumberOfIterationsList.Length; j++)
                {
                    parameters.NumberOfIterations = parameters.NumberOfIterationsList[j];
                    for (int i = 0; i < parameters.TabuLengthList.Length; i++)
                    {
                        parameters.TabuLength = parameters.TabuLengthList[i];
                        Solution solution = new Solution(problem, picker, parameters);
                        solution.solve(Solution.Algorithm.TabuSearch);
                        Console.WriteLine("{0}:{1}:{2}:{3}", parameters.TabuLength, parameters.NumberOfIterations, solution.TravelledDistance, solution.RunningTime);
                        wr.WriteLine("{0}:{1}:{2}:{3}", parameters.TabuLength, parameters.NumberOfIterations, solution.TravelledDistance, solution.RunningTime);
                    }
                }
                wr.WriteLine("");
            }

            wr.Close();
        }

        //Method to run our real world case with different algorithms
        public static void runRealWorldChallenge()
        {
            int S = 10;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 31;

            depot = new Coordinate(1, no_of_horizontal_aisles);
            room = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);
            parameters = new Parameters();

            parameters.TabuLength = 5;
            //parameters.NumberOfIterations = 20;
            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("C:\\masterTez/sshape1New5.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution = new Solution(room, picker, parameters);
            solution.solve(Solution.Algorithm.TabuSearch);

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            //room.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room, picker, parameters);
            //solution5.solve(Solution.Algorithm.SShape);
            //assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            //room.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room, picker, parameters);
            //solution5.solve(Solution.Algorithm.LargestGap);
            ////assert here
        }

        //Method to compare solution algorithms
        public static void compareAlgorithms(String listFilePath, String reportFilePath)
        {
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("instanceNumber" + delimiter + "tabuSearchDistanceMatrixRunningTime" + delimiter + "tabuSearchTravelledTotalDistance" + delimiter + "tabuSearchRunningTime" + delimiter + "SShapeTravelledTotalDistance" + delimiter + "SShapeRunningTime" + delimiter + "LargestGapTravelledTotalDistance" + delimiter + "LargestGapRunningTime");
           
            int S = 10;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 31;

            depot = new Coordinate(1, no_of_horizontal_aisles);
            room = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);
            parameters = new Parameters();
            parameters.TabuLength = 5;
            parameters.ItemListSet = Utils.readTestList(listFilePath);

            for (int i = 0; i < parameters.ItemListSet.Count; i++)
            {
                room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(i));
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.TabuSearch);

                double distanceMatrixRunningTime = solution.DistanceMatrixRunningTime;
                double travelledTotalDistance01 = solution.TravelledDistance;
                double runningTime01 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(i));
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.SShape);

                double travelledTotalDistance02 = solution.TravelledDistance;
                double runningTime02 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(i));
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.LargestGap);

                double travelledTotalDistance03 = solution.TravelledDistance;
                double runningTime03 = solution.RunningTime;

                wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}", 
                    i + 1, 
                    distanceMatrixRunningTime, 
                    travelledTotalDistance01, 
                    runningTime01, 
                    travelledTotalDistance02, 
                    runningTime02, 
                    travelledTotalDistance03, 
                    runningTime03);
            }

            wr.WriteLine("");
            wr.Close();
        }

        public static void Main(string[] args)
        {
            //Test.runTestCases();
            //TABU SEARCH BUNUN ICINDE
            //runRealWorldChallenge();
            String listFilePath = "../../../files/testListOfSize025ForAlgorithmComparison.txt";
            //String listFilePath = "../../../files/testListWithSize005.txt";
            String reportFilePath = "../../../files/AlgorithmComparisonReport.txt";
            compareAlgorithms(listFilePath, reportFilePath);
            
            //parameters.TabuLength = 5;
            //parameters.NumberOfIterations = 20;
            //Solution solution = new Solution(room1, picker, parameters);
            //solution.solve(Solution.Methods.TabuSearch);
            //solve((int)Methods.TabuSearch);
            //solve((int)Methods.SShape);
            //solve((int)Methods.LargestGap);

            //LIST GENERATION--BUNU BIR KERE YAP YAPARKEN PARAMETER TUNING KISMINI KAPAT SONRA LIST GENERATION I KAPAT
            //parameters.SizeOfLists = new int[] { 25, 50, 100 };
            //parameters.SizeOfLists = new int[] { 5 };
            //parameters.NumberOfLists = 5;
            //Utils.generateTestLists(room1, parameters.SizeOfLists, parameters.NumberOfLists);

            //PARAMETER TUNING--BUNU LIST GENERATION I YAPTIKTAN SONRA AC
            //parameters.ItemListSet = readTestList("C:\\masterTez/testListWithSize025.txt");
            //parameters.ItemListSet = readTestList("C:\\masterTez/testListWithSize005.txt");
            //parameters.TabuLengthList = new int[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            //parameters.TabuLengthList = new int[] { 5, 6 };
            //parameters.NumberOfIterationsList = new int[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            //parameters.NumberOfIterationsList = new int[] { 10, 20 };
            //tuneParameters(room1, depot, parameters, "C:\\masterTez/parameterTuning.txt");

            Console.ReadLine();
        }
    }
}