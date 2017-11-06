using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Texts;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAFitnessStagnationTermination : TerminationBase
    {
        #region Fields
        private double m_lastFitness;
        private int m_stagnantGenerationsCount;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessStagnationTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The ExpectedStagnantGenerationsNumber default value is 100.
        /// </remarks>
        public PickListGAFitnessStagnationTermination() : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FitnessStagnationTermination"/> class.
        /// </summary>
        /// <param name="expectedStagnantGenerationsNumber">The expected stagnant generations number to reach the termination.</param>
        public PickListGAFitnessStagnationTermination(int expectedStagnantGenerationsNumber)
        {
            ExpectedStagnantGenerationsNumber = expectedStagnantGenerationsNumber;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the expected stagnant generations number to reach the termination.
        /// </summary>
        public int ExpectedStagnantGenerationsNumber { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            var bestFitness = geneticAlgorithm.BestChromosome.Fitness.Value;
            //Console.WriteLine("Best Chromosome Fitness: {0}", bestFitness);

            if (m_lastFitness == bestFitness)
            {
                //Console.WriteLine("Best Chromosome Fitness Value is still same");
                m_stagnantGenerationsCount++;
            }
            else
            {
                //Console.WriteLine("Best Chromosome Fitness Value is changed");
                m_stagnantGenerationsCount = 1;
            }
            //Console.WriteLine("Stagnant Generation Count: {0}", m_stagnantGenerationsCount);

            m_lastFitness = bestFitness;

            //if (m_stagnantGenerationsCount >= ExpectedStagnantGenerationsNumber)
            //{
            //    Console.WriteLine("Stagnant Generation Count({0}) reached expected stagnant generation number({1})", m_stagnantGenerationsCount, ExpectedStagnantGenerationsNumber);
            //}
            //else
            //{
            //    Console.WriteLine("Expected stagnant generation number({0}) is not reached yet.", ExpectedStagnantGenerationsNumber);
            //}

            return m_stagnantGenerationsCount >= ExpectedStagnantGenerationsNumber;
        }

        public override string ToString()
        {
            return "{0} with {1} expected stagnant generation number".With(GetType().Name, ExpectedStagnantGenerationsNumber);
        }
        #endregion
    }
}
