using System;
using System.Collections.Generic;
using System.Linq;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
    /// Mutation service.
    /// </summary>
    public static class PickListGAMutationService
    {
        #region Methods
        /// <summary>
        /// Shuffle sequence.
        /// </summary>
        /// <returns>The shuffled sequence.</returns>
        /// <param name="source">source of sequence</param>
        /// <param name="rng">random number generator to select next index to shuffle</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rng.Next(0, i + 1);
                //Console.WriteLine("Random Swap Index to Shuffle: {0}", swapIndex);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        /// <summary>
        /// Shift sequence to left.
        /// </summary>
        /// <returns>The sequence shifted to left.</returns>
        /// <param name="source">source of sequence</param>
        /// <param name="valueToShift">count of units to be shifted</param>
        public static IEnumerable<T> LeftShift<T>(this IEnumerable<T> source, int valueToShift)
        {
            T[] sourceElements = source.ToArray();
            // all elements except for the first one... and at the end, the first one. to array.
            T[] shisftedElements = sourceElements.Skip(valueToShift).Concat(sourceElements.Take(valueToShift)).ToArray();
            foreach (T element in shisftedElements)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Shift sequence to right.
        /// </summary>
        /// <returns>The sequence shifted to right.</returns>
        /// <param name="source">source of sequence</param>
        /// <param name="valueToShift">count of units to be shifted</param>
        public static IEnumerable<T> RightShift<T>(this IEnumerable<T> source, int valueToShift)
        {
            T[] sourceElements = source.ToArray();
            // the last element (because we're skipping all but one)... then all but the last one.
            T[] shisftedElements = sourceElements.Skip(sourceElements.Length - valueToShift).Concat(sourceElements.Take(sourceElements.Length - valueToShift)).ToArray();
            foreach (T element in shisftedElements)
            {
                yield return element;
            }
        }
        #endregion
    }
}
