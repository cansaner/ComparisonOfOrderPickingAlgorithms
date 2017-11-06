using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGASwapMutation : MutationBase //TworsMutation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PickListGASwapMutation"/> class.
        /// </summary>
        public PickListGASwapMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            double randomDouble = ThreadSafeRandom.ThisThreadsRandom.NextDouble();
            //Console.WriteLine("Random Number generated to Mutate or Not: {0}", randomDouble);

            if (randomDouble <= probability)
            {
                //Console.WriteLine("...Swap Mutation started...");
                var indexes = Utils.GetUniqueInts(2, 0, chromosome.Length);
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                //Console.WriteLine("Chromosome Before Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("First Mutate Point Index: {0}", firstIndex);
                //Console.WriteLine("Second Mutate Point Index: {0}", secondIndex);
                var firstGene = chromosome.GetGene(firstIndex);
                var secondGene = chromosome.GetGene(secondIndex);

                chromosome.ReplaceGene(firstIndex, secondGene);
                chromosome.ReplaceGene(secondIndex, firstGene);
                //Console.WriteLine("Chromosome After Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("...Swap Mutation ended...");
            }
            //else
            //{
            //    Console.WriteLine("No Mutation operation is done");
            //}
        }
        #endregion
    }
}
