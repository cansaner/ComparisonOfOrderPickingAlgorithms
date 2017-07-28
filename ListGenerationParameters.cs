using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class ListGenerationParameters
    {
        private int[] pickListSizesOfTestLists;
        private int numberOfPickLists;

        public int[] PickListSizesOfTestLists
        {
            get
            {
                return pickListSizesOfTestLists;
            }
            set
            {
                pickListSizesOfTestLists = value;
            }
        }

        public int NumberOfPickLists
        {
            get
            {
                return numberOfPickLists;
            }
            set
            {
                numberOfPickLists = value;
            }
        }

        public ListGenerationParameters()
        {
        }
    }
}
