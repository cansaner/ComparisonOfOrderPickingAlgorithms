using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using System.Text;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAInversionMutation : MutationBase //ReverseSequenceMutation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PickListGAInversionMutation"/> class.
        /// </summary>
        public PickListGAInversionMutation()
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
            if (chromosome.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }

            double randomDouble = ThreadSafeRandom.ThisThreadsRandom.NextDouble();
            //Console.WriteLine("Random Number generated to Mutate or Not: {0}", randomDouble);

            if (randomDouble <= probability)
            {
                //Console.WriteLine("...Inversion Mutation started...");
                var indexes = Utils.GetUniqueInts(2, 0, chromosome.Length).OrderBy(i => i).ToArray();
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                //Console.WriteLine("Chromosome Before Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("First Mutate Point Index: {0}", firstIndex);
                //Console.WriteLine("Second Mutate Point Index: {0}", secondIndex);

                var revertedSequence = chromosome.GetGenes().Skip(firstIndex).Take((secondIndex - firstIndex) + 1).Reverse().ToArray();

                chromosome.ReplaceGenes(firstIndex, revertedSequence);
                //Console.WriteLine("Chromosome After Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("...Inversion Mutation ended...");
            }
            //else
            //{
            //    Console.WriteLine("No Mutation operation is done");
            //}
        }
        #endregion
    }
}
