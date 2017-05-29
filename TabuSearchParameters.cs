using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class TabuSearchParameters
    {
        private int numberOfIterations;
        private int tabuLength;

        public int NumberOfIterations
        {
            get
            {
                return numberOfIterations;
            }
            set
            {
                numberOfIterations = value;
            }
        }

        public int TabuLength
        {
            get
            {
                return tabuLength;
            }
            set
            {
                tabuLength = value;
            }
        }

        public TabuSearchParameters()
        {
        }
    }
}
