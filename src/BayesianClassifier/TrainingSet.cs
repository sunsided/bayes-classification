using System;
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
    public sealed class TrainingSet : ITrainingSet
    {
        /// <summary>
        /// Gets or sets the occurrence threshold.
        /// <para>
        /// Any token occurrence lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        private int _occurrenceThreshold;

        /// <summary>
        /// Gets or sets the percentage threshold.
        /// <para>
        /// Any per-class token percentage lower than the threshold will be assumed to be zero,
        /// resulting in the assumption that the given token was not seen during training.
        /// </para>
        /// </summary>
        private double _percentageThreshold;

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
            get { return _occurrenceThreshold; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than or equal to zero.");
                _occurrenceThreshold = value;
            }
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
            get { return _percentageThreshold; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Value must be greater than or equal to zero.");
                if (Double.IsInfinity(value) || Double.IsNaN(value)) throw new NotFiniteNumberException("Value must be finite", value);
                _percentageThreshold = value;
            }
        }

        /// <summary>
        /// The data sets
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<IClass, IDataSet> _dataSets = new ConcurrentDictionary<IClass, IDataSet>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingSet"/> class.
        /// </summary>
        public TrainingSet()
        {           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingSet"/> class.
        /// </summary>
        /// <param name="dataSets">The data sets.</param>
        public TrainingSet([NotNull] IEnumerable<IDataSet> dataSets)
        {
            Add(dataSets);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingSet"/> class.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="additionalDataSets">The additional data sets.</param>
        public TrainingSet([NotNull] IDataSet dataSet, [NotNull] params IDataSet[] additionalDataSets)
        {
            Add(dataSet, additionalDataSets);
        }

        /// <summary>
        /// Creates and registers a data set for the given class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        [NotNull]
        public IDataSet CreateDataSet([NotNull] IClass @class)
        {
            var dataSet = new DataSet(@class, this);
            Add(dataSet);
            return dataSet;
        }

        /// <summary>
        /// Gets the size of the vocabulary, i.e. the number of all tokens.
        /// </summary>
        /// <value>The size of the vocabulary.</value>
        public long VocabularySize
        {
            get { return _dataSets.Select(pair => pair.Value).Sum(set => set.TokenCount); }
        }

        /// <summary>
        /// Gets the <see cref="IDataSet"/> with the specified class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet&lt;IClass, IToken&gt;.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        public IDataSet this[IClass @class]
        {
            get { return GetSetForClass(@class); }
        }

        /// <summary>
        /// Clears all data set's tokens.
        /// </summary>
        public void ClearTokens()
        {
            foreach (var dataSet in _dataSets)
            {
                dataSet.Value.Clear();
            }
        }

        /// <summary>
        /// Gets the set for class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        public IDataSet GetSetForClass(IClass @class)
        {
            IDataSet set;
            if (_dataSets.TryGetValue(@class, out set)) return set;
            throw new ArgumentException("No data set was registered for the given class", "class");
        }

        /// <summary>
        /// Adds the specified data set.
        /// </summary>
        /// <param name="dataSet">The data set.</param>
        /// <param name="additionalDataSets">The additional data sets.</param>
        /// <exception cref="System.ArgumentNullException">dataSet</exception>
        /// <exception cref="System.ArgumentException">A data set for a given class was already registered.</exception>
        public void Add([NotNull] IDataSet dataSet, [NotNull] params IDataSet[] additionalDataSets)
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
        public void Add([NotNull] IEnumerable<IDataSet> dataSets)
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
        private void AddInternal([NotNull] IDataSet dataSet)
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
        public IEnumerator<IDataSet> GetEnumerator()
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

        /// <summary>
        /// Updates the class probabilities.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="factor">An optional factor to the probabilities.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Value must be greater than or equal to zero.</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        /// <exception cref="System.ArgumentException">An unknown mode was given.</exception>
        public void SetClassProbabilities(ClassProbabilities mode, double factor = 1.0D)
        {
            if (factor < 0) throw new ArgumentOutOfRangeException("factor", factor, "Value must be greater than or equal to zero.");
            if (Double.IsNaN(factor) || Double.IsInfinity(factor)) throw new NotFiniteNumberException("The value must be a finite number.", factor);

            var totalSize = this.Select(set => set.TokenCount).Sum();

            if (mode == ClassProbabilities.Automatic)
            {
                foreach (var set in this)
                {
                    set.Class.Probability = (double) set.TokenCount/totalSize * factor;
                }
            }
            else if (mode == ClassProbabilities.EqualDistributed)
            {
                var probability = (1.0 / _dataSets.Count) * factor;
                foreach (var set in this)
                {
                    set.Class.Probability = probability;
                }
            }
            else
            {
                throw new ArgumentException("Unknown mode " + mode + ".", "mode");
            }
        }
    }
}
