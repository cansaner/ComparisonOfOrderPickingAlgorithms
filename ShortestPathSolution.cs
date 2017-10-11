using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class ShortestPathSolution
    {
        private List<Coordinate> currentPath;
        private List<Coordinate> shortestPath;

        public List<Coordinate> ShortestPath
        {
            get
            {
                return shortestPath;
            }
            set
            {
                shortestPath = value;
            }
        }

        private double shortestDistance;

        public double ShortestDistance
        {
            get
            {
                return shortestDistance;
            }
            set
            {
                shortestDistance = value;
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

        public ShortestPathSolution(Problem room)
        {
            this.problem = room;
        }

        private int MinimumDistance(double[] distance, bool[] shortestPathTreeSet)
        {
            double min = double.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < this.problem.GraphOfJunctions.GetLength(0); ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }
        
        private double solveWithDijkstraForJunctions(int source, int destination)
        {
            double[] distance = new double[this.problem.GraphOfJunctions.GetLength(0)];
            bool[] shortestPathTreeSet = new bool[this.problem.GraphOfJunctions.GetLength(0)];
            List<Coordinate>[] paths = new List<Coordinate>[this.problem.GraphOfJunctions.GetLength(0)];

            for (int i = 0; i < this.problem.GraphOfJunctions.GetLength(0); ++i)
            {
                distance[i] = double.MaxValue;
                shortestPathTreeSet[i] = false;
            }

            distance[source] = 0;
            List<Coordinate> startPath = new List<Coordinate>();
            startPath.Add(this.problem.convertGraphOfJunctionPointsIndexToCoordinate(source));
            paths[source] = startPath;

            for (int count = 0; count < this.problem.GraphOfJunctions.GetLength(0) - 1; ++count)
            {
                int u = MinimumDistance(distance, shortestPathTreeSet);
                if (u == destination)
                {
                    break;
                }
                shortestPathTreeSet[u] = true;

                for (int v = 0; v < this.problem.GraphOfJunctions.GetLength(0); ++v)
                    if (!shortestPathTreeSet[v] && Convert.ToBoolean(this.problem.GraphOfJunctions[u, v]) && distance[u] != int.MaxValue && distance[u] + this.problem.GraphOfJunctions[u, v] < distance[v])
                    {
                        distance[v] = distance[u] + this.problem.GraphOfJunctions[u, v];
                        List<Coordinate> path = Utils.Clone<Coordinate>(paths[u]);
                        path.Add(this.problem.convertGraphOfJunctionPointsIndexToCoordinate(v));
                        paths[v] = path;
                    }
            }

            double travelledDistance = distance[destination];
            this.currentPath = paths[destination];
            //PrintSolution(distance, paths, source, destination);
            return travelledDistance;
        }

        public void Solve(Item sourceItem, Item destinationItem)
        {
            double[] solution = new double[4];
            List<Coordinate>[] path = new List<Coordinate>[4];

            double firstDistanceOfShelves = (this.problem.K * (sourceItem.DInfo - 1)) + (this.problem.K * (destinationItem.DInfo - 1));
            solution[0] = firstDistanceOfShelves + solveWithDijkstraForJunctions(getGraphOfJunctionPointsIndexOnBackAisleOfItem(sourceItem), getGraphOfJunctionPointsIndexOnBackAisleOfItem(destinationItem));
            path[0] = Utils.Clone<Coordinate>(this.currentPath);

            double secondDistanceOfShelves = (this.problem.K * (sourceItem.DInfo - 1)) + (this.problem.K * (this.problem.S - destinationItem.DInfo));
            solution[1] = secondDistanceOfShelves + solveWithDijkstraForJunctions(getGraphOfJunctionPointsIndexOnBackAisleOfItem(sourceItem), getGraphOfJunctionPointsIndexOnBackAisleOfItem(destinationItem) + this.problem.NumberOfAisles);
            path[1] = Utils.Clone<Coordinate>(this.currentPath);

            double thirdDistanceOfShelves = (this.problem.K * (this.problem.S - sourceItem.DInfo)) + (this.problem.K * (destinationItem.DInfo - 1));
            solution[2] = thirdDistanceOfShelves + solveWithDijkstraForJunctions(getGraphOfJunctionPointsIndexOnBackAisleOfItem(sourceItem) + this.problem.NumberOfAisles, getGraphOfJunctionPointsIndexOnBackAisleOfItem(destinationItem));
            path[2] = Utils.Clone<Coordinate>(this.currentPath);

            double fourthDistanceOfShelves = (this.problem.K * (this.problem.S - sourceItem.DInfo)) + (this.problem.K * (this.problem.S - destinationItem.DInfo));
            solution[3] = fourthDistanceOfShelves + solveWithDijkstraForJunctions(getGraphOfJunctionPointsIndexOnBackAisleOfItem(sourceItem) + this.problem.NumberOfAisles, getGraphOfJunctionPointsIndexOnBackAisleOfItem(destinationItem) + this.problem.NumberOfAisles);
            path[3] = Utils.Clone<Coordinate>(this.currentPath);
            
            double bestSolution = solution[0];
            List<Coordinate> bestPath = path[0];

            for (int count = 1; count < solution.Length; count++)
            {
                if (solution[count] < bestSolution)
                {
                    bestSolution = solution[count];
                    bestPath = path[count];
                }
            }

            this.shortestDistance = bestSolution;
            this.shortestPath = bestPath;
        }

        private int getGraphOfJunctionPointsIndexOnBackAisleOfItem(Item item)
        {
            int verticalAisleIndex = 0;
            if (item.CInfo == 0)
            {
                verticalAisleIndex = item.BInfo;
            }
            else
            {
                verticalAisleIndex = item.BInfo + 1;
            }
            int horizontalAisleIndex = item.AInfo;

            return ((horizontalAisleIndex - 1) * this.problem.NumberOfAisles) + (verticalAisleIndex - 1);
        }

        private void PrintSolution(double[] distance, List<Coordinate>[] paths, int source, int destination)
        {
            Console.WriteLine("Vertices\tDistance\tPath");
            Console.WriteLine("({0}---->{1})\t{2}\t{3}", source, destination, distance[destination], stringOfPath(paths[destination]));
        }

        public static String stringOfPath(List<Coordinate> pathList)
        {
            if (pathList == null)
            {
                return "NO_PATH_VALUE";
            }

            String path = "";
            for (int i = 0; i < pathList.Count; i++)
            {
                path += "(";
                path += pathList[i].Y;
                path += ",";
                path += pathList[i].X;
                path += ")";
                if (i != pathList.Count - 1)
                {
                    path += " -> ";
                }
            }
            return path;
        }
    }
}
