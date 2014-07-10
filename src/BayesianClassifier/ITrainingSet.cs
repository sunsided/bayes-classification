using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface ITrainingSet
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TToken">The type of the token.</typeparam>
    public interface ITrainingSet<TClass, TToken> : ITrainingSetAccessor<TClass, TToken> 
        where TClass : IClass 
        where TToken : IToken
    {
        /// <summary>
        /// Adds the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="additionalDataSets">The additional data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSet</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        void Add([NotNull] IDataSet<TClass, TToken> dataSet, [NotNull] params IDataSet<TClass, TToken>[] additionalDataSets);

        /// <summary>
        /// Adds the specified data sets.
        /// </summary>
        /// <param name="dataSets">The data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSets</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        void Add([NotNull] IEnumerable<IDataSet<TClass, TToken>> dataSets);
    }
}