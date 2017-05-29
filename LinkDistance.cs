using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class LinkDistance
    {
        private int count;
        private Problem.Codes code;

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        public Problem.Codes Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        public LinkDistance(int count, Problem.Codes code)
        {
            this.count = count;
            this.code = code;
        }
    }
}
