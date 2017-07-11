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

        //Method to write a random item pick list for specified warehouse
        public static void writeRandomItemList(Problem warehouse, int sizeOfList, String filepath, Random rand)
        {
            List<Item> itemList = new List<Item>();
            StreamWriter wr = new StreamWriter(filepath, true);

            for (int i = 0; i < sizeOfList; i++)
            {
                Item j = new Item();
                j.Index = i + 1;
                j.AInfo = rand.Next(1, warehouse.NumberOfCrossAisles); //inclusive lower bound & exclusive upper bound
                j.BInfo = rand.Next(1, warehouse.NumberOfAisles);
                j.CInfo = rand.Next(0, 2); //(0,1): 1 is exclusive
                j.DInfo = rand.Next(1, warehouse.S + 1);
                itemList.Add(j);
                wr.WriteLine("{0},{1},{2},{3},{4}", j.Index, j.AInfo, j.BInfo, j.CInfo, j.DInfo);
            }

            wr.WriteLine("");
            wr.Close();
        }

        //Method to automate generation of random pick lists for specified warehouse with different item pick list criteria
        public static void generateTestLists(Problem warehouse, int[] sizeOfLists, int numberOfLists)
        {
            String filepath;
            String fileNumberZeros;
            Random rand = new Random();

            for (int i = 0; i < sizeOfLists.Length; i++)
            {
                if (sizeOfLists[i] < 1 || sizeOfLists[i] > 999)
                    continue;
                fileNumberZeros = (sizeOfLists[i] < 10) ? "00" : ((sizeOfLists[i] < 100) ? "0" : "");
                filepath = "../../../files/testListWithSize" + fileNumberZeros + sizeOfLists[i] + ".txt";
                for (int j = 0; j < numberOfLists; j++)
                {
                    writeRandomItemList(warehouse, sizeOfLists[i], filepath, rand);
                }
                Console.WriteLine("Finished generating pick list of {0} items", sizeOfLists[i]);
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
