using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace BayesianClassifier.Tests;

public sealed class DataSetTests
{
    private readonly StringClass _class;
    private readonly DataSet _dataSet;

    public DataSetTests()
    {
        _class = new StringClass(Guid.NewGuid().ToString(), 1.0D);
        _dataSet = new DataSet(_class);
    }

    [Fact]
    public void AfterConstructionClassIsAccessible()
    {
        _dataSet.Class.Should().Be(_class, "because we set the class in the constructor");
    }

    [Fact]
    public void AfterConstructionTokenCountIsZero()
    {
        _dataSet.TokenCount.Should().Be(0, "because no tokens are registered");
    }

    [Fact]
    public void AfterConstructionSetSizeIsZero()
    {
        _dataSet.SetSize.Should().Be(0, "because no tokens are registered");
    }

    [Fact]
    public void AfterAddingATokenTheTokenCountIncrementsOnce()
    {
        var token = new StringToken("foo");
        _dataSet.AddToken(token);
        _dataSet.TokenCount.Should().Be(1, "because we added a single token");
        _dataSet.SetSize.Should().Be(1, "because we added a single token");
    }

    [Fact]
    public void AfterAddingATokenTwiceTheTokenCountIncrementsOnceAndTheSetSizeTwice()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        _dataSet.AddToken(token1);
        _dataSet.AddToken(token2);
        _dataSet.TokenCount.Should().Be(1, "because we added a single token");
        _dataSet.SetSize.Should().Be(2, "because we added the same token twice");
    }

    [Fact]
    public void AfterAddingTwoTokensTheTokenCountIncrementsTwiceAndTheSetSizeTwice()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("bar");
        _dataSet.AddToken(token1);
        _dataSet.AddToken(token2);
        _dataSet.TokenCount.Should().Be(2, "because we added two distinct tokens");
        _dataSet.SetSize.Should().Be(2, "because we added two tokens");
    }

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
    public void AddingMultipleTokensIncrementsCountAndSetSize()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        var token3 = new StringToken("bar");
        _dataSet.AddToken(token1, token2, token3);

        _dataSet.TokenCount.Should().Be(2, "because we added three tokens including two occurrences of the same distinct tokens");
        _dataSet.SetSize.Should().Be(3, "because we added three tokens");
    }

    [Fact]
    public void EnumeratingTheTokensYieldsTheCurrectCount()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        var token3 = new StringToken("bar");
        _dataSet.AddToken(token1, token2, token3);

        var counts = _dataSet.Select(static x => x).ToList();
        counts.Count.Should().Be(2, "because only two distinct items were added");
        counts.Aggregate(0L, static (i, entry) => i + entry.Count).Should().Be(3, "because three tokens were added");

        // select the only instance of the first token
        var element1 = counts.Where(entry => entry.Token.Equals(token1))
            .Select(static entry => entry.Count)
            .Single();

        // check the count
        element1
            .Should()
            .Be(2, "because two instances of that token were added");

        // select the same instance as above with the second instance of the first token as reference
        counts.Where(entry => entry.Token.Equals(token2))
            .Select(static entry => entry.Count)
            .Single()
            .Should()
            .Be(element1, "because two instances of that token were added");

        counts.Where(entry => entry.Token.Equals(token3))
            .Select(static entry => entry.Count)
            .Single()
            .Should()
            .Be(1, "because one instance of that token was added");
    }

    [Fact]
    public void GetCountReturnsTheCorrectCountPerToken()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        var token3 = new StringToken("bar");
        _dataSet.AddToken(token1, token2, token3);

        _dataSet.GetCount(token1).Should().Be(2, "because we added that token twice");
        _dataSet.GetCount(token3).Should().Be(1, "because we added that token once");
    }

    [Fact]
    public void GetPercentageReturnsTheCorrectPercentagePerToken()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        var token3 = new StringToken("bar");
        _dataSet.AddToken(token1, token2, token3);

        _dataSet.GetPercentage(token1)
            .Should()
            .BeApproximately(2/3D, 0.0001D, "because we added that token twice out of three tokens");
        _dataSet.GetPercentage(token3)
            .Should()
            .BeApproximately(1/3D, 0.0001D, "because we added that token once out of three tokens");
    }

    [Fact]
    public void IndexerReturnsTheCorrectTokenInformation()
    {
        var token1 = new StringToken("foo");
        var token2 = new StringToken("foo");
        var token3 = new StringToken("bar");
        _dataSet.AddToken(token1, token2, token3);

        _dataSet[token1].Count
            .Should()
            .Be(2, "because we added that token twice out of three tokens");

        _dataSet[token1].Percentage
            .Should()
            .BeApproximately(2 / 3D, 0.0001D, "because we added that token twice out of three tokens");

        _dataSet[token3].Count
            .Should()
            .Be(1, "because we added that token once out of three tokens");

        _dataSet[token3].Percentage
            .Should()
            .BeApproximately(1 / 3D, 0.0001D, "because we added that token once out of three tokens");
    }
}
