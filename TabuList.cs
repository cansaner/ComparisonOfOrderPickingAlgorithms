using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class TabuList
    {
        private int[,] list;

        public int[,] List
        {
            get
            {
                return list;
            }
            protected set
            {
                list = value;
            }
        }

        private int tabuLength;

        public int TabuLength
        {
            get
            {
                return tabuLength;
            }
            protected set
            {
                tabuLength = value;
            }
        }

        public TabuList(int numberOfCities, int tabuLength)
        {
            this.list = new int[numberOfCities, numberOfCities];
            this.tabuLength = tabuLength;
        }

        public void tabuMove(int item1, int item2)
        {
            list[item1, item2] += this.tabuLength;
            list[item2, item1] += this.tabuLength;
        }

        public void decrementTabu()
        {
            for (int i = 0; i < list.GetLength(1); i++)
            {
                for (int j = 0; j < list.GetLength(0); j++)
                {
                    list[i, j] -= list[i, j] <= 0 ? 0 : 1;
                }
            }
        }

        public void printTabuList()
        {
            for (int row = 0; row < this.list.GetLength(1); row++)
            {
                for (int col = 0; col < this.list.GetLength(0); col++)
                    Console.Write(String.Format("{0}\t", this.list[row, col]));
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
