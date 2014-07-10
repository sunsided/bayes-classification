using System;
using BayesianClassifier;
using FluentAssertions;
using NUnit.Framework;

namespace BayesianClassifierTests
{
    [TestFixture]
    public class DataSetTests
    {
        /// <summary>
        /// The class
        /// </summary>
        private StringClass _class;

        /// <summary>
        /// The data set
        /// </summary>
        private DataSet<StringClass, StringToken> _dataSet;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _class = new StringClass(Guid.NewGuid().ToString());
            _dataSet = new DataSet<StringClass, StringToken>(_class);
        }

        [Test]
        public void AfterConstructionClassIsAccessible()
        {
            _dataSet.Class.Should().Be(_class, "because we set the class in the constructor");
        }

        [Test]
        public void AfterConstructionTokenCountIsZero()
        {
            _dataSet.TokenCount.Should().Be(0, "because no tokens are registered");
        }

        [Test]
        public void AfterConstructionSetSizeIsZero()
        {
            _dataSet.SetSize.Should().Be(0, "because no tokens are registered");
        }

        [Test]
        public void AfterAddingATokenTheTokenCountIncrementsOnce()
        {
            var token = new StringToken("foo");
            _dataSet.AddToken(token);
            _dataSet.TokenCount.Should().Be(1, "because we added a single token");
            _dataSet.SetSize.Should().Be(1, "because we added a single token");
        }

        [Test]
        public void AfterAddingATokenTwiceTheTokenCountIncrementsOnceAndTheSetSizeTwice()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);
            _dataSet.TokenCount.Should().Be(1, "because we added a single token");
            _dataSet.SetSize.Should().Be(2, "because we added the same token twice");
        }

        [Test]
        public void AfterAddingTwoTokensTheTokenCountIncrementsTwiceAndTheSetSizeTwice()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("bar");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);
            _dataSet.TokenCount.Should().Be(2, "because we added two distinct tokens");
            _dataSet.SetSize.Should().Be(2, "because we added two tokens");
        }

        [Test]
        public void AfterRemovingATokenTheTokenCountAndSetSizeDecrements()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("bar");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);
            
            _dataSet.RemoveTokenOnce(token1);
            _dataSet.TokenCount.Should().Be(1, "because we added two distinct tokens and then removed one");
            _dataSet.SetSize.Should().Be(1, "because we added two tokens and then removed one");
        }

        [Test]
        public void AfterRemovingTwoTokenTheTokenCountAndSetSizeAreZero()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("bar");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);

            _dataSet.RemoveTokenOnce(token1);
            _dataSet.RemoveTokenOnce(token2);
            _dataSet.TokenCount.Should().Be(0, "because we added two distinct tokens and then removed both");
            _dataSet.SetSize.Should().Be(0, "because we added two tokens and then removed both");
        }

        [Test]
        public void AfterRemovingATokenWithMultipleOccurrencesTheSetSizeDecrements()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);

            _dataSet.RemoveTokenOnce(token1);
            _dataSet.TokenCount.Should().Be(1, "because we added two occurrences of the same distinct tokens and then removed one");
            _dataSet.SetSize.Should().Be(1, "because we added two tokens and then removed one");
        }

        [Test]
        public void AfterPurginATokenWithMultipleOccurrencesTheTokenCountAndSetSizeDecrements()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);

            _dataSet.PurgeToken(token1);
            _dataSet.TokenCount.Should().Be(0, "because we added two occurrences of the same distinct tokens and then purged it");
            _dataSet.SetSize.Should().Be(0, "because we added two tokens and then purged them");
        }

        [Test]
        public void AfterPurginATokenWithMultipleOccurrencesTheTokenCountAndSetSizeDecrementsButKeepsTheRemainingTokens()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            var token3 = new StringToken("bar");
            _dataSet.AddToken(token1);
            _dataSet.AddToken(token2);
            _dataSet.AddToken(token3);

            _dataSet.PurgeToken(token1);
            _dataSet.TokenCount.Should().Be(1, "because we added three tokens including two occurrences of the same distinct tokens and then purged that one");
            _dataSet.SetSize.Should().Be(1, "because we added three tokens and then purged two of them");
        }

        [Test]
        public void AddingMultipleTokensIncrementsCountAndSetSize()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            var token3 = new StringToken("bar");
            _dataSet.AddToken(token1, token2, token3);

            _dataSet.TokenCount.Should().Be(2, "because we added three tokens including two occurrences of the same distinct tokens");
            _dataSet.SetSize.Should().Be(3, "because we added three tokens");
        }
    }
}
