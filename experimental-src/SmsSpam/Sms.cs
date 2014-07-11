using System.Diagnostics;
using JetBrains.Annotations;

namespace SmsSpam
{
    /// <summary>
    /// Class Sms. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{Type} {Message}")]
    sealed class Sms
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public MessageType Type { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        [NotNull]
        public string Content { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sms"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="content">The content.</param>
        public Sms(MessageType type, [NotNull] string content)
        {
            Type = type;
            Content = content;
        }
    }
}
