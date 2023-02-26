using System;
using BayesianClassifier.Abstractions;

namespace BayesianClassifier;

/// <summary>
/// Struct TokenCount
/// </summary>
public struct TokenCount
{
    /// <summary>
    /// The token
    /// </summary>
    public readonly IToken Token;
        
    /// <summary>
    /// The number of occurrences
    /// </summary>
    public readonly long Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenCount" /> struct.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="count">The count.</param>
    /// <exception cref="System.ArgumentNullException">token</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">count;Count must be positive or zero.</exception>
    public TokenCount(IToken token, long count)
    {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be positive or zero.");

        Token = token ?? throw new ArgumentNullException(nameof(token));
        Count = count;
    }
}
