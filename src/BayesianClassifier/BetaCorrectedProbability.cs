using System;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class BetaCorrectedProbability. This class cannot be inherited.
    /// </summary>
    public sealed class BetaCorrectedProbability : ProbabilityCorrection
    {
        /// <summary>
        /// The background information strength
        /// </summary>
        private double _backgroundInformationStrength;

        /// <summary>
        /// Gets or sets the background information strength.
        /// </summary>
        /// <value>The background information strength.</value>
        public double BackgroundInformationStrength
        {
            get { return _backgroundInformationStrength; }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than zero.");
                if (Double.IsNaN(value) || Double.IsInfinity(value)) throw new NotFiniteNumberException("The value must be a finite number.", value);
                _backgroundInformationStrength = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BetaCorrectedProbability"/> class.
        /// </summary>
        /// <param name="backgroundInformationStrength">The strength we assign to background information.</param>
        public BetaCorrectedProbability(double backgroundInformationStrength = 3.0D)
        {
            BackgroundInformationStrength = backgroundInformationStrength;
        }

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
        public override double CorrectProbability(IClass @class, IDataSetAccessor dataSet, IToken token, double probability, int occurrenceThreshold)
        {
            var s = _backgroundInformationStrength;
            var n = dataSet.GetCount(token);

            // apply occurrence threshold
            if (n <= occurrenceThreshold) n = 0;

            var correctedProbability = (s*@class.Probability + n*probability)/(s + n);
            return correctedProbability;
        }
    }
}
