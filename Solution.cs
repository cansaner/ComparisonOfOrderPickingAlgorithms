using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;
using System.Diagnostics;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Solution
    {
        public enum Algorithm { TabuSearch, SShape, LargestGap, GeneticAlgorithm };
        public enum InitialSolutionType { Ordered, Random, Greedy};

        private Cplex cplex;
        private INumVar[, , ,] X;
        private IObjective obj;
        private ILinearNumExpr objective;
        private Stopwatch stopWatch;

        private IDictionary<int, Item> indexedItemDictionary = new Dictionary<int, Item>();

        public IDictionary<int, Item> IndexedItemDictionary
        {
            get
            {
                return indexedItemDictionary;
            }
            protected set
            {
                indexedItemDictionary = value;
            }
        }

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

        private List<Coordinate>[,] paths;

        public List<Coordinate>[,] PathMatrix
        {
            get
            {
                return paths;
            }
            protected set
            {
                paths = value;
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

        private double distanceMatrixRunningTime;

        public double DistanceMatrixRunningTime
        {
            get
            {
                return distanceMatrixRunningTime;
            }
            protected set
            {
                distanceMatrixRunningTime = value;
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

        public void solve(Algorithm method)
        {
            populateItemDictionary();
            stopWatch = Stopwatch.StartNew();
            switch (method)
            {
                case Algorithm.TabuSearch:
                    solveUsingTabuSearch(this.parameters.TabuLength, new Item(0, this.problem.NumberOfCrossAisles - 1, 1, 0, this.problem.S));
                    break;
                case Algorithm.SShape:
                    solveUsingSShape();
                    break;
                case Algorithm.LargestGap:
                    solveUsingLargestGap();
                    break;
                case Algorithm.GeneticAlgorithm:
                    solveUsingGeneticAlgorithm();
                    break;
                default:
                    solveUsingTabuSearch(this.parameters.TabuLength, new Item(0, this.problem.NumberOfCrossAisles - 1, 1, 0, this.problem.S));
                    break;
            }
            stopWatch.Stop();
            TimeSpan elapsed_Time = stopWatch.Elapsed;
            double elapsedTime = Math.Round((elapsed_Time).TotalSeconds, 3);
            if (method != Algorithm.TabuSearch)
                this.runningTime = elapsedTime;
            switch (method)
            {
                case Algorithm.TabuSearch:
                    Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", this.runningTime);
                    break;
                case Algorithm.SShape:
                    Console.WriteLine("S-SHAPE RUNNING TIME: {0} Seconds", this.runningTime);
                    break;
                case Algorithm.LargestGap:
                    Console.WriteLine("LARGEST GAP RUNNING TIME: {0} Seconds", this.runningTime);
                    break;
                default:
                    Console.WriteLine("TABU SEARCH RUNNING TIME: {0} Seconds", this.runningTime);
                    break;
            }
        }

        private void populateItemDictionary()
        {
            foreach (Item i in this.problem.ItemList)
            {
                if (!this.indexedItemDictionary.ContainsKey(i.Index))
                    this.indexedItemDictionary.Add(i.Index, i);
                else
                    Console.WriteLine("Encountered item with duplicate index while preparing item dictionary");
            }
        }

        private void prepareDistanceMatrix(Item picker)
        {
            if (picker != null)
            {
                this.distances = new double[this.problem.ItemList.Count + 1, this.problem.ItemList.Count + 1];
                this.paths = new List<Coordinate>[this.problem.ItemList.Count + 1, this.problem.ItemList.Count + 1];
            }
            else
            {
                this.distances = new double[this.problem.ItemList.Count, this.problem.ItemList.Count];
                this.paths = new List<Coordinate>[this.problem.ItemList.Count, this.problem.ItemList.Count];
            }
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
                            if (j > i)
                            {
                                if (i == 0)
                                {
                                    this.distances[i, j] = Solve_Shortest_Path(picker, this.indexedItemDictionary[j]);
                                }
                                else
                                {
                                    this.distances[i, j] = Solve_Shortest_Path(this.indexedItemDictionary[i], this.indexedItemDictionary[j]);
                                }
                            }
                            else
                            {
                                this.distances[i, j] = this.distances[j, i];
                            }
                        }
                        else
                        {
                            if (j > i)
                            {
                                this.distances[i, j] = Solve_Shortest_Path(this.indexedItemDictionary[i+1], this.indexedItemDictionary[j+1]);
                            }
                            else
                            {
                                this.distances[i, j] = this.distances[j, i];
                            }
                        }
                    }
                }
            }
        }

        public double Solve_Shortest_Path(Item sourceItem, Item destinationItem)
        {
            cplex = new Cplex();
            cplex.SetOut(null);

            Decision_Variables();
            Constraints(sourceItem.AInfo, sourceItem.BInfo, sourceItem.CInfo, destinationItem.AInfo, destinationItem.BInfo, destinationItem.CInfo);
            Objective_Function(sourceItem.AInfo, sourceItem.BInfo, sourceItem.CInfo, sourceItem.DInfo, destinationItem.AInfo, destinationItem.BInfo, destinationItem.CInfo, destinationItem.DInfo);
            Console.WriteLine("CPLEX runs from (" + sourceItem.Index + ") to (" + destinationItem.Index + ")");

            bool conclusion = cplex.Solve();
            string conclude = cplex.GetStatus().ToString();
            cplex.ExportModel("shortestpath.lp");

            //if (conclusion)
            //{
            //    Console.WriteLine("Status: " + conclude);
            //    Console.WriteLine("Objective function value: " + cplex.GetObjValue());
            //    Console.WriteLine("Optimal value: " + cplex.ObjValue);
            //}

            Assignments();
            //extractShortestPath(sourceItem, destinationItem);

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

        private void extractShortestPath(Item sourceItem, Item destinationItem)
        {
            List<Coordinate> path = new List<Coordinate>();
            Coordinate currentNode = new Coordinate(0, 0);

            //getting sourceNode coordinate
            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    if ((int)(cplex.GetValue(X[0, 0, i, j])) != 0)
                    {
                        Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, i, j, (int)cplex.GetValue(X[0, 0, i, j]));
                        currentNode.Y = i;
                        currentNode.X = j;
                        path.Add(new Coordinate(currentNode.X, currentNode.Y));
                    }
                }
            }

            while (currentNode.X != 100 && currentNode.Y != 100)
            {
                for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
                {
                    for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                    {
                        if ((X[currentNode.Y, currentNode.X, i, j] != null) && (int)(cplex.GetValue(X[currentNode.Y, currentNode.X, i, j])) == 1)
                        {
                            Console.WriteLine("X[{0},{1},{2},{3}]={4}", currentNode.Y, currentNode.X, i, j, (int)cplex.GetValue(X[currentNode.Y, currentNode.X, i, j]));
                            currentNode.Y = i;
                            currentNode.X = j;
                            path.Add(new Coordinate(currentNode.X, currentNode.Y));
                        }
                    }
                }
            }
        }

        //This functions exists only for testing and printing purpose.
        public void Assignments()
        {
            //Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, A, B+C, (int)cplex.GetValue(X[0, 0, A, B+C]));

            for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
            {
                for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                {
                    //if ((iprime == A && jprime == B + C) || (iprime == A + 1 && jprime == B + C))
                    if ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0) // || ((iprime == A + 1 &&  jprime == B + C) && ((int)(cplex.GetValue(X[0, 0, iprime, jprime])) != 0)))
                        Console.WriteLine("X[{0},{1},{2},{3}]={4}", 0, 0, iprime, jprime, (int)cplex.GetValue(X[0, 0, iprime, jprime]));
                        
                }
            }

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    for (int iprime = 1; iprime <= this.problem.NumberOfCrossAisles; iprime++)
                    {
                        for (int jprime = 1; jprime <= this.problem.NumberOfAisles; jprime++)
                        {
                            //if (
                            // (iprime < this.problem.NumberOfCrossAisles + 1 && jprime < this.problem.NumberOfAisles + 1 && i < this.problem.NumberOfCrossAisles + 1 && j < this.problem.NumberOfAisles + 1)
                            // &&
                            // ((i == iprime - 1 || i == iprime || i == iprime + 1) && (((i == iprime && jprime == j - 1) || (i == iprime && jprime == j + 1)) || (i != iprime && jprime == j)))
                            //    )
                                if (( X[i, j, iprime, jprime] != null ) &&(int)cplex.GetValue(X[i, j, iprime, jprime]) != 0)
                                    Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, iprime, jprime, (int)cplex.GetValue(X[i, j, iprime, jprime]));
                        }
                    }
                }
            }

            for (int i = 1; i <= this.problem.NumberOfCrossAisles; i++)
            {
                for (int j = 1; j <= this.problem.NumberOfAisles; j++)
                {
                    if ((int)cplex.GetValue(X[i, j, 100, 100]) != 0)
                        Console.WriteLine("X[{0},{1},{2},{3}]={4}", i, j, 100, 100, (int)cplex.GetValue(X[i, j, 100, 100]));
                }
            }
        }

        public double calculateTabuSearchObjectiveFunctionValue(int[] solutionIndices)
        {
            double distanceSum = 0.0;

            if (solutionIndices.Length == 0)
                return distanceSum;

            for (int i = 1; i < solutionIndices.Length; i++)
            {
                distanceSum += this.distances[solutionIndices[i-1], solutionIndices[i]];
            }

            distanceSum += this.distances[solutionIndices[solutionIndices.Length-1], solutionIndices[0]];

            return distanceSum;
        }

        public int[] getBestNeighbour(TabuList tabuList, int[] initialSolution, double bestCost)
        {
            int[] currentSolution;
            double currentCost;

            int item1 = -1;
            int item2 = -1;

            SortedDictionary<double, List<int[]>> allNeighbors = new SortedDictionary<double, List<int[]>>();

            for (int i = 0; i < initialSolution.Length - 1; i++)
            {
                int[] nextSolutionIndices = Utils.GetUniqueInts(2, 0, initialSolution.Length);

                currentSolution = swapOperator(nextSolutionIndices[0], nextSolutionIndices[1], initialSolution); //Swapping 2 items to get a neighbor
                currentCost = calculateTabuSearchObjectiveFunctionValue(currentSolution);

                if (allNeighbors.ContainsKey(currentCost))
                {
                    allNeighbors[currentCost].Add(nextSolutionIndices);
                }
                else
                {
                    allNeighbors.Add(currentCost, new List<int[]> { nextSolutionIndices });
                }
            }

            bool bestNeighborFound = false;
            int neighborIndexToCheck = 0;
            int[] solutionToCheck = new int[initialSolution.Length];

            while (!bestNeighborFound)
            {
                int[] bestNeighborsSwappedIndices = getSolutionAtIndex(allNeighbors, neighborIndexToCheck);

                if (tabuList.List[initialSolution[bestNeighborsSwappedIndices[0]], initialSolution[bestNeighborsSwappedIndices[1]]] > 0)
                {
                    solutionToCheck = swapOperator(bestNeighborsSwappedIndices[0], bestNeighborsSwappedIndices[1], initialSolution);
                    double costToCheck = calculateTabuSearchObjectiveFunctionValue(solutionToCheck);
                    if (costToCheck <= bestCost) //Tabu is overridden
                    {
                        bestNeighborFound = true;
                        item1 = initialSolution[bestNeighborsSwappedIndices[0]];
                        item2 = initialSolution[bestNeighborsSwappedIndices[1]];
                    }
                    else
                    {

                    }
                }
                else
                {
                    bestNeighborFound = true;
                    solutionToCheck = swapOperator(bestNeighborsSwappedIndices[0], bestNeighborsSwappedIndices[1], initialSolution);
                    item1 = initialSolution[bestNeighborsSwappedIndices[0]];
                    item2 = initialSolution[bestNeighborsSwappedIndices[1]];
                }
                neighborIndexToCheck++;
            }

            tabuList.decrementTabu();
            tabuList.tabuMove(item1, item2);

            return solutionToCheck;
        }

        private int[] getSolutionAtIndex(SortedDictionary<double, List<int[]>> allsolutions, int index)
        {
            if (allsolutions == null || allsolutions.Count == 0)
                return null;

            List<int[]> sortedSolutionsList = new List<int[]>();

            for(int i=0; i < allsolutions.Count; i++)
            {
                for (int j=0; j < allsolutions.ElementAt(i).Value.Count; j++)
                {
                    sortedSolutionsList.Add(allsolutions.ElementAt(i).Value.ElementAt(j));
                }
            }

            if (index < sortedSolutionsList.Count)
            {
                return sortedSolutionsList.ElementAt(index);
            }
            else
            {
                return null;
            }
        }

        public int[] swapOperator(int item1, int item2, int[] solutionIndices)
        {
            int[] copyOfIndices = new int[solutionIndices.Length];
            Array.Copy(solutionIndices, 0, copyOfIndices, 0, copyOfIndices.Length);
            int temp = copyOfIndices[item1];
            copyOfIndices[item1] = copyOfIndices[item2];
            copyOfIndices[item2] = temp;
            return copyOfIndices;
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

        private List<Item> generateInitialSolutionList(List<Item> itemList, InitialSolutionType type, Item picker)
        {
            List<Item> cloneOfList = Utils.Clone<Item>(itemList);
            switch (type)
            {
                case InitialSolutionType.Ordered:
                    cloneOfList.Sort();
                    if (picker != null)
                    {
                        cloneOfList.Insert(0, picker); //adding starting point to the start of the list
                    }
                    break;
                case InitialSolutionType.Random:
                    Utils.Shuffle<Item>(cloneOfList);
                    if (picker != null)
                    {
                        cloneOfList.Insert(0, picker); //adding starting point to the start of the list
                    }
                    break;
                case InitialSolutionType.Greedy:
                    cloneOfList = prepareGreedySolution(picker);
                    break;
                default:
                    cloneOfList.Sort();
                    break;
            }
            return cloneOfList;
        }

        private List<Item> prepareGreedySolution(Item picker)
        {
            List<Item> solutionListOfItems = new List<Item>();
            IEnumerable<int> enumerable = Enumerable.Range(0, this.distances.GetLength(0));
            List<int> availableItemIndices = enumerable.ToList();
            if (this.distances.GetLength(0) < 2)
            {
                if (picker == null)
                {
                    return null;
                }
                else
                {
                    solutionListOfItems = new List<Item>();
                    solutionListOfItems.Add(picker);
                    return solutionListOfItems;
                }
            }
            double minValue = this.distances[0, 1];
            int selectedFirstIndex = 0;
            int selectedSecondındex = 1;
            for (int i = 0; i < this.distances.GetLength(0); i++)
            {
                for (int j = 0; j < this.distances.GetLength(1); j++)
                {
                    if (j > i && this.distances[i, j] < minValue)
                    {
                        minValue = this.distances[i, j];
                        selectedFirstIndex = i;
                        selectedSecondındex = j;
                    }
                }
            }
            solutionListOfItems = addItemToSolutionList(solutionListOfItems, selectedFirstIndex);
            solutionListOfItems = addItemToSolutionList(solutionListOfItems, selectedSecondındex);
            availableItemIndices.Remove(selectedFirstIndex);
            availableItemIndices.Remove(selectedSecondındex);

            for (int i = 0; i < this.distances.GetLength(0) - 2; i++)
            {
                int selectedIndex = availableItemIndices.ElementAt(0);
                double minNextValue = this.distances[solutionListOfItems.ElementAt(solutionListOfItems.Count - 1).Index, selectedIndex];
                for (int j = 0; j < this.distances.GetLength(1); j++)
                {
                    if (j != solutionListOfItems.ElementAt(solutionListOfItems.Count - 1).Index)
                    {
                        if (this.distances[solutionListOfItems.ElementAt(solutionListOfItems.Count - 1).Index, j] < minNextValue && availableItemIndices.Contains(j))
                        {
                            minNextValue = this.distances[solutionListOfItems.ElementAt(solutionListOfItems.Count - 1).Index, j];
                            selectedIndex = j;
                        }
                    }
                }
                solutionListOfItems = addItemToSolutionList(solutionListOfItems, selectedIndex);
                availableItemIndices.Remove(selectedIndex);
            }
            return solutionListOfItems;
        }

        private List<Item> addItemToSolutionList(List<Item> itemList, int itemIndex)
        {
            Item pickerItem = new Item(0, this.problem.NumberOfCrossAisles - 1, 1, 0, this.problem.S);
            if (itemList == null || (!this.indexedItemDictionary.ContainsKey(itemIndex) && itemIndex != 0))
            {
                return itemList;
            }

            if (itemIndex == 0)
            {
                itemList.Add(pickerItem);
            }
            else
            {
                itemList.Add(this.indexedItemDictionary[itemIndex]);
            }
            return itemList;
        }

        private void solveUsingTabuSearch(int tabuLength, Item picker)
        {
            //preparing distance matrix
            stopWatch = Stopwatch.StartNew();
            prepareDistanceMatrix(picker);
            stopWatch.Stop();
            TimeSpan dmElapsed_Time = stopWatch.Elapsed;
            double dmElapsedTime = Math.Round((dmElapsed_Time).TotalSeconds, 3);
            //double dmElapsedTime = Math.Round(((double)stopwatch.ElapsedMilliseconds)/1000, 3);
            this.distanceMatrixRunningTime = dmElapsedTime;

            double totalBestCost = 0;
            stopWatch = Stopwatch.StartNew();
            for (int j = 0; j < this.problem.ItemList.Count; j++)
            {
                //generating an initial solution list
                List<Item> initialSolutionList = generateInitialSolutionList(this.problem.ItemList, InitialSolutionType.Greedy, picker);
                
                //Tabu Search is using only item indexes to define solution
                int[] currentSolution = new int[initialSolutionList.Count];
                for (int i = 0; i < initialSolutionList.Count; i++)
                {
                    currentSolution[i] = initialSolutionList[i].Index;
                }

                TabuList tabuList = new TabuList(this.distances.GetLength(0), tabuLength);

                int[] bestSolution = new int[currentSolution.GetLength(0)];
                Array.Copy(currentSolution, 0, bestSolution, 0, bestSolution.GetLength(0));
                double bestCost = calculateTabuSearchObjectiveFunctionValue(bestSolution);
                if (j == 0){
                    totalBestCost = bestCost;
                }

                int numberOfIterations = initialSolutionList.Count - 1;
                //int numberOfIterations = 10000;
                int counter = 0;
                int iterationCount = 0;

                //while (iterationCount < numberOfIterations)
                while (counter < numberOfIterations)
                {
                    currentSolution = getBestNeighbour(tabuList, currentSolution, bestCost);
                    //printTabuPath(currentSolution);
                    //tabuList.printTabuList();

                    double currentCost = calculateTabuSearchObjectiveFunctionValue(currentSolution);
                    if (currentCost < bestCost)
                    {
                        Array.Copy(currentSolution, 0, bestSolution, 0, bestSolution.GetLength(0));
                        bestCost = currentCost;
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                    iterationCount++;
                }

                if (bestCost < totalBestCost)
                {
                    totalBestCost = bestCost;
                }
            }
            //Console.WriteLine("\n\nSearch done! \nBest Solution cost found = " + bestCost + "\nBest Solution :");
            this.totalTravelledDistance = totalBestCost;

            //printTabuPath(bestSolution);
            stopWatch.Stop();
            TimeSpan elapsed_Time = stopWatch.Elapsed;
            double elapsedTime = Math.Round((elapsed_Time).TotalSeconds, 3);
            //double elapsedTime = Math.Round(((double)stopwatch.ElapsedMilliseconds) / 1000, 3);
            this.runningTime = elapsedTime;
        }

        public void solveUsingSShape()
        {
            picker.printLocation();
            picker.NextHorizontalMove = true;
            picker.NextVerticalMove = false;
            picker.goToLocation(picker.AInfo, this.problem.LeftPickAisle, this.problem);
            picker.goToLocation(this.problem.FarthestBlock + 1, picker.BInfo, this.problem);
            List<int> pickAisles = this.problem.getPickAislesOfBlock(this.problem.FarthestBlock);
            picker.goToLocation(picker.AInfo, pickAisles.ElementAt(0), this.problem);
            int farMostBlock;
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                picker.collectAisle(!picker.NextVerticalMove, true, this.problem);
                farMostBlock = this.problem.FarthestBlock + 1;
            }
            else
            {
                picker.goToLocation(this.problem.FarthestBlock, picker.BInfo, this.problem);
                farMostBlock = this.problem.FarthestBlock;
            }
            while (farMostBlock < this.problem.Depot.Y)
            {
                pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count > 0)
                {
                    if (Math.Abs(picker.BInfo - pickAisles.ElementAt(0)) //leftMostSubAisleB
                        < Math.Abs(picker.BInfo - pickAisles.ElementAt(pickAisles.Count - 1))) //rightMostSubAisleB
                    {
                        Console.WriteLine("LEFT MOST SUB AISLE IS SELECTED");
                        picker.NextHorizontalMove = true;
                    }
                    else
                    {
                        Console.WriteLine("RIGHT MOST SUB AISLE IS SELECTED");
                        picker.NextHorizontalMove = false;
                    }
                    if (picker.NextHorizontalMove == false)
                    {
                        pickAisles.Reverse();
                    }
                    for (int i = 0; i < pickAisles.Count-1; i++)
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(i), this.problem);
                        picker.goToLocation(farMostBlock + (picker.NextVerticalMove ? 0 : 1), picker.BInfo, this.problem);
                        picker.NextVerticalMove = !picker.NextVerticalMove;
                    }
                    picker.goToLocation(picker.AInfo, pickAisles.ElementAt(pickAisles.Count - 1), this.problem);
                    if (picker.NextVerticalMove)
                    {
                        picker.collectAisle(picker.NextVerticalMove, true, this.problem);
                    }
                    else
                    {
                        picker.goToLocation(farMostBlock + 1, picker.BInfo, this.problem);
                    }
                }
                else
                {
                    picker.goToLocation(farMostBlock + 1, picker.BInfo, this.problem);
                }
                picker.NextVerticalMove = false;
                farMostBlock++;
            }
            picker.goToLocation(picker.AInfo, this.problem.Depot.X, this.problem);
            this.totalTravelledDistance = picker.Distance;
            picker.printAllGatheredData();
        }

        public Aisle getLargestGapLimits(List<Item> aisleItems)
        {
            Aisle result = new Aisle();
            if (aisleItems == null || aisleItems.Count == 0)
            {
                return result;
            }

            //setting helper array
            int[] numbers = new int[aisleItems.Count + 2];
            numbers[0] = 0;

            for (int i = 0; i < aisleItems.Count; i++)
            {
                numbers[i + 1] = aisleItems.ElementAt(i).DInfo;
            }
            numbers[numbers.Length - 1] = this.problem.S + 1;

            int max = numbers[0];
            int min = numbers[0];

            for (int i = 1; i < numbers.Length; i++)
            {
                max = Math.Max(max, numbers[i]);
                min = Math.Min(min, numbers[i]);
            }

            // initialize an array of buckets
            AisleBucket[] buckets = new AisleBucket[numbers.Length + 1]; //project to (0 - n)
            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new AisleBucket();
            }

            double interval = (double)numbers.Length / (max - min);
            //distribute every number to a bucket array
            for (int i = 0; i < numbers.Length; i++)
            {
                int index = (int)((numbers[i] - min) * interval);

                if (buckets[index].Low == -1)
                {
                    buckets[index].Low = numbers[i];
                    buckets[index].High = numbers[i];
                }
                else
                {
                    buckets[index].Low = Math.Min(buckets[index].Low, numbers[i]);
                    buckets[index].High = Math.Max(buckets[index].High, numbers[i]);
                }
            }

            //scan buckets to find maximum gap
            int largestGapDistance = 0;
            int prev = buckets[0].High;
            for (int i = 1; i < buckets.Length; i++)
            {
                if (buckets[i].Low != -1)
                {
                    if ((buckets[i].Low - prev) > largestGapDistance)
                    {
                        largestGapDistance = buckets[i].Low - prev;
                        result.LowLargestGap = prev;
                        result.HighLargestGap = buckets[i].Low;
                    }
                    prev = buckets[i].High;
                }
            }

            return result;
        }

        public void solveUsingLargestGap()
        {
            picker.printLocation();
            picker.NextHorizontalMove = true;
            picker.NextVerticalMove = false;
            picker.goToLocation(picker.AInfo, this.problem.LeftPickAisle, this.problem);
            picker.goToLocation(this.problem.FarthestBlock + 1, picker.BInfo, this.problem);
            List<int> pickAisles = this.problem.getPickAislesOfBlock(this.problem.FarthestBlock);
            picker.goToLocation(picker.AInfo, pickAisles.ElementAt(0), this.problem);
            int farMostBlock;
            bool isItOnlyOne = (pickAisles.Count == 1);
            if (isItOnlyOne)
            {
                picker.collectAisle(!picker.NextVerticalMove, true, this.problem);
                farMostBlock = this.problem.FarthestBlock + 1;
            }
            else
            {
                picker.goToLocation(this.problem.FarthestBlock, picker.BInfo, this.problem);
                farMostBlock = this.problem.FarthestBlock;
            }
            while (farMostBlock < this.problem.Depot.Y)
            {
                pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                if (pickAisles.Count > 0)
                {
                    if (Math.Abs(picker.BInfo - pickAisles.ElementAt(0)) //leftMostSubAisleB
                        < Math.Abs(picker.BInfo - pickAisles.ElementAt(pickAisles.Count - 1))) //rightMostSubAisleB
                    {
                        Console.WriteLine("LEFT MOST SUB AISLE IS SELECTED");
                        picker.NextHorizontalMove = true;
                    }
                    else
                    {
                        Console.WriteLine("RIGHT MOST SUB AISLE IS SELECTED");
                        picker.NextHorizontalMove = false;
                    }
                    if (picker.NextHorizontalMove == false)
                    {
                        pickAisles.Reverse();
                    }
                    for (int i = 0; i < pickAisles.Count-1; i++)
                    {
                        Aisle aisle = getLargestGapLimits(this.problem.getNonPickedAisleItems(picker.AInfo, pickAisles.ElementAt(i)));
                        if (aisle.LowLargestGap > 0)
                        {
                            picker.goToLocation(picker.AInfo, pickAisles.ElementAt(i), this.problem);
                            picker.collectLGAisle(picker.NextVerticalMove, aisle, this.problem);
                        }
                    }
                    picker.goToLocation(picker.AInfo, pickAisles.ElementAt(pickAisles.Count - 1), this.problem);
                    picker.goToLocation(farMostBlock + 1, picker.BInfo, this.problem);
                    picker.NextHorizontalMove = !picker.NextHorizontalMove;
                    picker.NextVerticalMove = true;
                    pickAisles = this.problem.getPickAislesOfBlock(farMostBlock);
                    if (picker.NextHorizontalMove == false)
                    {
                        pickAisles.Reverse();
                    }
                    for (int i = 0; i < pickAisles.Count; i++)
                    {
                        picker.goToLocation(picker.AInfo, pickAisles.ElementAt(i), this.problem);
                        Aisle aisle = getLargestGapLimits(this.problem.getNonPickedAisleItems(picker.AInfo - 1, pickAisles.ElementAt(i)));
                        picker.collectLGAisle(picker.NextVerticalMove, aisle, this.problem);
                    }
                }
                else
                {
                    picker.goToLocation(farMostBlock + 1, picker.BInfo, this.problem);
                }
                picker.NextVerticalMove = false;
                farMostBlock++;
            }
            picker.goToLocation(picker.AInfo, this.problem.Depot.X, this.problem);
            this.totalTravelledDistance = picker.Distance;
            picker.printAllGatheredData();
        }

        public void solveUsingGeneticAlgorithm()
        {

        }
    }
}
