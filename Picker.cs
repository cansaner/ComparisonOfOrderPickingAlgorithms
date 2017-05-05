using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    class LinkDistance
    {
        private int count;
        private String code;

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        public String Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        public LinkDistance(int count, String code)
        {
            this.count = count;
            this.code = code;
        }
    }

    class Picker
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
            set
            {
                travelledDistances = value;
            }
        }

        public Picker(int x, int y)
        {
            this.location.X = x;
            this.location.Y = y;
            this.distance = 0;
            this.numberOfPickedItems = 0;
        }

        public Picker(Coordinate location)
        {
            this.location = location;
            this.distance = 0;
            this.numberOfPickedItems = 0;
        }
    }
}
