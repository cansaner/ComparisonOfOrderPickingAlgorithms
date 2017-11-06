using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    class PickListGACycleCrossover : CrossoverBase //CycleCrossover
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.CycleCrossover"/> class.
        /// </summary>
        public PickListGACycleCrossover()
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
            //Console.WriteLine("...Cycle Crossover started...");
            var parent1 = parents[0];
            var parent2 = parents[1];
            //Console.WriteLine("First Parent: [{0}]", string.Join(", ", Solution.extractChromosome(parent1)));
            //Console.WriteLine("Second Parent: [{0}]", string.Join(", ", Solution.extractChromosome(parent2)));

            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Cycle Crossover (CX) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var cycles = new List<List<int>>();
            var offspring1 = parent1.CreateNew();
            var offspring2 = parent2.CreateNew();

            var parent1Genes = parent1.GetGenes();
            var parent2Genes = parent2.GetGenes();

            // Search for the cycles.
            for (int i = 0; i < parent1.Length; i++)
            {
                if (!cycles.SelectMany(p => p).Contains(i))
                {
                    var cycle = new List<int>();
                    CreateCycle(parent1Genes, parent2Genes, i, cycle);
                    cycles.Add(cycle);
                }
            }
            int j = 0;
            //foreach (var cycle in cycles)
            //{
            //    Console.Write("Cycle {0}: ", j + 1);
            //    cycle.ForEach(g => Console.Write("{0} -> ", parent1Genes[g]));
            //    Console.WriteLine("");
            //    j++;
            //}

            // Copy the cycles to the offpring.
            for (int i = 0; i < cycles.Count; i++)
            {
                var cycle = cycles[i];

                if (i % 2 == 0)
                {
                    // Copy cycle index pair: values from Parent 1 and copied to Child 1, and values from Parent 2 will be copied to Child 2.
                    CopyCycleIndexPair(cycle, parent1Genes, offspring1, parent2Genes, offspring2);
                }
                else
                {
                    // Copy cycle index odd: values from Parent 1 will be copied to Child 2, and values from Parent 1 will be copied to Child 1.
                    CopyCycleIndexPair(cycle, parent1Genes, offspring2, parent2Genes, offspring1);
                }
            }

            //Console.WriteLine("First Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(offspring1)));
            //Console.WriteLine("Second Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(offspring2)));
            //Console.WriteLine("...Cycle Crossover ended...");

            return new List<IChromosome>() { offspring1, offspring2 };
        }

        /// <summary>
        /// Copies the cycle index pair.
        /// </summary>
        /// <param name="cycle">The cycle.</param>
        /// <param name="fromParent1Genes">From parent1 genes.</param>
        /// <param name="toOffspring1">To offspring1.</param>
        /// <param name="fromParent2Genes">From parent2 genes.</param>
        /// <param name="toOffspring2">To offspring2.</param>
        private static void CopyCycleIndexPair(IList<int> cycle, Gene[] fromParent1Genes, IChromosome toOffspring1, Gene[] fromParent2Genes, IChromosome toOffspring2)
        {
            int geneCycleIndex = 0;

            for (int j = 0; j < cycle.Count; j++)
            {
                geneCycleIndex = cycle[j];
                toOffspring1.ReplaceGene(geneCycleIndex, fromParent1Genes[geneCycleIndex]);
                toOffspring2.ReplaceGene(geneCycleIndex, fromParent2Genes[geneCycleIndex]);
            }
        }

        /// <summary>
        /// Creates the cycle recursively.
        /// </summary>
        /// <param name="parent1Genes">The parent one's genes.</param>
        /// <param name="parent2Genes">The parent two's genes.</param>
        /// <param name="geneIndex">The current gene index.</param>
        /// <param name="cycle">The cycle.</param>
        private void CreateCycle(Gene[] parent1Genes, Gene[] parent2Genes, int geneIndex, List<int> cycle)
        {
            if (!cycle.Contains(geneIndex))
            {
                var parent2Gene = parent2Genes[geneIndex];
                cycle.Add(geneIndex);
                var newGeneIndex = parent1Genes.Select((g, i) => new { Value = g.Value, Index = i }).First(g => g.Value.Equals(parent2Gene.Value));

                if (geneIndex != newGeneIndex.Index)
                {
                    CreateCycle(parent1Genes, parent2Genes, newGeneIndex.Index, cycle);
                }
            }
        }
        #endregion
    }
}
