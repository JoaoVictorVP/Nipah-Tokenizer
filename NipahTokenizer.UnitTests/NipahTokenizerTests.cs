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
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "Hello", ",", "World", "!" });
    }

    [Fact]
    public void TokenizeWithScopes()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(" ", IncludeMode.None), new("\\,"), new("\\!") },
            new Scope[] { new('"', '"'), new('\'', '\'') },
            Array.Empty<EndOfLine>(),
            Array.Empty<SplitAggregator>());
        var tokenizer = new Tokenizer();
        string sample = @"This is 'a good' and highly tokenizable ""text with scopes""!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "This", "is", "'a good'", "and", "highly", "tokenizable", @"""text with scopes""", "!" });
    }
}
