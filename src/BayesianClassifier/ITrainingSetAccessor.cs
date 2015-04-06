using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface ITrainingSetAccerssor
    /// </summary>
    public interface ITrainingSetAccessor : IEnumerable<IDataSet> 
    {
        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        int OccurrenceThreshold { get; set; }

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
        double PercentageThreshold { get; set; }

        /// <summary>
        /// Gets the size of the vocabulary, i.e. the number of all tokens.
        /// </summary>
        /// <value>The size of the vocabulary.</value>
        long VocabularySize { get; }

        /// <summary>
        /// Gets the <see cref="IDataSet"/> with the specified class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet&lt;TClass, TToken&gt;.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        IDataSet this[IClass @class] { get; }

        /// <summary>
        /// Gets the set for class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        [NotNull]
        IDataSet GetSetForClass([NotNull] IClass @class);
    }
}