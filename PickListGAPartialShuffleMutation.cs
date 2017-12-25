using System.Collections.Generic;
using System.Linq;

namespace ComparisonOfOrderPickingAlgorithms
{
    /// <summary>
	/// Partial Shuffle Mutation (PSM).
	/// <remarks>
	/// In the partial shuffle mutation operator, we take a sequence S limited by two 
	/// positions i and j randomly chosen, such that i&lt;j. The gene order in this sequence 
	/// will be shuffled. Sequence will be shuffled until it becomes different than the starting order
	/// <see href="http://arxiv.org/ftp/arxiv/papers/1203/1203.3099.pdf">Analyzing the Performance of Mutation Operators to Solve the Travelling Salesman Problem</see>
	/// </remarks>
	/// </summary>
    public class PickListGAPartialShuffleMutation : PickListGASequenceMutation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PickListGAPartialShuffleMutation"/> class.
        /// </summary>
        public PickListGAPartialShuffleMutation()
        {
            IsOrdered = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mutate selected sequence.
        /// </summary>
        /// <returns>The resulted sequence after mutation operation.</returns>
        /// <param name="sequence">The sequence to be mutated.</param>
        protected override IEnumerable<T> MutateOnSequence<T>(IEnumerable<T> sequence)
        {
            T[] sourceElements = sequence.ToArray();
            if (sourceElements.Length > 1)
            {
                IEnumerable<T> result = sequence.Shuffle(ThreadSafeRandom.ThisThreadsRandom);
                //T[] resultElements = result.ToArray();
                while (Enumerable.SequenceEqual(sourceElements, result.ToArray()))
                {
                    result = sequence.Shuffle(ThreadSafeRandom.ThisThreadsRandom);
                    //resultElements = result.ToArray();
                }

                //resultElements = result.ToArray();
                return result;
            }
            else
            {
                return sequence;
            }
        }
        #endregion
    }
}
