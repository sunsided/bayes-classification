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
        /// The tokens
        /// </summary>
        [NotNull]
        public IEnumerable<IToken> Tokens;

        /// <summary>
        /// The probability
        /// </summary>
        public double Probability;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedConditionalProbability" /> struct.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="tokens">The tokens.</param>
        public GroupedConditionalProbability([NotNull] IClass @class, double probability, [NotNull] IEnumerable<IToken> tokens)
        {
            if (ReferenceEquals(@class, null)) throw new ArgumentNullException("class");
            if (ReferenceEquals(tokens, null)) throw new ArgumentNullException("tokens");
            if (probability < 0) throw new ArgumentOutOfRangeException("probability", probability, "Probability must greater than or equal to zero");
            if (probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "Probability must less than or equal to one");

            Class = @class;
            Tokens = tokens;
            Probability = probability;
        }
    }
}
