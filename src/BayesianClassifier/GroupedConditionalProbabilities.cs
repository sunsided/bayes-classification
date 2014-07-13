using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Struct GroupedConditionalProbability
    /// </summary>
    public struct GroupedConditionalProbability
    {
        /// <summary>
        /// The class
        /// </summary>
        [NotNull]
        public readonly IClass Class;
        
        /// <summary>
        /// The token probabilities
        /// </summary>
        [NotNull]
        public IEnumerable<ConditionalProbability> TokenProbabilities;

        /// <summary>
        /// The probability
        /// </summary>
        public double Probability;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedConditionalProbability" /> struct.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="tokenProbabilities">The tokenProbabilities.</param>
        public GroupedConditionalProbability([NotNull] IClass @class, double probability, [NotNull] IEnumerable<ConditionalProbability> tokenProbabilities)
        {
            if (ReferenceEquals(@class, null)) throw new ArgumentNullException("class");
            if (ReferenceEquals(tokenProbabilities, null)) throw new ArgumentNullException("tokenProbabilities");
            if (probability < 0) throw new ArgumentOutOfRangeException("probability", probability, "Probability must greater than or equal to zero");
            if (probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "Probability must less than or equal to one");

            Class = @class;
            TokenProbabilities = tokenProbabilities;
            Probability = probability;
        }
    }
}
