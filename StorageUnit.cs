using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class StorageUnit
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

        private bool[,] shelves;

        public bool[,] Shelves
        {
            get
            {
                return shelves;
            }
            set
            {
                shelves = value;
            }
        }

        public StorageUnit(Coordinate location, bool[,] shelves)
        {
            this.location = location;
            this.shelves = shelves;
        }

        public StorageUnit(Coordinate location, int S){
            this.location = location;
            this.shelves = new bool[2,S];
        }

        public void fillShelf(int cInfo, int dInfo){
            this.shelves[cInfo, dInfo - 1] = true;
        }

        public void emptyShelf(int cInfo, int dInfo)
        {
            this.shelves[cInfo, dInfo - 1] = false;
        }
    }
}
