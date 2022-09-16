using NipahTokenizer;
using NipahTokenizer.Parsing;

var tokenizer = new Tokenizer();
var options = TokenizerOptions.Default;
var tokens = new Tokens(tokenizer.Tokenize("Lol, you're right!", options));
foreach (var token in tokens)
    Console.WriteLine(token.ToString());