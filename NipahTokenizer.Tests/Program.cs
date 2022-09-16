using NipahTokenizer;
using NipahTokenizer.Parsing;

var tokenizer = new Tokenizer();
var tokens = new Tokens(tokenizer.Tokenize("Lol, you're right!"));
foreach (var token in tokens)
    Console.WriteLine(token.ToString());