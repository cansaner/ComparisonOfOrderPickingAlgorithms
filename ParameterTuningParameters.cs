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
        private int[] numberOfStagnantGenerationList;
        private int[] populationSizeList;
        private float[] crossoverProbabilityList;
        private float[] mutationProbabilityList;
        private PickListGAParameters.Crossover[] crossoverOperatorList;
        private PickListGAParameters.Mutation[] mutationOperatorList;

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

        public int[] NumberOfStagnantGenerationList
        {
            get
            {
                return numberOfStagnantGenerationList;
            }
            set
            {
                numberOfStagnantGenerationList = value;
            }
        }

        public int[] PopulationSizeList
        {
            get
            {
                return populationSizeList;
            }
            set
            {
                populationSizeList = value;
            }
        }

        public float[] CrossoverProbabilityList
        {
            get
            {
                return crossoverProbabilityList;
            }
            set
            {
                crossoverProbabilityList = value;
            }
        }

        public float[] MutationProbabilityList
        {
            get
            {
                return mutationProbabilityList;
            }
            set
            {
                mutationProbabilityList = value;
            }
        }

        public PickListGAParameters.Crossover[] CrossoverOperatorList
        {
            get
            {
                return crossoverOperatorList;
            }
            set
            {
                crossoverOperatorList = value;
            }
        }

        public PickListGAParameters.Mutation[] MutationOperatorList
        {
            get
            {
                return mutationOperatorList;
            }
            set
            {
                mutationOperatorList = value;
            }
        }

        public ParameterTuningParameters()
        {
        }
    }
}
