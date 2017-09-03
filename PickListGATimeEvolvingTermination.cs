using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Texts;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    class PickListGATimeEvolvingTermination : TerminationBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
        /// </summary>
        /// <remarks>
        /// The default MaxTime is 1 minute.
        /// </remarks>
        public PickListGATimeEvolvingTermination() : this(TimeSpan.FromMinutes(1))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Terminations.TimeEvolvingTermination"/> class.
        /// </summary>
        /// <param name="maxTime">The execution time to consider the termination has been reached.</param>
        public PickListGATimeEvolvingTermination(TimeSpan maxTime)
        {
            MaxTime = maxTime;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the execution max time.
        /// </summary>
        /// <value>The max time.</value>
        public TimeSpan MaxTime { get; set; }
        #endregion

        #region implemented abstract members of TerminationBase
        /// <summary>
        /// Determines whether the specified geneticAlgorithm reached the termination condition.
        /// </summary>
        /// <returns>True if termination has been reached, otherwise false.</returns>
        /// <param name="geneticAlgorithm">The genetic algorithm.</param>
        protected override bool PerformHasReached(IGeneticAlgorithm geneticAlgorithm)
        {
            //Console.WriteLine("Genetic Algorithm Running Time: {0}", geneticAlgorithm.TimeEvolving);
            //Console.WriteLine("Genetic Algorithm Max Running Time: {0}", MaxTime);

            //if (geneticAlgorithm.TimeEvolving >= MaxTime)
            //{
            //    Console.WriteLine("Genetic Algorithm Running Time reached Max Time to Run.");
            //}
            //else
            //{
            //    Console.WriteLine("Genetic Algorithm has still some time to run.");
            //}

            return geneticAlgorithm.TimeEvolving >= MaxTime;
        }

        public override string ToString()
        {
            return "{0} with {1} max time to run".With(GetType().Name, MaxTime);
        }
        #endregion
    }
}
