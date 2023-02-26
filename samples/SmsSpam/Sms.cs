using System;
using System.Diagnostics;

namespace SmsSpam;

/// <summary>
/// Class Sms. This class cannot be inherited.
/// </summary>
[DebuggerDisplay("{Type} {Content}")]
internal sealed class Sms : IEquatable<Sms>
{
    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public MessageType Type { get; private init; }

    /// <summary>
    /// Gets the content.
    /// </summary>
    /// <value>The content.</value>
    public string Content { get; private init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sms"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="content">The content.</param>
    public Sms(MessageType type, string content)
    {
        Type = type;
        Content = content;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Sms" /> is equal to this instance.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public bool Equals(Sms? other) =>
        other is not null &&
        Type == other.Type &&
        string.Equals(Content, other.Content, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is Sms sms && Equals(sms);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((int) Type*397) ^ Content.ToLowerInvariant().GetHashCode();
        }
    }
}
