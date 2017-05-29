using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class ParameterTuningParameters
    {
        private List<List<Item>> itemListSet;
        private int[] numberOfIterationsList;
        private int[] tabuLengthList;

        public List<List<Item>> ItemListSet
        {
            get
            {
                return itemListSet;
            }
            set
            {
                itemListSet = value;
            }
        }

        public int[] NumberOfIterationsList
        {
            get
            {
                return numberOfIterationsList;
            }
            set
            {
                numberOfIterationsList = value;
            }
        }

        public int[] TabuLengthList
        {
            get
            {
                return tabuLengthList;
            }
            set
            {
                tabuLengthList = value;
            }
        }

        public ParameterTuningParameters()
        {
        }
    }
}
