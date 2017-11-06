using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAOrderCrossover : CrossoverBase //OrderedCrossover
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.OrderedCrossover"/> class.
        /// </summary>
        public PickListGAOrderCrossover()
            : base(2, 2)
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>The offspring (children) of the parents.</returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            //Console.WriteLine("...Order Crossover started...");
            var firstParent = parents[0];
            var secondParent = parents[1];
            //Console.WriteLine("First Parent: [{0}]", string.Join(", ", Solution.extractChromosome(firstParent)));
            //Console.WriteLine("Second Parent: [{0}]", string.Join(", ", Solution.extractChromosome(secondParent)));

            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Ordered Crossover (OX1) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var middleSectionIndexes = Utils.GetUniqueInts(2, 0, firstParent.Length);
            Array.Sort(middleSectionIndexes);
            var middleSectionBeginIndex = middleSectionIndexes[0];
            var middleSectionEndIndex = middleSectionIndexes[1];
            //Console.WriteLine("Middle Section Beginning Index: {0}", middleSectionBeginIndex);
            //Console.WriteLine("Middle Section Ending Index: {0}", middleSectionEndIndex);

            var firstChild = CreateChild(firstParent, secondParent, middleSectionBeginIndex, middleSectionEndIndex);
            var secondChild = CreateChild(secondParent, firstParent, middleSectionBeginIndex, middleSectionEndIndex);

            //Console.WriteLine("First Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(firstChild)));
            //Console.WriteLine("Second Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(secondChild)));
            //Console.WriteLine("...Order Crossover ended...");
            return new List<IChromosome>() { firstChild, secondChild };
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <returns>The child.</returns>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="middleSectionBeginIndex">Middle section begin index.</param>
        /// <param name="middleSectionEndIndex">Middle section end index.</param>
        private static IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int middleSectionBeginIndex, int middleSectionEndIndex)
        {
            var middleSectionGenes = firstParent.GetGenes().Skip(middleSectionBeginIndex).Take((middleSectionEndIndex - middleSectionBeginIndex) + 1);
            var secondParentRemainingGenes = secondParent.GetGenes().Except(middleSectionGenes).GetEnumerator();
            var child = firstParent.CreateNew();

            for (int i = 0; i < firstParent.Length; i++)
            {
                var firstParentGene = firstParent.GetGene(i);

                if (i >= middleSectionBeginIndex && i <= middleSectionEndIndex)
                {
                    child.ReplaceGene(i, firstParentGene);
                }
                else
                {
                    secondParentRemainingGenes.MoveNext();
                    child.ReplaceGene(i, secondParentRemainingGenes.Current);
                }
            }

            return child;
        }
        #endregion
    }
}
