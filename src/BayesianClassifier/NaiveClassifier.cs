using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <typeparam name="TClass">The type of the classes.</typeparam>
    /// <typeparam name="TToken">The type of the tokens.</typeparam>
    public sealed class NaiveClassifier<TClass, TToken>
        where TClass: IClass
        where TToken: IToken
    {
        /// <summary>
        /// The training sets
        /// </summary>
        [NotNull]
        private readonly ITrainingSetAccessor<TClass, TToken> _trainingSets;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveClassifier{TClass,TToken}"/> class.
        /// </summary>
        /// <param name="trainingSets">The training sets.</param>
        /// <exception cref="System.ArgumentNullException">trainingSets</exception>
        public NaiveClassifier([NotNull] ITrainingSetAccessor<TClass, TToken> trainingSets)
        {
            if (ReferenceEquals(trainingSets, null)) throw new ArgumentNullException("trainingSets");
            _trainingSets = trainingSets;
        }

        /// <summary>
        /// Calculates the probability of having the <see cref="TClass"/> 
        /// given the occurrence of the <see cref="TToken"/>.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="token">The token.</param>
        /// <returns>System.Double.</returns>
        public double CalculateProbability([NotNull] TClass classUnderTest, [NotNull] TToken token)
        {
            ICollection<IDataSetAccessor<TClass, TToken>> remainingSets;
            var setForClassUnderTest = SplitDataSets(classUnderTest, out remainingSets);

            // calculate the token's probability in the class under test
            var percentageInClassUnderTest = setForClassUnderTest.GetPercentage(token);
            var probabilityInClassUnderTest = percentageInClassUnderTest * classUnderTest.Probability;

            // calculate the token's probabilities for the remaining classes
            double sumOfRemainingProbabilites;
            CalculateTokenProbabilityGivenClass(token, remainingSets, out sumOfRemainingProbabilites);

            // calculate total probability
            var totalProbability = probabilityInClassUnderTest + sumOfRemainingProbabilites;

            // calculate the class' probability given the token
            var probabilityForClass = probabilityInClassUnderTest/totalProbability;
            return probabilityForClass;
        }
        
        /// <summary>
        /// Calculates the probability of having the 
        /// <see cref="TClass" />
        /// given the occurrence of the 
        /// <see cref="TToken" />.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Double.</returns>
        [NotNull]
        public IEnumerable<ConditionalProbability<TClass, TToken>> CalculateProbabilities([NotNull] TToken token)
        {
            // calculate the token's probabilities for all classes
            double totalProbability;
            var probabilities = CalculateTokenProbabilityGivenClass(token, _trainingSets, out totalProbability);

            // apply Bayes theorem
            var inverseOfTotalProbability = 1.0D/totalProbability;
            return from cp in probabilities
                   let conditionalProbability = cp.Probability * inverseOfTotalProbability
                   select new ConditionalProbability<TClass, TToken>(cp.Class, cp.Token, conditionalProbability);
        }

        /// <summary>
        /// Calculates the token probabilities given a class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;TClass, TToken&gt;&gt;.</returns>
        [NotNull]
        private IEnumerable<ConditionalProbability<TClass, TToken>> CalculateTokenProbabilityGivenClass([NotNull] TToken token, [NotNull] IEnumerable<IDataSetAccessor<TClass, TToken>> sets)
        {
            return from set in sets
                   let @class = set.Class
                   let classProbability = @class.Probability
                   let percentageInClass = set.GetPercentage(token)
                   let probabilityInClass = percentageInClass * classProbability
                   select new ConditionalProbability<TClass, TToken>(@class, token, probabilityInClass);
        }

        /// <summary>
        /// Calculates the token probabilities given a class.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sets">The sets.</param>
        /// <param name="totalProbability">The total probability for the given classes.</param>
        /// <returns>IEnumerable&lt;ConditionalProbability&lt;TClass, TToken&gt;&gt;.</returns>
        [NotNull]
        private IEnumerable<ConditionalProbability<TClass, TToken>> CalculateTokenProbabilityGivenClass([NotNull] TToken token, [NotNull] IEnumerable<IDataSetAccessor<TClass, TToken>> sets, out double totalProbability)
        {
            var probabilities = CalculateTokenProbabilityGivenClass(token, sets).ToCollection();
            totalProbability = probabilities.Sum(p => p.Probability);
            return probabilities;
        }

        /// <summary>
        /// Splits the data sets.
        /// </summary>
        /// <param name="classUnderTest">The class under test.</param>
        /// <param name="remainingSets">The remaining sets.</param>
        /// <returns>IDataSet&lt;TClass, TToken&gt;.</returns>
        [NotNull]
        private IDataSetAccessor<TClass, TToken> SplitDataSets(TClass classUnderTest, [NotNull] out ICollection<IDataSetAccessor<TClass, TToken>> remainingSets)
        {
            IDataSet<TClass, TToken> setForClassUnderTest = null;
            remainingSets = new Collection<IDataSetAccessor<TClass, TToken>>();

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
            return setForClassUnderTest ?? new EmptyDataSet<TClass, TToken>(classUnderTest);
        }
    }
}
