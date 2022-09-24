using NipahTokenizer;

var tokenizer = new Tokenizer();
var options = TokenizerOptions.Default;
var tokens = new Tokens(tokenizer.Tokenize("Is 3 * 3 = 9? Sure!", options));
foreach (var token in tokens)
    Console.WriteLine(token.ToString());