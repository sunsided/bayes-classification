using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class NaiveClassifier. This class cannot be inherited.
    /// <para>
    /// Assumes that all token occurrences are statistically independent.
    /// </para>
    /// </summary>
    public sealed class NaiveClassifier : IClassifier
    {
        /// <summary>
        /// The training sets
        /// </summary>
        [NotNull]
        private readonly ITrainingSetAccessor _trainingSets;

        /// <summary>
        /// Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.
        /// </summary>
        private double _smoothingAlpha = 1.0D;

        /// <summary>
        /// Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.
        /// </summary>
        public double SmoothingAlpha
        {
            get { return _smoothingAlpha; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than or equal to zero.");
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
        public double CalculateProbability([NotNull] IClass classUnderTest, [NotNull] IToken token, double? alpha = null)
        {
            var smoothingAlpha = alpha ?? _smoothingAlpha;

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
            return probabilityForClass;
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

            // apply Bayes theorem
            var inverseOfTotalProbability = 1.0D/totalProbability;
            return from cp in probabilities
                   let conditionalProbability = cp.Probability * inverseOfTotalProbability
                   select new ConditionalProbability(cp.Class, cp.Token, conditionalProbability);
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
        public IEnumerable<CombinedConditionalProbability> CalculateProbabilities([NotNull] ICollection<IToken> tokens, double? alpha = null)
        {
            var smoothingAlpha = alpha ?? _smoothingAlpha;
            var combinedConditionalProbabilities = CalculateCombinedConditionalProbabilities(tokens, smoothingAlpha).ToCollection();
            var totalProbability = combinedConditionalProbabilities.Sum(ccp => ccp.Probability);
            foreach (var ccp in combinedConditionalProbabilities)
            {
                var realProbability = ccp.Probability/totalProbability;
                yield return new CombinedConditionalProbability(ccp.Class, realProbability, ccp.TokenProbabilities);
            }
        }

        /// <summary>
        /// Calculates the combined conditional probabilities of all tokens appearing for each given class.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="smoothingAlpha">The smoothing alpha.</param>
        [NotNull]
        private IEnumerable<CombinedConditionalProbability> CalculateCombinedConditionalProbabilities([NotNull] ICollection<IToken> tokens, double smoothingAlpha)
        {
            foreach (var set in _trainingSets)
            {
                var @class = set.Class;

                // store conditional probabilities
                var conditionalProbabilities = new Collection<ConditionalProbability>();

                // determine the probability of all the tokens appearing in the given class
                var probabilityInClass = @class.Probability;
                foreach (var token in tokens)
                {
                    // calculate probability of the token appearing in the class
                    var percentage = set.GetPercentage(token, smoothingAlpha);
                    Debug.Assert(percentage > 0, "percentage of token in set is expected to be larger than zero. Is smoothing applied?");

                    probabilityInClass *= percentage;
                    Debug.Assert(probabilityInClass > 0, "combined probability of tokens in set is expected to be larger than zero. Is smoothing applied?");

                    conditionalProbabilities.Add(new ConditionalProbability(@class, token, percentage));
                }

                // create grouped conditional probability
                yield return new CombinedConditionalProbability(@class, probabilityInClass, conditionalProbabilities);
            }
        }

        /// <summary>
        /// Calculates the token probabilities given a class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;IClass, IToken&gt;&gt;.</returns>
        [NotNull]
        private IEnumerable<ConditionalProbability> CalculateTokenProbabilityGivenClass([NotNull] IToken token, [NotNull] IEnumerable<IDataSetAccessor> sets, double alpha)
        {
            return from set in sets
                   let @class = set.Class
                   let classProbability = @class.Probability
                   let percentageInClass = set.GetPercentage(token, alpha)
                   let probabilityInClass = percentageInClass * classProbability
                   select new ConditionalProbability(@class, token, probabilityInClass);
        }

        /// <summary>
        /// Calculates the token probabilities given a class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <param name="totalProbability">The total probability for the given classes.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;IClass, IToken&gt;&gt;.</returns>
        [NotNull]
        private IEnumerable<ConditionalProbability> CalculateTokenProbabilityGivenClass([NotNull] IToken token, [NotNull] IEnumerable<IDataSetAccessor> sets, out double totalProbability, double alpha)
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
        private IDataSetAccessor SplitDataSets([NotNull] IClass classUnderTest, [NotNull] out ICollection<IDataSetAccessor> remainingSets)
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
