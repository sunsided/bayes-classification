﻿using System;
using System.Diagnostics;
using BayesianClassifier.Abstractions;

namespace BayesianClassifier;

/// <summary>
/// Class StringToken. This class cannot be inherited.
/// </summary>
[DebuggerDisplay("{Value}")]
public sealed class StringToken : IToken
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringToken"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public StringToken(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; private init; }

    /// <summary>
    /// Determines whether the specified <see cref="StringToken" /> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
    /// <returns><see langword="true" /> if the specified <see cref="StringToken" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
    private bool Equals(StringToken other) => string.Equals(Value, other.Value);

    /// <summary>
    /// Determines whether the specified <see cref="IToken" /> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
    /// <returns><see langword="true" /> if the specified <see cref="StringToken" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
    bool IEquatable<IToken>.Equals(IToken? other)
    {
        var otherAsObject = (object?)other;
        return Equals(otherAsObject);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
    /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object? obj)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is StringToken token && Equals(token);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => Value;
}
