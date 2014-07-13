using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using BayesianClassifier;
using JetBrains.Annotations;

namespace SmsSpam
{
    internal class Program
    {
        /// <summary>
        /// Whitespace-like characters for string splitting
        /// </summary>
        private static readonly char[] Whitespace = { ' ', '\r', '\n', '(', ')' };

        /// <summary>
        /// Punctuation characters for string cleaning
        /// </summary>
        private static readonly char[] Punctuation = { '.', ',', '!', '?', ':', ';', '&', '\'', '"', '`', '´', '-', '+' };

        /// <summary>
        /// Digit characters for string cleaning
        /// </summary>
        private static readonly char[] Digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main([NotNull] string[] args)
        {
            const string path = @".\data\SMSSpamCollection";
            var fileInfo = new FileInfo(path);
            var dataSet = new SmsDataReader(fileInfo);

            var hamClass = new EnumClass(MessageType.Ham, 0.5D);
            var hamSet = new DataSet(hamClass);

            var spamClass = new EnumClass(MessageType.Spam, 0.5D);
            var spamSet = new DataSet(spamClass);

            var trainingSet = new TrainingSet(hamSet, spamSet);

            var classifier = new NaiveClassifier(trainingSet)
                             {
                                 SmoothingAlpha = 1
                             };

            Console.WriteLine("Reading data file ...");
            var smsCollection = dataSet.ToList();
            int hamMessageCount = 0;
            int spamMessageCount = 0;

            Console.WriteLine("Training Bayes filter ...");
            foreach (var sms in dataSet)
            {
                // register sms for later testing
                smsCollection.Add(sms);

                // select correct data set
                var set = sms.Type == MessageType.Ham
                    ? hamSet
                    : spamSet;

                // count training data
                if (sms.Type == MessageType.Ham) ++hamMessageCount;
                else ++spamMessageCount;

                // get the tokens and add them to the set
                var tokens = GetTokensFromSms(sms);
                set.AddToken(tokens);
            }

            Console.WriteLine("Reducing data sets ...");
            const int minimumCount = 10;
            hamSet.PurgeWhere(tc => tc.Count <= minimumCount);
            spamSet.PurgeWhere(tc => tc.Count <= minimumCount);

            // Console.WriteLine("Adjust class base probabilities ...");
            // hamClass.Probability = (double)hamMessageCount / (hamMessageCount + spamMessageCount);
            // spamClass.Probability = (double)spamMessageCount / (hamMessageCount + spamMessageCount);

            Console.WriteLine("Testing Bayes filter ...");
            foreach (var sms in smsCollection)
            {
                var tokens = GetTokensFromSms(sms).Distinct().ToList();
                var classes = classifier.CalculateProbabilities(tokens)
                    .OrderByDescending(c => c.Probability)
                    .ToList();
                var bestClass = classes.First();

                var realType = sms.Type;
                var estimatedType = ((EnumClass)bestClass.Class).Type;
                var probability = bestClass.Probability;
                var result = realType == estimatedType ? "GOOD" : "BAD";
                Console.WriteLine("{3,-4}: SMS of type [{0,-4}] - estimated [{1,-4}] with {2:P}", realType, estimatedType, probability, result);
                Debug.Assert(realType == estimatedType, "realType == estimatedType");
            }

            if (!Debugger.IsAttached) return;
            Console.WriteLine("Press key to exit.");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Gets the tokens from the SMS.
        /// </summary>
        /// <param name="sms">The SMS.</param>
        /// <returns>IEnumerable&lt;IToken&gt;.</returns>
        [NotNull]
        private static IEnumerable<IToken> GetTokensFromSms([NotNull] Sms sms)
        {
            var words = GetWords(sms);
            var tokens = words.Where(CanKeepWord).Select(CreateToken);
            return tokens;
        }

        /// <summary>
        /// Gets the words.
        /// </summary>
        /// <param name="sms">The SMS.</param>
        [NotNull]
        private static IEnumerable<string> GetWords([NotNull] Sms sms)
        {
            var cleanedContent = sms.Content
                .Select(FilterCharacter)
                .Glue();

            return cleanedContent.Split(Whitespace, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Filters the character.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>IEnumerable&lt;System.Char&gt;.</returns>
        private static char FilterCharacter(char c)
        {
            const char space = ' ';
            if (Punctuation.Contains(c)) return space;
            if (Digits.Contains(c)) return space;
            return c;
        }

        /// <summary>
        /// Determines if the word should be discarded
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns><c>true</c> if the word should be discarded, <c>false</c> otherwise.</returns>
        private static bool ShouldDiscardWord([NotNull] string word)
        {
            return word.Length <= 1;
        }

        /// <summary>
        /// Determines whether this word can be kept, i.e. should not be discarded.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns><c>true</c> if this word can be kept; otherwise, <c>false</c>.</returns>
        private static bool CanKeepWord([NotNull] string word)
        {
            return !ShouldDiscardWord(word);
        }

        /// <summary>
        /// Creates a token from a given word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>IToken.</returns>
        [NotNull]
        private static IToken CreateToken([NotNull] string word)
        {
            return new StringToken(word.ToLowerInvariant());
        }
    }
}
