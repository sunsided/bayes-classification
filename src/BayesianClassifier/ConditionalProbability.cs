using System;
using System.Diagnostics;

namespace BayesianClassifier;

/// <summary>
/// Struct ConditionalProbability
/// </summary>
[DebuggerDisplay("P({Class}|{Token})={Probability}")]
public readonly struct ConditionalProbability : IEquatable<ConditionalProbability>
{
    /// <summary>
    /// The class
    /// </summary>
    public readonly IClass Class;

    /// <summary>
    /// The token
    /// </summary>
    public readonly IToken Token;

    /// <summary>
    /// The conditional probability
    /// </summary>
    public readonly double Probability;

    /// <summary>
    /// The occurrence of the token during the training phase.
    /// </summary>
    public readonly long Occurrence;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionalProbability" /> struct.
    /// </summary>
    /// <param name="class">The class.</param>
    /// <param name="token">The token.</param>
    /// <param name="probability">The probability.</param>
    /// <param name="occurrence">The occurrence.</param>
    /// <exception cref="System.ArgumentNullException">@class
    /// or
    /// token</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">probability;Probability must greater than or equal to zero
    /// or
    /// probability;Probability must less than or equal to one</exception>
    public ConditionalProbability(IClass @class, IToken token, double probability, long occurrence)
    {
        if (probability < 0) throw new ArgumentOutOfRangeException(nameof(probability), probability, "Probability must greater than or equal to zero");
        if (probability > 1) throw new ArgumentOutOfRangeException(nameof(probability), probability, "Probability must less than or equal to one");
        if (probability < 0) throw new ArgumentOutOfRangeException(nameof(occurrence), occurrence, "Occurrence must greater than or equal to zero");

        Class = @class ?? throw new ArgumentNullException(nameof(@class));
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Probability = probability;
        Occurrence = occurrence;
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">Another object to compare to.</param>
    /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        return obj is ConditionalProbability probability && Equals(probability);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
    public bool Equals(ConditionalProbability other) =>
        Class.Equals(other.Class)
        && Token.Equals(other.Token)
        && Probability.Equals(other.Probability);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        var hash = 27;
        hash = (13 * hash) + Class.GetHashCode();
        hash = (13 * hash) + Token.GetHashCode();
        hash = (13 * hash) + Probability.GetHashCode();
        return hash;
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => $"P({Class}|{Token})={Probability:P}";
}
