using System.Collections.Generic;
using System.Linq;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class PickListGAInversionMutation : PickListGASequenceMutation //ReverseSequenceMutation
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PickListGAInversionMutation"/> class.
        /// </summary>
        public PickListGAInversionMutation()
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
            return sequence.Reverse();
        }
        #endregion
    }
}
