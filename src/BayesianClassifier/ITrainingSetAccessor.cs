using System.Collections.Generic;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface ITrainingSetAccerssor
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TToken">The type of the token.</typeparam>
    public interface ITrainingSetAccessor<TClass, TToken> : IEnumerable<IDataSet<TClass, TToken>> 
        where TClass : IClass 
        where TToken : IToken
    {
        /// <summary>
        /// Gets the <see cref="IDataSet{TClass, TToken}"/> with the specified class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>IDataSet&lt;TClass, TToken&gt;.</returns>
        /// <exception cref="System.ArgumentException">No data set was registered for the given class;class</exception>
        IDataSet<TClass, TToken> this[TClass @class] { get; }
    }
}