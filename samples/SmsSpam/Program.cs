using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BayesianClassifier;

namespace SmsSpam;

internal static class Program
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
    public static void Main(string[] args)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SmsSpam.data.SMSSpamCollection");
        if (stream is null) throw new UnreachableException("The SMS Spam Collection data is expected to be an embedded resource");

        var dataSet = new SmsDataReader(stream);

        var hamClass = new EnumClass(MessageType.Ham, 0.5D);
        var hamSet = new DataSet(hamClass);

        var spamClass = new EnumClass(MessageType.Spam, 0.5D);
        var spamSet = new DataSet(spamClass);

        var trainingSet = new TrainingSet(hamSet, spamSet);

        var classifier = new NaiveClassifier(trainingSet)
        {
            SmoothingAlpha = 0.00001
        };

        Console.WriteLine("Reading data file ...");
        var smsCollection = dataSet.Distinct().ToList();
        var hamMessageCount = 0;
        var spamMessageCount = 0;

        Console.WriteLine("Training Bayes filter ...");
        foreach (var sms in smsCollection)
        {
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

        const int count = 20;
        Console.WriteLine("Determining {0} highest frequency tokens ...", count);
        var highestFrequencies = hamSet.Concat(spamSet).OrderByDescending(static e => e.Count).Take(count);
        var stopWords = new Collection<IToken>();
        foreach (var token in highestFrequencies)
        {
            Console.WriteLine("- {0} ({1})", token.Token, token.Count);

            // register as stop-words and remove from dataset
            var stopWord = token.Token;
            stopWords.Add(stopWord);
            spamSet.PurgeToken(stopWord);
            hamSet.PurgeToken(stopWord);
        }

#if false
            Console.WriteLine("Reducing data sets ...");
            const int minimumCount = 10;
            hamSet.PurgeWhere(tc => tc.Count <= minimumCount);
            spamSet.PurgeWhere(tc => tc.Count <= minimumCount);
#endif

#if true
        Console.WriteLine("Adjust class base probabilities ...");
        hamClass.Probability = (double)hamMessageCount / (hamMessageCount + spamMessageCount);
        spamClass.Probability = (double)spamMessageCount / (hamMessageCount + spamMessageCount);
        Console.WriteLine("- ham base probability:  {0:P}", hamClass.Probability);
        Console.WriteLine("- spam base probability: {0:P}", spamClass.Probability);
#endif

        Console.WriteLine("Testing Bayes filter ...");
        var correctPredictions = 0;
        var wrongPredictions = 0;
        var mispredictions = new Collection<KeyValuePair<Sms, double>>();
        foreach (var sms in smsCollection)
        {
            var tokens = GetTokensFromSms(sms).Where(token => !stopWords.Contains(token)).ToList();
            if (!tokens.Any()) continue;

            var classes = classifier.CalculateProbabilities(tokens)
                .OrderByDescending(static c => c.Probability)
                .ToList();
            var bestClass = classes.First();

            var realType = sms.Type;
            var estimatedType = ((EnumClass)bestClass.Class).Type;
            var probability = bestClass.Probability;

            // assume correct match and select output
            var result = "GOOD";
            ++correctPredictions;

            // correct for mismatch
            if (realType != estimatedType)
            {
                --correctPredictions;
                ++wrongPredictions;
                mispredictions.Add(new KeyValuePair<Sms, double>(sms, probability));
                result = "BAD";
            }

            Console.WriteLine("{3,-4}: SMS of type [{0,-4}] - estimated [{1,-4}] with {2,8:P}", realType, estimatedType, probability, result);
            // Debug.Assert(realType == estimatedType, "realType == estimatedType");
        }

        Console.WriteLine();
        Console.WriteLine("Ham messages:  {0}", hamMessageCount);
        Console.WriteLine("Spam messages: {0}", spamMessageCount);

        Console.WriteLine();
        var totalPredictions = correctPredictions + wrongPredictions;
        Console.WriteLine("Correct predictions: {0}/{1} ({2:P})", correctPredictions, totalPredictions,
            (double) correctPredictions/totalPredictions);
        Console.WriteLine("Mispredictions:      {0}/{1} ({2:P})", wrongPredictions, totalPredictions,
            (double)wrongPredictions / totalPredictions);

        Console.WriteLine();
        Console.WriteLine("False negatives:");
        foreach (var misprediction in mispredictions.Where(static sms => sms.Key.Type == MessageType.Ham))
        {
            Console.WriteLine("- [{0,-4} - {2,8:P} SPAM] {1}", misprediction.Key.Type, misprediction.Key.Content, misprediction.Value);
        }

        Console.WriteLine();
        Console.WriteLine("False positives:");
        foreach (var misprediction in mispredictions.Where(static sms => sms.Key.Type == MessageType.Spam))
        {
            Console.WriteLine("- [{0,-4} - {2,8:P} HAM ] {1}", misprediction.Key.Type, misprediction.Key.Content, misprediction.Value);
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
    private static IEnumerable<IToken> GetTokensFromSms(Sms sms)
    {
        var words = GetWords(sms);
        var tokens = words.Where(CanKeepWord).Select(CreateToken);
        return tokens;
    }

    /// <summary>
    /// Gets the words.
    /// </summary>
    /// <param name="sms">The SMS.</param>
    private static IEnumerable<string> GetWords(Sms sms)
    {
        var cleanedContent = sms.Content
            .ToLowerInvariant()
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
    private static bool ShouldDiscardWord(string word) => false; // word.Length <= 1;

    /// <summary>
    /// Determines whether this word can be kept, i.e. should not be discarded.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns><c>true</c> if this word can be kept; otherwise, <c>false</c>.</returns>
    private static bool CanKeepWord(string word) => !ShouldDiscardWord(word);

    /// <summary>
    /// Creates a token from a given word.
    /// </summary>
    /// <param name="word">The word.</param>
    /// <returns>IToken.</returns>
    private static IToken CreateToken(string word) => new StringToken(word.ToLowerInvariant());
}
