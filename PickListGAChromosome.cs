using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
    /// Warehouses with Multiple Cross Aisles Problem chromosome.
    /// <remarks>
    /// Each gene represents an item index.
    /// </remarks>
    /// </summary>
    [Serializable]
    public class PickListGAChromosome : ChromosomeBase
    {
        #region Fields
        private int numberOfItems;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ComparisonOfOrderPickingAlgorithms.PickListChromosome class.
        /// </summary>
        /// <param name="numberOfItems">Number of items in pick list.</param>
        public PickListGAChromosome(int numberOfItems) : base(numberOfItems+1)
        {
            this.numberOfItems = numberOfItems;//Picker is included with index 0
            int[] itemIndices = Utils.GetUniqueInts(numberOfItems + 1, 0, numberOfItems + 1);

            for (int i = 0; i < numberOfItems + 1; i++)
            {
                ReplaceGene(i, new Gene(itemIndices[i]));
            }
        }

        public PickListGAChromosome(int[] itemIndices) : base(itemIndices.Length)
        {
            this.numberOfItems = itemIndices.Length - 1;//Number of items is the number of items with indices having index value more than 0, 0 is the depot item.
            for (int i = 0; i < itemIndices.Length; i++)
            {
                ReplaceGene(i, new Gene(itemIndices[i]));
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the distance.
        /// </summary>
        /// <value>The distance.</value>
        public double Distance { get; internal set; }
        #endregion

        #region implemented abstract members of ChromosomeBase
        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(ThreadSafeRandom.ThisThreadsRandom.Next(0, this.numberOfItems));
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new PickListGAChromosome(this.numberOfItems);
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as PickListGAChromosome;
            clone.Distance = Distance;

            return clone;
        }
        #endregion
    }
}