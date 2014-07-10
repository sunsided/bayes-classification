using System;
using System.Linq;
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

        [Test]
        public void EnumeratingTheTokensYieldsTheCurrectCount()
        {
            var token1 = new StringToken("foo");
            var token2 = new StringToken("foo");
            var token3 = new StringToken("bar");
            _dataSet.AddToken(token1, token2, token3);

            var counts = _dataSet.Select(x => x).ToList();
            counts.Count.Should().Be(2, "because only two distinct items were added");
            counts.Aggregate(0L, (i, entry) => i + entry.Count).Should().Be(3, "because three tokens were added");

            // select the only instance of the first token
            var element1 = counts.Where(entry => entry.Token.Equals(token1))
                .Select(entry => entry.Count)
                .Single();

            // check the count
            element1
                .Should()
                .Be(2, "because two instances of that token were added");

            // select the same instance as above with the second instance of the first token as reference
            counts.Where(entry => entry.Token.Equals(token2))
                .Select(entry => entry.Count)
                .Single()
                .Should()
                .Be(element1, "because two instances of that token were added");

            counts.Where(entry => entry.Token.Equals(token3))
                .Select(entry => entry.Count)
                .Single()
                .Should()
                .Be(1, "because one instance of that token was added");
        }
    }
}
