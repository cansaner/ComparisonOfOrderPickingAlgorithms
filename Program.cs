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
        public static void tuneTabuSearchParameters(String pickListsFilePath, String outputFilePath)
        {
            parameters.ItemListSet = Utils.readTestList(pickListsFilePath);
            String delimiter = "\t";            
            StreamWriter wr = new StreamWriter(outputFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                pickListsFilePath);
            wr.WriteLine("tabuLength" + delimiter + "numberOfIterations" + delimiter + "travelledDistance" + delimiter + "runningTime");
            
            for (int k = 0; k < parameters.ItemListSet.Count; k++)
            {
                List<Item> itemList = parameters.ItemListSet.ElementAt(k);
                for (int j = 0; j < parameters.NumberOfIterationsList.Length; j++)
                {
                    parameters.NumberOfIterations = parameters.NumberOfIterationsList[j];
                    for (int i = 0; i < parameters.TabuLengthList.Length; i++)
                    {
                        parameters.TabuLength = parameters.TabuLengthList[i];
                        room.ItemList = Utils.Clone<Item>(itemList);
                        picker = new Picker(depot);
                        Solution solution = new Solution(room, picker, parameters);
                        Console.WriteLine("Solving #{0} pick list of test file: {1}", k+1, pickListsFilePath);
                        solution.solve(Solution.Algorithm.TabuSearch);
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}",
                            parameters.TabuLength,
                            parameters.NumberOfIterations,
                            solution.TravelledDistance,
                            solution.RunningTime);
                        Console.WriteLine();
                        Console.ResetColor();
                        wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}",
                            parameters.TabuLength,
                            parameters.NumberOfIterations,
                            solution.TravelledDistance,
                            solution.RunningTime);
                    }
                }
                wr.WriteLine("");
            }

            wr.Close();
        }

        //Method to make a set of item pick list tryouts to tune parameters of Genetic algorithm
        public static void tuneGeneticAlgorithmParameters(String pickListsFilePath, String outputFilePath)
        {
            parameters.ItemListSet = Utils.readTestList(pickListsFilePath);
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(outputFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                pickListsFilePath);
            wr.WriteLine("NumberOfStagnantGeneration" + delimiter + "PopulationSize" + delimiter + "CrossoverProbability" + delimiter + "MutationProbability" + delimiter + "CrossoverOperator" + delimiter + "MutationOperator" + delimiter + "TravelledDistance" + delimiter + "RunningTime");
            wr.Close();

            for (int k = 0; k < parameters.ItemListSet.Count; k++)
            {
                List<Item> itemList = parameters.ItemListSet.ElementAt(k);
                for (int j = 0; j < parameters.NumberOfStagnantGenerationList.Length; j++)
                {
                    parameters.NumberOfStagnantGeneration = parameters.NumberOfStagnantGenerationList[j];
                    for (int i = 0; i < parameters.PopulationSizeList.Length; i++)
                    {
                        parameters.PopulationSize = parameters.PopulationSizeList[i];
                        for (int m = 0; m < parameters.CrossoverProbabilityList.Length; m++)
                        {
                            parameters.CrossoverProbability = parameters.CrossoverProbabilityList[m];
                            for (int n = 0; n < parameters.MutationProbabilityList.Length; n++)
                            {
                                parameters.MutationProbability = parameters.MutationProbabilityList[n];
                                for (int p = 0; p < parameters.CrossoverOperatorList.Length; p++)
                                {
                                    parameters.CrossoverOperator = parameters.CrossoverOperatorList[p];
                                    for (int r = 0; r < parameters.MutationOperatorList.Length; r++)
                                    {
                                        parameters.MutationOperator = parameters.MutationOperatorList[r];

                                        room.ItemList = Utils.Clone<Item>(itemList);
                                        picker = new Picker(depot);
                                        Solution solution = new Solution(room, picker, parameters);
                                        Console.WriteLine("Solving #{0} pick list of test file: {1}", k + 1, pickListsFilePath);
                                        solution.solve(Solution.Algorithm.GeneticAlgorithm);
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}",
                                            parameters.NumberOfStagnantGeneration,
                                            parameters.PopulationSize,
                                            parameters.CrossoverProbability,
                                            parameters.MutationProbability,
                                            parameters.CrossoverOperator.ToString(),
                                            parameters.MutationOperator.ToString(),
                                            solution.TravelledDistance,
                                            solution.RunningTime);
                                        Console.WriteLine();
                                        Console.ResetColor();
                                        wr = new StreamWriter(outputFilePath, true);
                                        wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}",
                                            parameters.NumberOfStagnantGeneration,
                                            parameters.PopulationSize,
                                            parameters.CrossoverProbability,
                                            parameters.MutationProbability,
                                            parameters.CrossoverOperator.ToString(),
                                            parameters.MutationOperator.ToString(),
                                            solution.TravelledDistance,
                                            solution.RunningTime);
                                        wr.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                wr = new StreamWriter(outputFilePath, true);
                wr.WriteLine("");
                wr.Close();
            }
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
            parameters.NumberOfIterations = 0;
            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testListWithPickListSize005.txt");
            room.ItemList = parameters.ItemListSet.ElementAt(0);
            solution = new Solution(room, picker, parameters);
            //solution.solve(Solution.Algorithm.TabuSearch);
            //solution.solve(Solution.Algorithm.SShape);
            //solution.solve(Solution.Algorithm.LargestGap);
            solution.solve(Solution.Algorithm.GeneticAlgorithm);
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

            //Add one additional Tabu search solution at the beginning to initiate multi-core process and having less values for distance matrix calculation at report
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.solve(Solution.Algorithm.TabuSearch);

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

        //Method to setup parameter tuning of Tabu Search algorithm
        public static void setupTabuSearchParameterTuning(bool generateNewTestLists)
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

            if (generateNewTestLists)
            {
                //Setup test list generation parameters here
                parameters.PickListSizesOfTestLists = new int[] { 25, 50, 100 };
                //parameters.PickListSizesOfTestLists = new int[] { 5 };
                parameters.NumberOfPickLists = 50;
                Utils.generateTestLists(room, parameters.PickListSizesOfTestLists, parameters.NumberOfPickLists);
            }

            parameters.TabuLengthList = new int[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            //parameters.TabuLengthList = new int[] { 5, 6 };
            parameters.NumberOfIterationsList = new int[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 }; //if parameters.NumberOfIterations <=0 than numberOfIterations will be taken as ItemList.Count
            //parameters.NumberOfIterationsList = new int[] { 10, 20 };

            String[] itemListFilePaths = new String[]
            {
                "../../../files/testListWithPickListSize025.txt",
                "../../../files/testListWithPickListSize050.txt",
                "../../../files/testListWithPickListSize100.txt"
            };
            String parameterTuningReportFilePath = "../../../files/parameterTuning.txt";

            foreach (string path in itemListFilePaths)
            {
                if (File.Exists(path))
                {
                    tuneTabuSearchParameters(path, parameterTuningReportFilePath);
                }
            }
        }

        //Method to setup parameter tuning of Genetic Algorithm
        public static void setupGeneticAlgorithmParameterTuning(bool generateNewTestLists)
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

            if (generateNewTestLists)
            {
                //Setup test list generation parameters here
                parameters.PickListSizesOfTestLists = new int[] { 25 };
                parameters.NumberOfPickLists = 500;
                Utils.generateTestLists(room, parameters.PickListSizesOfTestLists, parameters.NumberOfPickLists);
            }

            parameters.NumberOfStagnantGenerationList = new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50};
            parameters.PopulationSizeList = new int[] { 10, 20, 30, 40, 50 };
            parameters.CrossoverProbabilityList = new float[] { 0.6f, 0.7f, 0.8f, 0.9f};
            parameters.MutationProbabilityList = new float[] { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f };
            parameters.CrossoverOperatorList = new PickListGAParameters.Crossover[] { PickListGAParameters.Crossover.Ordered };
            parameters.MutationOperatorList = new PickListGAParameters.Mutation[] { PickListGAParameters.Mutation.Inversion };
            
            String[] itemListFilePaths = new String[]
            {
                "../../../files/testListWithPickListSize025.txt"
            };
            String parameterTuningReportFilePath = "../../../files/parameterTuningGA.txt";

            foreach (string path in itemListFilePaths)
            {
                if (File.Exists(path))
                {
                    tuneGeneticAlgorithmParameters(path, parameterTuningReportFilePath);
                }
            }
        }

        public static void Main(string[] args)
        {
            //Test.runTestCases();
            //runRealWorldChallenge();
            setupGeneticAlgorithmParameterTuning(true);
            //setupTabuSearchParameterTuning(true);//false yaparsan generate etmez
            //String listFilePath = "../../../files/testListOfSize025ForAlgorithmComparison.txt";
            String listFilePath = "../../../files/testListWithPickListSize005.txt";
            String reportFilePath = "../../../files/AlgorithmComparisonReport.txt";
            //compareAlgorithms(listFilePath, reportFilePath);
            Console.ReadLine();
        }
    }
}