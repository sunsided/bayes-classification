using System;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class ProbabilityCorrection.
    /// </summary>
    public abstract class ProbabilityCorrection : IProbabilityCorrection
    {
        /// <summary>
        /// The _default
        /// </summary>
        [NotNull] private static readonly Lazy<IProbabilityCorrection> DefaultCorrection = new Lazy<IProbabilityCorrection>(() => new NoProbabilityCorrection());

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        [NotNull]
        public static IProbabilityCorrection Default { get { return DefaultCorrection.Value; } }

        /// <summary>
        /// Calculates a corrected probability given the original probability
        /// <c>P(class|token)</c>
        /// and information about the learning set.
        /// </summary>
        /// <param name="class">The class the token is in.</param>
        /// <param name="dataSet">The class' data set.</param>
        /// <param name="token">The token.</param>
        /// <param name="probability">The class' probability given the token.</param>
        /// <param name="occurrenceThreshold">The occurrence threshold.</param>
        /// <returns>System.Double.</returns>
        public abstract double CorrectProbability(IClass @class, IDataSetAccessor dataSet, IToken token, double probability, int occurrenceThreshold);
    }
}
