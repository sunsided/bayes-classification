using System;
using System.Collections;
using System.Collections.Generic;
using BayesianClassifier;

namespace BayesianClassifierTests
{
    /// <summary>
    /// Class NullTrainingSetAccessor. This class cannot be inherited.
    /// </summary>
    internal sealed class NullTrainingSetAccessor : ITrainingSetAccessor
    {
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IDataSet> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        public int OccurrenceThreshold
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the percentage threshold.
        /// <para>
        /// Any per-class token percentage lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        /// <value>The occurrence threshold.</value>
        public double PercentageThreshold
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the size of the vocabulary, i.e. the number of all tokens.
        /// </summary>
        /// <value>The size of the vocabulary.</value>
        public long VocabularySize
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the <see cref="IDataSet" /> with the specified class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        public IDataSet this[IClass @class]
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the set for class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        public IDataSet GetSetForClass(IClass @class)
        {
            throw new NotImplementedException();
        }
    }
}
