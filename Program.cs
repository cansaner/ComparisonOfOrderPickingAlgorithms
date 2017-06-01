using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    struct PickerPosition
    {
        public int aPos;
        public int bPos;
    };

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

        public static void Main(string[] args)
        {
            //setting the logistics challenge
            Picker picker;
            Coordinate depot = new Coordinate(1, 4);

            //Setting parameters of the basic challenge
            int S = 7;
            double W = 2.6;
            double L = 30.4;
            double K = 2.77;
            int no_of_horizontal_aisles = 4;
            int no_of_vertical_aisles = 8;
            
            //Setting parameters of the real world challenge
            //int S = 10;
            //double W = 2.6;
            //double L = 30.4;
            //double K = 2.77;
            //int no_of_horizontal_aisles = 4;
            //int no_of_vertical_aisles = 31;

            Problem room1 = new Problem(S, W, L, K, no_of_horizontal_aisles - 1, no_of_vertical_aisles, depot);
            
            
            //room1.prepareItemList(true, "C:\\masterTez/sshape1New.txt", 0);
            //room1.prepareItemList(false, "C:\\masterTez/cansaner.txt", 5);
            //List<Item> itemList = room1.ItemList;

            Parameters parameters = new Parameters();
            //S_SHAPE
            picker = new Picker(depot);
            parameters.ItemListSet = readTestList("C:\\masterTez/sshape1New.txt");
            room1.ItemList = parameters.ItemListSet.ElementAt(0);
            Solution solution = new Solution(room1, picker, parameters);
            solution.solve(Solution.Methods.SShape);
            
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

        public static int getMinOfArray(int[] arr, int max)
        {
            int minVal = max;

            foreach (int i in arr)
            {
                if (i < minVal)
                {
                    minVal = i;
                }

            }
            return minVal;
        }

        /* CANSANER COMMENT OUT //I DON'T KNOW WHY THESE FUNCTIONS EXIST!!!
        public static void reWriteData(int size_of_picklist, List<Item> ITEMLIST) {
            //Writing items on another text file
            string FN = "PICKLIST-SIZE" + size_of_picklist + ".txt";
            TextWriter WriterSolution = new StreamWriter(FN);
            WriterSolution.WriteLine();

            WriterSolution.WriteLine("\tITEMNO\tA\tB\tC\tD");
            WriterSolution.WriteLine();

            foreach (Item itm in ITEMLIST)
            {
                WriterSolution.WriteLine("\t" + (itm.index + 1) + "\t" + itm.A_info + "\t" + itm.B_info + "\t" + itm.C_info + "\t" + itm.D_info);
            }

            WriterSolution.Close();

            Console.WriteLine("No of items:{0}", ITEMLIST.Count());
            //initial solution generated!
        }
        
        public static void findMaximumGap(int aPos, int bPos)
        {
            List<Item> aisleItems = getAisleItems(aPos, bPos);
            List<Item> sortedAisleItems = sortAisleItems(aisleItems);
            
            
        }
        
        public static List<Item> getAisleItems(int aPos, int bPos)
        {
            List<Item> AISLE_ITEMS = new List<Item>();
            foreach (Item i in ITEMLIST)
            {
                if (i.A_info == aPos)
                {
                    if ((i.B_info == bPos && i.C_info == 0) || (i.B_info == bPos - 1 && i.C_info == 1))
                    {
                        AISLE_ITEMS.Add(i);
                    }
                }
            }
            return AISLE_ITEMS;
        }

         * 
         * 
         * //public static int SShapeHeuristic() //http://www.roodbergen.com/whopt/
        //{
        //    bool main_aisle_found = false;
        //    int vertical_aisle_for_main_aisle=0;
        //    int index_of_last_picked_item=0;

        //    foreach (Item iii in ITEMLIST)
        //        iii.picked_during_sshape = false;
            
        //    foreach (Item iii in ITEMLIST)
        //        for(int b=1; b<no_of_vertical_aisles-1; b++)
        //            for(int c=0; c<2; c++)
        //                if (iii.B_info == b && iii.C_info == c && main_aisle_found == false)
        //                {
        //                    main_aisle_found = true;
        //                    if (c == 0)
        //                        vertical_aisle_for_main_aisle = iii.B_info;
        //                    if (c == 1)
        //                        vertical_aisle_for_main_aisle = iii.B_info + 1;
        //                }

        //    Console.WriteLine("MAIN VERTICAL AISLE:{0}", vertical_aisle_for_main_aisle);

        //    if (main_aisle_found == true) // && SSHAPE_ITEMS.Count()!=0
        //    {                
        //        for (int a = no_of_horizontal_aisles - 1; a > 0; a--)
        //            for (int d = S; d > 0; d--)
        //                foreach (Item iii in ITEMLIST) 
        //                    if ((iii.A_info == a && iii.B_info == vertical_aisle_for_main_aisle - 1 && iii.C_info == 1 && iii.D_info == d) ||
        //                    (iii.A_info == a && iii.B_info == vertical_aisle_for_main_aisle && iii.C_info == 0 && iii.D_info == d))
        //                    {
        //                        Console.WriteLine("ITEM {0} IS PICKED!", iii.index + 1);
        //                        index_of_last_picked_item = iii.index;
        //                        iii.picked_during_sshape = true;
        //                        SSHAPE_ITEMS.Remove(iii); 
        //                    }
        //    }//end of if

        //    for (int b = ITEMLIST[index_of_last_picked_item].B_info; b < no_of_horizontal_aisles; b++)
        //        for (int d = 1; d <= S; d++)
        //            foreach (Item iii in ITEMLIST)
        //                if (iii.picked_during_sshape == false && SSHAPE_ITEMS.Count() != 0)
        //                    if ((iii.A_info == 1 && iii.B_info == b && iii.C_info == 1 && iii.D_info == d) ||
        //                    (iii.A_info == 1 && iii.B_info == b + 1 && iii.C_info == 0 && iii.D_info == d))
        //                    {
        //                        Console.WriteLine("ITEM {0} IS PICKED!", iii.index + 1);
        //                        index_of_last_picked_item = iii.index;
        //                        iii.picked_during_sshape = true;
        //                        SSHAPE_ITEMS.Remove(iii);
        //                    }

        //    for (int b = ITEMLIST[index_of_last_picked_item].B_info; b < no_of_horizontal_aisles; b++)
        //        for (int d = S; d > 0; d--)
        //            foreach (Item iii in ITEMLIST)
        //                if (iii.picked_during_sshape == false && SSHAPE_ITEMS.Count() != 0)
        //                    if ((iii.A_info == 1 && iii.B_info == b && iii.C_info == 1 && iii.D_info == d) ||
        //                    (iii.A_info == 1 && iii.B_info == b + 1 && iii.C_info == 0 && iii.D_info == d))
        //                    {
        //                        Console.WriteLine("ITEM {0} IS PICKED!", iii.index + 1);
        //                        index_of_last_picked_item = iii.index;
        //                        iii.picked_during_sshape = true;
        //                        SSHAPE_ITEMS.Remove(iii);
        //                    }     

        //    return travelled_distance_with_SSHAPEHEURISTIC;
        //}
        CANSANER COMMENT OUT */
    }
}