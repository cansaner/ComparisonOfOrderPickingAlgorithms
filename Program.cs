using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
//using namespaces:
//using TBLST;
//using TBSRCH;
//using TSPENVRNMNT;
using ILOG.Concert;
using ILOG.CPLEX;
//using ITEMDEFINITION;

namespace ComparisonOfOrderPickingAlgorithms
{
    struct PickerPosition
    {
        public int aPos;
        public int bPos;
    };

    public class Program
    {
        //Parameters
        public static int tabuLength = 5;

        

        //static int S = 10 ; //no of shelves
        //static double W = 2.6 ; //width of corridor: W=1.2 & W'=1.2+0.7+0.7=2.6 
        //static double L = 30.4 ; //length of corridor: L=27.7 & L'=27.7+1.35+1.35=30.4
        //static double K = 2.77 ; //length of one shelf

        //static int no_of_horizontal_aisles = 4;
        //static int no_of_vertical_aisles = 8;

        enum Methods { TabuSearch, SShape, LargestGap };
        enum AislePart { All, Rear, Front };
        static PickerPosition pickerPosition;
        static double totalDistance = 0;
        
        static int A;  //horizontal block of the initial item   
        static int B; //vertical block of the initial item 
        static int C; //side on the block for the initial item (0 for Left; 1 for Right)
        static int D; //shelf no of the initial item counted from the upper corridor 
        static int APRIME; //horizontal block of the final item 
        static int BPRIME; //vertical block of the final item  
        static int CPRIME; //side on the block for the final item 
        static int DPRIME; //shelf no of the final item counted from the upper corridor 
        
        static int[] arrayA; // an array to hold coordinate A of all items
        static int[] arrayB; // an array to hold coordinate B of all items
        static int[] arrayC; // an array to hold coordinate C of all items
        static int[] arrayD; // an array to hold coordinate D of all items

        static double DISTANCE; 
   
        static int MAX_A = no_of_horizontal_aisles-1;
        static int MAX_B = no_of_vertical_aisles-1;
        //static int MAX_C = 1;
        static int MAX_D = S;

        static int IND1;
        static int IND2;
        static int IND3;
        static int IND4;
  
        static Cplex cplex;
        static INumVar[, , ,] X;
        static IObjective obj;
        static ILinearNumExpr objective;

        static TSPEnvironment tspEnvironmentWAREHOUSE;

        static List<Item> ITEMLIST;
        static List<Item> SORTED_ITEMS;
        //static List<Item> SSHAPE_ITEMS;


        static Random rand;

        static int size_of_picklist;
        //static int travelled_distance_with_SSHAPEHEURISTIC;
        //int index_of_last_picked_item;

        
        public static void Main(string[] args)
        {
            //setting the logistics challenge
            Problem room1 = new Problem(10, 2.6, 30.4, 2.77, 3, 8);
            Coordinate depot = new Coordinate(1, 4);
            Picker picker = new Picker(depot);

            //ITEMLIST.Clear(); 

            size_of_picklist = 0; // 22; ITEM'LARI RANDOM ILE GENERATE EDERKEN EN BAŞTA SAYI VERİLMELİ!

            Reading("C:\\masterTez/sshape.txt"); //GenerateItems(); name of the function can be changed if not read

            //Generating();
            
            //reWriteData();
            generateInitialSolution();

            populateHelperArrays();

            //createDistanceMatrix();

            solve((int)Methods.TabuSearch);
            //solve((int)Methods.SShape);
            //solve((int)Methods.LargestGap);

            Console.ReadLine();
        }//end of Main

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

        public static void generateInitialSolution() {
            //initial solution generation:B->C->A->D
            SORTED_ITEMS = new List<Item>(); //Copying ITEMLIST to another List for Manipulation Purposes

            foreach (Item jj in ITEMLIST)
            {
                SORTED_ITEMS.Add(jj);
            }

            SORTED_ITEMS.Sort(); //comparing items based on coordinates (B->C->A->D)

            foreach (Item iii in SORTED_ITEMS)
                Console.WriteLine("SORTED LIST: {0}", iii.index + 1);
        }

        public static void createDistanceMatrix() {
            tspEnvironmentWAREHOUSE = new TSPEnvironment();

            tspEnvironmentWAREHOUSE.distances = new int[ITEMLIST.Count(), ITEMLIST.Count()];

            for (int i = 1; i <= tspEnvironmentWAREHOUSE.distances.GetLength(0); i++)
            {
                for (int j = 1; j <= tspEnvironmentWAREHOUSE.distances.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        tspEnvironmentWAREHOUSE.distances[i - 1, j - 1] = 0;
                    }
                    else
                    {
                        Console.WriteLine("SOLVING SHORTEST PATH FOR ITEM {0} TO ITEM {1}", i, j);
                        A = arrayA[i - 1]; //ITEMLIST[i - 1].A_info;
                        B = arrayB[i - 1];
                        C = arrayC[i - 1];
                        D = arrayD[i - 1];
                        APRIME = arrayA[j - 1];
                        BPRIME = arrayB[j - 1];
                        CPRIME = arrayC[j - 1];
                        DPRIME = arrayD[j - 1];
                        Console.WriteLine("A={0}, B={1}, C={2}, D={3}, APRIME={4}, BPRIME={5}, CPRIME={6}, DPRIME={7}", A, B, C, D, APRIME, BPRIME, CPRIME, DPRIME);
                        tspEnvironmentWAREHOUSE.distances[i - 1, j - 1] = Solve_Shortest_Path(A, B, C, D, APRIME, BPRIME, CPRIME, DPRIME);
                        //Assignments(); BUNU CPLEX.NULL'DAN ÖNCE YAZMAK GEREK!!!
                    }
                }
                //next row
                Console.WriteLine();
            }


            //Writing distance matrix based on shortest path between items
            for (int i = 0; i < tspEnvironmentWAREHOUSE.distances.GetLength(0); i++)
            {
                for (int j = 0; j < tspEnvironmentWAREHOUSE.distances.GetLength(1); j++)
                {
                    Console.Write(tspEnvironmentWAREHOUSE.distances[i, j] + "\t");
                }
                //next row
                Console.WriteLine();
            }
        }

        public static void populateHelperArrays() {
            arrayA = new int[ITEMLIST.Count()];
            arrayB = new int[ITEMLIST.Count()];
            arrayC = new int[ITEMLIST.Count()];
            arrayD = new int[ITEMLIST.Count()];

            foreach (Item jjj in ITEMLIST)
            {
                arrayA[jjj.index] = jjj.A_info;
                arrayB[jjj.index] = jjj.B_info;
                arrayC[jjj.index] = jjj.C_info;
                arrayD[jjj.index] = jjj.D_info;
                //Console.WriteLine("FIRST ELEMENTS: {0}, {1}, {2}, {3}", arrayA[jjj.index], arrayB[jjj.index], arrayC[jjj.index], arrayD[jjj.index]);
            }
        }

        public static void reWriteData() {
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

        public static void solve(int method) {
            DateTime stTime = DateTime.Now; //BURADA YAZARSAK DISTANCE MATRIX HESAPLANMASI İÇİN GEREKLİ SÜREYİ DE KAPSIYOR!
            switch (method)
            {
                case (int)Methods.TabuSearch:
                    solveUsingTabuSearch();
                    break;
                case (int)Methods.SShape:
                    solveUsingSShapeHeuristic();
                    break;
                case (int)Methods.LargestGap:
                    solveUsingLargestGap();
                    break;
                default:
                    solveUsingTabuSearch();
                    break;
            }
            DateTime etTime = DateTime.Now;
            TimeSpan elapsed_Time = etTime.Subtract(stTime);
            double elapsedTime = Math.Round((elapsed_Time).TotalSeconds, 3);
            switch (method) {
                case (int)Methods.TabuSearch:
                    Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", elapsedTime);
                    break;
                case (int)Methods.SShape:
                    Console.WriteLine("S-SHAPE RUNNING TIME: {0} Seconds", elapsedTime);
                    break;
                default:
                    Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", elapsedTime);
                    break;
            }
        }

        public static void solveUsingTabuSearch() {
            //if initial solution is to be generated through B->C->A->D:
            //int[] currSolution = new int[SORTED_ITEMS.Count() + 1];
            //currSolution[0] = 0;
            //currSolution[SORTED_ITEMS.Count()] = 0;
            int[] currSolution = new int[SORTED_ITEMS.Count()];

            //for (int i = 1; i < SORTED_ITEMS.Count(); i++)
            for (int i = 0; i < SORTED_ITEMS.Count(); i++)
                {
                currSolution[i] = SORTED_ITEMS[i].index;
            }

            for (int i = 0; i < SORTED_ITEMS.Count(); i++)
            {
                Console.WriteLine("CURRENT SOLUTION: {0}", currSolution[i]);
            }

            //TABU SEARCH
            int numberOfIterations = 20; //Stopping criterion
            //int tabuLength = 5; //Tabu Tenure
            //TabuList tabuList = new TabuList(tabuLength); //Why tabuLength??? but not numCities??? because when swapping city 1&2, taking (1,2) and (2,1) i.e. 2 moves?
            TabuList tabuList = new TabuList(tspEnvironmentWAREHOUSE.distances.GetLength(0));

            int[] bestSol = new int[currSolution.GetLength(0)]; //this is the best Solution So Far
            Array.Copy(currSolution, 0, bestSol, 0, bestSol.GetLength(0));
            int bestCost = tspEnvironmentWAREHOUSE.getObjectiveFunctionValue(bestSol);
            //***
            Console.WriteLine("BEST COST:{0}", bestCost);

            for (int i = 0; i < numberOfIterations; i++)
            { //perform iterations here

                //***
                Console.WriteLine("ITERATION NO: {0}", i + 1);

                currSolution = TabuSearch.getBestNeighbour(tabuList, tspEnvironmentWAREHOUSE, currSolution);
                TabuSearch.printSolution(currSolution);
                //TabuSearch.printSolution(tabuList);
                for (int row = 0; row < tabuList.tabuList.GetLength(1); row++)
                {
                    for (int col = 0; col < tabuList.tabuList.GetLength(0); col++)
                        Console.Write(String.Format("{0}\t", tabuList.tabuList[row, col]));
                    Console.WriteLine();
                }
                Console.WriteLine();

                int currCost = tspEnvironmentWAREHOUSE.getObjectiveFunctionValue(currSolution);
                //***
                Console.WriteLine("CURRENT COST:{0}", currCost);

                //Console.WriteLine("Current best cost = " + tspEnvironment.getObjectiveFunctionValue(currSolution));

                if (currCost < bestCost)
                {
                    Array.Copy(currSolution, 0, bestSol, 0, bestSol.GetLength(0));
                    bestCost = currCost;
                    //***
                    Console.WriteLine("CURRENT COST IS LESS THAN BEST COST");
                }
            }

            //burda bestCost'a P/D pointten ilk item (bestSol'un ilk elemanı)'a gelme mesafesi + son item  (bestSol'un son elemanı)'dan P/D pointe gitme mesafesi eklenmeli!!!
            Console.WriteLine("\n\nSearch done! \nBest Solution cost found = " + bestCost + "\nBest Solution :");

            TabuSearch.printSolution(bestSol);
        }

        public static void solveUsingLargestGap() {
            //Setting Picker to starting Point
            int depotAPos = MAX_A + 1;
            int depotBPos = 1;
            pickerPosition.aPos = depotAPos;
            pickerPosition.bPos = depotBPos;
            int farthestBlockA = determineFarthestBlock();
            int leftPickAisleB = findLeftPickAisle(0);
            Console.WriteLine("LEFT PICK AISLE: {0}", leftPickAisleB);
            Console.WriteLine("FARTHEST BLOCK: {0}", farthestBlockA);
            printLocation();
            totalDistance = totalDistance + (Math.Abs(leftPickAisleB - pickerPosition.bPos) * W);
            Console.WriteLine("TRAVELLED DISTANCE {0}W", Math.Abs(leftPickAisleB - pickerPosition.bPos));
            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
            goToLocation(pickerPosition.aPos, leftPickAisleB);
            goVertical(farthestBlockA + 1);
            printLocation();
            List<int> pickAisles = getPickAislesOfBlock(farthestBlockA);
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                collectAisle(pickerPosition.aPos - 1, pickerPosition.bPos, true, true, (int)AislePart.All);
            }
            else
            {
                goVertical(farthestBlockA);
            }
            int farMostBlock = farthestBlockA;
            printLocation();
            while (farMostBlock < depotAPos)
            {
                pickAisles = getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count() > 0)
                {
                    int leftMostSubAisleB = pickAisles.ElementAt(0);
                    int lastSubAisleB = pickAisles.ElementAt(pickAisles.Count() - 1);
                    List<int> rearPickAisles = filterPickAislesOfBlock(farMostBlock, pickAisles, (int)AislePart.Rear);
                    List<int> frontPickAisles = filterPickAislesOfBlock(farMostBlock, pickAisles, (int)AislePart.Front);

                    totalDistance = totalDistance + (Math.Abs(leftMostSubAisleB - pickerPosition.bPos) * W);
                    Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(leftMostSubAisleB - pickerPosition.bPos)));
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                    pickerPosition.bPos = leftMostSubAisleB;
                    printLocation();
                    if (frontPickAisles.Count() > 1)
                    {
                        frontPickAisles.Reverse();
                    }

                    for (int i = 0; i < rearPickAisles.Count(); i++)
                    {
                        if (rearPickAisles.ElementAt(i) != lastSubAisleB)
                        {
                            printLocation();
                            totalDistance = totalDistance + (Math.Abs(rearPickAisles.ElementAt(i) - pickerPosition.bPos) * W);
                            Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(rearPickAisles.ElementAt(i) - pickerPosition.bPos)));
                            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                            pickerPosition.bPos = rearPickAisles.ElementAt(i);
                            collectAisle(farMostBlock, pickerPosition.bPos, false, true, (int)AislePart.Rear);
                            printLocation();
                        }
                    }
                    totalDistance = totalDistance + (Math.Abs(lastSubAisleB - pickerPosition.bPos) * W);
                    Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(lastSubAisleB - pickerPosition.bPos)));
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                    pickerPosition.bPos = lastSubAisleB;
                    printLocation();

                    goVertical(farMostBlock + 1);
                    for (int i = 0; i < frontPickAisles.Count(); i++)
                    {
                        if (frontPickAisles.ElementAt(i) != lastSubAisleB)
                        {
                            printLocation();
                            totalDistance = totalDistance + (Math.Abs(frontPickAisles.ElementAt(i) - pickerPosition.bPos) * W);
                            Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(frontPickAisles.ElementAt(i) - pickerPosition.bPos)));
                            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                            pickerPosition.bPos = frontPickAisles.ElementAt(i);
                            collectAisle(farMostBlock, pickerPosition.bPos, true, true, (int)AislePart.Front);
                            printLocation();
                        }
                    }
                }
                printLocation();
                farMostBlock++;
            }
            totalDistance = totalDistance + (Math.Abs(depotBPos - pickerPosition.bPos) * W);
            Console.WriteLine("TRAVELLED DISTANCE {0}W", Math.Abs(depotBPos - pickerPosition.bPos));
            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
            pickerPosition.aPos = depotAPos;
            pickerPosition.bPos = depotBPos;
            printLocation();
            Console.WriteLine("PICKER IS FINISHED ITS JOB");
        }

        public static void solveUsingSShapeHeuristic() {
            //Setting Picker to starting Point
            int depotAPos = MAX_A + 1;
            int depotBPos = 1;
            pickerPosition.aPos = depotAPos;
            pickerPosition.bPos = depotBPos;
            int farthestBlockA = determineFarthestBlock();
            int leftPickAisleB = findLeftPickAisle(farthestBlockA);
            Console.WriteLine("LEFT PICK AISLE: {0}", leftPickAisleB);
            Console.WriteLine("FARTHEST BLOCK: {0}", farthestBlockA);
            printLocation();
            totalDistance = totalDistance + (Math.Abs(leftPickAisleB - pickerPosition.bPos) * W);
            Console.WriteLine("TRAVELLED DISTANCE {0}W", Math.Abs(leftPickAisleB - pickerPosition.bPos));
            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
            goToLocation(pickerPosition.aPos, leftPickAisleB);
            goVertical(farthestBlockA + 1);
            printLocation();
            List<int> pickAisles = getPickAislesOfBlock(farthestBlockA);
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                collectAisle(pickerPosition.aPos - 1, pickerPosition.bPos, true, true, (int)AislePart.All);
            }
            else
            {
                goVertical(farthestBlockA);
            }
            int farMostBlock = farthestBlockA;
            printLocation();
            bool goRight = true;
            bool goUp = false;
            while (farMostBlock < depotAPos)
            {
                pickAisles = getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count() > 0)
                {
                    int leftMostSubAisleB = pickAisles.ElementAt(0);
                    int rightMostSubAisleB = pickAisles.ElementAt(pickAisles.Count() - 1);
                    //Console.WriteLine("LEFT MOST SUB AISLE: {0}", leftMostSubAisleB);
                    //Console.WriteLine("RIGHT MOST SUB AISLE: {0}", rightMostSubAisleB);
                    if (Math.Abs(pickerPosition.bPos - leftMostSubAisleB) < Math.Abs(pickerPosition.bPos - rightMostSubAisleB))
                    {
                        Console.WriteLine("LEFT MOST SUB AISLE IS SELECTED");
                        totalDistance = totalDistance + (Math.Abs(leftMostSubAisleB - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(leftMostSubAisleB - pickerPosition.bPos)));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = leftMostSubAisleB;
                        goRight = true;
                    }
                    else
                    {
                        Console.WriteLine("RIGHT MOST SUB AISLE IS SELECTED");
                        totalDistance = totalDistance + (Math.Abs(rightMostSubAisleB - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(rightMostSubAisleB - pickerPosition.bPos)));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = rightMostSubAisleB;
                        goRight = false;
                    }
                    printLocation();
                    goVertical(farMostBlock + 1);
                    goUp = true;
                    printLocation();
                }
                pickAisles = getPickAislesOfBlock(farMostBlock);
                while (pickAisles.Count() > 1)
                {
                    if (goRight)
                    {
                        totalDistance = totalDistance + (Math.Abs(pickAisles.ElementAt(0) - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(pickAisles.ElementAt(0) - pickerPosition.bPos)));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = pickAisles.ElementAt(0);
                    }
                    else
                    {
                        totalDistance = totalDistance + (Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - pickerPosition.bPos)));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = pickAisles.ElementAt(pickAisles.Count() - 1);
                    }
                    printLocation();
                    if (goUp)
                    {
                        goVertical(farMostBlock);
                        goUp = false;
                    }
                    else
                    {
                        goVertical(farMostBlock + 1);
                        goUp = true;
                    }
                    pickAisles = getPickAislesOfBlock(farMostBlock);
                }
                if (pickAisles.Count() > 0)
                {
                    if (goRight)
                    {
                        totalDistance = totalDistance + (Math.Abs(pickAisles.ElementAt(0) - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", (Math.Abs(pickAisles.ElementAt(0) - pickerPosition.bPos)));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = pickAisles.ElementAt(0);
                    }
                    else
                    {
                        totalDistance = totalDistance + (Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - pickerPosition.bPos) * W);
                        Console.WriteLine("TRAVELLED DISTANCE {0}W", Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - pickerPosition.bPos));
                        Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                        pickerPosition.bPos = pickAisles.ElementAt(pickAisles.Count() - 1);
                    }
                }
                else { 
                  //Step 4.b should be written
                }
                
                printLocation();
                if (goUp)
                {
                    collectAisle(pickerPosition.aPos - 1, pickerPosition.bPos, true, true, (int)AislePart.All);
                }
                else
                {
                    goVertical(farMostBlock + 1);
                }
                printLocation();
                farMostBlock++;
            }
            totalDistance = totalDistance + (Math.Abs(depotBPos - pickerPosition.bPos) * W);
            Console.WriteLine("TRAVELLED DISTANCE {0}W", Math.Abs(depotBPos - pickerPosition.bPos));
            Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
            pickerPosition.aPos = depotAPos;
            pickerPosition.bPos = depotBPos;
            printLocation();
            Console.WriteLine("PICKER IS FINISHED ITS JOB");
        }

        public static void printLocation()
        {
            Console.WriteLine("Picker is on location: {0}, {1}", pickerPosition.aPos, pickerPosition.bPos);
        }

        public static void printDistance()
        {
            Console.WriteLine("Picker travelled: {0}m", totalDistance);
        }

        public static void goToLocation(int aLoc, int bLoc) {
            pickerPosition.aPos = aLoc;
            pickerPosition.bPos = bLoc;
            printLocation();
        }

        public static void goVertical(int stop)
        {
            int start = pickerPosition.aPos;
            if (start < stop)
            {
                Console.WriteLine("GOING DOWN");
                for (int a = start; a < stop; a = a + 1)
                {
                    Console.WriteLine("GOING FROM {0},{1} TO {2},{3}", a, pickerPosition.bPos, a + 1, pickerPosition.bPos);
                    collectAisle(a, pickerPosition.bPos, false, false, (int)AislePart.All);
                    pickerPosition.aPos = a + 1;
                    totalDistance = totalDistance + L;
                    Console.WriteLine("TRAVELLED DISTANCE 1L");
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                }
            }
            else
            {
                Console.WriteLine("GOING UP");
                for (int a = start; a > stop; a = a - 1)
                {
                    Console.WriteLine("GOING FROM {0},{1} TO {2},{3}", a, pickerPosition.bPos, a - 1, pickerPosition.bPos);
                    collectAisle(a - 1, pickerPosition.bPos, true, false, (int)AislePart.All);
                    pickerPosition.aPos = a - 1;
                    totalDistance = totalDistance + L;
                    Console.WriteLine("TRAVELLED DISTANCE 1L");
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                }
            }
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

        public static List<Item> sortAisleItems(List<Item> aisleItems) {
            if (aisleItems.Count == 0) {
                return aisleItems;
            }
            List<Item> sortedItems = new List<Item>();
            sortedItems.Add(aisleItems.ElementAt(0));
            
            for (int i = 1; i < aisleItems.Count; i++)
            {
                bool inserted = false;
                for (int j = 0; j < sortedItems.Count && inserted == false; j++)
                {
                    if (aisleItems.ElementAt(i).D_info <= sortedItems.ElementAt(j).D_info)
                    {
                        sortedItems.Insert(j, aisleItems.ElementAt(i));
                        inserted = true;
                    }
                    else {
                        if (j + 1 == sortedItems.Count)
                        {
                            sortedItems.Add(aisleItems.ElementAt(i));
                            inserted = true;
                        }
                    }
                }
                
            }

            return sortedItems;
        }

        public static void collectAisle(int aPos, int bPos, bool up, bool countDistance, int aislePart)
        {
            List<Item> aisleItems = getNonPickedAisleItems(aPos, bPos, aislePart);
            if (aisleItems.Count() == 0)
            {
                return;
            }
            List<Item> sortedAisleItems = sortAisleItems(aisleItems);
            if (up)
            {
                sortedAisleItems.Reverse();
            }
            foreach (Item i in sortedAisleItems)
            {
                if (i.picked_during_sshape == false)
                {
                    i.picked_during_sshape = true;
                    Console.WriteLine("Collected Item Info: {0}, {1}, {2}, {3}: Index: {4}", i.A_info, i.B_info, i.C_info, i.D_info, i.index+1);
                }
            }
            if (countDistance) {
                if (up)
                {
                    totalDistance = totalDistance + (Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info - MAX_D - 1) * K * 2);
                    Console.WriteLine("TRAVELLED DISTANCE COLLECTING {0}K", (Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info - MAX_D - 1)));
                    Console.WriteLine("TRAVELLED DISTANCE RETURNING {0}K", (Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info - MAX_D - 1)));
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                }
                else
                {
                    totalDistance = totalDistance + (sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info * K * 2);
                    Console.WriteLine("TRAVELLED DISTANCE COLLECTING {0}K", (sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info));
                    Console.WriteLine("TRAVELLED DISTANCE RETURNING {0}K", (sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).D_info));
                    Console.WriteLine("TOTAL DISTANCE {0}M", totalDistance);
                }
            }
        }

        public static void findMaximumGap(int aPos, int bPos)
        {
            List<Item> aisleItems = getAisleItems(aPos, bPos);
            List<Item> sortedAisleItems = sortAisleItems(aisleItems);
            
            
        }

        public static List<Item> getNonPickedAisleItems(int aPos, int bPos, int aislePart) {
            List<Item> aisleItems = new List<Item>();
            int rearMost = (int)Math.Ceiling(System.Convert.ToDouble(MAX_D) / 2);

            foreach (Item i in ITEMLIST)
            {
                if (i.picked_during_sshape == false && i.A_info == aPos)
                {
                    if ((i.B_info == bPos && i.C_info == 0) || (i.B_info == bPos - 1 && i.C_info == 1))
                    {
                        switch (aislePart)
                        {
                            case (int)AislePart.All:
                                aisleItems.Add(i);
                                break;
                            case (int)AislePart.Rear:
                                if (i.D_info <= rearMost)
                                {
                                    aisleItems.Add(i);
                                }
                                break;
                            case (int)AislePart.Front:
                                if (i.D_info > rearMost)
                                {
                                    aisleItems.Add(i);
                                }
                                break;
                            default:
                                aisleItems.Add(i);
                                break;
                        }
                    }
                }
            }
            return aisleItems;
        }

        public static int countAisle(int aPos, int bPos, int aislePart)
        {
            if (getNonPickedAisleItems(aPos, bPos, aislePart) == null){
                return 0;
            }
            return getNonPickedAisleItems(aPos, bPos, aislePart).Count();
        }

        public static List<int> filterPickAislesOfBlock(int aPos, List<int> pickAisles, int aislePart) {
            List<int> rearPickAisles = new List<int>();
            List<int> frontPickAisles = new List<int>();

            for (int i = 0; i < pickAisles.Count(); i++)
            {
                if (countAisle(aPos, pickAisles.ElementAt(i), (int)AislePart.Rear) > 0)
                {
                    rearPickAisles.Add(pickAisles.ElementAt(i));
                }
                if (countAisle(aPos, pickAisles.ElementAt(i), (int)AislePart.Front) > 0)
                {
                    frontPickAisles.Add(pickAisles.ElementAt(i));
                }
            }

            if (aislePart == (int)AislePart.Rear)
            {
                return rearPickAisles;
            }
            if (aislePart == (int)AislePart.Front)
            {
                return frontPickAisles;
            }
            return pickAisles;
        }



        public static List<int> getPickAislesOfBlock(int aPos)
        {
            List<int> result = new List<int>();
            for (int i = 2; i < MAX_B + 1; i++)
            {
                if (countAisle(aPos, i, (int)AislePart.All) > 0) {
                    result.Add(i);
                }
            }
            return result;
        }

        public static int findLeftPickAisle(int limit)
        {
            int minVal = MAX_B;
            Item minItem = null;

            if (ITEMLIST.Count == 0) {
                return -1;
            }
            foreach (Item i in ITEMLIST)
            {
                if (i.B_info <= minVal && i.A_info > limit)
                {
                    minVal = i.B_info;
                    if (minItem != null)
                    {
                        if (minItem.B_info == i.B_info)
                        {
                            if (i.C_info < minItem.C_info)
                            {
                                minItem = i;
                            }
                        }
                        else
                        {
                            minItem = i;
                        }
                    }
                    else {
                        minItem = i;
                    }
                }
            }
            if (minItem != null){
                Console.WriteLine("Min Item Info: {0}, {1}, {2}, {3}", minItem.A_info, minItem.B_info, minItem.C_info, minItem.D_info);
            }
            else{
                minItem = ITEMLIST.ElementAt(0);
                Console.WriteLine("Min Item Info: {0}, {1}, {2}, {3}", minItem.A_info, minItem.B_info, minItem.C_info, minItem.D_info);
            }
            

            if (minItem.C_info == 0)
            {
                return minItem.B_info;
            }
            else
            {
                return minItem.B_info + 1;
            }
        }

        public static int determineFarthestBlock()
        {
            return getMinOfArray(arrayA, MAX_A);
        }

        public static void Reading(string filepath)
        {
            ITEMLIST = new List<Item>();

            //ÇOK ÖNEMLİ NOT!!! INPUT DOSYASINDA CURSOR'I SON SATIRIN SONUNDA BIRAKIP KAYDETMEZSEK (MESELA SON SATIRDAN 1 SATIR AŞAĞIDA BIRAKIRSAK) SORUN ÇIKIYOR!!!
            if (System.IO.File.Exists(filepath))
            {
                StreamReader re = new StreamReader(File.OpenRead(filepath));
                string line = String.Empty;

                while ((line = re.ReadLine()) != null)
                {
                    string[] values = line.Split(','); // '\t' OR   ','
                    Item j = new Item();
                    j.index = ITEMLIST.Count(); //size_of_picklist + 1;   //Math.Max(0,ITEMLIST.Count());
                    j.A_info = int.Parse(values[0]);
                    j.B_info = int.Parse(values[1]);
                    j.C_info = int.Parse(values[2]);
                    j.D_info = int.Parse(values[3]);
                    ITEMLIST.Add(j);
                    size_of_picklist++;

                }//end of while
                re.Close();

               foreach (Item jj in ITEMLIST)
                    Console.WriteLine("ITEM NO:{0} - A={1}, B={2}, C={3}, D={4}", jj.index+1, jj.A_info, jj.B_info, jj.C_info, jj.D_info);

            }//end of if 

            else
                Console.WriteLine("NO FILE!");
        }// end of Reading

        public static void Generating()
        {
            ITEMLIST = new List<Item>();

            // excelde generate edip txt ye aktarıp okutmak yerine direk random classtan generate etmek istenirse:
            rand = new Random();

            for (int i = 0; i < size_of_picklist; i++)
            {
                Item j = new Item();
                j.index = i;
                j.A_info = rand.Next(1, no_of_horizontal_aisles); //inclusive lower bound & exclusive upper bound
                j.B_info = rand.Next(1, no_of_vertical_aisles);
                j.C_info = rand.Next(0, 2); //(0,1): 1 is exclusive
                j.D_info = rand.Next(1, S + 1);
                ITEMLIST.Add(j);
            }


        }// end of Generating


        //public static int SShapeHeuristic() //http://www.roodbergen.com/whopt/
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
                
        
        public static void Assignments()
        {
            //Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, A, B+C, (int)cplex.GetValue(X[0, 0, A, B+C]));

            for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
            {
                for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                {
                    //if ((iprime == A && jprime == B + C) || (iprime == A + 1 && jprime == B + C))
                    if ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0) // || ((iprime == A + 1 &&  jprime == B + C) && ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0)))
                        Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, iprime, jprime, (int)cplex.GetValue(X[0, 0, iprime, jprime]));
                }
            }
            
            
            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles; j++)
                {
                    for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                        {
                            if (
                             (iprime < no_of_horizontal_aisles + 1 && jprime < no_of_vertical_aisles + 1 && i < no_of_horizontal_aisles + 1 && j < no_of_vertical_aisles + 1)
                             &&
                             ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                                )
                                if ((int)cplex.GetValue(X[i, j, iprime, jprime]) != 0)
                                    Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, iprime, jprime, (int)cplex.GetValue(X[i, j, iprime, jprime]));
                        }
                    }
                }
            }

            
            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles; j++)
                {
                    if ((int)cplex.GetValue(X[i, j, 100, 100]) != 0)
                        Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, 100, 100, (int)cplex.GetValue(X[i, j, 100, 100]));
                }
            }
            
        }        
        
        public static int Solve_Shortest_Path(int A, int B, int C, int D, int APRIME, int BPRIME, int CPRIME, int DPRIME)
        {
            cplex = new Cplex();

            Decision_Variables();
            Constraints();
            Objective_Function();

            bool conclusion = cplex.Solve();
            string conclude = cplex.GetStatus().ToString();
            cplex.ExportModel("shortestpath.lp");

            if (conclusion)
            {
                Console.WriteLine("Status: " + conclude);
                Console.WriteLine("Objective function value: " + cplex.GetObjValue());
                Console.WriteLine("Optimal value: " + cplex.ObjValue);
            }//end of if 

            Assignments(); //cplex.null'dan önce decision variable'ların değerini yazdırmak lazım!

            int travelled_distance = Convert.ToInt32(cplex.GetObjValue());
            
            cplex.End();
            cplex = null;

            Console.WriteLine("TOTAL TRAVELLED DISTANCE={0}", travelled_distance);
            return travelled_distance;
            
        }

        public static void Decision_Variables()
        {
            X = new INumVar[101,101,101,101]; //??? for loop'larda 1!den başlattığım için 100+1; bir elemanın alabileceği max değer

            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles ; j++)
                {
                    for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                        {
                            if (
                            (iprime < no_of_horizontal_aisles + 1 && jprime < no_of_vertical_aisles + 1 && i < no_of_horizontal_aisles + 1 && j < no_of_vertical_aisles + 1)
                            &&
                            ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                               )
                            X[i, j, iprime, jprime] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + (i).ToString() + "," + (j).ToString() + "," + (iprime).ToString() + "," + (jprime).ToString() + ")");
                        }
                    }
                }
            }


            for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
            {
                for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                {
                    //if ((iprime == A && jprime == B + C) || (iprime == A + 1 && jprime == B + C)) // CAUSES TO A PROBLEM IN CONST1 REGARDING INDICATOR FUNCTIONS
                          X[0, 0, iprime, jprime] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + 0 + "," + 0 + "," + (iprime).ToString() + "," + (jprime).ToString() + ")");
                }
            }

            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles; j++)
                {
                    //if ((i == APRIME && j == BPRIME + CPRIME) || (i == APRIME + 1 && j == BPRIME + CPRIME))  // CAUSES TO A PROBLEM IN CONST1 REGARDING INDICATOR FUNCTIONS
                        X[i, j, 100, 100] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + (i).ToString() + "," + (j).ToString() + "," + 100 + "," + 100 + ")");
                }
            }
            
        }//end of Decision_Variables()

        public static void Constraints()
        {
            Const1();
            Const2();
            Const3();
        }//end of Constraints()

        public static double Distance_Function(int i, int j, int iprime, int jprime) //???
        {
            if ((iprime == i + 1 && jprime == j) || (iprime == i - 1 && jprime == j))
                DISTANCE = L;
            else if ((iprime == i && jprime == j + 1) || (iprime == i && jprime == j - 1))
                DISTANCE = W;
            else
                DISTANCE = 10000; //(int)Convert.ToInt32(System.Double.PositiveInfinity)???;

            return DISTANCE;
        }
         
        public static int Indicator_Function1(int i, int j)
        {
            if (i == APRIME + 1 && j == BPRIME + CPRIME)
                IND1 = 1;
            else
                IND1 = 0;

             return IND1;
        }

        public static int Indicator_Function2(int i, int j)
        {
            if (i == APRIME  && j == BPRIME + CPRIME)
                IND2 = 1;
            else
                IND2 = 0;

            return IND2;
        }

        public static int Indicator_Function3(int i, int j)
        {
            if (i == A+1 && j == B + C)
                IND3 = 1;
            else
                IND3 = 0;

            return IND3;
        }

        public static int Indicator_Function4(int i, int j)
        {
            if (i == A && j == B + C)
                IND4 = 1;
            else
                IND4 = 0;

            return IND4;
        }
        
        public static void Const1() //Flow balance for intermediate nodes
        {
            IRange[,] c1 = new IRange[251, 251]; //???       

            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles; j++) //< or <=?
                {
                    ILinearNumExpr exprc1 = cplex.LinearNumExpr();

                    for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                        {

                            //LHS:
                            if (
                            (iprime < no_of_horizontal_aisles+1 && jprime < no_of_vertical_aisles+1 && i < no_of_horizontal_aisles+1 && j < no_of_vertical_aisles+1)
                            &&
                            ((iprime == i - 1 || iprime == i || iprime == i + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                               )
                            {
                                exprc1.AddTerm(1, X[i, j, iprime, jprime]);
                            }
                           
                             
                            //RHS:
                            if (
                            (i < no_of_horizontal_aisles+1 && j < no_of_vertical_aisles+1 && iprime < no_of_horizontal_aisles+1 && jprime < no_of_vertical_aisles+1)
                            &&
                            ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((iprime == i && j == jprime - 1) || (iprime == i && j == jprime + 1)) || (iprime != i && j == jprime)))
                               )
                            {
                                exprc1.AddTerm(-1, X[iprime, jprime, i, j]);
                            }   
                                                                                                                             
                        }
                    }

                    exprc1.AddTerm(Indicator_Function1(i, j), X[i, j, 100, 100]);
                    
                    exprc1.AddTerm(Indicator_Function2(i, j), X[i, j, 100, 100]);
                    
                    exprc1.AddTerm(-Indicator_Function3(i, j), X[0, 0, i, j]);
                    
                    exprc1.AddTerm(-Indicator_Function4(i, j), X[0, 0, i, j]);
                    
                    c1[i, j] = cplex.AddEq(exprc1, 0, "constraint1(" + (i).ToString() + "," + (j).ToString() + ")");

                }
            }
                        
        }//end of Const1()
        
        public static void Const2() //Getting out of source node
        {
            IRange[] c2 = new IRange[1];
            
            ILinearNumExpr exprc2 = cplex.LinearNumExpr();

            exprc2.AddTerm(1, X[0, 0, A+1, B+C]);
            exprc2.AddTerm(1, X[0, 0, A, B+C]);

            c2[0] = cplex.AddEq(exprc2, 1, "constraint2(" + 0 + "," + 0 +  ")");

        }//end of Const2()

        public static void Const3() //Getting into sink node
        {
            IRange[] c3 = new IRange[1];

            ILinearNumExpr exprc3 = cplex.LinearNumExpr();

            exprc3.AddTerm(1, X[APRIME+1, BPRIME+CPRIME, 100, 100]);
            exprc3.AddTerm(1, X[APRIME, BPRIME+CPRIME, 100, 100]);

            c3[0] = cplex.AddEq(exprc3, 1, "constraint3(" + 100 + "," + 100 +  ")");

        }//end of Const3()
   
        public static void Objective_Function()
        {
            objective = cplex.LinearNumExpr();

            for (int i = 1; i <= no_of_horizontal_aisles; i++)
            {
                for (int j = 1; j <= no_of_vertical_aisles; j++)
                {
                    for (int iprime = 1; iprime <= no_of_horizontal_aisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= no_of_vertical_aisles; jprime++)
                        {
                            if (
                            (iprime < no_of_horizontal_aisles + 1 && jprime < no_of_vertical_aisles + 1 && i < no_of_horizontal_aisles + 1 && j < no_of_vertical_aisles + 1)
                            &&
                            ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                               )
                            {
                                objective.AddTerm(Distance_Function(i, j, iprime, jprime), X[i, j, iprime, jprime]);
                            }

                        }
                    }
                }
            }

            objective.AddTerm(K * (S - D), X[0, 0, A + 1, B + C]);

            objective.AddTerm(K * (D - 1), X[0, 0, A, B + C]);

            objective.AddTerm(K * (S - DPRIME), X[APRIME + 1, BPRIME + CPRIME, 100, 100]);

            objective.AddTerm(K * (DPRIME - 1), X[APRIME, BPRIME + CPRIME, 100, 100]);

            obj = cplex.AddMinimize(objective, "shortestdistance");

        }//end of Objective_Function()

    }//end of class Program
}//end of namespace

