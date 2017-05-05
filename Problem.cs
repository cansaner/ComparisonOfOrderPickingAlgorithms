using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Problem
    {
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

        public Problem(int S, double W, double L, double K, int numberOfBlocks, int numberOfAisles)
        {

            this.numberOfShelves = S;
            this.widthOfStorageUnit = W;
            this.heightOfStorageUnit = L;
            this.lengthOfStorageUnitShelf = K;
            this.numberOfCrossAisles = numberOfBlocks + 1;
            this.numberOfAisles = numberOfAisles;
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
                itemList = value;
            }
        }

        public prepareItemList(bool )
        {

        }

        public void generateItemList(int sizeOfList)
        {
            itemList = new List<Item>();
            

        }

        public void readItemList(string filepath)
        {
            itemList = new List<Item>();

            if (System.IO.File.Exists(filepath))
            {
                StreamReader re = new StreamReader(File.OpenRead(filepath));
                String line = String.Empty;

                while ((line = re.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    Item item = new Item(itemList.Count(), int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                    itemList.Add(item);
                }
                re.Close();

                foreach (Item item in itemList)
                    Console.WriteLine("ITEM NO:{0} - A={1}, B={2}, C={3}, D={4}", item.Index+1, item.AInfo, item.BInfo, item.CInfo, item.DInfo);
            } 
            else
                Console.WriteLine("NO FILE to parse!");
        }
    }
}