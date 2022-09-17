using FluentAssertions;
using System;
using Xunit;

namespace NipahTokenizer.UnitTests;

public class NipahTokenizerTests
{
    [Fact]
    public void TokenizeSimpleEntry()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(" ", IncludeMode.None), new("\\,"), new("\\!") }, 
            Array.Empty<Scope>(), 
            Array.Empty<EndOfLine>(), 
            Array.Empty<SplitAggregator>());
        var tokenizer = new Tokenizer();
        string sample = "Hello, World!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.text).Should().BeEquivalentTo(new[] { "Hello", ",", "World", "!" });
    }
}
