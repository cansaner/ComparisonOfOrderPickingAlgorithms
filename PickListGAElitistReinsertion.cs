using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAElitistReinsertion : ReinsertionBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Reinsertions.ElitistReinsertion"/> class.
        /// </summary>
        public PickListGAElitistReinsertion() : base(false, true)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Selects the chromosomes which will be reinserted.
        /// </summary>
        /// <returns>The chromosomes to be reinserted in next generation..</returns>
        /// <param name="population">The population.</param>
        /// <param name="offspring">The offspring.</param>
        /// <param name="parents">The parents.</param>
        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents)
        {
            //Console.WriteLine("...Elitist Reinsertion started...");
            var diff = population.MinSize - offspring.Count;
            //Console.WriteLine("Number of Chromosomes to be reinserted: {0}", diff);

            if (diff > 0)
            {
                var bestParents = parents.OrderByDescending(p => p.Fitness).Take(diff);

                foreach (var p in bestParents)
                {
                    offspring.Add(p);
                    //Console.WriteLine("Reinserted Chromosome: [{0}].Fitness = {1}", string.Join(", ", Solution.extractChromosome(p)), p.Fitness.Value);
                }
            }

            //Console.WriteLine("...Elitist Reinsertion ended...");
            return offspring;
        }
        #endregion
    }
}
