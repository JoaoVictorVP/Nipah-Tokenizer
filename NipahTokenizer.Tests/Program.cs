using NipahTokenizer;
using NipahTokenizer.Parsing;

var tokenizer = new Tokenizer();
var options = TokenizerOptions.Default;
var tokens = new Tokens(tokenizer.Tokenize("So -3.13 is PI? lmao == true? 'haha' ", options));
foreach (var token in tokens)
    Console.WriteLine(token.ToString());