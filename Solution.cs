using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Solution
    {
        public enum Methods { TabuSearch, SShape, LargestGap };

        private Cplex cplex;
        private INumVar[, , ,] X;
        private IObjective obj;
        private ILinearNumExpr objective;

        //Helpers... we are using helper arrays for simplicity
        private int[] arrayA; // an array to hold coordinate A of all items
        private int[] arrayB; // an array to hold coordinate B of all items
        private int[] arrayC; // an array to hold coordinate C of all items
        private int[] arrayD; // an array to hold coordinate D of all items

        private double[,] distances;

        public double[,] DistanceMatrix
        {
            get
            {
                return distances;
            }
            protected set
            {
                distances = value;
            }
        }

        private double runningTime;

        public double RunningTime
        {
            get
            {
                return runningTime;
            }
            protected set
            {
                runningTime = value;
            }
        }

        private double totalTravelledDistance;

        public double TravelledDistance
        {
            get
            {
                return totalTravelledDistance;
            }
            protected set
            {
                totalTravelledDistance = value;
            }
        }

        private Problem problem;

        public Problem Problem
        {
            get
            {
                return problem;
            }
            set
            {
                problem = value;
            }
        }

        private Picker picker;

        public Picker Picker
        {
            get
            {
                return picker;
            }
            set
            {
                picker = value;
            }
        }

        private Parameters parameters;

        public Parameters Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        public Solution(Problem problem, Picker picker, Parameters parameters)
        {
            this.problem = problem;
            this.picker = picker;
            this.parameters = parameters;
        }

        public void solve(Methods method)
        {
            populateHelperArrays();
            DateTime stTime = DateTime.Now;
            switch (method)
            {
                case Methods.TabuSearch:
                    solveUsingTabuSearch(this.parameters.NumberOfIterations, this.parameters.TabuLength, new Item(this.problem.ItemList.Count,this.problem.NumberOfCrossAisles-1, 1, 0, this.problem.S));
                    break;
                case Methods.SShape:
                    solveUsingSShapeHeuristic();
                    break;
                case Methods.LargestGap:
                    solveUsingLargestGap();
                    break;
                default:
                    solveUsingTabuSearch(this.parameters.NumberOfIterations, this.parameters.TabuLength, new Item(this.problem.ItemList.Count, this.problem.NumberOfCrossAisles - 1, 1, 0, this.problem.S));
                    break;
            }
            DateTime etTime = DateTime.Now;
            TimeSpan elapsed_Time = etTime.Subtract(stTime);
            double elapsedTime = Math.Round((elapsed_Time).TotalSeconds, 3);
            this.runningTime = elapsedTime;
            //switch (method)
            //{
            //    case Methods.TabuSearch:
            //        Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", elapsedTime);
            //        break;
            //    case Methods.SShape:
            //        Console.WriteLine("S-SHAPE RUNNING TIME: {0} Seconds", elapsedTime);
            //        break;
            //    default:
            //        Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", elapsedTime);
            //        break;
            //}
        }

        private void populateHelperArrays()
        {
            int itemCount = this.problem.ItemList.Count;
            arrayA = new int[itemCount];
            arrayB = new int[itemCount];
            arrayC = new int[itemCount];
            arrayD = new int[itemCount];

            foreach (Item i in this.problem.ItemList)
            {
                arrayA[i.Index-1] = i.AInfo;
                arrayB[i.Index-1] = i.BInfo;
                arrayC[i.Index-1] = i.CInfo;
                arrayD[i.Index-1] = i.DInfo;
            }
        }

        private void prepareDistanceMatrix(Item picker)
        {
            if (picker != null)
            {
                this.distances = new double[this.problem.ItemList.Count+2, this.problem.ItemList.Count+2];
            }
            else 
            {
                this.distances = new double[this.problem.ItemList.Count, this.problem.ItemList.Count];
            }

            int iA, iB, iC, iD, fA, fB, fC, fD;
            for (int i = 0; i < this.distances.GetLength(0); i++)
            {
                for (int j = 0; j < this.distances.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        this.distances[i, j] = 0;
                    }
                    else
                    {
                        if (picker != null)
                        {
                            if (i == 0 || i == this.distances.GetLength(0) - 1)
                            {
                                iA = picker.AInfo;
                                iB = picker.BInfo;
                                iC = picker.CInfo;
                                iD = picker.DInfo;
                                if (j == 0 || j == this.distances.GetLength(1) - 1)
                                {
                                    fA = picker.AInfo;
                                    fB = picker.BInfo;
                                    fC = picker.CInfo;
                                    fD = picker.DInfo;
                                }
                                else
                                {
                                    fA = arrayA[j - 1];
                                    fB = arrayB[j - 1];
                                    fC = arrayC[j - 1];
                                    fD = arrayD[j - 1];
                                }
                                this.distances[i, j] = Solve_Shortest_Path(iA, iB, iC, iD, fA, fB, fC, fD);
                            }
                            else 
                            {
                                iA = arrayA[i - 1]; //ITEMLIST[i - 1].A_info;
                                iB = arrayB[i - 1];
                                iC = arrayC[i - 1];
                                iD = arrayD[i - 1];
                                if (j == 0 || j == this.distances.GetLength(1) - 1)
                                {
                                    fA = picker.AInfo;
                                    fB = picker.BInfo;
                                    fC = picker.CInfo;
                                    fD = picker.DInfo;
                                }
                                else
                                {
                                    fA = arrayA[j - 1];
                                    fB = arrayB[j - 1];
                                    fC = arrayC[j - 1];
                                    fD = arrayD[j - 1];
                                }
                                this.distances[i, j] = Solve_Shortest_Path(iA, iB, iC, iD, fA, fB, fC, fD);
                            }
                        }
                        else
                        {
                            iA = arrayA[i]; //ITEMLIST[i - 1].A_info;
                            iB = arrayB[i];
                            iC = arrayC[i];
                            iD = arrayD[i];
                            fA = arrayA[j];
                            fB = arrayB[j];
                            fC = arrayC[j];
                            fD = arrayD[j];
                            this.distances[i, j] = Solve_Shortest_Path(iA, iB, iC, iD, fA, fB, fC, fD);
                        }
                    }
                }
            }
        }

        public double Solve_Shortest_Path(int A, int B, int C, int D, int APRIME, int BPRIME, int CPRIME, int DPRIME)
        {
            cplex = new Cplex();
            cplex.SetOut(null);

            Decision_Variables();
            Constraints(A, B, C, APRIME, BPRIME, CPRIME);
            Objective_Function(A, B, C, D, APRIME, BPRIME, CPRIME, DPRIME);

            bool conclusion = cplex.Solve();
            string conclude = cplex.GetStatus().ToString();
            cplex.ExportModel("shortestpath.lp");

            //if (conclusion)
            //{
            //    Console.WriteLine("Status: " + conclude);
            //    Console.WriteLine("Objective function value: " + cplex.GetObjValue());
            //    Console.WriteLine("Optimal value: " + cplex.ObjValue);
            //}

            //Assignments();

            double travelled_distance = cplex.GetObjValue();

            cplex.End();
            cplex = null;

            //Console.WriteLine("TOTAL TRAVELLED DISTANCE={0}", travelled_distance);
            return travelled_distance;
        }

        public void Decision_Variables()
        {
            X = new INumVar[101, 101, 101, 101]; //??? for loop'larda 1!den başlattığım için 100+1; bir elemanın alabileceği max değer

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                        {
                            if (
                            (iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1 && i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1)
                            &&
                            ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                               )
                                X[i, j, iprime, jprime] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + (i).ToString() + "," + (j).ToString() + "," + (iprime).ToString() + "," + (jprime).ToString() + ")");
                        }
                    }
                }
            }


            for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
            {
                for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                {
                    //if ((iprime == A && jprime == B + C) || (iprime == A + 1 && jprime == B + C)) // CAUSES TO A PROBLEM IN CONST1 REGARDING INDICATOR FUNCTIONS
                    X[0, 0, iprime, jprime] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + 0 + "," + 0 + "," + (iprime).ToString() + "," + (jprime).ToString() + ")");
                }
            }

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    //if ((i == APRIME && j == BPRIME + CPRIME) || (i == APRIME + 1 && j == BPRIME + CPRIME))  // CAUSES TO A PROBLEM IN CONST1 REGARDING INDICATOR FUNCTIONS
                    X[i, j, 100, 100] = cplex.NumVar(0, 1, NumVarType.Bool, "X(" + (i).ToString() + "," + (j).ToString() + "," + 100 + "," + 100 + ")");
                }
            }
        }

        public void Constraints(int A, int B, int C, int APRIME, int BPRIME, int CPRIME)
        {
            Const1(APRIME, BPRIME, CPRIME, A, B, C);
            Const2(A, B, C);
            Const3(APRIME, BPRIME, CPRIME);
        }

        public int Indicator_Function1(int i, int j, int APRIME, int BPRIME, int CPRIME)
        {
            int IND1;
            if (i == APRIME + 1 && j == BPRIME + CPRIME)
                IND1 = 1;
            else
                IND1 = 0;

            return IND1;
        }

        public int Indicator_Function2(int i, int j, int APRIME, int BPRIME, int CPRIME)
        {
            int IND2;
            if (i == APRIME && j == BPRIME + CPRIME)
                IND2 = 1;
            else
                IND2 = 0;

            return IND2;
        }

        public int Indicator_Function3(int i, int j, int A, int B, int C)
        {
            int IND3;
            if (i == A + 1 && j == B + C)
                IND3 = 1;
            else
                IND3 = 0;

            return IND3;
        }

        public int Indicator_Function4(int i, int j, int A, int B, int C)
        {
            int IND4;
            if (i == A && j == B + C)
                IND4 = 1;
            else
                IND4 = 0;

            return IND4;
        }

        public void Const1(int APRIME, int BPRIME, int CPRIME, int A, int B, int C) //Flow balance for intermediate nodes
        {
            IRange[,] c1 = new IRange[251, 251]; //???       

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++) //< or <=?
                {
                    ILinearNumExpr exprc1 = cplex.LinearNumExpr();

                    for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                        {

                            //LHS:
                            if (
                            (iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1 && i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1)
                            &&
                            ((iprime == i - 1 || iprime == i || iprime == i + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                               )
                            {
                                exprc1.AddTerm(1, X[i, j, iprime, jprime]);
                            }


                            //RHS:
                            if (
                            (i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1 && iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1)
                            &&
                            ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((iprime == i && j == jprime - 1) || (iprime == i && j == jprime + 1)) || (iprime != i && j == jprime)))
                               )
                            {
                                exprc1.AddTerm(-1, X[iprime, jprime, i, j]);
                            }

                        }
                    }

                    exprc1.AddTerm(Indicator_Function1(i, j, APRIME, BPRIME, CPRIME), X[i, j, 100, 100]);

                    exprc1.AddTerm(Indicator_Function2(i, j, APRIME, BPRIME, CPRIME), X[i, j, 100, 100]);

                    exprc1.AddTerm(-Indicator_Function3(i, j, A, B, C), X[0, 0, i, j]);

                    exprc1.AddTerm(-Indicator_Function4(i, j, A, B, C), X[0, 0, i, j]);

                    c1[i, j] = cplex.AddEq(exprc1, 0, "constraint1(" + (i).ToString() + "," + (j).ToString() + ")");
                }
            }

        }

        public void Const2(int A, int B, int C) //Getting out of source node
        {
            IRange[] c2 = new IRange[1];

            ILinearNumExpr exprc2 = cplex.LinearNumExpr();

            exprc2.AddTerm(1, X[0, 0, A + 1, B + C]);
            exprc2.AddTerm(1, X[0, 0, A, B + C]);

            c2[0] = cplex.AddEq(exprc2, 1, "constraint2(" + 0 + "," + 0 + ")");

        }

        public void Const3(int APRIME, int BPRIME, int CPRIME) //Getting into sink node
        {
            IRange[] c3 = new IRange[1];

            ILinearNumExpr exprc3 = cplex.LinearNumExpr();

            exprc3.AddTerm(1, X[APRIME + 1, BPRIME + CPRIME, 100, 100]);
            exprc3.AddTerm(1, X[APRIME, BPRIME + CPRIME, 100, 100]);

            c3[0] = cplex.AddEq(exprc3, 1, "constraint3(" + 100 + "," + 100 + ")");
        }

        public double Distance_Function(int i, int j, int iprime, int jprime) //???
        {
            double distance;
            if ((iprime == i + 1 && jprime == j) || (iprime == i - 1 && jprime == j))
                distance = this.problem.L;
            else if ((iprime == i && jprime == j + 1) || (iprime == i && jprime == j - 1))
                distance = this.problem.W;
            else
                distance = 10000; //(int)Convert.ToInt32(System.Double.PositiveInfinity)???;

            return distance;
        }

        public void Objective_Function(int A, int B, int C, int D, int APRIME, int BPRIME, int CPRIME, int DPRIME)
        {
            objective = cplex.LinearNumExpr();

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                        {
                            if (
                            (iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1 && i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1)
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

            objective.AddTerm(this.problem.K * (this.problem.S - D), X[0, 0, A + 1, B + C]);

            objective.AddTerm(this.problem.K * (D - 1), X[0, 0, A, B + C]);

            objective.AddTerm(this.problem.K * (this.problem.S - DPRIME), X[APRIME + 1, BPRIME + CPRIME, 100, 100]);

            objective.AddTerm(this.problem.K * (DPRIME - 1), X[APRIME, BPRIME + CPRIME, 100, 100]);

            obj = cplex.AddMinimize(objective, "shortestdistance");
        }

        //This functions exists only for testing and printing purpose.
        //public void Assignments()
        //{
        //    //Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, A, B+C, (int)cplex.GetValue(X[0, 0, A, B+C]));

        //    for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
        //    {
        //        for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
        //        {
        //            //if ((iprime == A && jprime == B + C) || (iprime == A + 1 && jprime == B + C))
        //            if ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0) // || ((iprime == A + 1 &&  jprime == B + C) && ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0)))
        //                Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, iprime, jprime, (int)cplex.GetValue(X[0, 0, iprime, jprime]));
        //        }
        //    }


        //    for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
        //    {
        //        for (int j = 1; j <= this.problem.NumberOfAisles; j++)
        //        {
        //            for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
        //            {
        //                for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
        //                {
        //                    if (
        //                     (iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1 && i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1)
        //                     &&
        //                     ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
        //                        )
        //                        if ((int)cplex.GetValue(X[i, j, iprime, jprime]) != 0)
        //                            Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, iprime, jprime, (int)cplex.GetValue(X[i, j, iprime, jprime]));
        //                }
        //            }
        //        }
        //    }


        //    for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
        //    {
        //        for (int j = 1; j <= this.problem.NumberOfAisles; j++)
        //        {
        //            if ((int)cplex.GetValue(X[i, j, 100, 100]) != 0)
        //                Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, 100, 100, (int)cplex.GetValue(X[i, j, 100, 100]));
        //        }
        //    }
        //}

        public double calculateTabuSearchObjectiveFunctionValue(int[] solution)
        { 
            double cost = 0;

            for (int i = 0; i < (solution.GetLength(0) - 1); i++)
            {
                cost += this.distances[solution[i], solution[i + 1]];
            }

            return cost;
        }

        public int[] getBestNeighbour(TabuList tabuList, int[] initialSolution)
        {
            int[] bestSolution = new int[initialSolution.GetLength(0)]; //this is the best Solution So Far
            Array.Copy(initialSolution, 0, bestSolution, 0, bestSolution.GetLength(0));
            double bestCost = calculateTabuSearchObjectiveFunctionValue(initialSolution);
            int city1 = 0;
            int city2 = 0;
            bool firstNeighbor = true;

            for (int i = 1; i < (bestSolution.GetLength(0) - 1); i++)
            {
                for (int j = 2; j < (bestSolution.GetLength(0) - 1); j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    int[] newBestSolution = new int[bestSolution.GetLength(0)]; //this is the best Solution So Far
                    Array.Copy(bestSolution, 0, newBestSolution, 0, newBestSolution.GetLength(0));

                    newBestSolution = swapOperator(i, j, initialSolution); //Try swapping cities i and j
                    //printTabuPath(newBestSolution);
                    double newBestCost = calculateTabuSearchObjectiveFunctionValue(newBestSolution);

                    if ((newBestCost > bestCost || firstNeighbor) && tabuList.List[i, j] == 0) //tabuList.tabuList[i,j] == 0 means It is not in the list so that move can be performed
                    { //if better move found, store it
                        firstNeighbor = false;
                        city1 = i;
                        city2 = j;
                        Array.Copy(newBestSolution, 0, bestSolution, 0, newBestSolution.GetLength(0));
                        bestCost = newBestCost;
                    }
                }
            }

            if (city1 != 0)
            {
                tabuList.decrementTabu();
                tabuList.tabuMove(city1, city2);
            }
            return bestSolution;
        }

        public int[] swapOperator(int city1, int city2, int[] solution)
        {
            int temp = solution[city1];
            solution[city1] = solution[city2];
            solution[city2] = temp;
            return solution;
        }

        public void printTabuPath(int[] solution)
        {
            String path = "";
            for (int i = 0; i < solution.GetLength(0); i++)
            {
                path += (solution[i] + 1);
                if (i != solution.GetLength(0) - 1)
                {
                    path += " -> ";
                }
            }
            Console.WriteLine(path);
            Console.WriteLine();
        }

        private void solveUsingTabuSearch(int numberOfIterations, int tabuLength, Item picker)
        {
            //generating an initial solution
            List<Item> sortedItems = new List<Item>();
            foreach (Item i in this.problem.ItemList)
            {
                sortedItems.Add(i);
            }
            sortedItems.Sort();
            sortedItems.Insert(0, picker);
            sortedItems.Add(picker);
            int[] currentSolution = new int[sortedItems.Count()];
            for (int i = 0; i < sortedItems.Count(); i++)
            {
                currentSolution[i] = sortedItems[i].Index;
            }

            prepareDistanceMatrix(picker);

            TabuList tabuList = new TabuList(this.distances.GetLength(0), tabuLength);

            int[] bestSolution = new int[currentSolution.GetLength(0)];
            Array.Copy(currentSolution, 0, bestSolution, 0, bestSolution.GetLength(0));
            double bestCost = calculateTabuSearchObjectiveFunctionValue(bestSolution);

            for (int i = 0; i < numberOfIterations; i++)
            {

                currentSolution = getBestNeighbour(tabuList, currentSolution);
                //printTabuPath(currentSolution);
                //tabuList.printTabuList();

                double currentCost = calculateTabuSearchObjectiveFunctionValue(currentSolution);
                if (currentCost < bestCost)
                {
                    Array.Copy(currentSolution, 0, bestSolution, 0, bestSolution.GetLength(0));
                    bestCost = currentCost;
                }
            }

            //Console.WriteLine("\n\nSearch done! \nBest Solution cost found = " + bestCost + "\nBest Solution :");
            this.totalTravelledDistance = bestCost;

            //printTabuPath(bestSolution);
        }

        public int getMinOfArray(int[] arr, int max)
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

        public int determineFarthestBlock()
        {
            return getMinOfArray(arrayA, this.picker.AInfo-1);
        }

        public int findLeftPickAisle(int limit)
        {
            int minVal = this.problem.NumberOfAisles - 1;
            Item minItem = null;

            if (this.problem.ItemList.Count() == 0)
            {
                return -1;
            }
            foreach (Item i in this.problem.ItemList)
            {
                if (i.BInfo <= minVal && i.AInfo > limit)
                {
                    minVal = i.BInfo;
                    if (minItem != null)
                    {
                        if (minItem.BInfo == i.BInfo)
                        {
                            if (i.CInfo < minItem.CInfo)
                            {
                                minItem = i;
                            }
                        }
                        else
                        {
                            minItem = i;
                        }
                    }
                    else
                    {
                        minItem = i;
                    }
                }
            }
            if (minItem != null)
            {
                Console.WriteLine("Min Item Info: {0}, {1}, {2}, {3}", minItem.AInfo, minItem.BInfo, minItem.CInfo, minItem.DInfo);
            }
            else
            {
                minItem = this.problem.ItemList.ElementAt(0);
                Console.WriteLine("Min Item Info: {0}, {1}, {2}, {3}", minItem.AInfo, minItem.BInfo, minItem.CInfo, minItem.DInfo);
            }


            if (minItem.CInfo == 0)
            {
                return minItem.BInfo;
            }
            else
            {
                return minItem.BInfo + 1;
            }
        }

        public void solveUsingSShapeHeuristic()
        {
            int farthestBlockA = determineFarthestBlock();
            int leftPickAisleB = findLeftPickAisle(farthestBlockA);
            Console.WriteLine("LEFT PICK AISLE: {0}", leftPickAisleB);
            Console.WriteLine("FARTHEST BLOCK: {0}", farthestBlockA);
            picker.printLocation();
            picker.goToLocation(picker.AInfo, leftPickAisleB, new LinkDistance(Math.Abs(leftPickAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(leftPickAisleB - picker.BInfo) * this.problem.W);
            picker.goVertical(farthestBlockA + 1, this.problem);
            List<int> pickAisles = this.problem.getPickAislesOfBlock(farthestBlockA);
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                picker.collectAisle(picker.AInfo - 1, picker.BInfo, true, true, Problem.AislePart.All, this.problem);
            }
            else
            {
                picker.goVertical(farthestBlockA, this.problem);
            }
            int farMostBlock = farthestBlockA;
            //printLocation();
            bool goRight = true;
            bool goUp = false;
            while (farMostBlock < this.problem.Depot.Y)
            {
                pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count() > 0)
                {
                    int leftMostSubAisleB = pickAisles.ElementAt(0);
                    int rightMostSubAisleB = pickAisles.ElementAt(pickAisles.Count() - 1);
                    //Console.WriteLine("LEFT MOST SUB AISLE: {0}", leftMostSubAisleB);
                    //Console.WriteLine("RIGHT MOST SUB AISLE: {0}", rightMostSubAisleB);
                    if (Math.Abs(picker.BInfo - leftMostSubAisleB) < Math.Abs(picker.BInfo - rightMostSubAisleB))
                    {
                        Console.WriteLine("LEFT MOST SUB AISLE IS SELECTED");
                        picker.goToLocation(picker.AInfo, leftMostSubAisleB, new LinkDistance(Math.Abs(leftMostSubAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(leftMostSubAisleB - picker.BInfo) * this.problem.W);
                        goRight = true;
                    }
                    else
                    {
                        Console.WriteLine("RIGHT MOST SUB AISLE IS SELECTED");
                        picker.goToLocation(picker.AInfo, rightMostSubAisleB, new LinkDistance(Math.Abs(rightMostSubAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(rightMostSubAisleB - picker.BInfo) * this.problem.W);
                        goRight = false;
                    }
                    //printLocation();
                    picker.goVertical(farMostBlock + 1, this.problem);
                    goUp = true;
                    //printLocation();
                }
                pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                while (pickAisles.Count() > 1)
                {
                    if (goRight)
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(0), new LinkDistance(Math.Abs(pickAisles.ElementAt(0) - picker.BInfo), Problem.Codes.W), Math.Abs(pickAisles.ElementAt(0) - picker.BInfo) * this.problem.W);
                    }
                    else
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(pickAisles.Count() - 1), new LinkDistance(Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - picker.BInfo), Problem.Codes.W), Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - picker.BInfo) * this.problem.W);
                    }
                    //printLocation();
                    if (goUp)
                    {
                        picker.goVertical(farMostBlock, this.problem);
                        goUp = false;
                    }
                    else
                    {
                        picker.goVertical(farMostBlock + 1, this.problem);
                        goUp = true;
                    }
                    pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                }
                if (pickAisles.Count() > 0)
                {
                    if (goRight)
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(0), new LinkDistance(Math.Abs(pickAisles.ElementAt(0) - picker.BInfo), Problem.Codes.W), Math.Abs(pickAisles.ElementAt(0) - picker.BInfo) * this.problem.W);
                    }
                    else
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(pickAisles.Count() - 1), new LinkDistance(Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - picker.BInfo), Problem.Codes.W), Math.Abs(pickAisles.ElementAt(pickAisles.Count() - 1) - picker.BInfo) * this.problem.W);
                    }
                }
                else
                {
                    //Step 4.b should be written
                }

                //printLocation();
                if (goUp)
                {
                    picker.collectAisle(picker.AInfo - 1, picker.BInfo, true, true, Problem.AislePart.All, this.problem);
                }
                else
                {
                    picker.goVertical(farMostBlock + 1, this.problem);
                }
                //printLocation();
                farMostBlock++;
            }
            picker.goToLocation(picker.AInfo, this.problem.Depot.X, new LinkDistance(Math.Abs(this.problem.Depot.X - picker.BInfo), Problem.Codes.W), Math.Abs(this.problem.Depot.X - picker.BInfo) * this.problem.W);
            Console.WriteLine("PICKER IS FINISHED ITS JOB");
            picker.printPath();
            picker.printTravelledDistances();
            picker.printTravelledTotalDistance();
        }

        public void solveUsingLargestGap()
        {
            int farthestBlockA = determineFarthestBlock();
            int leftPickAisleB = findLeftPickAisle(farthestBlockA);
            Console.WriteLine("LEFT PICK AISLE: {0}", leftPickAisleB);
            Console.WriteLine("FARTHEST BLOCK: {0}", farthestBlockA);
            picker.printLocation();
            picker.goToLocation(picker.AInfo, leftPickAisleB, new LinkDistance(Math.Abs(leftPickAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(leftPickAisleB - picker.BInfo) * this.problem.W);
            picker.goVertical(farthestBlockA + 1, this.problem);
            List<int> pickAisles = this.problem.getPickAislesOfBlock(farthestBlockA);
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                picker.collectAisle(picker.AInfo - 1, picker.BInfo, true, true, Problem.AislePart.All, this.problem);
            }
            else
            {
                picker.goVertical(farthestBlockA, this.problem);
            }
            int farMostBlock = farthestBlockA;
            while (farMostBlock < this.problem.Depot.Y)
            {
                pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count() > 0)
                {
                    int leftMostSubAisleB = pickAisles.ElementAt(0);
                    int lastSubAisleB = pickAisles.ElementAt(pickAisles.Count() - 1);
                    List<int> rearPickAisles = this.problem.filterPickAislesOfBlock(farMostBlock, pickAisles, Problem.AislePart.Rear);
                    List<int> frontPickAisles = this.problem.filterPickAislesOfBlock(farMostBlock, pickAisles, Problem.AislePart.Front);
                    picker.goToLocation(picker.AInfo, leftMostSubAisleB, new LinkDistance(Math.Abs(leftMostSubAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(leftMostSubAisleB - picker.BInfo) * this.problem.W);

                    if (frontPickAisles.Count > 1)
                    {
                        frontPickAisles.Reverse();
                    }

                    for (int i = 0; i < rearPickAisles.Count; i++)
                    {
                        if (rearPickAisles.ElementAt(i) != lastSubAisleB)
                        {
                            picker.goToLocation(picker.AInfo, rearPickAisles.ElementAt(i), new LinkDistance(Math.Abs(rearPickAisles.ElementAt(i) - picker.BInfo), Problem.Codes.W), Math.Abs(rearPickAisles.ElementAt(i) - picker.BInfo) * this.problem.W);
                            picker.collectAisle(farMostBlock, picker.BInfo, false, true, Problem.AislePart.Rear, this.problem);
                        }
                    }
                    picker.goToLocation(picker.AInfo, lastSubAisleB, new LinkDistance(Math.Abs(lastSubAisleB - picker.BInfo), Problem.Codes.W), Math.Abs(lastSubAisleB - picker.BInfo) * this.problem.W);

                    picker.goVertical(farMostBlock + 1, this.problem);
                    for (int i = 0; i < frontPickAisles.Count(); i++)
                    {
                        if (frontPickAisles.ElementAt(i) != lastSubAisleB)
                        {
                            picker.goToLocation(picker.AInfo, frontPickAisles.ElementAt(i), new LinkDistance(Math.Abs(frontPickAisles.ElementAt(i) - picker.BInfo), Problem.Codes.W), Math.Abs(frontPickAisles.ElementAt(i) - picker.BInfo) * this.problem.W);
                            picker.collectAisle(farMostBlock, picker.BInfo, true, true, Problem.AislePart.Front, this.problem);
                        }
                    }
                }
                farMostBlock++;
            }
            picker.goToLocation(picker.AInfo, this.problem.Depot.X, new LinkDistance(Math.Abs(this.problem.Depot.X - picker.BInfo), Problem.Codes.W), Math.Abs(this.problem.Depot.X - picker.BInfo) * this.problem.W);
            Console.WriteLine("PICKER IS FINISHED ITS JOB");
            picker.printPath();
            picker.printTravelledDistances();
            picker.printTravelledTotalDistance();
        }
    }
}
