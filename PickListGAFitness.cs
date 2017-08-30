using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
    /// Warehouses with Multiple Cross Aisles Problem fitness function.
    /// <remarks>
    /// Warehouses with Multiple Cross Aisles problem asks the following question: 
    /// Given a list of items and the distances between each pair of items, what is the shortest possible 
    /// route that visits each item exactly once and returns to the depot as starting point?
    /// </remarks>
    /// </summary>
    public class PickListGAFitness : IFitness
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ComparisonOfOrderPickingAlgorithms.PickListFitness class.
        /// </summary>
        /// <param name="itemList">The list of items with depot item indexed on first elemet</param>
        /// <param name="solution">The solution that GA is used to solve.</param>
        public PickListGAFitness(List<Item> itemList, Solution solution)
        {
            Items = itemList;
            Solution = solution;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<Item> Items { get; private set; }

        /// <summary>
        /// Gets the solution to have distance matrix.
        /// </summary>
        /// <value>The solution.</value>
        public Solution Solution { get; private set; }
        #endregion

        #region IFitness implementation
        /// <summary>
        /// Performs the evaluation against the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome to be evaluated.</param>
        /// <returns>The fitness of the chromosome.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            var distanceSum = 0.0;
            var lastItemIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
            var itemIndices = new List<int>();

            foreach (var g in genes)
            {
                var currentItemIndex = Convert.ToInt32(g.Value, CultureInfo.InvariantCulture);
                distanceSum += Solution.DistanceMatrix[lastItemIndex, currentItemIndex];
                lastItemIndex = currentItemIndex;
                itemIndices.Add(lastItemIndex);
            }

            distanceSum += Solution.DistanceMatrix[itemIndices.Last(), itemIndices.First()];

            //var fitness = 1.0 - (distanceSum / (Items.Count * 1000.0)); //Used fitness function for TSP
            var fitness = (1.0 / distanceSum) * 1000;

            ((PickListGAChromosome)chromosome).Distance = distanceSum;

            if (fitness < 0)
            {
                fitness = 0;
            }

            return fitness;
        }
        #endregion
    }
}
