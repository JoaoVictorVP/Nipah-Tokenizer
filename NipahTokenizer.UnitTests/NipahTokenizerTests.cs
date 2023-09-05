using FluentAssertions;
using System;
using Xunit;

namespace NipahTokenizer.UnitTests;

public class NipahTokenizerTests
{
    [Fact]
    public void TokenizeWithAggregators()
    {
        // Arrange
        var options = new TokenizerOptions(
            TokenizerOptions.DefaultSeparators,
            TokenizerOptions.DefaultScopes,
            TokenizerOptions.DefaultEndOfLines,
            new[]
            {
                new SplitAggregator("?", ":")
            }
        );
        var tokenizer = new Tokenizer();
        var text = "[\"fish\"]?:lol";

        // Act
        var tokens = tokenizer.Tokenize(text, options);

        // Assert
        tokens.Should().HaveCount(5);
        (tokens switch
        {
            [{ Type: TokenType.OpenSquares }, { Type: TokenType.StringLiteral }, { Type: TokenType.CloseSquares },
            { Text: "?:" }, { Text: "lol" }] => true,
            _ => false
        }).Should().BeTrue();
    }

    [Fact]
    public void TokenizeSimpleEntry()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(' ', IncludeMode.None), new(','), new('!') }, 
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
            new Separator[] { new(' ', IncludeMode.None), new(','), new('!') },
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

    [Fact]
    public void TokenizeWithEndOfLine()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(' ', IncludeMode.None), new(','), new('!'), new('\n') },
            Array.Empty<Scope>(),
            new EndOfLine[] { new('\n') },
            Array.Empty<SplitAggregator>());
        var tokenizer = new Tokenizer();
        string sample = @"Hello World!
Life is truly great!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "Hello", "World", "!", "\n", "Life", "is", "truly", "great", "!" });
        tokens.Should().ContainSingle(x => x.Type == TokenType.EOF);
    }

    [Fact]
    public void TokenizeWithNonClosedScope()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(' ', IncludeMode.None), new(','), new('!') },
            new Scope[] { new('"', '"'), new('\'', '\'') },
            Array.Empty<EndOfLine>(),
            Array.Empty<SplitAggregator>());
        var tokenizer = new Tokenizer();
        string sample = @"This is 'a good and highly tokenizable ""text with scopes!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "This", "is", @"'a good and highly tokenizable ""text with scopes!" });
    }

    [Fact]
    public void TokenizeWithScopesAndEscapedScopeCharacters()
    {
        // Arrange
        var options = new TokenizerOptions(
            new Separator[] { new(' ', IncludeMode.None), new(','), new('!') },
            new Scope[] { new('"', '"'), new('\'', '\'') },
            Array.Empty<EndOfLine>(),
            Array.Empty<SplitAggregator>());
        var tokenizer = new Tokenizer();
        string sample = @"This is 'a good' and highly tokenizable ""text with \""smart\"" scopes""!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "This", "is", "'a good'", "and", "highly", "tokenizable", @"""text with ""smart"" scopes""", "!" });
    }

    [Fact]
    public void TokenizeWithCustomDefaultOptions_NoNegativeNumbers()
    {
        // Arrange
        var options = TokenizerOptions.BuildDefault(DefaultTokenizerOptions.Defaults | DefaultTokenizerOptions.IgnoreNegativeNumberAggregators);
        var tokenizer = new Tokenizer();
        string sample = @"-1";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "-", "1" });
    }

    [Fact]
    public void TokenizeWithCustomDefaultOptions_NoFloatingNumbers()
    {
        // Arrange
        var options = TokenizerOptions.BuildDefault(DefaultTokenizerOptions.Defaults | DefaultTokenizerOptions.IgnoreFloatingPointNumberAggregators);
        var tokenizer = new Tokenizer();
        string sample = @"1.0";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "1", ".", "0" });
    }

    [Fact]
    public void TokenizeWithCustomDefaultOptions_SpacesAreSeparatedTokens()
    {
        // Arrange
        var options = TokenizerOptions.BuildDefault(DefaultTokenizerOptions.Defaults | DefaultTokenizerOptions.SpacesDoSeparate);
        var tokenizer = new Tokenizer();
        string sample = @"Hello World!";

        // Act
        var tokens = tokenizer.Tokenize(sample, options);

        // Assert
        tokens.Select(x => x.Text).Should().BeEquivalentTo(new[] { "Hello", " ", "World", "!" });
    }
}
