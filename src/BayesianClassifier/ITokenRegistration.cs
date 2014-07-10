using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface ITokenRegistration
    /// </summary>
    /// <typeparam name="TToken">The type of the t token.</typeparam>
    public interface ITokenRegistration<in TToken> 
        where TToken : IToken
    {
        /// <summary>
        /// Adds the given tokens a single time, incrementing the <see cref="DataSet{TClass,TToken}.SetSize"/>
        /// and, at the first addition, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        void AddToken([NotNull] TToken token, [NotNull] params TToken[] additionalTokens);

        /// <summary>
        /// Adds the given tokens a single time, incrementing the <see cref="DataSet{TClass,TToken}.SetSize"/>
        /// and, at the first addition, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        void AddToken([NotNull] IEnumerable<TToken> tokens);

        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="DataSet{TClass,TToken}.SetSize"/> and,
        /// eventually, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        /// <seealso cref="DataSet{TClass,TToken}.PurgeToken(TToken,TToken[])"/>
        void RemoveTokenOnce([NotNull] TToken token, [NotNull] params TToken[] additionalTokens);

        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="DataSet{TClass,TToken}.SetSize"/> and,
        /// eventually, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        /// <seealso cref="DataSet{TClass,TToken}.PurgeToken(System.Collections.Generic.IEnumerable{TToken})"/>
        void RemoveTokenOnce([NotNull] IEnumerable<TToken> tokens);

        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="DataSet{TClass,TToken}.SetSize"/> and,
        /// eventually, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        /// <seealso cref="DataSet{TClass,TToken}.RemoveTokenOnce(TToken,TToken[])"/>
        void PurgeToken([NotNull] TToken token, [NotNull] params TToken[] additionalTokens);

        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="DataSet{TClass,TToken}.SetSize"/> and,
        /// eventually, the <see cref="DataSet{TClass,TToken}.TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        /// <seealso cref="DataSet{TClass,TToken}.RemoveTokenOnce(System.Collections.Generic.IEnumerable{TToken})"/>
        void PurgeToken([NotNull] IEnumerable<TToken> tokens);
    }
}