using NipahTokenizer;
using NipahTokenizer.Parsing;

var tokenizer = new Tokenizer();
var options = TokenizerOptions.Default;
var tokens = new Tokens(tokenizer.Tokenize("1000.0", options));
foreach (var token in tokens)
    Console.WriteLine(token.ToString());