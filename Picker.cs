using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Picker
    {
        public enum FaceType { Up, Down, Right, Left};

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

        private FaceType face;

        public FaceType Face
        {
            get
            {
                return face;
            }
            protected set
            {
                face = value;
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
                            collectAisle(a, this.BInfo, false, false, Problem.AislePart.All, room);
                            Console.WriteLine("Picker went down");
                            Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, a + 1, this.BInfo);
                            travelledDistance = new LinkDistance(Math.Abs(a + 1 - this.AInfo), Problem.Codes.L);
                            distanceToIncrement = Math.Abs(a + 1 - this.AInfo) * room.L;
                            this.location = new Coordinate(this.BInfo, a + 1);
                            this.addToPath(this.location);
                            this.addToTravelledDistances(travelledDistance);
                            this.addToDistance(distanceToIncrement);
                        }

                        //if (Math.Abs(aLoc - this.AInfo) > 1)
                        //{
                        //    //collecting every aisle traversed while going vertical
                        //    for (int a = this.AInfo; a < aLoc; a = a + 1)
                        //    {
                        //        collectAisle(a, this.BInfo, false, false, Problem.AislePart.All, room);
                        //        goToLocation(a + 1, this.BInfo, room);
                        //    }
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Picker went down");
                        //    Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, aLoc, bLoc);
                        //    travelledDistance = new LinkDistance(Math.Abs(aLoc - this.AInfo), Problem.Codes.L);
                        //    distanceToIncrement = Math.Abs(aLoc - this.AInfo) * room.L;
                        //    this.location = newLocation;
                        //    this.addToPath(newLocation);
                        //    this.addToTravelledDistances(travelledDistance);
                        //    this.addToDistance(distanceToIncrement);
                        //}
                    }
                    else // aLoc can not be same with this.AInfo already...
                    {
                        for (int a = this.AInfo; a > aLoc; a = a - 1)
                        {
                            collectAisle(a - 1, this.BInfo, true, false, Problem.AislePart.All, room);
                            Console.WriteLine("Picker went up");
                            Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, a - 1, this.BInfo);
                            travelledDistance = new LinkDistance(Math.Abs(a - 1 - this.AInfo), Problem.Codes.L);
                            distanceToIncrement = Math.Abs(a - 1 - this.AInfo) * room.L;
                            this.location = new Coordinate(this.BInfo, a - 1);
                            this.addToPath(this.location);
                            this.addToTravelledDistances(travelledDistance);
                            this.addToDistance(distanceToIncrement);
                        }
                        //if (Math.Abs(aLoc - this.AInfo) > 1)
                        //{
                        //    //collecting every aisle traversed while going vertical
                        //    for (int a = this.AInfo; a > aLoc; a = a - 1)
                        //    {
                        //        collectAisle(a - 1, this.BInfo, true, false, Problem.AislePart.All, room);
                        //        goToLocation(a - 1, this.BInfo, room);
                        //    }
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Picker went up");
                        //    Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, aLoc, bLoc);
                        //    travelledDistance = new LinkDistance(Math.Abs(aLoc - this.AInfo), Problem.Codes.L);
                        //    distanceToIncrement = Math.Abs(aLoc - this.AInfo) * room.L;
                        //    this.location = newLocation;
                        //    this.addToPath(newLocation);
                        //    this.addToTravelledDistances(travelledDistance);
                        //    this.addToDistance(distanceToIncrement);
                        //}
                    }
                }
                else
                {
                    Console.WriteLine("Picker can not move diagonally. Picker is not moved any coordinate");
                    return;
                }

            }
        }

        //public void goToLocation(int aLoc, int bLoc, LinkDistance travelledDistance, double distanceToIncrement)
        //{
        //    if (aLoc == this.AInfo && bLoc == this.BInfo)
        //    {
        //        Console.WriteLine("Picker is not moved any coordinate");
        //        return;
        //    }
        //    Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, aLoc, bLoc);
        //    Coordinate newLocation = new Coordinate(bLoc, aLoc);
        //    this.location = newLocation;
        //    this.addToPath(newLocation);
        //    this.addToTravelledDistances(travelledDistance);
        //    this.addToDistance(distanceToIncrement);
        //}

        //public void goVertical(int stop, Problem problem)
        //{
        //    int start = this.AInfo;
        //    if (start < stop)
        //    {
        //        for (int a = start; a < stop; a = a + 1)
        //        {
        //            collectAisle(a, this.BInfo, false, false, Problem.AislePart.All, problem);
        //            goToLocation(a + 1, this.BInfo, new LinkDistance(1, Problem.Codes.L), 1 * problem.L);
        //        }
        //    }
        //    else
        //    {
                
        //        for (int a = start; a > stop; a = a - 1)
        //        {
        //            collectAisle(a - 1, this.BInfo, true, false, Problem.AislePart.All, problem);
        //            goToLocation(a - 1, this.BInfo, new LinkDistance(1, Problem.Codes.L), 1 * problem.L);
        //        }
        //    }
        //}

        //public void collectAisle(int aPos, int bPos, bool up, bool enterExitSame, Problem.AislePart aislePart, Problem problem)
        //{
        //    List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPos, this.BInfo, aislePart));
        //    if (sortedAisleItems.Count == 0)
        //    {
        //        return;
        //    }
        //    if (up)
        //    {
        //        sortedAisleItems.Reverse();
        //    }
        //    foreach (Item i in sortedAisleItems)
        //    {
        //        if (i.Picked == false)
        //        {
        //            i.Picked= true;
        //            this.addToPickedItems(i);
        //        }
        //    }
        //    if (enterExitSame)
        //    {
        //        if (up)
        //        {
        //            this.addToTravelledDistances(new LinkDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1), Problem.Codes.K));
        //            this.addToTravelledDistances(new LinkDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1), Problem.Codes.K));
        //            this.addToDistance(Math.Abs(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo - problem.S - 1) * problem.K * 2);
        //        }
        //        else
        //        {
        //            this.addToTravelledDistances(new LinkDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo, Problem.Codes.K));
        //            this.addToTravelledDistances(new LinkDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo, Problem.Codes.K));
        //            this.addToDistance(sortedAisleItems.ElementAt(sortedAisleItems.Count() - 1).DInfo * problem.K * 2);
        //        }
        //    }
        //}

        public void collectLGAisle(bool up, Aisle aisle, Problem problem)
        {
            int aPosit;
            if (up)
            {
                aPosit = this.AInfo - 1;
            }
            else
            {
                aPosit = this.AInfo;
            }
            List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPosit, this.BInfo, Problem.AislePart.All));

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

        //public Aisle getLargestGapLimit(List<Item> aisleItems, bool up, int numberOfShelves)
        //{
        //    if (aisleItems == null || aisleItems.Count == 0)
        //    {
        //        return -1;
        //    }

        //    //setting helper array
        //    int[] numbers = new int[aisleItems.Count+2];
        //    numbers[0] = 0;

        //    for (int i = 0; i < aisleItems.Count; i++)
        //    {
        //        numbers[i+1] = aisleItems.ElementAt(i).DInfo;
        //    }
        //    numbers[numbers.Length - 1] = numberOfShelves + 1;

        //    int max = numbers[0];
        //    int min = numbers[0];

        //    for (int i = 1; i < numbers.Length; i++)
        //    {
        //        max = Math.Max(max, numbers[i]);
        //        min = Math.Min(min, numbers[i]);
        //    }

        //    // initialize an array of buckets
        //    AisleBucket[] buckets = new AisleBucket[numbers.Length + 1]; //project to (0 - n)
        //    for (int i = 0; i < buckets.Length; i++)
        //    {
        //        buckets[i] = new AisleBucket();
        //    }

        //    double interval = (double)numbers.Length / (max - min);
        //    //distribute every number to a bucket array
        //    for (int i = 0; i < numbers.Length; i++)
        //    {
        //        int index = (int)((numbers[i] - min) * interval);

        //        if (buckets[index].Low == -1)
        //        {
        //            buckets[index].Low = numbers[i];
        //            buckets[index].High = numbers[i];
        //        }
        //        else
        //        {
        //            buckets[index].Low = Math.Min(buckets[index].Low, numbers[i]);
        //            buckets[index].High = Math.Max(buckets[index].High, numbers[i]);
        //        }
        //    }

        //    int largestGapLow = -1;
        //    int largestGapHigh = -1;
        //    //scan buckets to find maximum gap
        //    int result = 0;
        //    int prev = buckets[0].High;
        //    for (int i = 1; i < buckets.Length; i++)
        //    {
        //        if (buckets[i].Low != -1)
        //        {
        //            if ((buckets[i].Low - prev) > result)
        //            {
        //                result = buckets[i].Low - prev;
        //                largestGapLow = prev;
        //                largestGapHigh = buckets[i].Low;
        //            }
        //            prev = buckets[i].High;
        //        }
        //    }

        //    List<Item> sortedAisleItems = problem.sortAisleItems(aisleItems);
        //    if (up)
        //    {
        //        sortedAisleItems.Reverse();
        //    }
        //    upDistance = aisleItems.ElementAt(0).DInfo - 1;
        //    downDistance = numberOfShelves - aisleItems.ElementAt(0).DInfo;

        //    if (up)
        //        return maxGapHigh;
        //    else
        //        return maxGapLow;

        //    //int largestGapLow = -1;
        //    //int largestGapHigh = -1;
        //    //int upDistance = 0;
        //    //int downDistance = 0;
        //    //if (aisleItems.Count == 1)
        //    //{
        //    //    upDistance = aisleItems.ElementAt(0).DInfo - 1;
        //    //    downDistance = numberOfShelves - aisleItems.ElementAt(0).DInfo;
        //    //    if (upDistance <= downDistance)
        //    //    {
        //    //        largestGapLow = aisleItems.ElementAt(0).DInfo;
        //    //        largestGapHigh = numberOfShelves + 1;
        //    //    }
        //    //    else
        //    //    {
        //    //        largestGapLow = 0;
        //    //        largestGapHigh = aisleItems.ElementAt(0).DInfo;
        //    //    }
        //    //    if (up)
        //    //    {
        //    //        if (largestGapHigh == numberOfShelves + 1)
        //    //            largestGapHigh = -1;
        //    //        return largestGapHigh;
        //    //    }
        //    //    else
        //    //    {
        //    //        if (largestGapLow == 0)
        //    //            largestGapLow = -1;
        //    //        return largestGapLow;
        //    //    }
        //    //}

        //    ////items more than 1
        //    //int max = aisleItems.ElementAt(0).DInfo;
        //    //int min = aisleItems.ElementAt(0).DInfo;

        //    //for (int i = 1; i < aisleItems.Count; i++)
        //    //{
        //    //    max = Math.Max(max, aisleItems.ElementAt(i).DInfo);
        //    //    min = Math.Min(min, aisleItems.ElementAt(i).DInfo);
        //    //}

        //    //// initialize an array of buckets
        //    //AisleBucket[] buckets = new AisleBucket[aisleItems.Count + 1]; //project to (0 - n)
        //    //for (int i = 0; i < buckets.Length; i++)
        //    //{
        //    //    buckets[i] = new AisleBucket();
        //    //}

        //    //double interval = (double)aisleItems.Count / (max - min);
        //    ////distribute every number to a bucket array
        //    //for (int i = 0; i < aisleItems.Count; i++)
        //    //{
        //    //    int index = (int)((aisleItems.ElementAt(i).DInfo - min) * interval);

        //    //    if (buckets[index].Low == -1)
        //    //    {
        //    //        buckets[index].Low = aisleItems.ElementAt(i).DInfo;
        //    //        buckets[index].High = aisleItems.ElementAt(i).DInfo;
        //    //    }
        //    //    else
        //    //    {
        //    //        buckets[index].Low = Math.Min(buckets[index].Low, aisleItems.ElementAt(i).DInfo);
        //    //        buckets[index].High = Math.Max(buckets[index].High, aisleItems.ElementAt(i).DInfo);
        //    //    }
        //    //}

        //    ////scan buckets to find maximum gap
        //    //int result = 0;
        //    //int prev = buckets[0].High;
        //    //for (int i = 1; i < buckets.Length; i++)
        //    //{
        //    //    if (buckets[i].Low != -1)
        //    //    {
        //    //        if ((buckets[i].Low - prev) > result)
        //    //        {
        //    //            result = buckets[i].Low - prev;
        //    //            largestGapLow = prev;
        //    //            largestGapHigh = buckets[i].Low;
        //    //        }
        //    //        prev = buckets[i].High;
        //    //    }
        //    //}

        //    //List<Item> sortedAisleItems = problem.sortAisleItems(aisleItems);
        //    //if (up)
        //    //{
        //    //    sortedAisleItems.Reverse();
        //    //}
        //    //upDistance = aisleItems.ElementAt(0).DInfo - 1;
        //    //downDistance = numberOfShelves - aisleItems.ElementAt(0).DInfo;

        //    //if (up)
        //    //    return maxGapHigh;
        //    //else
        //    //    return maxGapLow;
        //}

        public void collectAisle(int aPos, int bPos, bool up, bool enterExitSame, Problem.AislePart aislePart, Problem problem)
        {
            int aPosit;
            if (up)
            {
                aPosit = this.AInfo - 1;
            }
            else
            {
                aPosit = this.AInfo;
            }
            List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPosit, this.BInfo, aislePart));
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

        public void printTravelledTotalDistance()
        {
            Console.WriteLine("Picker travelled {0}M totally", this.distance);
        }

        public void printPath()
        {
            String path = "Travelled path: ";
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
            Console.WriteLine(path);
            Console.WriteLine();
        }

        public void printTravelledDistances()
        {
            String travelledDistances = "Travelled distances: ";
            for (int i = 0; i < this.travelledDistances.Count; i++)
            {
                travelledDistances += this.travelledDistances[i].Count;
                travelledDistances += this.travelledDistances[i].Code;
                if (i != this.travelledDistances.Count - 1)
                {
                    travelledDistances += " + ";
                }
            }
            Console.WriteLine(travelledDistances);
            Console.WriteLine();
        }

        public void printPickedItems()
        {
            String pickedItems = "Item Picking Sequence: ";
            for (int i = 0; i < this.pickedItems.Count; i++)
            {
                pickedItems += this.pickedItems[i].Index;
                if (i != this.pickedItems.Count - 1)
                {
                    pickedItems += " -> ";
                }
            }
            Console.WriteLine(pickedItems);
            Console.WriteLine();
        }
    }
}
