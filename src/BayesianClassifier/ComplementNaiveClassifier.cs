using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class ComplementNaiveClassifier.
    /// </summary>
    public class ComplementNaiveClassifier : IClassifier
    {
        /// <summary>
        /// The training sets
        /// </summary>
        [NotNull]
        private readonly ITrainingSetAccessor _trainingSet;

        /// <summary>
        /// Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.
        /// </summary>
        private double _smoothingAlpha = 0.01D;

        /// <summary>
        /// The norm length
        /// </summary>
        private double _normLength;
        
        /// <summary>
        /// Gets or sets the probability correction.
        /// </summary>
        /// <value>The probability correction.</value>
        public IProbabilityCorrection ProbabilityCorrection { get; set; }

        /// <summary>
        /// Gets the probability correction internal.
        /// </summary>
        /// <value>The probability correction internal.</value>
        [NotNull]
        private IProbabilityCorrection ProbabilityCorrectionInternal
        {
            [DebuggerStepThrough]
            get { return ProbabilityCorrection ?? BayesianClassifier.ProbabilityCorrection.Default; }
        }

        /// <summary>
        /// Gets or sets the messages norm length.
        /// </summary>
        /// <value>The norm length of a message.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public double NormLength
        {
            get { return _normLength; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than or equal to zero.");
                if (Double.IsNaN(value) || Double.IsInfinity(value)) throw new NotFiniteNumberException("The value must be a finite number.", value);
                _normLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        public int OccurrenceThreshold
        {
            get { return _trainingSet.OccurrenceThreshold; }
            set { _trainingSet.OccurrenceThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the percentage threshold.
        /// <para>
        /// Any per-class token percentage lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="NotFiniteNumberException">value;Value must be finite.</exception>
        public double PercentageThreshold
        {
            get { return _trainingSet.PercentageThreshold; }
            set { _trainingSet.PercentageThreshold = value; }
        }

        /// <summary>
        /// Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.
        /// </summary>
        [DefaultValue(0.01D)]
        public double SmoothingAlpha
        {
            get { return _smoothingAlpha; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than zero.");
                _smoothingAlpha = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveClassifier"/> class.
        /// </summary>
        /// <param name="trainingSet">The training sets.</param>
        /// <exception cref="System.ArgumentNullException">trainingSets</exception>
        public ComplementNaiveClassifier([NotNull] ITrainingSetAccessor trainingSet)
        {
            if (ReferenceEquals(trainingSet, null)) throw new ArgumentNullException("trainingSet");

            NormLength = 1;
            _trainingSet = trainingSet;
        }

        /// <summary>
        /// Calculates the probability of having the 
        /// <see cref="IClass" /> 
        /// given the occurrence of the 
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null" /> defaults to the values set in <see cref="SmoothingAlpha" />.</param>
        /// <returns>System.Double.</returns>
        public double CalculateProbability(IClass classUnderTest, IToken token, double? alpha = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the probability of having the 
        /// <see cref="IClass" />
        /// given the occurrence of the 
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null" /> defaults to the values set in <see cref="SmoothingAlpha" />.</param>
        /// <returns>System.Double.</returns>
        public IEnumerable<ConditionalProbability> CalculateProbabilities(IToken token, double? alpha = null)
        {
            var correction = ProbabilityCorrectionInternal;
            var threshold = OccurrenceThreshold;

            // calculate all P(token|class) * P(class)
            var probabilities = PreCalculateProbabilities(token, alpha).ToList();

            // calculate total probability by marginalizing
            // P(token) = sum over P(token|class) for all classes
            var totalProbability = probabilities.Sum(cp => cp.Probability);
            var inverseOfTotalProbability = totalProbability > 0 ? 1.0D/totalProbability : 0;

            // apply Bayes theorem by scaling with the total probability
            // so that P(class|token) = P(token|class) * P(class) / P(token)
            foreach (var cp in probabilities)
            {
                var conditionalProbability = cp.Probability*inverseOfTotalProbability;

                var @class = cp.Class;
                var set = _trainingSet.GetSetForClass(@class);
                var correctedProbability = correction.CorrectProbability(@class, set, token, conditionalProbability, threshold);
                
                yield return new ConditionalProbability(cp.Class, cp.Token, correctedProbability, cp.Occurrence);
            }
        }

        /// <summary>
        /// Pres the calculate probabilities.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&gt;.</returns>
        [NotNull]
        private IEnumerable<ConditionalProbability> PreCalculateProbabilities([NotNull] IToken token, double? alpha)
        {
            var smootingAlpha = alpha ?? SmoothingAlpha;

            // for every token, calculate the probability of 
            // each class given the token
            foreach (var dataSet in _trainingSet)
            {
                var currentClass = dataSet.Class;
                Debug.WriteLine("Calculating probability for [{0}] given token [{1}]", currentClass.Name, token);

                // since this is the complement filter, calculate P(token|class)
                // based on all OTHER classes
                var logProbability = 0.0D;
                foreach (var otherDataSet in _trainingSet.Where(ds => !ds.Class.Equals(currentClass)))
                {
                    var stats = otherDataSet.GetStats(token, smootingAlpha);
                    logProbability += Math.Log(stats.Probability);
                }

                // calculate P(token|class) * P(class)
                var probability = Math.Exp(logProbability);
                probability = (1 - probability) * currentClass.Probability;

                var conditionalProbability = new ConditionalProbability(currentClass, token, probability, dataSet.GetCount(token));
                yield return conditionalProbability;
            }
        }

        /// <summary>
        /// Calculates the probability of having the
        /// <see cref="IClass" />
        /// given the occurrence of the
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied, setting to <see langword="null" /> defaults to the values set in <see cref="SmoothingAlpha" />.</param>
        /// <returns>System.Double.</returns>
        public IEnumerable<CombinedConditionalProbability> CalculateProbabilities(ICollection<IToken> tokens, double? alpha = null)
        {
            var normLength = NormLength;
            var documentLength = (double)tokens.Count;

            // calculate the probabilities for all tokens and group them by class
            var probabilities = tokens.SelectMany(token => CalculateProbabilities(token, alpha)); //.ToList();

            // group the probabilities by class
            var groups = probabilities.GroupBy(cp => cp.Class); // .ToList();
            foreach (var @group in groups)
            {
                var @class = @group.Key;
                var tokenProbabilities = @group.ToList();

                var eta = tokenProbabilities.Select(cp => cp.Probability).Sum(p => Math.Log(1.0D - p) - Math.Log(p));
                var probability = 1.0D / (1.0D + Math.Exp(eta));

                if (normLength > 0)
                {
                    probability = Math.Pow(probability, normLength / documentLength);
                }

                yield return new CombinedConditionalProbability(@class, probability, tokenProbabilities);
            }
        }
    }
}
