using System;
using BayesianClassifier;
using JetBrains.Annotations;

namespace SmsSpam
{
    /// <summary>
    /// Class EnumClass.
    /// </summary>
    class EnumClass : ClassBase, IEquatable<EnumClass>, IEquatable<MessageType>
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public MessageType Type { get; private set; }

        public EnumClass(MessageType type, double probability)
            : base(type.ToString(), probability)
        {
            Type = type;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public override bool Equals([CanBeNull] IClass other)
        {
            return other is EnumClass && Equals((EnumClass) other);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] EnumClass other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Type.Equals(other.Type);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(MessageType other)
        {
            return Type.Equals(other);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Type.ToString();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }
    }
}
