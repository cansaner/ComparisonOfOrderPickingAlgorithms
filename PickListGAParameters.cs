using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAParameters
    {
        public enum Crossover { Cycle, PMX, Ordered, OX2, PositionBased };
        public enum Mutation { Swap, Inversion, Shuffle, Insertion, Displacement };

        private int numberOfStagnantGeneration;
        private int populationSize;
        private float crossoverProbability;
        private float mutationProbability;
        private Crossover crossoverOperator;
        private Mutation mutationOperator;

        public int NumberOfStagnantGeneration
        {
            get
            {
                return numberOfStagnantGeneration;
            }
            set
            {
                numberOfStagnantGeneration = value;
            }
        }

        public int PopulationSize
        {
            get
            {
                return populationSize;
            }
            set
            {
                populationSize = value;
            }
        }

        public float CrossoverProbability
        {
            get
            {
                return crossoverProbability;
            }
            set
            {
                crossoverProbability = value;
            }
        }

        public float MutationProbability
        {
            get
            {
                return mutationProbability;
            }
            set
            {
                mutationProbability = value;
            }
        }

        public Crossover CrossoverOperator
        {
            get
            {
                return crossoverOperator;
            }
            set
            {
                crossoverOperator = value;
            }
        }

        public Mutation MutationOperator
        {
            get
            {
                return mutationOperator;
            }
            set
            {
                mutationOperator = value;
            }
        }

        public PickListGAParameters()
        {
        }
    }
}
