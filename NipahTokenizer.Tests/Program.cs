using NipahTokenizer;
using System.Diagnostics;

var tokenizer = new Tokenizer();
var options = new TokenizerOptions(
        TokenizerOptions.DefaultSeparators,
        Array.Empty<Scope>(),
        Array.Empty<EndOfLine>(),
        Array.Empty<SplitAggregator>(),
        Parallel: true);
string text = File.ReadAllText(Console.ReadLine());
//string text = "Hello, World!!!";
List<Token> tokens;
tokens = tokenizer.Tokenize(text, options);
var sw = new Stopwatch();
sw.Start();
for(int i = 0; i < 100; i++)
{
    tokens = tokenizer.Tokenize(text, options);
}
sw.Stop();

Console.WriteLine($"Time to tokenize {tokens.Count} tokens: " + sw.Elapsed);

//foreach (var token in tokens)
//    Console.WriteLine(token.ToString());