using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class AisleBucket
    {
        private int low;

        public int Low
        {
            get
            {
                return low;
            }
            set
            {
                low = value;
            }
        }
        private int high;

        public int High
        {
            get
            {
                return high;
            }
            set
            {
                high = value;
            }
        }

        public AisleBucket()
        {
            this.low = -1;
            this.high = -1;
        }
    }
}
