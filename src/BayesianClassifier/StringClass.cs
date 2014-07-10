using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace BayesianClassifier
{
    /// <summary>
    /// Class StringClass. This class cannot be inherited.
    /// </summary>
    public sealed class StringClass : IClass
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringClass" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public StringClass([NotNull] string name)
        {
            if (ReferenceEquals(name, null)) throw new ArgumentNullException("name");
            Name = name;
        }
    }
}
