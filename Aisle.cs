using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Aisle
    {
        private int lowerItemDInfoOnLargestGap;

        public int LowLargestGap
        {
            get
            {
                return lowerItemDInfoOnLargestGap;
            }
            set
            {
                lowerItemDInfoOnLargestGap = value;
            }
        }

        private int higherItemDInfoOnLargestGap;

        public int HighLargestGap
        {
            get
            {
                return higherItemDInfoOnLargestGap;
            }
            set
            {
                higherItemDInfoOnLargestGap = value;
            }
        }

        public Aisle()
        {
        }
    }
}
