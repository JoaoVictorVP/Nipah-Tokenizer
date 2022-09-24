Nipah Tokenizer

[![NuGet version (NipahTokenizer)](https://img.shields.io/nuget/v/NipahTokenizer.svg?style=flat-square)](https://www.nuget.org/packages/NipahTokenizer)

# Installing
```dotnet add package NipahTokenizer```

# Using
First setup the tokenizer options as needed, you can use the default as follows:
```csharp
var tokenizerOptions = TokenizerOptions.Default;
```
Then just create a new instance of tokenizer and call "Tokenize" passing tokenizer options as arg:
```csharp
var tokenizer = new Tokenizer();
var tokens = tokenizer.Tokenize("any-text-here", tokenizerOptions);
```
You can now just iterate over tokens as needed.
