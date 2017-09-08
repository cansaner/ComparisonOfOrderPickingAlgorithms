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
            String delimiter = "\t";            
            StreamWriter wr = new StreamWriter(outputFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                pickListsFilePath);
            wr.WriteLine("tabuLength" + delimiter + "numberOfIterations" + delimiter + "travelledDistance" + delimiter + "runningTime");
            parameters.ItemListSet = Utils.readTestList(pickListsFilePath);

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
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(outputFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                pickListsFilePath);
            wr.WriteLine("instanceNumber" + delimiter + "DistanceMatrixRunningTime" + delimiter + "NumberOfStagnantGeneration" + delimiter + "PopulationSize" + delimiter + "CrossoverProbability" + delimiter + "MutationProbability" + delimiter + "CrossoverOperator" + delimiter + "MutationOperator" + delimiter + "TravelledDistance" + delimiter + "RunningTime");
            wr.Close();

            parameters.ItemListSet = Utils.readTestList(pickListsFilePath);

            bool distanceMatrixShouldBeCalculated = true;
            double[,] calculatedDistanceMatrix = new double[1, 1]; ;
            List<Coordinate>[,] calculatedPathMatrix = new List<Coordinate>[1, 1];
            double calculatedDistanceMatrixRunningTime = 0;

            //Add one additional Distance Matrix calculation at the beginning to initiate multi-core process and having less values for distance matrix calculation at report
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));

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
                                        solution = new Solution(room, picker, parameters);
                                        if (distanceMatrixShouldBeCalculated)
                                        {
                                            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));
                                            calculatedDistanceMatrix = solution.DistanceMatrix;
                                            calculatedPathMatrix = solution.PathMatrix;
                                            calculatedDistanceMatrixRunningTime = solution.DistanceMatrixRunningTime;
                                            distanceMatrixShouldBeCalculated = false;
                                        }
                                        else
                                        {
                                            //carrying already calculated distance matrix and path matrix to the solution
                                            solution.DistanceMatrix = calculatedDistanceMatrix;
                                            solution.PathMatrix = calculatedPathMatrix;
                                            solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                                        }
                                        
                                        //Console.WriteLine("Solving #{0} pick list of test file: {1}", k + 1, pickListsFilePath);
                                        solution.solve(Solution.Algorithm.GeneticAlgorithm);
                                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                                        Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}",
                                            k + 1,
                                            solution.DistanceMatrixRunningTime,
                                            parameters.NumberOfStagnantGeneration,
                                            parameters.PopulationSize,
                                            parameters.CrossoverProbability,
                                            parameters.MutationProbability,
                                            parameters.CrossoverOperator.ToString(),
                                            parameters.MutationOperator.ToString(),
                                            solution.TravelledDistance,
                                            solution.RunningTime);
                                        //Console.WriteLine();
                                        Console.ResetColor();
                                        wr = new StreamWriter(outputFilePath, true);
                                        wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}",
                                            k + 1,
                                            solution.DistanceMatrixRunningTime,
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
                calculatedDistanceMatrix = null;
                calculatedPathMatrix = null;
                calculatedDistanceMatrixRunningTime = 0;
                distanceMatrixShouldBeCalculated = true;
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
        public static void compareAlgorithmsWithTabu(String listFilePath, String reportFilePath)
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

        //Method to compare solution algorithms
        public static void compareAlgorithmsWithGeneticAlgorithm(String listFilePath, String reportFilePath)
        {
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                listFilePath);
            wr.WriteLine("InstanceNumber" + delimiter + "DistanceMatrixRunningTime" 
                + delimiter + "GACycleSwapTotalDistance" + delimiter + "GACycleSwapRunningTime" + delimiter + "GACycleInversionTotalDistance" + delimiter + "GACycleInversionRunningTime"
                + delimiter + "GAOrderedSwapTotalDistance" + delimiter + "GAOrderedSwapRunningTime" + delimiter + "GAOrderedInversionTotalDistance" + delimiter + "GAOrderedInversionRunningTime"
                + delimiter + "GAPMXSwapTotalDistance" + delimiter + "GAPMXSwapRunningTime" + delimiter + "GAPMXInversionTotalDistance" + delimiter + "GAPMXInversionRunningTime"
                + delimiter + "SShapeTravelledTotalDistance" + delimiter + "SShapeRunningTime" + delimiter + "LargestGapTravelledTotalDistance" + delimiter + "LargestGapRunningTime");
            wr.Close();

            parameters.NumberOfStagnantGeneration = 100;
            parameters.PopulationSize = 450;
            parameters.CrossoverProbability = 0.8f;
            parameters.MutationProbability = 0.08f;
            parameters.CrossoverOperator = PickListGAParameters.Crossover.Cycle;
            parameters.MutationOperator = PickListGAParameters.Mutation.Swap;
            parameters.ItemListSet = Utils.readTestList(listFilePath);
            
            double[,] calculatedDistanceMatrix = new double[1, 1]; ;
            List<Coordinate>[,] calculatedPathMatrix = new List<Coordinate>[1, 1];
            double calculatedDistanceMatrixRunningTime = 0;

            //Add one additional Distance Matrix calculation at the beginning to initiate multi-core process and having less values for distance matrix calculation at report
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));
            
            for (int i = 0; i < parameters.ItemListSet.Count; i++)
            {
                List<Item> itemList = parameters.ItemListSet.ElementAt(i);

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Cycle;
                parameters.MutationOperator = PickListGAParameters.Mutation.Swap;
                solution = new Solution(room, picker, parameters);
                
                solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));
                calculatedDistanceMatrix = solution.DistanceMatrix;
                calculatedPathMatrix = solution.PathMatrix;
                calculatedDistanceMatrixRunningTime = solution.DistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double distanceMatrixRunningTime = solution.DistanceMatrixRunningTime;
                double travelledTotalDistance01 = solution.TravelledDistance;
                double runningTime01 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Cycle;
                parameters.MutationOperator = PickListGAParameters.Mutation.Inversion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance02 = solution.TravelledDistance;
                double runningTime02 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Ordered;
                parameters.MutationOperator = PickListGAParameters.Mutation.Swap;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance03 = solution.TravelledDistance;
                double runningTime03 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Ordered;
                parameters.MutationOperator = PickListGAParameters.Mutation.Inversion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance04 = solution.TravelledDistance;
                double runningTime04 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PMX;
                parameters.MutationOperator = PickListGAParameters.Mutation.Swap;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance05 = solution.TravelledDistance;
                double runningTime05 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PMX;
                parameters.MutationOperator = PickListGAParameters.Mutation.Inversion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance06 = solution.TravelledDistance;
                double runningTime06 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.SShape);

                double travelledTotalDistance07 = solution.TravelledDistance;
                double runningTime07= solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.LargestGap);

                double travelledTotalDistance08 = solution.TravelledDistance;
                double runningTime08 = solution.RunningTime;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}" + delimiter + "{10}" + delimiter + "{11}" + delimiter + "{12}" + delimiter + "{13}" + delimiter + "{14}" + delimiter + "{15}" + delimiter + "{16}" + delimiter + "{17}",
                    i + 1,
                    distanceMatrixRunningTime,
                    travelledTotalDistance01,
                    runningTime01,
                    travelledTotalDistance02,
                    runningTime02,
                    travelledTotalDistance03,
                    runningTime03,
                    travelledTotalDistance04,
                    runningTime04,
                    travelledTotalDistance05,
                    runningTime05,
                    travelledTotalDistance06,
                    runningTime06,
                    travelledTotalDistance07,
                    runningTime07,
                    travelledTotalDistance08,
                    runningTime08);
                Console.ResetColor();

                wr = new StreamWriter(reportFilePath, true);
                wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}" + delimiter + "{10}" + delimiter + "{11}" + delimiter + "{12}" + delimiter + "{13}" + delimiter + "{14}" + delimiter + "{15}" + delimiter + "{16}" + delimiter + "{17}",
                    i + 1,
                    distanceMatrixRunningTime,
                    travelledTotalDistance01,
                    runningTime01,
                    travelledTotalDistance02,
                    runningTime02,
                    travelledTotalDistance03,
                    runningTime03,
                    travelledTotalDistance04,
                    runningTime04,
                    travelledTotalDistance05,
                    runningTime05,
                    travelledTotalDistance06,
                    runningTime06,
                    travelledTotalDistance07,
                    runningTime07,
                    travelledTotalDistance08,
                    runningTime08);
                wr.Close();
                
                calculatedDistanceMatrix = null;
                calculatedPathMatrix = null;
                calculatedDistanceMatrixRunningTime = 0;
            }

            wr = new StreamWriter(reportFilePath, true);
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

            parameters.NumberOfStagnantGenerationList = new int[] { 50, 60, 70, 80, 90, 100, 110, 120 };
            parameters.PopulationSizeList = new int[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600 };
            parameters.CrossoverProbabilityList = new float[] { 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f };
            parameters.MutationProbabilityList = new float[] { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.11f, 0.12f };
            //parameters.NumberOfStagnantGenerationList = new int[] { 100 };
            //parameters.PopulationSizeList = new int[] { 450 };
            //parameters.CrossoverProbabilityList = new float[] { 0.8f };
            //parameters.MutationProbabilityList = new float[] { 0.08f };
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

        //Method to setup parameter tuning of Genetic Algorithm
        public static void setupGeneticAlgorithmComparison(bool generateNewTestLists)
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
                parameters.NumberOfPickLists = 500;
                Utils.generateTestLists(room, parameters.PickListSizesOfTestLists, parameters.NumberOfPickLists);
            }
            
            String[] itemListFilePaths = new String[]
            {
                "../../../files/testListWithPickListSize100_part05.txt",
                "../../../files/testListWithPickListSize100_part04.txt",
                "../../../files/testListWithPickListSize100_part03.txt",
                "../../../files/testListWithPickListSize100_part02.txt",
                "../../../files/testListWithPickListSize100_part01.txt"
            };
            String comparisonReportFilePath = "../../../files/GeneticAlgorithmComparisonReport_100_parts.txt";

            foreach (string path in itemListFilePaths)
            {
                if (File.Exists(path))
                {
                    compareAlgorithmsWithGeneticAlgorithm(path, comparisonReportFilePath);
                }
            }
        }

        public static void Main(string[] args)
        {
            //Test.runTestCases();
            //runRealWorldChallenge();
            //setupTabuSearchParameterTuning(true);//false yaparsan generate etmez
            //String listFilePath = "../../../files/testListOfSize025ForAlgorithmComparison.txt";
            //String listFilePath = "../../../files/testListWithPickListSize005.txt";
            //String reportFilePath = "../../../files/AlgorithmComparisonReport.txt";
            //compareAlgorithms(listFilePath, reportFilePath);
            //setupGeneticAlgorithmParameterTuning(true);
            setupGeneticAlgorithmComparison(false);
            Console.ReadLine();
        }
    }
}