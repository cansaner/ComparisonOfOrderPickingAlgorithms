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
            parameters.NumberOfStagnantGeneration = 10;
            parameters.PopulationSize = 10;
            parameters.CrossoverProbability = 0.5f;
            parameters.MutationProbability = 0.5f;
            parameters.CrossoverOperator = PickListGAParameters.Crossover.PositionBased;
            parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
            picker = new Picker(depot);
            parameters.ItemListSet = Utils.readTestList("../../../files/testListWithPickListSize005.txt");
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));

            double[,] calculatedDistanceMatrix = new double[1, 1]; ;
            List<Coordinate>[,] calculatedPathMatrix = new List<Coordinate>[1, 1];
            double calculatedDistanceMatrixRunningTime = 0;
            
            solution = new Solution(room, picker, parameters);

            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));
            calculatedDistanceMatrix = solution.DistanceMatrix;
            calculatedPathMatrix = solution.PathMatrix;
            calculatedDistanceMatrixRunningTime = solution.DistanceMatrixRunningTime;
            
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
            wr.WriteLine("{0}" + delimiter + "{1}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                    listFilePath);
            wr.WriteLine("instanceNumber" + delimiter + "tabuSearchDistanceMatrixRunningTime" + delimiter + "tabuSearchTravelledTotalDistance" + delimiter + "tabuSearchRunningTime" + delimiter + "SShapeTravelledTotalDistance" + delimiter + "SShapeRunningTime" + delimiter + "LargestGapTravelledTotalDistance" + delimiter + "LargestGapRunningTime");
            wr.Close();

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

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}",
                    i + 1,
                    distanceMatrixRunningTime,
                    travelledTotalDistance01,
                    runningTime01,
                    travelledTotalDistance02,
                    runningTime02,
                    travelledTotalDistance03,
                    runningTime03);
                Console.ResetColor();

                wr = new StreamWriter(reportFilePath, true);
                wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}", 
                    i + 1, 
                    distanceMatrixRunningTime, 
                    travelledTotalDistance01, 
                    runningTime01, 
                    travelledTotalDistance02, 
                    runningTime02, 
                    travelledTotalDistance03, 
                    runningTime03);
                wr.Close();
            }
            wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("");
            wr.Close();
        }

        //Method to test new implemented operators
        public static void testNewOperatorsWithGeneticAlgorithm(String listFilePath, String reportFilePath)
        {
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                listFilePath);
            wr.WriteLine("InstanceNumber" + delimiter + "DistanceMatrixRunningTime"
                + delimiter + "GAOX2ShuffleTotalDistance" + delimiter + "GAOX2ShuffleRunningTime" + delimiter + "GAOX2InsertionTotalDistance" + delimiter + "GAOX2InsertionRunningTime"
                + delimiter + "GAOX2DisplacementTotalDistance" + delimiter + "GAOX2DisplacementRunningTime" + delimiter + "GAPositionBasedShuffleTotalDistance" + delimiter + "GAPositionBasedShuffleRunningTime"
                + delimiter + "GAPositionBasedInsertionTotalDistance" + delimiter + "GAPositionBasedInsertionRunningTime" + delimiter + "GAPositionBasedDisplacementTotalDistance" + delimiter + "GAPositionBasedDisplacementRunningTime"
                + delimiter + "SShapeTravelledTotalDistance" + delimiter + "SShapeRunningTime" + delimiter + "LargestGapTravelledTotalDistance" + delimiter + "LargestGapRunningTime");
            wr.Close();

            parameters.NumberOfStagnantGeneration = 100;
            parameters.PopulationSize = 450;
            parameters.CrossoverProbability = 0.8f;
            parameters.MutationProbability = 0.08f;
            parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
            parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
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
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
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
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance02 = solution.TravelledDistance;
                double runningTime02 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance03 = solution.TravelledDistance;
                double runningTime03 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PositionBased;
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance04 = solution.TravelledDistance;
                double runningTime04 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PositionBased;
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance05 = solution.TravelledDistance;
                double runningTime05 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PositionBased;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
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
                double runningTime07 = solution.RunningTime;

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

        //Method to test new implemented operators
        public static void testNewMutationOperatorsWithGeneticAlgorithm(String listFilePath, String reportFilePath)
        {
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                listFilePath);
            wr.WriteLine("InstanceNumber" + delimiter + "DistanceMatrixRunningTime"
                + delimiter + "GACycleShuffleTotalDistance" + delimiter + "GACycleShuffleRunningTime" + delimiter + "GACycleInsertionTotalDistance" + delimiter + "GACycleInsertionRunningTime"
                + delimiter + "GACycleDisplacementTotalDistance" + delimiter + "GACycleDisplacementRunningTime" + delimiter + "GAOrderedShuffleTotalDistance" + delimiter + "GAOrderedShuffleRunningTime"
                + delimiter + "GAOrderedInsertionTotalDistance" + delimiter + "GAOrderedInsertionRunningTime" + delimiter + "GAOrderedDisplacementTotalDistance" + delimiter + "GAOrderedDisplacementRunningTime"
                + delimiter + "GAPMXShuffleTotalDistance" + delimiter + "GAPMXShuffleRunningTime" + delimiter + "GAPMXInsertionTotalDistance" + delimiter + "GAPMXInsertionRunningTime"
                + delimiter + "GAPMXDisplacementTotalDistance" + delimiter + "GAPMXDisplacementRunningTime" + delimiter + "GAOX2ShuffleTotalDistance" + delimiter + "GAOX2ShuffleRunningTime"
                + delimiter + "GAOX2InsertionTotalDistance" + delimiter + "GAOX2InsertionRunningTime" + delimiter + "GAOX2DisplacementTotalDistance" + delimiter + "GAOX2DisplacementRunningTime"
                + delimiter + "SShapeTravelledTotalDistance" + delimiter + "SShapeRunningTime" + delimiter + "LargestGapTravelledTotalDistance" + delimiter + "LargestGapRunningTime");
            wr.Close();

            parameters.NumberOfStagnantGeneration = 100;
            parameters.PopulationSize = 450;
            parameters.CrossoverProbability = 0.8f;
            parameters.MutationProbability = 0.08f;
            parameters.CrossoverOperator = PickListGAParameters.Crossover.Cycle;
            parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
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
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
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
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance02 = solution.TravelledDistance;
                double runningTime02 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Cycle;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
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
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance04 = solution.TravelledDistance;
                double runningTime04 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Ordered;
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance05 = solution.TravelledDistance;
                double runningTime05 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.Ordered;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance06 = solution.TravelledDistance;
                double runningTime06 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PMX;
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance07 = solution.TravelledDistance;
                double runningTime07 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PMX;
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance08 = solution.TravelledDistance;
                double runningTime08 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.PMX;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance09 = solution.TravelledDistance;
                double runningTime09 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Shuffle;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance10 = solution.TravelledDistance;
                double runningTime10 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Insertion;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance11 = solution.TravelledDistance;
                double runningTime11 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                parameters.CrossoverOperator = PickListGAParameters.Crossover.OX2;
                parameters.MutationOperator = PickListGAParameters.Mutation.Displacement;
                solution = new Solution(room, picker, parameters);
                solution.DistanceMatrix = calculatedDistanceMatrix;
                solution.PathMatrix = calculatedPathMatrix;
                solution.DistanceMatrixRunningTime = calculatedDistanceMatrixRunningTime;
                solution.solve(Solution.Algorithm.GeneticAlgorithm);

                double travelledTotalDistance12 = solution.TravelledDistance;
                double runningTime12 = solution.RunningTime;
                
                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.SShape);

                double travelledTotalDistance13 = solution.TravelledDistance;
                double runningTime13 = solution.RunningTime;

                room.ItemList = Utils.Clone<Item>(itemList);
                picker = new Picker(depot);
                solution = new Solution(room, picker, parameters);
                solution.solve(Solution.Algorithm.LargestGap);

                double travelledTotalDistance14 = solution.TravelledDistance;
                double runningTime14 = solution.RunningTime;
                
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}" + delimiter + "{10}" + delimiter + "{11}" + delimiter + "{12}" + delimiter + "{13}" + delimiter + "{14}" + delimiter + "{15}" + delimiter + "{16}" + delimiter + "{17}" + delimiter + "{18}" + delimiter + "{19}" + delimiter + "{20}" + delimiter + "{21}" + delimiter + "{22}" + delimiter + "{23}" + delimiter + "{24}" + delimiter + "{25}" + delimiter + "{26}" + delimiter + "{27}" + delimiter + "{28}" + delimiter + "{29}",
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
                    runningTime08,
                    travelledTotalDistance09,
                    runningTime09,
                    travelledTotalDistance10,
                    runningTime10,
                    travelledTotalDistance11,
                    runningTime11,
                    travelledTotalDistance12,
                    runningTime12,
                    travelledTotalDistance13,
                    runningTime13,
                    travelledTotalDistance14,
                    runningTime14);
                Console.ResetColor();

                wr = new StreamWriter(reportFilePath, true);
                wr.WriteLine("{0}" + delimiter + "{1}" + delimiter + "{2}" + delimiter + "{3}" + delimiter + "{4}" + delimiter + "{5}" + delimiter + "{6}" + delimiter + "{7}" + delimiter + "{8}" + delimiter + "{9}" + delimiter + "{10}" + delimiter + "{11}" + delimiter + "{12}" + delimiter + "{13}" + delimiter + "{14}" + delimiter + "{15}" + delimiter + "{16}" + delimiter + "{17}" + delimiter + "{18}" + delimiter + "{19}" + delimiter + "{20}" + delimiter + "{21}" + delimiter + "{22}" + delimiter + "{23}" + delimiter + "{24}" + delimiter + "{25}" + delimiter + "{26}" + delimiter + "{27}" + delimiter + "{28}" + delimiter + "{29}",
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
                    runningTime08,
                    travelledTotalDistance09,
                    runningTime09,
                    travelledTotalDistance10,
                    runningTime10,
                    travelledTotalDistance11,
                    runningTime11,
                    travelledTotalDistance12,
                    runningTime12,
                    travelledTotalDistance13,
                    runningTime13,
                    travelledTotalDistance14,
                    runningTime14);
                wr.Close();

                calculatedDistanceMatrix = null;
                calculatedPathMatrix = null;
                calculatedDistanceMatrixRunningTime = 0;
            }

            wr = new StreamWriter(reportFilePath, true);
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

            parameters.NumberOfStagnantGenerationList = new int[] { 130, 140 };
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

        //Method to setup pcomparison of Genetic Algorithm
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
                "../../../files/testListWithPickListSize025.txt",
                "../../../files/testListWithPickListSize050.txt",
                "../../../files/testListWithPickListSize100.txt"
                //"../../../files/testListWithPickListSize005.txt"
            };

            String comparisonReportFilePath = "../../../files/GeneticAlgorithmComparisonReportNewMutationOperators.txt";

            foreach (string path in itemListFilePaths)
            {
                if (File.Exists(path))
                {
                    //compareAlgorithmsWithGeneticAlgorithm(path, comparisonReportFilePath);
                    //testNewOperatorsWithGeneticAlgorithm(path, comparisonReportFilePath);
                    testNewMutationOperatorsWithGeneticAlgorithm(path, comparisonReportFilePath);
                }
            }
        }

        //Method to setup pcomparison of Tabu Search
        public static void setupTabuAlgorithmComparison(bool generateNewTestLists)
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
                "../../../files/testListWithPickListSize025.txt",
                "../../../files/testListWithPickListSize050.txt",
                "../../../files/testListWithPickListSize100.txt"
                //"../../../files/testListWithPickListSize005.txt"
            };

            String comparisonReportFilePath = "../../../files/TabuAlgorithmComparisonReportAspirationNotAllowed.txt";

            foreach (string path in itemListFilePaths)
            {
                if (File.Exists(path))
                {
                    compareAlgorithmsWithTabu(path, comparisonReportFilePath);
                }
            }
        }

        //Method to setup testing of Shortest Path
        public static void setupShortestPathTest(bool generateNewTestLists)
        {
            String testListPath = "../../../files/testListForShortestPathTestSmall.txt";
            String shortestPathReportFilePath = "../../../files/reportForShortestPathTestSmall.txt";
            int S = 10;
            double W = 7;
            double L = 30;
            double K = 3;
            int no_of_horizontal_aisles = 3;
            int no_of_vertical_aisles = 4;

            depot = new Coordinate(1, no_of_horizontal_aisles);
            room = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);
            parameters = new Parameters();

            if (generateNewTestLists)
            {
                Utils.writeShortestPathItemList(room, testListPath);
            }

            compareShortestPathAlgorithms(testListPath, shortestPathReportFilePath);
        }

        //Method to compare solution algorithms
        public static void compareShortestPathAlgorithms(String testListPath, String reportFilePath)
        {
            String delimiter = "\t";
            StreamWriter wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("{0}" + delimiter + "{1}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                testListPath);
            wr.Close();
            
            parameters.ItemListSet = Utils.readTestList(testListPath);

            //Add one additional Distance Matrix calculation at the beginning to initiate multi-core process and having less values for distance matrix calculation at report
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));
            
            //Old algorithm
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.prepareDistanceMatrix(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));

            double[,]  calculatedDistanceMatrix1 = solution.DistanceMatrix;
            List<Coordinate>[,] calculatedPathMatrix1 = solution.PathMatrix;

            wr = new StreamWriter(reportFilePath, true);
            Console.WriteLine("Writing Distance Matrix using Cplex Algorithm...");
            wr.WriteLine("Prepared Distance Matrix using Cplex Algorithm:");
            for (int i = 0; i < calculatedDistanceMatrix1.GetLength(1); i++)
            {
                wr.Write("i" + i + delimiter);
            }
            wr.WriteLine();
            
            for (int i = 0; i < calculatedDistanceMatrix1.GetLength(0); i++)
            {
                wr.Write("i" + i + delimiter);
                for (int j = 0; j < calculatedDistanceMatrix1.GetLength(1); j++)
                {
                    wr.Write(calculatedDistanceMatrix1[i, j] + "\t");
                }
                wr.WriteLine();
            }
            wr.WriteLine();
            wr.Close();

            wr = new StreamWriter(reportFilePath, true);
            Console.WriteLine("Writing Path Matrix using Cplex Algorithm...");
            wr.WriteLine("Prepared Path Matrix using Cplex Algorithm:");
            for (int i = 0; i < calculatedPathMatrix1.GetLength(0); i++)
            {
                for (int j = 0; j < calculatedPathMatrix1.GetLength(1); j++)
                {
                    wr.WriteLine("Path from (item{0}) to (item{1}):", i, j);
                    wr.WriteLine(ShortestPathSolution.stringOfPath(calculatedPathMatrix1[i, j]));
                }
            }
            wr.WriteLine();
            wr.Close();

            double calculatedDistanceMatrixRunningTime1 = solution.DistanceMatrixRunningTime;

            //Dijkstra algorithm
            room.ItemList = Utils.Clone<Item>(parameters.ItemListSet.ElementAt(0));
            picker = new Picker(depot);
            solution = new Solution(room, picker, parameters);
            solution.prepareDistanceMatrix_With_Dijkstra(new Item(0, solution.Problem.NumberOfCrossAisles - 1, 1, 0, solution.Problem.S));

            double[,] calculatedDistanceMatrix2 = solution.DistanceMatrix;
            List<Coordinate>[,] calculatedPathMatrix2 = solution.PathMatrix;

            wr = new StreamWriter(reportFilePath, true);
            Console.WriteLine("Writing Distance Matrix using Dijkstra Algorithm...");
            wr.WriteLine("Prepared Distance Matrix using Dijkstra Algorithm:");
            for (int i = 0; i < calculatedDistanceMatrix2.GetLength(1); i++)
            {
                wr.Write("i" + i + delimiter);
            }
            wr.WriteLine();

            for (int i = 0; i < calculatedDistanceMatrix2.GetLength(0); i++)
            {
                wr.Write("i" + i + delimiter);
                for (int j = 0; j < calculatedDistanceMatrix2.GetLength(1); j++)
                {
                    wr.Write(calculatedDistanceMatrix2[i, j] + "\t");
                }
                wr.WriteLine();
            }
            wr.WriteLine();
            wr.Close();

            wr = new StreamWriter(reportFilePath, true);
            Console.WriteLine("Writing Path Matrix using Dijkstra Algorithm...");
            wr.WriteLine("Prepared Path Matrix using Dijkstra Algorithm:");
            for (int i = 0; i < calculatedPathMatrix2.GetLength(0); i++)
            {
                for (int j = 0; j < calculatedPathMatrix2.GetLength(1); j++)
                {
                    wr.WriteLine("Path from (item{0}) to (item{1}):", i, j);
                    wr.WriteLine(ShortestPathSolution.stringOfPath(calculatedPathMatrix2[i, j]));
                }
            }
            wr.WriteLine();
            wr.Close();

            double calculatedDistanceMatrixRunningTime2 = solution.DistanceMatrixRunningTime;

            wr = new StreamWriter(reportFilePath, true);
            wr.WriteLine("Running Time for Shortest Path Algorithms:");
            wr.WriteLine("Cplex" + delimiter + "Dijkstra");
            wr.WriteLine("{0}" + delimiter + "{1}",
                    calculatedDistanceMatrixRunningTime1,
                    calculatedDistanceMatrixRunningTime2);
            wr.Close();
            Console.WriteLine("Running Time for Shortest Path Algorithms:");
            Console.WriteLine("Cplex" + delimiter + "Dijkstra");
            Console.WriteLine("{0}" + delimiter + "{1}",
                    calculatedDistanceMatrixRunningTime1,
                    calculatedDistanceMatrixRunningTime2);
        }

        public static void Main(string[] args)
        {
            //Test.runTestCases();
            //runRealWorldChallenge();
            //setupTabuSearchParameterTuning(true);//false yaparsan generate etmez
            //String listFilePath = "../../../files/testListOfSize025ForAlgorithmComparison.txt";
            //String listFilePath = "../../../files/testListWithPickListSize005.txt";
            //String reportFilePath = "../../../files/AlgorithmComparisonReport.txt";
            //compareAlgorithmsWithTabu(listFilePath, reportFilePath);
            //setupGeneticAlgorithmParameterTuning(false);
            setupGeneticAlgorithmComparison(false);
            //setupShortestPathTest(false);
            //setupTabuAlgorithmComparison(false);
            Console.ReadLine();
        }
    }
}