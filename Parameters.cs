using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Parameters
    {
        private TabuSearchParameters tabuSearchParameters;
        private PickListGAParameters pickListGAParameters;
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

        public PickListGAParameters PickListGAParameters
        {
            get
            {
                return pickListGAParameters;
            }
            set
            {
                pickListGAParameters = value;
            }
        }

        public int NumberOfStagnantGeneration
        {
            get
            {
                return pickListGAParameters.NumberOfStagnantGeneration;
            }
            set
            {
                pickListGAParameters.NumberOfStagnantGeneration = value;
            }
        }

        public int PopulationSize
        {
            get
            {
                return pickListGAParameters.PopulationSize;
            }
            set
            {
                pickListGAParameters.PopulationSize = value;
            }
        }

        public float CrossoverProbability
        {
            get
            {
                return pickListGAParameters.CrossoverProbability;
            }
            set
            {
                pickListGAParameters.CrossoverProbability = value;
            }
        }

        public float MutationProbability
        {
            get
            {
                return pickListGAParameters.MutationProbability;
            }
            set
            {
                pickListGAParameters.MutationProbability = value;
            }
        }

        public PickListGAParameters.Crossover CrossoverOperator
        {
            get
            {
                return pickListGAParameters.CrossoverOperator;
            }
            set
            {
                pickListGAParameters.CrossoverOperator = value;
            }
        }

        public PickListGAParameters.Mutation MutationOperator
        {
            get
            {
                return pickListGAParameters.MutationOperator;
            }
            set
            {
                pickListGAParameters.MutationOperator = value;
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

        public int[] PickListSizesOfTestLists
        {
            get
            {
                return listGenerationParameters.PickListSizesOfTestLists;
            }
            set
            {
                listGenerationParameters.PickListSizesOfTestLists = value;
            }
        }

        public int NumberOfPickLists
        {
            get
            {
                return listGenerationParameters.NumberOfPickLists;
            }
            set
            {
                listGenerationParameters.NumberOfPickLists = value;
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

        public int[] NumberOfStagnantGenerationList
        {
            get
            {
                return parameterTuningParameters.NumberOfStagnantGenerationList;
            }
            set
            {
                parameterTuningParameters.NumberOfStagnantGenerationList = value;
            }
        }

        public int[] PopulationSizeList
        {
            get
            {
                return parameterTuningParameters.PopulationSizeList;
            }
            set
            {
                parameterTuningParameters.PopulationSizeList = value;
            }
        }

        public float[] CrossoverProbabilityList
        {
            get
            {
                return parameterTuningParameters.CrossoverProbabilityList;
            }
            set
            {
                parameterTuningParameters.CrossoverProbabilityList = value;
            }
        }

        public float[] MutationProbabilityList
        {
            get
            {
                return parameterTuningParameters.MutationProbabilityList;
            }
            set
            {
                parameterTuningParameters.MutationProbabilityList = value;
            }
        }

        public PickListGAParameters.Crossover[] CrossoverOperatorList
        {
            get
            {
                return parameterTuningParameters.CrossoverOperatorList;
            }
            set
            {
                parameterTuningParameters.CrossoverOperatorList = value;
            }
        }

        public PickListGAParameters.Mutation[] MutationOperatorList
        {
            get
            {
                return parameterTuningParameters.MutationOperatorList;
            }
            set
            {
                parameterTuningParameters.MutationOperatorList = value;
            }
        }

        public Parameters()
        {
            tabuSearchParameters = new TabuSearchParameters();
            pickListGAParameters = new PickListGAParameters();
            listGenerationParameters = new ListGenerationParameters();
            parameterTuningParameters = new ParameterTuningParameters();
        }
    }
}