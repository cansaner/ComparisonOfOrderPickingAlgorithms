using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Problem
    {
        public enum Codes { W, L, K, S };
        public enum AislePart { All, Rear, Front };

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

        //private List<Item> generateItemList(int sizeOfList, String filepath)
        //{
        //    List<Item> itemList = new List<Item>();

        //    Random rand = new Random();
        //    StreamWriter wr = new StreamWriter(filepath, true);

        //    for (int i = 0; i < sizeOfList; i++)
        //    {
        //        Item j = new Item();
        //        j.Index = i+1;
        //        j.AInfo = rand.Next(1, this.numberOfCrossAisles); //inclusive lower bound & exclusive upper bound
        //        j.BInfo = rand.Next(1, this.numberOfAisles);
        //        j.CInfo = rand.Next(0, 2); //(0,1): 1 is exclusive
        //        j.DInfo = rand.Next(1, this.numberOfShelves + 1);
        //        itemList.Add(j);
        //        wr.WriteLine("{0},{1},{2},{3},{4}", j.Index, j.AInfo, j.BInfo, j.CInfo, j.DInfo);
        //    }

        //    wr.WriteLine("");
        //    wr.Close();

        //    return itemList;
        //}

        //private List<Item> readTestList(String filepath)
        //{
        //    List<List<Item>> listOfLists = new List<List<Item>>();
        //    List<Item> itemList = new List<Item>();

        //    if (System.IO.File.Exists(filepath))
        //    {
        //        StreamReader re = new StreamReader(File.OpenRead(filepath));
        //        String line = String.Empty;

        //        while ((line = re.ReadLine()) != null)
        //        {
        //            if (line.Equals(String.Empty))
        //            {
        //                listOfLists.Add(itemList);
        //                itemList = new List<Item>();
        //                continue;
        //            }
        //            string[] values = line.Split(',');
        //            Item item = new Item(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
        //            itemList.Add(item);
        //        }
        //        re.Close();

        //        //foreach (List<Item> itemL in listOfLists)
        //        //    foreach (Item item in itemL)
        //        //        Console.WriteLine("ITEM NO:{0} - A={1}, B={2}, C={3}, D={4}", item.Index, item.AInfo, item.BInfo, item.CInfo, item.DInfo);
        //    } 
        //    else
        //        Console.WriteLine("NO FILE to parse!");

        //    this.itemListSet = listOfLists;
        //    return this.itemListSet.ElementAt(0);
        //}

        public List<Item> getNonPickedAisleItems(int aPos, int bPos, AislePart aislePart)
        {
            List<Item> aisleItems = new List<Item>();
            int rearMost = (int)Math.Ceiling(System.Convert.ToDouble(this.S) / 2);

            foreach (Item i in this.itemList)
            {
                if (i.Picked == false && i.AInfo == aPos)
                {
                    if ((i.BInfo == bPos && i.CInfo == 0) || (i.BInfo == bPos - 1 && i.CInfo == 1))
                    {
                        switch (aislePart)
                        {
                            case AislePart.All:
                                aisleItems.Add(i);
                                break;
                            case AislePart.Rear:
                                if (i.DInfo <= rearMost)
                                {
                                    aisleItems.Add(i);
                                }
                                break;
                            case AislePart.Front:
                                if (i.DInfo > rearMost)
                                {
                                    aisleItems.Add(i);
                                }
                                break;
                            default:
                                aisleItems.Add(i);
                                break;
                        }
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
    }
}