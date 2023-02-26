using System;
using BayesianClassifier.Abstractions;

namespace BayesianClassifier;

/// <summary>
/// Struct TokenInformation
/// </summary>
public struct TokenInformation<TToken>
    where TToken: IToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenInformation{TToken}" /> struct.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="count">The count.</param>
    /// <param name="percentage">The percentage.</param>
    /// <exception cref="System.ArgumentNullException">token</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// count;Count must be positive or zero
    /// or
    /// percentage;Percentage must be positive or zero
    /// </exception>
    public TokenInformation(TToken token, long count, double percentage)
    {
        if (ReferenceEquals(token, null)) throw new ArgumentNullException(nameof(token));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be positive or zero");
        if (percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage), percentage, "Percentage must be positive or zero");

        Token = token;
        Count = count;
        Percentage = percentage;
    }

    /// <summary>
    /// The token
    /// </summary>
    public TToken Token { get; }

    /// <summary>
    /// The count in the class
    /// </summary>
    public long Count { get; }

    /// <summary>
    /// The occurrence percentage of the token in the class.
    /// </summary>
    public double Percentage { get; }
}
