# bayes-classification #

A naive Bayes classifier with Laplace smoothing for arbitrary features written in C#.

## Directory structure ##

* `src` contains the library's source code
* `test-src` contains the unit tests
* `experimenta-src` contains test applications

## SmsSpam example ##

The SmsSpam demo application uses the [SMS Spam Collection v.1](http://www.dt.fee.unicamp.br/~tiago/smsspamcollection/) in order to train the classifier with Spam and Ham SMS text data and then match individual SMS against that. Cleaned, but otherwise raw words are used (i.e. stripped from whitespace, numbers, punctuation, ...) as tokens. 

## Framework ##

Requires .NET Framework 4.0.3 (http://blogs.msdn.com/b/dotnet/p/dotnet_sdks.aspx) or equivalent.

## License ##

This code is, if not otherwise stated, available under MIT License. See `LICENSE` file for more information.