using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TBLST;
using TSPENVRNMNT;

namespace TBSRCH
{
    public class TabuSearch
    {

        public static int[] getBestNeighbour(TabuList tabuList,
                TSPEnvironment tspEnviromnet,
                int[] initSolution)
        {
            int[] bestSol = new int[initSolution.GetLength(0)]; //this is the best Solution So Far
            Array.Copy(initSolution, 0, bestSol, 0, bestSol.GetLength(0));
            int bestCost = tspEnviromnet.getObjectiveFunctionValue(initSolution);
            int city1 = 0;
            int city2 = 0;
            bool firstNeighbor = true;

            for (int i = 1; i < (bestSol.GetLength(0) - 1); i++)
            {
                for (int j = 2; j < (bestSol.GetLength(0) - 1); j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int[] newBestSol = new int[bestSol.GetLength(0)]; //this is the best Solution So Far
                    Array.Copy(bestSol, 0, newBestSol, 0, newBestSol.GetLength(0));

                    newBestSol = swapOperator(i, j, initSolution); //Try swapping cities i and j
                    // , maybe we get a bettersolution
                    //***
                    TabuSearch.printSolution(newBestSol);
                    int newBestCost = tspEnviromnet.getObjectiveFunctionValue(newBestSol);

                    //***
                    Console.WriteLine("NEW BEST COST:{0}", newBestCost);

                    if ((newBestCost > bestCost || firstNeighbor) && tabuList.tabuList[i, j] == 0) //tabuList.tabuList[i,j] == 0 yani tabu listte yoksa! that move can be performed
                    { //if better move found, store it
                        firstNeighbor = false;
                        city1 = i;
                        city2 = j;
                        Array.Copy(newBestSol, 0, bestSol, 0, newBestSol.GetLength(0));
                        bestCost = newBestCost;
                        //***
                        Console.WriteLine("NEW COST > (BEST COST OR FIRST NEIGHBOR) AND NOT A TABU MOVE");
                        //***
                        Console.WriteLine("BEST COST:{0}", bestCost);
                    }
                }
            }

            if (city1 != 0)
            {
                tabuList.decrementTabu();
                tabuList.tabuMove(city1, city2);
            }
            return bestSol;


        }

        //swaps two cities
        public static int[] swapOperator(int city1, int city2, int[] solution)
        {
            int temp = solution[city1];
            solution[city1] = solution[city2];
            solution[city2] = temp;
            return solution;
        }


        public static void printSolution(int[] solution)
        {
            for (int i = 0; i < solution.GetLength(0); i++)
            {
                Console.WriteLine(solution[i] + " "); //ORIGINAL: FOR CITIES
                //Console.WriteLine((solution[i]+1) + " "); //ITEM İÇİN +1 OLARAK YAZDIRALIM
            }
            Console.WriteLine();
        }//end of printSolution

    }//end of class TabuSearch
    
}
