using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface ITrainingSet
    /// </summary>
    public interface ITrainingSet : ITrainingSetAccessor
    {
        /// <summary>
        /// Adds the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="additionalDataSets">The additional data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSet</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        void Add([NotNull] IDataSet dataSet, [NotNull] params IDataSet[] additionalDataSets);

        /// <summary>
        /// Adds the specified data sets.
        /// </summary>
        /// <param name="dataSets">The data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSets</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        void Add([NotNull] IEnumerable<IDataSet> dataSets);
    }
}