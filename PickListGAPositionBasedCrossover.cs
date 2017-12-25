using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
    /// Position-based crossover (POS).
    /// <remarks>
    /// The position-based crossover operator (POS), which was also suggested in connection with schedule problems, 
    /// is a second modification of the OX1 operator. It also starts with selecting a random set of positions in 
    /// the parent strings. However, this operator imposes the position of the selected elements on the 
    /// corresponding elements of the other parent. For example, consider the parents (1 2 3 4 5 6 7 8) 
    /// and (2 4 6 8 7 5 3 1), and suppose that the second, third and sixth positions are selected. 
    /// This leads to the following offspring: (1 4 6 2 3 5 7 8) and (4 2 3 8 7 6 5 1).
    /// </remarks>
    /// </summary>
    public class PickListGAPositionBasedCrossover : PickListGAOrderBasedCrossover
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.PositionBasedCrossover"/> class.
        /// </summary>
        public PickListGAPositionBasedCrossover()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates the parents.
        /// </summary>
        /// <param name="parents">The parents.</param>
        protected override void ValidateParents(IList<IChromosome> parents)
        {
            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Position-based Crossover (POS) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }
        }

        /// <summary>
        /// Creates the child.
        /// </summary>
        /// <param name="firstParent">First parent.</param>
        /// <param name="secondParent">Second parent.</param>
        /// <param name="swapIndexes">The swap indexes.</param>
        /// <returns>
        /// The child.
        /// </returns>
        protected override IChromosome CreateChild(IChromosome firstParent, IChromosome secondParent, int[] swapIndexes)
        {
            ////First Parent: [1, 5, 4, 0, 3, 2]
            ////Second Parent: [2, 3, 5, 0, 1, 4]
            //firstParent = new PickListGAChromosome(5);

            //firstParent.ReplaceGenes(0, new Gene[] {
            //    new Gene(2),
            //    new Gene(3),
            //    new Gene(5),
            //    new Gene(0),
            //    new Gene(1),
            //    new Gene(4)
            //});
            
            //secondParent = new PickListGAChromosome(5);

            //secondParent.ReplaceGenes(0, new Gene[] {
            //    new Gene(1),
            //    new Gene(5),
            //    new Gene(4),
            //    new Gene(0),
            //    new Gene(3),
            //    new Gene(2)
            //});

            //swapIndexes = new int[3] { 2, 4, 3};

            ////First Offspring: [4, 3, 5, 0, 1, 2]
            ////Second Offspring: [2, 5, 4, 0, 3, 1]

            var secondParentSwapGenes = secondParent.GetGenes()
                .Select((g, i) => new { Gene = g, Index = i })
                .Where((g) => swapIndexes.Contains(g.Index))
                .ToArray();

            //var swappedSecondParentgenes = secondParentSwapGenes.Select(element => element.Gene).ToArray();

            //var swappedSecondParentgenes = new Gene[swapIndexes.Length];
            //for (int i = 0; i < swapIndexes.Length; i++)
            //{
            //    swappedSecondParentgenes[i] = secondParentSwapGenes[i].Gene;
            //}

            var firstParentRemainingGenes = firstParent.GetGenes().Except(secondParentSwapGenes.Select(element => element.Gene).ToArray()).GetEnumerator();

            var child = firstParent.CreateNew();
            var secondParentSwapGensIndex = 0;

            for (int i = 0; i < firstParent.Length; i++)
            {
                if (secondParentSwapGenes.Any(f => f.Index == i))
                {
                    child.ReplaceGene(i, secondParentSwapGenes[secondParentSwapGensIndex++].Gene);
                }
                else
                {
                    firstParentRemainingGenes.MoveNext();
                    child.ReplaceGene(i, firstParentRemainingGenes.Current);
                }
            }

            return child;

            ///////////

            //var firstParentGenes = new List<Gene>(firstParent.GetGenes());

            //var child = firstParent.CreateNew();

            //for (int i = 0; i < firstParent.Length; i++)
            //{
            //    if (swapIndexes.Contains(i))
            //    {
            //        var gene = secondParent.GetGene(i);
            //        firstParentGenes.Remove(gene);
            //        firstParentGenes.Insert(i, gene);
            //    }
            //}

            //child.ReplaceGenes(0, firstParentGenes.ToArray());

            //return child;
        }

        #endregion
    }
}
