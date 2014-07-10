﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class TrainingSet. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TToken">The type of the token.</typeparam>
    public sealed class TrainingSet<TClass, TToken> : ITrainingSet<TClass, TToken> 
        where TClass: IClass
        where TToken: IToken
    {
        /// <summary>
        /// The data sets
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<TClass, IDataSet<TClass, TToken>> _dataSets = new ConcurrentDictionary<TClass, IDataSet<TClass, TToken>>();

        /// <summary>
        /// Gets the <see cref="IDataSet{TClass, TToken}"/> with the specified class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet&lt;TClass, TToken&gt;.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        public IDataSet<TClass, TToken> this[TClass @class]
        {
            get
            {
                IDataSet<TClass, TToken> set;
                if (_dataSets.TryGetValue(@class, out set)) return set;
                throw new ArgumentException("No data set was registered for the given class", "class");
            }
        }

        /// <summary>
        /// Adds the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="additionalDataSets">The additional data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSet</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        public void Add([NotNull] IDataSet<TClass, TToken> dataSet, [NotNull] params IDataSet<TClass, TToken>[] additionalDataSets)
        {
            if (ReferenceEquals(dataSet, null)) throw new ArgumentNullException("dataSet");

            try
            {
                AddInternal(dataSet);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("A data set for a given class was already registered.", e);
            }

            // may throw, that's anticipated
            Add(additionalDataSets);
        }

        /// <summary>
        /// Adds the specified data sets.
        /// </summary>
        /// <param name="dataSets">The data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSets</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        public void Add([NotNull] IEnumerable<IDataSet<TClass, TToken>> dataSets)
        {
            if (ReferenceEquals(dataSets, null)) throw new ArgumentNullException("dataSets");

            try
            {
                foreach (var dataSet in dataSets)
                {
                    AddInternal(dataSet);
                }
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("A data set for a given class was already registered.", e);
            }
        }

        /// <summary>
        /// Adds the data set internally.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <exception cref="System.ArgumentException">Data set for the given class was already registered.</exception>
        private void AddInternal([NotNull] IDataSet<TClass, TToken> dataSet)
        {
            if (!_dataSets.TryAdd(dataSet.Class, dataSet))
            {
                throw new ArgumentException("Data set for the given class was already registered.");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<IDataSet<TClass, TToken>> GetEnumerator()
        {
            return _dataSets.Select(dataSet => dataSet.Value).GetEnumerator();
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
