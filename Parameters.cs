using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Parameters
    {
        private TabuSearchParameters tabuSearchParameters;
        private ListGenerationParameters listGenerationParameters;
        private ParameterTuningParameters parameterTuningParameters;

        public TabuSearchParameters TabuSearchParameters
        {
            get
            {
                return tabuSearchParameters;
            }
            set
            {
                tabuSearchParameters = value;
            }
        }

        public int NumberOfIterations
        {
            get
            {
                return tabuSearchParameters.NumberOfIterations;
            }
            set
            {
                tabuSearchParameters.NumberOfIterations = value;
            }
        }

        public int TabuLength
        {
            get
            {
                return tabuSearchParameters.TabuLength;
            }
            set
            {
                tabuSearchParameters.TabuLength = value;
            }
        }

        public ListGenerationParameters ListGenerationParameters
        {
            get
            {
                return listGenerationParameters;
            }
            set
            {
                listGenerationParameters = value;
            }
        }

        public int[] SizeOfLists
        {
            get
            {
                return listGenerationParameters.SizeOfLists;
            }
            set
            {
                listGenerationParameters.SizeOfLists = value;
            }
        }

        public int NumberOfLists
        {
            get
            {
                return listGenerationParameters.NumberOfLists;
            }
            set
            {
                listGenerationParameters.NumberOfLists = value;
            }
        }

        public ParameterTuningParameters ParameterTuningParameters
        {
            get
            {
                return parameterTuningParameters;
            }
            set
            {
                parameterTuningParameters = value;
            }
        }

        public List<List<Item>> ItemListSet
        {
            get
            {
                return parameterTuningParameters.ItemListSet;
            }
            set
            {
                parameterTuningParameters.ItemListSet = value;
            }
        }

        public int[] NumberOfIterationsList
        {
            get
            {
                return parameterTuningParameters.NumberOfIterationsList;
            }
            set
            {
                parameterTuningParameters.NumberOfIterationsList = value;
            }
        }

        public int[] TabuLengthList
        {
            get
            {
                return parameterTuningParameters.TabuLengthList;
            }
            set
            {
                parameterTuningParameters.TabuLengthList = value;
            }
        }

        public Parameters()
        {
            tabuSearchParameters = new TabuSearchParameters();
            listGenerationParameters = new ListGenerationParameters();
            parameterTuningParameters = new ParameterTuningParameters();
        }
    }
}