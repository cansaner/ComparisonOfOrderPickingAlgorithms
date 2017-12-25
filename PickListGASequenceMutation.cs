using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;
using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
    /// Base class for Mutations on a Sub-Sequence.
    /// </summary>
    public abstract class PickListGASequenceMutation : MutationBase
    {
        #region Methods
        /// <summary>
        /// Mutate the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <param name="probability">The probability to mutate each chromosome.</param>
        protected override void PerformMutate(IChromosome chromosome, float probability)
        {
            ValidateLength(chromosome);

            double randomDouble = ThreadSafeRandom.ThisThreadsRandom.NextDouble();
            //Console.WriteLine("Random Number generated to Mutate or Not: {0}", randomDouble);

            if (randomDouble <= probability)
            {
                //Console.WriteLine("...Sequence Mutation started...");
                var indexes = Utils.GetUniqueInts(2, 0, chromosome.Length).OrderBy(i => i).ToArray();
                //var indexes = new int[2] { 0, 1};
                var firstIndex = indexes[0];
                var secondIndex = indexes[1];
                var sequenceLength = (secondIndex - firstIndex) + 1;
                //Console.WriteLine("Chromosome Before Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("First Mutate Point Index: {0}", firstIndex);
                //Console.WriteLine("Second Mutate Point Index: {0}", secondIndex);

                var mutatedSequence = MutateOnSequence(chromosome.GetGenes().Skip(firstIndex).Take(sequenceLength)).ToArray();

                chromosome.ReplaceGenes(firstIndex, mutatedSequence);
                //Console.WriteLine("Chromosome After Mutation: [{0}]", string.Join(", ", Solution.extractChromosome(chromosome)));
                //Console.WriteLine("...Sequence Mutation ended...");
            }
            //else
            //{
            //    Console.WriteLine("No Mutation operation is done");
            //}
        }

        /// <summary>
        /// Validate length of the chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        protected virtual void ValidateLength(IChromosome chromosome)
        {
            if (chromosome.Length < 3)
            {
                throw new MutationException(this, "A chromosome should have, at least, 3 genes. {0} has only {1} gene.".With(chromosome.GetType().Name, chromosome.Length));
            }
        }

        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected abstract IEnumerable<T> MutateOnSequence<T>(IEnumerable<T> sequence);
        #endregion
    }
}
