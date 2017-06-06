using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Block
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

        private StorageUnit[] storageUnits;

        public StorageUnit[] StorageUnits
        {
            get
            {
                return storageUnits;
            }
            set
            {
                storageUnits = value;
            }
        }

        public Block(int aInfo, int numberOfStorageUnits)
        {
            this.location = new Coordinate(0, aInfo);
            this.storageUnits = new StorageUnit[numberOfStorageUnits];
        }
    }
}
