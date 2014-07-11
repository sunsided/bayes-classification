using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace SmsSpam
{
    sealed class SmsDataReader : IEnumerable<Sms>
    {
        /// <summary>
        /// The data file
        /// </summary>
        [NotNull]
        private readonly FileInfo _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsDataReader"/> class.
        /// </summary>
        /// <param name="filePath">The data file.</param>
        public SmsDataReader([NotNull] FileInfo filePath)
        {
            _filePath = filePath;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<Sms> GetEnumerator()
        {
            var filePath = _filePath;
            using (var fs = filePath.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var bs = new BufferedStream(fs))
            using (var reader = new StreamReader(bs, Encoding.UTF8))
            {
                string line;
                while (!String.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    var parts = line.Split('\t');
                    Debug.Assert(parts.Length == 2,
                        "Expected class separated from message by a single tab, but found multiple tabs in message");

                    var typeCode = parts[0].ToLowerInvariant();
                    var message = parts[1];

                    // select type
                    MessageType type;
                    switch (typeCode)
                    {
                        case "spam":
                            type = MessageType.Spam;
                            break;
                        case "ham":
                            type = MessageType.Ham;
                            break;
                        default:
                            throw new Exception("Expected message type to be either \"spam\" or \"ham\", but found \"" +
                                                typeCode + "\"");
                    }

                    yield return new Sms(type, message);
                }
            }
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
