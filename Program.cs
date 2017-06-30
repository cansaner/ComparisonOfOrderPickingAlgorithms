using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Program
    {
        //Enumerations
        enum Methods { TabuSearch, SShape, LargestGap };
        enum AislePart { All, Rear, Front };

        public static List<List<Item>> readTestList(String filepath)
        {
            List<List<Item>> listOfLists = new List<List<Item>>();
            List<Item> itemList = new List<Item>();

            if (System.IO.File.Exists(filepath))
            {
                StreamReader re = new StreamReader(File.OpenRead(filepath));
                String line = String.Empty;

                while ((line = re.ReadLine()) != null)
                {
                    if (line.Equals(String.Empty))
                    {
                        listOfLists.Add(itemList);
                        itemList = new List<Item>();
                        continue;
                    }
                    string[] values = line.Split(',');
                    Item item = new Item(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
                    itemList.Add(item);
                }
                re.Close();

                //foreach (List<Item> itemL in listOfLists)
                //    foreach (Item item in itemL)
                //        Console.WriteLine("ITEM NO:{0} - A={1}, B={2}, C={3}, D={4}", item.Index, item.AInfo, item.BInfo, item.CInfo, item.DInfo);
            }
            else
                Console.WriteLine("NO FILE to parse!");

            return listOfLists;
        }

        public static void writeRandomItemList(Problem problem, int sizeOfList, String filepath, Random rand)
        {
            List<Item> itemList = new List<Item>();
            StreamWriter wr = new StreamWriter(filepath, true);

            for (int i = 0; i < sizeOfList; i++)
            {
                Item j = new Item();
                j.Index = i + 1;
                j.AInfo = rand.Next(1, problem.NumberOfCrossAisles); //inclusive lower bound & exclusive upper bound
                j.BInfo = rand.Next(1, problem.NumberOfAisles);
                j.CInfo = rand.Next(0, 2); //(0,1): 1 is exclusive
                j.DInfo = rand.Next(1, problem.S + 1);
                itemList.Add(j);
                wr.WriteLine("{0},{1},{2},{3},{4}", j.Index, j.AInfo, j.BInfo, j.CInfo, j.DInfo);
            }

            wr.WriteLine("");
            wr.Close();
        }

        public static void generateTestLists(Problem problem, int[] sizeOfLists, int numberOfLists)
        {
            String filepath;
            String fileNumberZeros;
            Random rand = new Random();

            for (int i = 0; i < sizeOfLists.Length; i++)
            {
                if (sizeOfLists[i] < 1 || sizeOfLists[i] > 999)
                    continue;
                fileNumberZeros = (sizeOfLists[i] < 10) ? "00" : ((sizeOfLists[i] < 100) ? "0" : "");
                filepath = "C:\\masterTez/testListWithSize" + fileNumberZeros + sizeOfLists[i] + ".txt";
                for (int j = 0; j < numberOfLists; j++)
                {
                    writeRandomItemList(problem, sizeOfLists[i], filepath, rand);
                }
                Console.WriteLine("Finished: {0}", i);
            }
        }

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
                        solution.solve(Solution.Methods.TabuSearch);
                        Console.WriteLine("{0}:{1}:{2}:{3}", parameters.TabuLength, parameters.NumberOfIterations, solution.TravelledDistance, solution.RunningTime);
                        wr.WriteLine("{0}:{1}:{2}:{3}", parameters.TabuLength, parameters.NumberOfIterations, solution.TravelledDistance, solution.RunningTime);
                    }
                }
                wr.WriteLine("");
            }

            wr.Close();
        }

        public static void runTestCases()
        {
            Parameters parameters = new Parameters();
            Picker picker;
            Coordinate depot = new Coordinate(1, 4);
            Problem room1;
            Solution solution1, solution2, solution3, solution4, solution5, solution6;

            //Setting parameters of the basic challenge
            int S = 7;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 8;

            room1 = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);

            picker = new Picker(depot);
            parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            room1.ItemList = parameters.ItemListSet.ElementAt(0);
            solution1 = new Solution(room1, picker, parameters);
            solution1.solve(Solution.Methods.SShape);
            ////assert here
            
            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution2 = new Solution(room1, picker, parameters);
            //solution2.solve(Solution.Methods.LargestGap);
            ////assert here

            S = 10;
            no_of_horizontal_aisles = 4;
            no_of_vertical_aisles = 12;

            //room1 = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase01.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution3 = new Solution(room1, picker, parameters);
            //solution3.solve(Solution.Methods.SShape);
            ////assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase01.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution4 = new Solution(room1, picker, parameters);
            //solution4.solve(Solution.Methods.LargestGap);
            //assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase02.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.SShape);
            ////assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase02.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.LargestGap);
            ////assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase03.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.SShape);
            //assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase03.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.LargestGap);
            ////assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/SStestCase04.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution6 = new Solution(room1, picker, parameters);
            //solution6.solve(Solution.Methods.SShape);
            //assert here
        }

        public static void runRealWorldChallenge()
        {
            Parameters parameters = new Parameters();
            Picker picker;
            Coordinate depot = new Coordinate(1, 4);
            Problem room1;
            Solution solution1;

            int S = 10;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 31;

            room1 = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);

            parameters.TabuLength = 5;
            parameters.NumberOfIterations = 20;
            picker = new Picker(depot);
            parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New5.txt");
            room1.ItemList = parameters.ItemListSet.ElementAt(0);
            solution1 = new Solution(room1, picker, parameters);
            solution1.solve(Solution.Methods.TabuSearch);

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.SShape);
            //assert here

            //picker = new Picker(depot);
            //parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            //room1.ItemList = parameters.ItemListSet.ElementAt(0);
            //solution5 = new Solution(room1, picker, parameters);
            //solution5.solve(Solution.Methods.LargestGap);
            ////assert here
        }

        public static void Main(string[] args)
        {
            runTestCases();
            //TABU SEARCH BUNUN ICINDE
            //runRealWorldChallenge();
            
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
            //generateTestLists(room1, parameters.SizeOfLists, parameters.NumberOfLists);

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