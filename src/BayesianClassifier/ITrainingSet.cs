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
        /// Creates and registers a data set for the given class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        [NotNull]
        IDataSet CreateDataSet([NotNull] IClass @class);

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

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void ClearTokens();

        /// <summary>
        /// Updates the class probabilities.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="factor">An optional factor to the probabilities.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        /// <exception cref="System.ArgumentException">An unknown mode was given.</exception>
        void SetClassProbabilities(ClassProbabilities mode, double factor = 1.0D);
    }
}