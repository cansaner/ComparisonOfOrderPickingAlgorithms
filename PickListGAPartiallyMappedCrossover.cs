using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAPartiallyMappedCrossover : CrossoverBase //PartiallyMappedCrossover
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Crossovers.PartiallyMappedCrossover"/> class.
        /// </summary>
        public PickListGAPartiallyMappedCrossover() : base(2, 2, 3)
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods        
        /// <summary>
        /// Performs the cross with specified parents generating the children.
        /// </summary>
        /// <param name="parents">The parents chromosomes.</param>
        /// <returns>
        /// The offspring (children) of the parents.
        /// </returns>
        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            //Console.WriteLine("...Partially Mapped Crossover started...");
            if (parents.AnyHasRepeatedGene())
            {
                throw new CrossoverException(this, "The Partially Mapped Crossover (PMX) can be only used with ordered chromosomes. The specified chromosome has repeated genes.");
            }

            var parent1 = parents[0];
            var parent2 = parents[1];
            //Console.WriteLine("First Parent: [{0}]", string.Join(", ", Solution.extractChromosome(parent1)));
            //Console.WriteLine("Second Parent: [{0}]", string.Join(", ", Solution.extractChromosome(parent2)));

            var cutPointsIndexes = Utils.GetUniqueInts(2, 0, parent1.Length);
            Array.Sort(cutPointsIndexes);  //BURASI GENETIC SHARP IN BUG I
            var firstCutPointIndex = cutPointsIndexes[0];
            var secondCutPointIdnex = cutPointsIndexes[1];
            //Console.WriteLine("First Cut Point Index: {0}", firstCutPointIndex);
            //Console.WriteLine("Second Cut Point Index: {0}", secondCutPointIdnex);

            var parent1Genes = parent1.GetGenes();
            var parent1MappingSection = parent1Genes.Skip(firstCutPointIndex).Take((secondCutPointIdnex - firstCutPointIndex) + 1).ToArray();

            var parent2Genes = parent2.GetGenes();
            var parent2MappingSection = parent2Genes.Skip(firstCutPointIndex).Take((secondCutPointIdnex - firstCutPointIndex) + 1).ToArray();

            var offspring1 = parent1.CreateNew();
            var offspring2 = parent2.CreateNew();

            offspring2.ReplaceGenes(firstCutPointIndex, parent1MappingSection);
            offspring1.ReplaceGenes(firstCutPointIndex, parent2MappingSection);

            var length = parent1.Length;

            for (int i = 0; i < length; i++)
            {
                if (i >= firstCutPointIndex && i <= secondCutPointIdnex)
                {
                    continue;
                }

                var geneForoffspring1 = GetGeneNotInMappingSection(parent1Genes[i], parent2MappingSection, parent1MappingSection);
                offspring1.ReplaceGene(i, geneForoffspring1);

                var geneForoffspring2 = GetGeneNotInMappingSection(parent2Genes[i], parent1MappingSection, parent2MappingSection);
                offspring2.ReplaceGene(i, geneForoffspring2);
            }

            //Console.WriteLine("First Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(offspring1)));
            //Console.WriteLine("Second Offspring: [{0}]", string.Join(", ", Solution.extractChromosome(offspring2)));
            //Console.WriteLine("...Partially Mapped Crossover ended...");

            return new List<IChromosome>() { offspring1, offspring2 };
        }

        private Gene GetGeneNotInMappingSection(Gene candidateGene, Gene[] mappingSection, Gene[] otherParentMappingSection)
        {
            var indexOnMappingSection = mappingSection
                .Select((item, index) => new { Gene = item, Index = index })
                .FirstOrDefault(g => g.Gene.Equals(candidateGene));

            if (indexOnMappingSection != null)
            {
                return GetGeneNotInMappingSection(otherParentMappingSection[indexOnMappingSection.Index], mappingSection, otherParentMappingSection);
            }

            return candidateGene;
        }
        #endregion
    }
}
