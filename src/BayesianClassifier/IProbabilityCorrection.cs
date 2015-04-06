using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface IProbabilityCorrection
    /// </summary>
    public interface IProbabilityCorrection
    {
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
        double CorrectProbability([NotNull] IClass @class, [NotNull] IDataSetAccessor dataSet, [NotNull] IToken token, double probability, int occurrenceThreshold);
    }
}
