using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGARouletteWheelSelection : SelectionBase //RouletteWheelSelection
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Domain.Selections.RouletteWheelSelection"/> class.
        /// </summary>
        public PickListGARouletteWheelSelection() : base(2)
        {
        }
        #endregion

        #region ISelection implementation
        /// <summary>
        /// Selects from wheel.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rouletteWheel">The roulette wheel.</param>
        /// <param name="getPointer">The get pointer.</param>
        /// <returns>The selected chromosomes.</returns>
        protected static IList<IChromosome> SelectFromWheel(int number, IList<IChromosome> chromosomes, IList<double> rouletteWheel, Func<double> getPointer)
        {
            var selected = new List<IChromosome>();

            for (int i = 0; i < number; i++)
            {
                var pointer = getPointer();
                //Console.WriteLine("Random Number generated to select from Roulette Wheel: {0}", pointer);

                var chromosomeIndex = rouletteWheel.Select((value, index) => new { Value = value, Index = index }).FirstOrDefault(r => r.Value >= pointer).Index;
                //Console.WriteLine("Selected chromosome index for that random number: {0}", chromosomeIndex);

                selected.Add(chromosomes[chromosomeIndex]);
            }

            //Console.WriteLine("Selected Parent Chromosomes of current generation");
            //selected.ToList().ForEach(c => Console.WriteLine("[{0}].Fitness = {1}", string.Join(", ", Solution.extractChromosome(c)), c.Fitness.Value));

            //Console.WriteLine("...Roulette Wheel Selection ended...");

            return selected;
        }

        /// <summary>
        /// Calculates the cumulative percent.
        /// </summary>
        /// <param name="chromosomes">The chromosomes.</param>
        /// <param name="rouletteWheel">The roulette wheel.</param>
        protected static void CalculateCumulativePercentFitness(IList<IChromosome> chromosomes, IList<double> rouletteWheel)
        {
            //Console.WriteLine("Chromosomes of current generation:");
            //chromosomes.ToList().ForEach(c => Console.WriteLine("[{0}].Fitness = {1}", string.Join(", ", Solution.extractChromosome(c)), c.Fitness.Value));
            var sumFitness = chromosomes.Sum(c => c.Fitness.Value);
            //Console.WriteLine("Sum of Fitness values: {0}", sumFitness);

            var cumulativePercent = 0.0;

            foreach (var c in chromosomes)
            {
                //Console.WriteLine("Percent of Fitness for [{0}]: {1}", string.Join(", ", Solution.extractChromosome(c)), c.Fitness.Value / sumFitness);
                cumulativePercent += c.Fitness.Value / sumFitness;
                //Console.WriteLine("Cumulative Percent: {0}", cumulativePercent);
                rouletteWheel.Add(cumulativePercent);
            }
        }

        /// <summary>
        /// Performs the selection of chromosomes from the generation specified.
        /// </summary>
        /// <param name="number">The number of chromosomes to select.</param>
        /// <param name="generation">The generation where the selection will be made.</param>
        /// <returns>The select chromosomes.</returns>
        protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
        {
            //Console.WriteLine("...Roulette Wheel Selection started...");
            var chromosomes = generation.Chromosomes;
            var rouletteWheel = new List<double>();
            var rnd = ThreadSafeRandom.ThisThreadsRandom;

            CalculateCumulativePercentFitness(chromosomes, rouletteWheel);

            return SelectFromWheel(number, chromosomes, rouletteWheel, () => rnd.NextDouble());
        }
        #endregion
    }
}
