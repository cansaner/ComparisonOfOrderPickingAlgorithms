using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Picker
    {
        private Coordinate location;

        public Coordinate Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public int AInfo
        {
            get
            {
                return location.Y;
            }
            set
            {
                location.Y = value;
            }
        }

        public int BInfo
        {
            get
            {
                return location.X;
            }
            set
            {
                location.X = value;
            }
        }

        private bool right;

        public bool NextHorizontalMove
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        private bool up;

        public bool NextVerticalMove
        {
            get
            {
                return up;
            }
            set
            {
                up = value;
            }
        }

        private int numberOfPickedItems;

        public int NumberOfPickedItems
        {
            get
            {
                return numberOfPickedItems;
            }
            set
            {
                numberOfPickedItems = value;
            }
        }

        private List<Item> pickedItems;

        public List<Item> PickedItems
        {
            get
            {
                return pickedItems;
            }
            set
            {
                pickedItems = value;
            }
        }

        private List<Coordinate> path;

        public List<Coordinate> Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        private double distance;

        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        private List<LinkDistance> travelledDistances;

        public List<LinkDistance> TravelledDistances
        {
            get
            {
                return travelledDistances;
            }
            protected set
            {
                travelledDistances = value;
            }
        }

        public Picker(Coordinate depotLocation)
        {
            this.location = new Coordinate(depotLocation.X, depotLocation.Y);
            this.distance = 0;
            this.numberOfPickedItems = 0;
            this.path = new List<Coordinate>();
            this.path.Add(this.location);
            this.travelledDistances = new List<LinkDistance>();
            this.pickedItems = new List<Item>();
            Console.WriteLine("Picker is dropped into the room");
        }

        public Picker(int x, int y) : this(new Coordinate(x,y)) {
        }

        public void addToDistance(double increment){
            this.distance = this.distance + increment;
        }

        public void addToPath(Coordinate coordinate)
        {
            this.path.Add(coordinate);
        }

        public void addToTravelledDistances(LinkDistance distance)
        {
            this.travelledDistances.Add(distance);
            Console.WriteLine("Picker is travelled {0}{1}", distance.Count, distance.Code);
        }

        public void addToPickedItems(Item item)
        {
            this.pickedItems.Add(item);
        }

        public void goToLocation(Coordinate coord, Problem room)
        {
            goToLocation(coord.Y, coord.X, room);
        }

        public void goToLocation(int aLoc, int bLoc, Problem room)
        {
            LinkDistance travelledDistance;
            double distanceToIncrement;
            if (aLoc == this.AInfo)
            {
                if (bLoc == this.BInfo)
                {
                    Console.WriteLine("Destination location is same with picker location. Picker is not moved any coordinate");
                    return;
                }
                else
                {
                    //Picker is going horizontal
                    if (bLoc > this.BInfo)
                    {
                        Console.WriteLine("Picker went right");
                    }
                    else
                    {
                        Console.WriteLine("Picker went left");
                    }
                    Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, aLoc, bLoc);
                    travelledDistance = new LinkDistance(Math.Abs(bLoc - this.BInfo), Problem.Codes.W);
                    distanceToIncrement = Math.Abs(bLoc - this.BInfo) * room.W;
                    this.location = new Coordinate(bLoc, this.AInfo);
                    this.addToPath(this.location);
                    this.addToTravelledDistances(travelledDistance);
                    this.addToDistance(distanceToIncrement);
                }
            }
            else
            {
                if (bLoc == this.BInfo)
                {
                    //Picker is going vertical
                    //If picker's vertical direction is important. It is here to look for.
                    if (aLoc > this.AInfo)
                    {
                        //collecting every aisle traversed while going vertical
                        for (int a = this.AInfo; a < aLoc; a = a + 1)
                        {
                            collectAisle(false, false, room);
                            Console.WriteLine("Picker went down");
                            Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, a + 1, this.BInfo);
                            travelledDistance = new LinkDistance(Math.Abs(a + 1 - this.AInfo), Problem.Codes.L);
                            distanceToIncrement = Math.Abs(a + 1 - this.AInfo) * room.L;
                            this.location = new Coordinate(this.BInfo, a + 1);
                            this.addToPath(this.location);
                            this.addToTravelledDistances(travelledDistance);
                            this.addToDistance(distanceToIncrement);
                        }
                    }
                    else // aLoc can not be same with this.AInfo already...
                    {
                        for (int a = this.AInfo; a > aLoc; a = a - 1)
                        {
                            collectAisle(true, false, room);
                            Console.WriteLine("Picker went up");
                            Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, a - 1, this.BInfo);
                            travelledDistance = new LinkDistance(Math.Abs(a - 1 - this.AInfo), Problem.Codes.L);
                            distanceToIncrement = Math.Abs(a - 1 - this.AInfo) * room.L;
                            this.location = new Coordinate(this.BInfo, a - 1);
                            this.addToPath(this.location);
                            this.addToTravelledDistances(travelledDistance);
                            this.addToDistance(distanceToIncrement);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Picker can not move diagonally. Picker is not moved any coordinate");
                    return;
                }

            }
        }

        public void collectLGAisle(bool up, Aisle aisle, Problem problem)
        {
            int aPos;
            if (up)
            {
                aPos = this.AInfo - 1;
            }
            else
            {
                aPos = this.AInfo;
            }
            List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPos, this.BInfo));

            if (sortedAisleItems.Count == 0)
            {
                return;
            }
            if (up)
            {
                sortedAisleItems.Reverse();
            }
            foreach (Item i in sortedAisleItems)
            {
                if (up)
                {
                    if (i.DInfo >= aisle.HighLargestGap)
                    {
                        if (i.Picked == false)
                        {
                            i.Picked = true;
                            this.addToPickedItems(i);
                        }
                    }
                }
                else
                {
                    if (i.DInfo <= aisle.LowLargestGap)
                    {
                        if (i.Picked == false)
                        {
                            i.Picked = true;
                            this.addToPickedItems(i);
                        }
                    }
                }
            }

            if (up)
            {
                this.addToTravelledDistances(new LinkDistance(Math.Abs(aisle.HighLargestGap - problem.S - 1), Problem.Codes.K));
                this.addToTravelledDistances(new LinkDistance(Math.Abs(aisle.HighLargestGap - problem.S - 1), Problem.Codes.K));
                this.addToDistance(Math.Abs(aisle.HighLargestGap - problem.S - 1) * problem.K * 2);
            }
            else
            {
                this.addToTravelledDistances(new LinkDistance(aisle.LowLargestGap, Problem.Codes.K));
                this.addToTravelledDistances(new LinkDistance(aisle.LowLargestGap, Problem.Codes.K));
                this.addToDistance(aisle.LowLargestGap * problem.K * 2);
            }
        }

        public void collectAisle(bool up, bool enterExitSame, Problem problem)
        {
            int aPos;
            if (up)
            {
                aPos = this.AInfo - 1;
            }
            else
            {
                aPos = this.AInfo;
            }
            List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPos, this.BInfo));
            if (sortedAisleItems.Count == 0)
            {
                return;
            }
            if (up)
            {
                sortedAisleItems.Reverse();
            }
            foreach (Item i in sortedAisleItems)
            {
                if (i.Picked == false)
                {
                    i.Picked = true;
                    this.addToPickedItems(i);
                }
            }
            if (enterExitSame)
            {
                if (up)
                {
                    this.addToTravelledDistances(new LinkDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1), Problem.Codes.K));
                    this.addToTravelledDistances(new LinkDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1), Problem.Codes.K));
                    this.addToDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1) * problem.K * 2);
                }
                else
                {
                    this.addToTravelledDistances(new LinkDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo, Problem.Codes.K));
                    this.addToTravelledDistances(new LinkDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo, Problem.Codes.K));
                    this.addToDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo * problem.K * 2);
                }
            }
        }

        //PRINT FUNCTIONS
        public void printLocation()
        {
            Console.WriteLine("Picker is on location: {0}, {1}", this.AInfo, this.BInfo);
        }

        public double printTravelledTotalDistance()
        {
            return this.distance;
        }

        public String printPath()
        {
            String path = "";
            for (int i = 0; i < this.path.Count; i++)
            {
                path += "(";
                path += this.path[i].Y;
                path += ",";
                path += this.path[i].X;
                path += ")";
                if (i != this.path.Count - 1)
                {
                    path += " -> ";
                }
            }
            return path;
        }

        public String printTravelledDistances()
        {
            String travelledDistances = "";
            for (int i = 0; i < this.travelledDistances.Count; i++)
            {
                travelledDistances += this.travelledDistances[i].Count;
                travelledDistances += this.travelledDistances[i].Code;
                if (i != this.travelledDistances.Count - 1)
                {
                    travelledDistances += " + ";
                }
            }
            return travelledDistances;
        }

        public String printPickedItems()
        {
            String pickedItems = "";
            for (int i = 0; i < this.pickedItems.Count; i++)
            {
                pickedItems += this.pickedItems[i].Index;
                if (i != this.pickedItems.Count - 1)
                {
                    pickedItems += " -> ";
                }
            }
            return pickedItems;
        }

        public void printAllGatheredData()
        {
            Console.WriteLine("PICKER IS FINISHED ITS JOB");
            Console.WriteLine("Travelled path: " + printPath());
            Console.WriteLine();
            Console.WriteLine("Travelled distances: " + printTravelledDistances());
            Console.WriteLine();
            Console.WriteLine("Item Picking Sequence: " + printPickedItems());
            Console.WriteLine();
            Console.WriteLine("Picker travelled {0}M totally", printTravelledTotalDistance());
        }
    }
}
