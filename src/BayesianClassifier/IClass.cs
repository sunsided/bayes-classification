using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Interface IClass
    /// </summary>
    public interface IClass
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [NotNull]
        string Name { get; }
    }
}
