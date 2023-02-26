# Naive Bayes Classification

A naive Bayes classifier with Laplace smoothing for arbitrary features written in C#.

## Directory structure

* `src` contains the library's source code
* `test` contains the unit tests
* `samples` contains the test application

## SmsSpam example

The SmsSpam demo application uses the [SMS Spam Collection v.1](http://www.dt.fee.unicamp.br/~tiago/smsspamcollection/) in order to train the classifier with Spam and Ham SMS text data and then match individual SMS against that. Cleaned, but otherwise raw words are used (i.e. stripped from whitespace, numbers, punctuation, ...) as tokens. 

## License

This code is, if not otherwise stated, available under MIT License. See `LICENSE` file for more information.
