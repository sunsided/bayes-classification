using System.Linq;
using BayesianClassifier;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace BayesianClassifierTests
{
    [TestFixture]
    public class ClassifierTests
    {
        /// <summary>
        /// The training set
        /// </summary>
        private ITrainingSet<StringClass, StringToken> _trainingSet;

        /// <summary>
        /// The classifier
        /// </summary>
        private NaiveClassifier<StringClass, StringToken> _classifier;

        /// <summary>
        /// The spam class
        /// </summary>
        private static StringClass _spamClass;

        /// <summary>
        /// The ham class
        /// </summary>
        private static StringClass _hamClass;

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _trainingSet = BuildTrainingSet();
            _classifier = BuildClassifier(_trainingSet);
        }

        /// <summary>
        /// Builds the classifier.
        /// </summary>
        /// <returns>Classifier&lt;StringClass, StringToken&gt;.</returns>
        [NotNull]
        private NaiveClassifier<StringClass, StringToken> BuildClassifier([NotNull] ITrainingSetAccessor<StringClass, StringToken> trainingSet)
        {
            return new NaiveClassifier<StringClass, StringToken>(trainingSet);
        }

        /// <summary>
        /// Builds the training set.
        /// </summary>
        /// <returns>ITrainingSet&lt;StringClass, StringToken&gt;.</returns>
        [NotNull]
        private static ITrainingSet<StringClass, StringToken> BuildTrainingSet()
        {
            var trainingSet = new TrainingSet<StringClass, StringToken>();

            // build data sets
            var spamSet = BuildSpamDataSet();
            var hamSet = BuildHamDataSet();

            // monkey test
            spamSet.SetSize.Should()
                .Be(hamSet.SetSize, "because this test relies on identical set sizes for exact probability testing");

            // register classes
            _spamClass = spamSet.Class;
            _hamClass = hamSet.Class;

            // add the sets and return
            trainingSet.Add(spamSet, hamSet);
            return trainingSet;
        }

        /// <summary>
        /// Builds the spam data set.
        /// </summary>
        /// <returns>IDataSet&lt;StringClass, StringToken&gt;.</returns>
        [NotNull]
        private static IDataSet<StringClass, StringToken> BuildSpamDataSet()
        {
            return BuildDataSet("spam", 0.5D, "rolex", "watches", "viagra", "prince", "money", "send", "xyzzy");
        }

        /// <summary>
        /// Builds the spam data set.
        /// </summary>
        /// <returns>IDataSet&lt;StringClass, StringToken&gt;.</returns>
        [NotNull]
        private static IDataSet<StringClass, StringToken> BuildHamDataSet()
        {
            return BuildDataSet("ham", 0.5D, "love", "flowers", "unicorn", "friendship", "money", "send", "send");
        }

        /// <summary>
        /// Builds the data set.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="classProbability">The class probability.</param>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <returns>IDataSet&lt;StringClass, StringToken&gt;.</returns>
        [NotNull]
        private static IDataSet<StringClass, StringToken> BuildDataSet([NotNull] string className, double classProbability, [NotNull] string token, [NotNull] params string[] additionalTokens)
        {
            var @class = new StringClass(className, classProbability);
            var dataSet = new DataSet<StringClass, StringToken>(@class);

            dataSet.AddToken(new StringToken(token));
            dataSet.AddToken(additionalTokens.Select(t => new StringToken(t)));

            return dataSet;
        }

        [Test]
        public void CalculateProbabilityReturnsOneHundredPercentForAKnownSpamWord()
        {
            var token = new StringToken("rolex");
            var probability = _classifier.CalculateProbability(_spamClass, token);
            probability.Should().BeApproximately(1.0D, 0.0001D, "because the word is known the be a spam word");
        }

        [Test]
        public void CalculateProbabilityReturnsOneHundredPercentForAKnownHamWord()
        {
            var token = new StringToken("unicorn");
            var probability = _classifier.CalculateProbability(_hamClass, token);
            probability.Should().BeApproximately(1.0D, 0.0001D, "because the word is known the be a ham word");
        }

        [Test]
        public void CalculateProbabilitiesWithHamWordReturnsProbabilitiesForAllClasses()
        {
            var token = new StringToken("unicorn");
            var probabilities = _classifier.CalculateProbabilities(token).ToList();
            probabilities.Single(p => p.Class.Equals(_spamClass))
                .Probability.Should()
                .BeApproximately(0, 0.000001, "because the token is known to be a ham word");

            probabilities.Single(p => p.Class.Equals(_hamClass))
                .Probability.Should()
                .BeApproximately(1, 0.000001, "because the token is known to be a ham word");
        }

        [Test]
        public void CalculateProbabilitiesWithMixedWordReturnsProbabilitiesForAllClasses()
        {
            var token = new StringToken("money");
            var probabilities = _classifier.CalculateProbabilities(token).ToList();
            probabilities.Single(p => p.Class.Equals(_spamClass))
                .Probability.Should()
                .BeApproximately(0.5, 0.000001, "because the token is known to be a ham and spam word");

            probabilities.Single(p => p.Class.Equals(_hamClass))
                .Probability.Should()
                .BeApproximately(0.5, 0.000001, "because the token is known to be a ham and spam  word");
        }

        [Test]
        public void CalculateProbabilitiesWithMixedWordThatIsMoreLikelyHamThanSpamReturnsProbabilitiesForAllClasses()
        {
            var token = new StringToken("send");
            var probabilities = _classifier.CalculateProbabilities(token).ToList();
            probabilities.Single(p => p.Class.Equals(_spamClass))
                .Probability.Should()
                .BeApproximately(1/3D, 0.000001, "because the token is more likely to be a ham than spam word");

            probabilities.Single(p => p.Class.Equals(_hamClass))
                .Probability.Should()
                .BeApproximately(2/3D, 0.000001, "because the token is more likely to be a ham than spam word");
        }
    }
}
