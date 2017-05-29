using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class ListGenerationParameters
    {
        private int[] sizeOfLists;
        private int numberOfLists;

        public int[] SizeOfLists
        {
            get
            {
                return sizeOfLists;
            }
            set
            {
                sizeOfLists = value;
            }
        }

        public int NumberOfLists
        {
            get
            {
                return numberOfLists;
            }
            set
            {
                numberOfLists = value;
            }
        }

        public ListGenerationParameters()
        {
        }
    }
}
