using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class DataSet.
    /// </summary>
    [DebuggerDisplay("Data set for class P({Class.Name})={Class.Probability}")]
    public sealed class DataSet : IDataSet
    {
        /// <summary>
        /// The training set
        /// </summary>
        private readonly ITrainingSetAccessor _trainingSet;

        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        public int OccurrenceThreshold
        {
            get { return _trainingSet.OccurrenceThreshold; }
            set { _trainingSet.OccurrenceThreshold = value; }
        }

        /// <summary>
        /// Gets or sets the percentage threshold.
        /// <para>
        /// Any per-class token percentage lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="NotFiniteNumberException">value;Value must be finite.</exception>
        public double PercentageThreshold
        {
            get { return _trainingSet.PercentageThreshold; }
            set { _trainingSet.PercentageThreshold = value; }
        }

        /// <summary>
        /// The default smoothing alpha
        /// </summary>
        public const double DefaultSmoothingAlpha = 0D;

        /// <summary>
        /// The token count
        /// </summary>
        private readonly ConcurrentDictionary<IToken, long> _tokenCount = new ConcurrentDictionary<IToken, long>();

        /// <summary>
        /// The set size, i.e. the number of all tokens 
        /// </summary>
        private long _setSize;

        /// <summary>
        /// Gets the number of distinct tokens, 
        /// i.e. every token counted at exactly once.
        /// </summary>
        /// <value>The token count.</value>
        /// <seealso cref="SetSize"/>
        public long TokenCount
        {
            get { return _tokenCount.Count; }
        }

        /// <summary>
        /// Gets the size of the set.
        /// </summary>
        /// <value>The size of the set.</value>
        /// <seealso cref="TokenCount"/>
        public long SetSize
        {
            get { return _setSize; }
        }

        /// <summary>
        /// Gets the class.
        /// </summary>
        /// <value>The class.</value>
        [NotNull]
        public IClass Class { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet" /> class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="trainingSet">The training set.</param>
        /// <exception cref="System.ArgumentNullException">@class</exception>
        public DataSet([NotNull] IClass @class, [NotNull] ITrainingSetAccessor trainingSet)
        {
            if (ReferenceEquals(@class, null)) throw new ArgumentNullException("class");
            if (ReferenceEquals(trainingSet, null)) throw new ArgumentNullException("trainingSet");
            
            Class = @class;
            _trainingSet = trainingSet;
        }

        /// <summary>
        /// Gets the <see cref="TokenInformation{IToken}" /> with the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>TokenInformation&lt;IToken&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        public TokenInformation<IToken> this[IToken token, double alpha = DefaultSmoothingAlpha]
        {
            get
            {
                if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");

                long count;
                if (!_tokenCount.TryGetValue(token, out count))
                {
                    return new TokenInformation<IToken>(token, 0L, 0D);
                }

                var percentage = GetPercentage(count, alpha);
                return new TokenInformation<IToken>(token, count, percentage);
            }
        }

        /// <summary>
        /// Gets the number of occurrences of the given token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetPercentage" />
        public long GetCount([NotNull] IToken token)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");

            long count;
            if (!_tokenCount.TryGetValue(token, out count)) return 0;
            if (count <= OccurrenceThreshold) return 0;
            return count;
        }

        /// <summary>
        /// Gets the approximated percentage of the given
        /// <see cref="IToken" /> in this data set
        /// by determining its occurrence count over the whole population.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">alpha;Smoothing parameter alpha must be greater than or equal to zero.</exception>
        /// <seealso cref="GetCount" />
        public double GetPercentage([NotNull] IToken token, double alpha = DefaultSmoothingAlpha)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (alpha < 0) throw new ArgumentOutOfRangeException("alpha", alpha, "Smoothing parameter alpha must be greater than or equal to zero.");

            var count = GetCount(token);
            
            // check unsmoothed percentage for threshold
            if (count > 0 && GetPercentage(count) <= PercentageThreshold) count = 0;

            // get smoothed percentage
            return GetPercentage(count, alpha);
        }

        /// <summary>
        /// Gets the approximated percentage of the given
        /// <see cref="IToken" /> in this data set
        /// by determining its occurrence count over the whole population.
        /// </summary>
        /// <param name="tokenCount">The token count.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <seealso cref="GetCount" />
        private double GetPercentage(long tokenCount, double alpha = DefaultSmoothingAlpha)
        {
            Debug.Assert(alpha >= 0, "alpha >= 0");
            Debug.Assert(tokenCount >= 0, "tokenCount >= 0");

#if false
            var totalCount = _setSize;
            var vocabularySize = TokenCount;
#else
            //var totalCount = TokenCount;
            var totalCount = _setSize;
            var vocabularySize = _trainingSet.VocabularySize;
            
#endif

            return (double)(tokenCount + alpha)/(double)(totalCount + alpha*vocabularySize);
        }

        /// <summary>
        /// Calculates the minimal smoothed value.
        /// </summary>
        /// <param name="tokenCount">The count.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns>System.Double.</returns>
        public double LapaceSmoothing(long tokenCount, long totalCount, double? alpha = 0)
        {
            var a = alpha ?? DefaultSmoothingAlpha;

            var vocabularySize = (double)_trainingSet.VocabularySize;
            return (tokenCount + a) / (totalCount + a * vocabularySize);
        }

        /// <summary>
        /// Gets the approximated percentage of the given
        /// <see cref="IToken" /> in this data set
        /// by determining its occurrence count over the whole population.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="alpha">Additive smoothing parameter. If set to zero, no Laplace smoothing will be applied.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">alpha;Smoothing parameter alpha must be greater than or equal to zero.</exception>
        /// <seealso cref="GetCount" />
        public TokenStats GetStats([NotNull] IToken token, double alpha)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (alpha < 0) throw new ArgumentOutOfRangeException("alpha", alpha, "Smoothing parameter alpha must be greater than or equal to zero.");

            var count = GetCount(token);

            // check unsmoothed percentage for threshold
            if (count > 0 && GetPercentage(count) <= PercentageThreshold) count = 0;

            // get smoothed percentage and return
            var percentage = GetPercentage(count, alpha);
            return new TokenStats(percentage, count);
        }

        /// <summary>
        /// Adds the given tokens a single time, incrementing the <see cref="SetSize"/>
        /// and, at the first addition, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        public void AddToken([NotNull] IToken token, [NotNull] params IToken[] additionalTokens)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (ReferenceEquals(additionalTokens, null)) throw new ArgumentNullException("additionalTokens");

            _tokenCount.AddOrUpdate(token, AddFirstToken, IncrementTokenCount);
            Interlocked.Increment(ref _setSize);

            AddToken(additionalTokens);
        }

        /// <summary>
        /// Adds the given tokens a single time, incrementing the <see cref="SetSize"/>
        /// and, at the first addition, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        public void AddToken([NotNull] IEnumerable<IToken> tokens)
        {
            if (ReferenceEquals(tokens, null)) throw new ArgumentNullException("tokens");

            foreach (var token in tokens)
            {
                _tokenCount.AddOrUpdate(token, AddFirstToken, IncrementTokenCount);
                Interlocked.Increment(ref _setSize);
            }
        }

        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="SetSize"/> and,
        /// eventually, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        /// <seealso cref="PurgeToken(IToken,IToken[])"/>
        public void RemoveTokenOnce([NotNull] IToken token, [NotNull] params IToken[] additionalTokens)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (ReferenceEquals(additionalTokens, null)) throw new ArgumentNullException("additionalTokens");

            RemoveSingleTokenInternal(token);
            RemoveTokenOnce(additionalTokens);
        }
        
        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="SetSize"/> and,
        /// eventually, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        /// <seealso cref="PurgeToken(IEnumerable&lt;IToken&gt;)"/>
        public void RemoveTokenOnce([NotNull] IEnumerable<IToken> tokens)
        {
            if (ReferenceEquals(tokens, null)) throw new ArgumentNullException("tokens");

            foreach (var token in tokens)
            {
                RemoveSingleTokenInternal(token);
            }
        }
        
        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="SetSize"/> and,
        /// eventually, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="additionalTokens">The additional tokens.</param>
        /// <exception cref="System.ArgumentNullException">
        /// token
        /// or
        /// additionalTokens
        /// </exception>
        /// <seealso cref="RemoveTokenOnce(IToken,IToken[])"/>
        public void PurgeToken([NotNull] IToken token, [NotNull] params IToken[] additionalTokens)
        {
            if (ReferenceEquals(token, null)) throw new ArgumentNullException("token");
            if (ReferenceEquals(additionalTokens, null)) throw new ArgumentNullException("additionalTokens");

            PurgeTokenInternal(token);
            PurgeToken(additionalTokens);
        }
        
        /// <summary>
        /// Removes the given tokens a single time, decrementing the <see cref="SetSize"/> and,
        /// eventually, the <see cref="TokenCount"/>.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.ArgumentNullException">tokens</exception>
        /// <seealso cref="RemoveTokenOnce(IEnumerable&lt;IToken&gt;)"/>
        public void PurgeToken([NotNull] IEnumerable<IToken> tokens)
        {
            if (ReferenceEquals(tokens, null)) throw new ArgumentNullException("tokens");

            foreach (var token in tokens)
            {
                PurgeTokenInternal(token);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _tokenCount.Clear();
        }

        /// <summary>
        /// Purges the tokens fulfilling the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void PurgeWhere([NotNull] Predicate<TokenCount> predicate)
        {
            var candidateForPurge = from pair in _tokenCount
                let tokenCount = new TokenCount(pair.Key, pair.Value)
                where predicate(tokenCount)
                select pair.Key;
            PurgeToken(candidateForPurge);
        }

        /// <summary>
        /// Removes the single token internally.
        /// </summary>
        /// <param name="token">The token.</param>
        private void RemoveSingleTokenInternal([NotNull] IToken token)
        {
            long count;
            while (_tokenCount.TryGetValue(token, out count))
            {
                var newValue = count - 1;
                var collectionUpdated = _tokenCount.TryUpdate(token, newValue: newValue, comparisonValue: count);
                if (!collectionUpdated) continue;
                Interlocked.Decrement(ref _setSize);

                if (newValue == 0)
                {
                    // explicit removal if the count is zero
                    var collection = _tokenCount as ICollection<KeyValuePair<IToken, long>>;
                    collection.Remove(new KeyValuePair<IToken, long>(token, 0));
                }

                break;
            }
        }

        /// <summary>
        /// Purges a single token internally.
        /// </summary>
        /// <param name="token">The token.</param>
        private void PurgeTokenInternal([NotNull] IToken token)
        {
            long count;
            if (!_tokenCount.TryRemove(token, out count)) return;

            // decrement 'count' times
            // TODO: use Interlocked.CompareExchange
            for (int i = 0; i < count; ++i)
            {
                Interlocked.Decrement(ref _setSize);
            }
        }

        /// <summary>
        /// Factory to initialize the value in <see cref="_tokenCount"/> for the given <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.Int64.</returns>
        private static long AddFirstToken(IToken token)
        {
            return 1;
        }

        /// <summary>
        /// Factory to increment the value in <see cref="_tokenCount"/> for the given <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="count">The number of tokens.</param>
        /// <returns>System.Int64.</returns>
        private static long IncrementTokenCount(IToken token, long count)
        {
            return count + 1;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TokenCount> GetEnumerator()
        {
            var ot = OccurrenceThreshold;
            return _tokenCount.Select(token => new TokenCount(token.Key, token.Value <= ot ? 0 : token.Value)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
