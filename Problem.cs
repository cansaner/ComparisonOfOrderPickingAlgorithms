using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Problem
    {
        public enum Codes { W, L, K, S };

        private int numberOfShelves;

        public int S
        {
            get
            {
                return numberOfShelves;
            }
            set
            {
                numberOfShelves = value;
            }
        }

        private double widthOfStorageUnit;

        public double W
        {
            get
            {
                return widthOfStorageUnit;
            }
            set
            {
                widthOfStorageUnit = value;
            }
        }

        private double heightOfStorageUnit;

        public double L
        {
            get
            {
                return heightOfStorageUnit;
            }
            set
            {
                heightOfStorageUnit = value;
            }
        }

        private double lengthOfStorageUnitShelf;

        public double K
        {
            get
            {
                return lengthOfStorageUnitShelf;
            }
            set
            {
                lengthOfStorageUnitShelf = value;
            }
        }

        private int numberOfAisles;

        public int NumberOfAisles
        {
            get
            {
                return numberOfAisles;
            }
            set
            {
                numberOfAisles = value;
            }
        }

        private int numberOfCrossAisles;

        public int NumberOfCrossAisles
        {
            get
            {
                return numberOfCrossAisles;
            }
            set
            {
                numberOfCrossAisles = value;
            }
        }

        private Block[] blocks;

        public Block[] Blocks
        {
            get
            {
                return blocks;
            }
            //set
            //{
            //    bool isOK = true;
            //    for (int i = 0; i <= value.Length; i++) {
            //        if (value[i].StorageUnits.Length != this.numberOfAisles - 1) {
            //            isOK = false;
            //        }
            //    }
            //    if (isOK) {
            //        blocks = value;
            //    }
            //}
        }

        private List<int> pickedItemIndices;

        public List<int> PickingSequence
        {
            get
            {
                return pickedItemIndices;
            }
            set
            {
                pickedItemIndices = value;
            }
        }

        private Coordinate depot;

        public Coordinate Depot
        {
            get
            {
                return depot;
            }
            set
            {
                depot = value;
            }
        }


        public Problem(int S, double W, double L, double K, int numberOfBlocks, int numberOfAisles, Coordinate depot)
        {
            this.numberOfShelves = S;
            this.widthOfStorageUnit = W;
            this.heightOfStorageUnit = L;
            this.lengthOfStorageUnitShelf = K;
            this.numberOfCrossAisles = numberOfBlocks + 1;
            this.numberOfAisles = numberOfAisles;
            this.blocks = new Block[numberOfBlocks];
            for (int i = 0; i < numberOfBlocks; i++)
            {
                this.blocks[i] = new Block(i + 1, numberOfAisles - 1);
                for (int j = 0; j < numberOfAisles - 1; j++){
                    this.blocks[i].StorageUnits[j] = new StorageUnit(new Coordinate(j + 1, i + 1), S);
                }
            }
            this.depot = depot;
        }

        private int leftPickAisle;

        public int LeftPickAisle
        {
            get
            {
                return leftPickAisle;
            }
            protected set
            {
                leftPickAisle = value;
            }
        }

        private int farthestBlock;

        public int FarthestBlock
        {
            get
            {
                return farthestBlock;
            }
            protected set
            {
                farthestBlock = value;
            }
        }

        private List<Item> itemList;

        public List<Item> ItemList
        {
            get
            {
                return itemList;
            }
            set
            {
                //foreach (Item i in value)
                //{
                //    this.blocks[i.AInfo - 1].StorageUnits[i.BInfo - 1].fillShelf(i.CInfo, i.DInfo);
                //}
                itemList = value;
                farthestBlock = locateFarthestBlockFromDepotThatContainsOnePickLocation();
                leftPickAisle = locateLeftMostPickAisleThatContainsOnePickLocation();
                Console.WriteLine("LEFT PICK AISLE: {0}", leftPickAisle);
                Console.WriteLine("FARTHEST BLOCK: {0}", farthestBlock);
            }
        }

        //public void prepareItemList(bool useWrittenList, String filepath, int sizeOfList)
        //{
        //    if (useWrittenList)
        //    {
        //        this.itemList = readTestList(filepath);
        //    }
        //    else
        //    {
        //        this.itemList = generateItemList(sizeOfList, filepath);
        //    }
        //}

        public int locateFarthestBlockFromDepotThatContainsOnePickLocation()
        {
            int minValue = this.depot.Y - 1;

            if (this.itemList.Count == 0)
            {
                return -1;
            }
            foreach (Item i in itemList)
            {
                if (i.AInfo < minValue)
                {
                    minValue = i.AInfo;
                }
            }
            return minValue;
        }

        public int locateLeftMostPickAisleThatContainsOnePickLocation()
        {
            int minValue = this.numberOfAisles - 1;
            Item minItem = null;

            if (this.itemList.Count == 0)
            {
                return -1;
            }
            foreach (Item i in this.itemList)
            {
                if (i.BInfo <= minValue)
                {
                    minValue = i.BInfo;
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
                minItem = this.itemList.ElementAt(0);
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

        public List<Item> getNonPickedAisleItems(int aPos, int bPos)
        {
            List<Item> aisleItems = new List<Item>();
            //int rearMost = (int)Math.Ceiling(System.Convert.ToDouble(this.S) / 2);

            foreach (Item i in this.itemList)
            {
                if (i.Picked == false && i.AInfo == aPos)
                {
                    if ((i.BInfo == bPos && i.CInfo == 0) || (i.BInfo == bPos - 1 && i.CInfo == 1))
                    {
                        aisleItems.Add(i);
                        //switch (aislePart)
                        //{
                        //    case AislePart.All:
                        //        aisleItems.Add(i);
                        //        break;
                        //    case AislePart.Rear:
                        //        if (i.DInfo <= rearMost)
                        //        {
                        //            aisleItems.Add(i);
                        //        }
                        //        break;
                        //    case AislePart.Front:
                        //        if (i.DInfo > rearMost)
                        //        {
                        //            aisleItems.Add(i);
                        //        }
                        //        break;
                        //    default:
                        //        aisleItems.Add(i);
                        //        break;
                        //}
                    }
                }
            }
            return aisleItems;
        }

        public List<Item> sortAisleItems(List<Item> aisleItems)
        {
            if (aisleItems.Count == 0)
            {
                return aisleItems;
            }
            List<Item> sortedItems = new List<Item>();
            sortedItems.Add(aisleItems.ElementAt(0));

            for (int i = 1; i < aisleItems.Count; i++)
            {
                bool inserted = false;
                for (int j = 0; j < sortedItems.Count && inserted == false; j++)
                {
                    if (aisleItems.ElementAt(i).DInfo <= sortedItems.ElementAt(j).DInfo)
                    {
                        sortedItems.Insert(j, aisleItems.ElementAt(i));
                        inserted = true;
                    }
                    else
                    {
                        if (j + 1 == sortedItems.Count)
                        {
                            sortedItems.Add(aisleItems.ElementAt(i));
                            inserted = true;
                        }
                    }
                }

            }

            return sortedItems;
        }

        public List<int> getPickAislesOfBlock(int aPos)
        {
            List<int> result = new List<int>();
            for (int i = 2; i < this.numberOfAisles; i++)
            {
                if (countAisle(aPos, i) > 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        public int countAisle(int aPos, int bPos)
        {
            if (getNonPickedAisleItems(aPos, bPos) == null)
            {
                return 0;
            }
            return getNonPickedAisleItems(aPos, bPos).Count;
        }
    }
}