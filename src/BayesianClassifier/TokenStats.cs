using System.Diagnostics;

namespace BayesianClassifier
{
    /// <summary>
    /// Struct TokenStats
    /// </summary>
    [DebuggerDisplay("{Probability}, {Occurrence}x")]
    public struct TokenStats
    {
        /// <summary>
        /// The token's probability in the class.
        /// </summary>
        public readonly double Probability;
        
        /// <summary>
        /// The token's occurrence in the class
        /// </summary>
        public readonly long Occurrence;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenStats"/> struct.
        /// </summary>
        /// <param name="probability">The probability.</param>
        /// <param name="occurrence">The occurrence.</param>
        public TokenStats(double probability, long occurrence)
        {
            Probability = probability;
            Occurrence = occurrence;
        }
    }
}
