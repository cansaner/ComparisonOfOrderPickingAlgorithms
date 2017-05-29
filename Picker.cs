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

        public Picker(Coordinate location)
        {
            this.location = location;
            this.distance = 0;
            this.numberOfPickedItems = 0;
            this.path = new List<Coordinate>();
            this.path.Add(location);
            this.travelledDistances = new List<LinkDistance>();
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

        public void printTravelledTotalDistance(){
            Console.WriteLine("Picker travelled {0}M totally", this.distance);
        }

        public void printLocation()
        {
            Console.WriteLine("Picker is on location: {0}, {1}", this.AInfo, this.BInfo);
        }

        public void goToLocation(int aLoc, int bLoc, LinkDistance travelledDistance, double distanceToIncrement)
        {
            if (aLoc == this.AInfo && bLoc == this.BInfo)
            {
                Console.WriteLine("Picker is not moved any coordinate");
                return;
            }
            Console.WriteLine("Picker is moved from: ({0}, {1}) to: ({2}, {3})", this.AInfo, this.BInfo, aLoc, bLoc);
            Coordinate newLocation = new Coordinate(bLoc, aLoc);
            this.Location = newLocation;
            this.addToPath(newLocation);
            this.addToTravelledDistances(travelledDistance);
            this.addToDistance(distanceToIncrement);
        }

        public void goVertical(int stop, Problem problem)
        {
            int start = this.AInfo;
            if (start < stop)
            {
                for (int a = start; a < stop; a = a + 1)
                {
                    collectAisle(a, this.BInfo, false, false, problem);
                    goToLocation(a + 1, this.BInfo, new LinkDistance(1, Problem.Codes.L), 1 * problem.L);
                }
            }
            else
            {
                
                for (int a = start; a > stop; a = a - 1)
                {
                    collectAisle(a - 1, this.BInfo, true, false, problem);
                    goToLocation(a - 1, this.BInfo, new LinkDistance(1, Problem.Codes.L), 1 * problem.L);
                }
            }
        }

        public void collectAisle(int aPos, int bPos, bool up, bool enterExitSame, Problem problem)
        {
            List<Item> sortedAisleItems = problem.sortAisleItems(problem.getNonPickedAisleItems(aPos, this.BInfo, Problem.AislePart.All));
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
                    i.Picked= true;
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
    }
}
