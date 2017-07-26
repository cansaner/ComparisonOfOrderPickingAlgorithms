using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;

namespace ComparisonOfOrderPickingAlgorithms
{
    [Serializable]
    public class PickListChromosome : ChromosomeBase
    {
        #region Fields
        private int numberOfItems;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ComparisonOfOrderPickingAlgorithms.PickListChromosome class.
        /// </summary>
        /// <param name="numberOfItems">Number of items in pick list.</param>
        public PickListChromosome(int numberOfItems) : base(numberOfItems)
        {
            this.numberOfItems = numberOfItems + 1;//Picker is included with index 0
            int[] itemIndices = Utils.GetUniqueInts(numberOfItems + 1, 0, numberOfItems + 1);

            for (int i = 0; i < numberOfItems; i++)
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
            return new PickListChromosome(this.numberOfItems);
        }

        /// <summary>
        /// Creates a clone.
        /// </summary>
        /// <returns>The chromosome clone.</returns>
        public override IChromosome Clone()
        {
            var clone = base.Clone() as PickListChromosome;
            clone.Distance = Distance;

            return clone;
        }
        #endregion
    }
}
