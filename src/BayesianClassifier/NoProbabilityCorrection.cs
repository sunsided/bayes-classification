using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class NoProbabilityCorrection. This class cannot be inherited.
    /// </summary>
    public sealed class NoProbabilityCorrection : ProbabilityCorrection
    {
        /// <summary>
        /// Calculates a corrected probability given the original probability
        /// <c>P(class|token)</c>
        /// and information about the learning set.
        /// </summary>
        /// <param name="class">The class the token is in.</param>
        /// <param name="dataSet">The class' data set.</param>
        /// <param name="token">The token.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="occurrenceThreshold">The occurrence threshold.</param>
        /// <returns>Always returns <paramref name="probability" />.</returns>
        public override double CorrectProbability([NotNull] IClass @class, IDataSetAccessor dataSet, IToken token, double probability, int occurrenceThreshold)
        {
            return probability;
        }
    }
}
