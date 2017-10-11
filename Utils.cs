using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ComparisonOfOrderPickingAlgorithms
{
    public static class Utils
    {
        //Method to clone a list
        public static List<T> Clone<T>(List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        //Method to shuffle a list randomly
        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
		/// Gets an integer array with unique values between minimum value (inclusive) and maximum value (exclusive).
		/// </summary>
		/// <returns>The integer array.</returns>
		/// <param name="length">The array length</param>
		/// <param name="min">Minimum value (inclusive).</param>
		/// <param name="max">Maximum value (exclusive).</param>
		public static int[] GetUniqueInts(int length, int min, int max)
        {
            var diff = max - min;

            if (diff < length)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    "The length is "+length+", but the possible unique values between "+min+" (inclusive) and "+max+" (exclusive) are "+diff+".");
            }

            var orderedValues = Enumerable.Range(min, diff).ToList();
            var ints = new int[length];

            for (int i = 0; i < length; i++)
            {
                var removeIndex = ThreadSafeRandom.ThisThreadsRandom.Next(0, orderedValues.Count);
                ints[i] = orderedValues[removeIndex];
                orderedValues.RemoveAt(removeIndex);
            }

            return ints;
        }

        //Method to write pick list to test shortest path
        public static void writeShortestPathItemList(Problem warehouse, String filepath)
        {
            List<Item> itemList = new List<Item>();
            StreamWriter wr = new StreamWriter(filepath, true);

            int index = 0;
            for (int i = 0; i < warehouse.NumberOfCrossAisles - 1; i++)
            {
                for (int j = 0; j < warehouse.NumberOfAisles - 1; j++)
                {
                    Item item1 = new Item();
                    item1.Index = ++index;
                    item1.AInfo = i + 1;
                    item1.BInfo = j + 1;
                    item1.CInfo = 0;
                    item1.DInfo = 2;
                    itemList.Add(item1);
                    wr.WriteLine("{0},{1},{2},{3},{4}", item1.Index, item1.AInfo, item1.BInfo, item1.CInfo, item1.DInfo);

                    Item item2 = new Item();
                    item2.Index = ++index;
                    item2.AInfo = i + 1;
                    item2.BInfo = j + 1;
                    item2.CInfo = 0;
                    item2.DInfo = 9;
                    itemList.Add(item2);
                    wr.WriteLine("{0},{1},{2},{3},{4}", item2.Index, item2.AInfo, item2.BInfo, item2.CInfo, item2.DInfo);
                }
                Item itemLast1 = new Item();
                itemLast1.Index = ++index;
                itemLast1.AInfo = i + 1;
                itemLast1.BInfo = warehouse.NumberOfAisles - 1;
                itemLast1.CInfo = 1;
                itemLast1.DInfo = 2;
                itemList.Add(itemLast1);
                wr.WriteLine("{0},{1},{2},{3},{4}", itemLast1.Index, itemLast1.AInfo, itemLast1.BInfo, itemLast1.CInfo, itemLast1.DInfo);

                Item itemLast2 = new Item();
                itemLast2.Index = ++index;
                itemLast2.AInfo = i + 1;
                itemLast2.BInfo = warehouse.NumberOfAisles - 1;
                itemLast2.CInfo = 1;
                itemLast2.DInfo = 9;
                itemList.Add(itemLast2);
                wr.WriteLine("{0},{1},{2},{3},{4}", itemLast2.Index, itemLast2.AInfo, itemLast2.BInfo, itemLast2.CInfo, itemLast2.DInfo);
            }

            wr.WriteLine("");
            wr.Close();
        }

        //Method to write a random item pick list for specified warehouse
        public static void writeRandomItemList(Problem warehouse, int sizeOfList, String filepath)
        {
            List<Item> itemList = new List<Item>();
            StreamWriter wr = new StreamWriter(filepath, true);

            for (int i = 0; i < sizeOfList; i++)
            {
                Item j = new Item();
                j.Index = i + 1;
                j.AInfo = ThreadSafeRandom.ThisThreadsRandom.Next(1, warehouse.NumberOfCrossAisles); //inclusive lower bound & exclusive upper bound
                j.BInfo = ThreadSafeRandom.ThisThreadsRandom.Next(1, warehouse.NumberOfAisles);
                j.CInfo = ThreadSafeRandom.ThisThreadsRandom.Next(0, 2); //(0,1): 1 is exclusive
                j.DInfo = ThreadSafeRandom.ThisThreadsRandom.Next(1, warehouse.S + 1);
                itemList.Add(j);
                wr.WriteLine("{0},{1},{2},{3},{4}", j.Index, j.AInfo, j.BInfo, j.CInfo, j.DInfo);
            }

            wr.WriteLine("");
            wr.Close();
        }

        //Method to automate generation of random pick lists for specified warehouse with different item pick list criteria
        public static void generateTestLists(Problem warehouse, int[] pickListSizesOfTestLists, int numberOfPickLists)
        {
            String filepath;
            String fileNumberZeros;

            for (int i = 0; i < pickListSizesOfTestLists.Length; i++)
            {
                if (pickListSizesOfTestLists[i] < 1 || pickListSizesOfTestLists[i] > 999)
                    continue;
                fileNumberZeros = (pickListSizesOfTestLists[i] < 10) ? "00" : ((pickListSizesOfTestLists[i] < 100) ? "0" : "");
                filepath = "../../../files/testListWithPickListSize" + fileNumberZeros + pickListSizesOfTestLists[i] + ".txt";
                if (File.Exists(filepath))
                {
                    Console.WriteLine("{0} already exists. Generating a test list with same name would append more lines to the file if any previously test list exists with same name though It will mess previously created test list. Please remove previously created test list with same name manually on the path or try to create another test list with pick list size other than {1}", filepath, pickListSizesOfTestLists[i]);
                }
                else
                {
                    for (int j = 0; j < numberOfPickLists; j++)
                    {
                        writeRandomItemList(warehouse, pickListSizesOfTestLists[i], filepath);
                    }
                    Console.WriteLine("Finished generating {0} pick list(s) of {1} items", numberOfPickLists, pickListSizesOfTestLists[i]);
                }
            }
        }
        
        //Method to read a pick list from a written text file
        public static List<List<Item>> readTestList(String filepath)
        {
            List<List<Item>> listOfLists = new List<List<Item>>();
            List<Item> itemList = new List<Item>();

            if (System.IO.File.Exists(filepath))
            {
                StreamReader re = new StreamReader(File.OpenRead(filepath));
                String line = String.Empty;

                while ((line = re.ReadLine()) != null)
                {
                    if (line.Equals(String.Empty))
                    {
                        listOfLists.Add(itemList);
                        itemList = new List<Item>();
                        continue;
                    }
                    string[] values = line.Split(',');
                    Item item = new Item(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
                    itemList.Add(item);
                }
                re.Close();

                //foreach (List<Item> itemL in listOfLists)
                //    foreach (Item item in itemL)
                //        Console.WriteLine("ITEM NO:{0} - A={1}, B={2}, C={3}, D={4}", item.Index, item.AInfo, item.BInfo, item.CInfo, item.DInfo);
            }
            else
                Console.WriteLine("NO FILE to parse!");

            return listOfLists;
        }
    }

    //Class to generate thread safe random number
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
