using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class NaiveClassifier. This class cannot be inherited.
    /// <para>
    /// Assumes that all token occurrences are statistically independent.
    /// </para>
    /// </summary>
    public class NaiveClassifier : IClassifier
    {
        /// <summary>
        /// The training sets
        /// </summary>
        [NotNull]
        private readonly ITrainingSetAccessor _trainingSets;

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
            get { return _trainingSets.OccurrenceThreshold; }
            set { _trainingSets.OccurrenceThreshold = value; }
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
            get { return _trainingSets.PercentageThreshold; }
            set { _trainingSets.PercentageThreshold = value; }
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
        /// <param name="trainingSets">The training sets.</param>
        /// <exception cref="System.ArgumentNullException">trainingSets</exception>
        public NaiveClassifier([NotNull] ITrainingSetAccessor trainingSets)
        {
            if (ReferenceEquals(trainingSets, null)) throw new ArgumentNullException("trainingSets");

            NormLength = 1;
            _trainingSets = trainingSets;
        }

        /// <summary>
        /// Calculates the probability of having the <see cref="IClass"/> 
        /// given the occurrence of the <see cref="IToken"/>.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        public virtual double CalculateProbability([NotNull] IClass classUnderTest, [NotNull] IToken token, double? alpha = null)
        {
            var smoothingAlpha = alpha ?? _smoothingAlpha;
            var occurenceThreshold = OccurrenceThreshold;
            var percentageThreshold = PercentageThreshold;

            ICollection<IDataSetAccessor> remainingSets;
            var setForClassUnderTest = SplitDataSets(classUnderTest, out remainingSets);

            // calculate the token's probability in the class under test
            var percentageInClassUnderTest = setForClassUnderTest.GetPercentage(token, smoothingAlpha);
            var probabilityInClassUnderTest = percentageInClassUnderTest * classUnderTest.Probability;

            // calculate the token's probabilities for the remaining classes
            double sumOfRemainingProbabilites;
            CalculateTokenProbabilityGivenClass(token, remainingSets, out sumOfRemainingProbabilites, smoothingAlpha).Run();

            // calculate total probability
            var totalProbability = probabilityInClassUnderTest + sumOfRemainingProbabilites;

            // calculate the class' probability given the token
            var probabilityForClass = probabilityInClassUnderTest/totalProbability;

            // adjust probability for the given class
            var correctedProbabilityForClass = ProbabilityCorrectionInternal.CorrectProbability(classUnderTest, setForClassUnderTest, token, probabilityForClass, occurenceThreshold);

            // correct for rare words
            return correctedProbabilityForClass;
        }
        
        /// <summary>
        /// Calculates the probability of having the 
        /// <see cref="IClass" />
        /// given the occurrence of the 
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        [NotNull]
        public IEnumerable<ConditionalProbability> CalculateProbabilities([NotNull] IToken token, double? alpha = null)
        {
            var smoothingAlpha = alpha ?? _smoothingAlpha;

            // calculate the token's probabilities for all classes
            double totalProbability;
            var probabilities = CalculateTokenProbabilityGivenClass(token, _trainingSets, out totalProbability, smoothingAlpha);
            
            var correction = ProbabilityCorrectionInternal;
            var threshold = OccurrenceThreshold;

            // apply Bayes theorem
            var inverseOfTotalProbability = totalProbability > 0 ? 1.0D/totalProbability : 0;
            return from cp in probabilities
                   let conditionalProbability = cp.Probability * inverseOfTotalProbability
                   
                   let @class = cp.Class
                   let set = _trainingSets.GetSetForClass(@class)
                   let correctedProbability = correction.CorrectProbability(@class, set, token, conditionalProbability, threshold)

                   select new ConditionalProbability(cp.Class, cp.Token, correctedProbability, cp.Occurrence);
        }

        /// <summary>
        /// Calculates the probability of having the
        /// <see cref="IClass" />
        /// given the occurrence of the
        /// <see cref="IToken" />.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        [NotNull]
        public virtual IEnumerable<CombinedConditionalProbability> CalculateProbabilities([NotNull] ICollection<IToken> tokens, double? alpha = null)
        {
            var smoothingAlpha = alpha ?? _smoothingAlpha;

            var conditionalProbabilityGroups = tokens
                .SelectMany(token => CalculateProbabilities(token, smoothingAlpha))
                .GroupBy(cp => cp.Class)
                .ToCollection();

            var normLength = NormLength;

            return from @group in conditionalProbabilityGroups
                let conditionalProbabilities = @group.ToCollection()
                let tokenCount = conditionalProbabilities.Count // TODO: sollte das nicht Anzahl der Token im Dokument sein, also tokens.Count()?
                let eta =
                    conditionalProbabilities.Select(cp => cp.Probability).Sum(p => Math.Log(1.0D - p) - Math.Log(p))
                let probability = 1.0D/(1.0D + Math.Exp(eta))
                // AUCH: http://lingpipe-blog.com/2009/02/13/document-length-normalized-naive-bayes/#comment-3952
                let lengthNormalizedProbability =
                    normLength > 0 ? Math.Pow(probability, normLength/tokenCount) : probability
                select
                    new CombinedConditionalProbability(@group.Key, lengthNormalizedProbability, conditionalProbabilities);
        }

        /// <summary>
        /// Calculates the probability of the given <paramref name="token"/> being in the classes of the given data <paramref name="sets"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;IClass, IToken&gt;&gt;.</returns>
        [NotNull]
        protected virtual IEnumerable<ConditionalProbability> CalculateTokenProbabilityGivenClass([NotNull] IToken token, [NotNull] IEnumerable<IDataSetAccessor> sets, double alpha)
        {
            var occurenceThreshold = OccurrenceThreshold;
            var percentageThreshold = PercentageThreshold;

            return from set in sets
                let @class = set.Class
                let classProbability = @class.Probability
                   let stats = set.GetStats(token, alpha)
                let percentageInClass = stats.Probability
                let countInClass = stats.Occurrence
                let probabilityInClass = percentageInClass*classProbability
                select new ConditionalProbability(@class, token, probabilityInClass, countInClass);
        }

        /// <summary>
        /// Calculates the probability of the given <paramref name="token"/> being in the classes of the given data <paramref name="sets"/>,
        /// as well as the total probability.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <param name="totalProbability">The total probability for the given classes.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;IClass, IToken&gt;&gt;.</returns>
        [NotNull]
        protected virtual IEnumerable<ConditionalProbability> CalculateTokenProbabilityGivenClass([NotNull] IToken token, [NotNull] IEnumerable<IDataSetAccessor> sets, out double totalProbability, double alpha)
        {
            var probabilities = CalculateTokenProbabilityGivenClass(token, sets, alpha).ToCollection();
            totalProbability = probabilities.Sum(p => p.Probability);
            return probabilities;
        }

        /// <summary>
        /// Splits the data sets.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="remainingSets">The remaining sets.</param>
        /// <returns>IDataSet&lt;IClass, IToken&gt;.</returns>
        [NotNull]
        protected IDataSetAccessor SplitDataSets([NotNull] IClass classUnderTest, [NotNull] out ICollection<IDataSetAccessor> remainingSets)
        {
            IDataSet setForClassUnderTest = null;
            remainingSets = new Collection<IDataSetAccessor>();

            // split data sets by selected class and other classes
            foreach (var trainingSet in _trainingSets)
            {
                // select the set for the class under test
                if (trainingSet.Class.Equals(classUnderTest))
                {
                    Debug.Assert(setForClassUnderTest == null,
                        "The class under test must not have multiple sets registered in the DataSet");
                    setForClassUnderTest = trainingSet;
                    continue;
                }

                // select remaining sets
                remainingSets.Add(trainingSet);
            }

            // return the found set or an empty set
            return setForClassUnderTest ?? new EmptyDataSet(classUnderTest);
        }
    }
}
