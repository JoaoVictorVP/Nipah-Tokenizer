using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace NipahTokenizer
{
	public struct Token : IEquatable<Token>
	{
		public static bool LineBreakCountAsEOF { get; set; } = true;

		public readonly string Text { get; init; }
		public readonly TokenType Type { get; init; }
		public readonly int Position { get; init; }
		public readonly int Line { get; init; }
		public readonly DynValue Value { get; init; }

		public Token Or(Token? other)
		{
			return other ?? this;
		}

		#region REFERENCE
		public static bool operator ==(Token left, Token right) => left.Equals(right);
		public static bool operator !=(Token left, Token right) => !(left == right);
		#endregion

		public bool IsValue => Tokenizer.IsValue(Type);
		public bool IsOperator => Tokenizer.IsOperator(Type);
		public bool IsMathOperator => Tokenizer.IsMathOperator(Type);
		public bool IsComparer => Tokenizer.IsComparisson(Type);
		public bool IsConditional => Tokenizer.IsConditional(Type) || Text.ToLower() == "xor";
		public bool IsId => Type == TokenType.ID;

        public Token Modify<T>(string text, TokenType type, T value)
            => this with { Text = text, Type = type, Value = DynValue.From(value) };

		public Token WithValue<T>(T value)
			=> this with { Value = DynValue.From(value) };

        public void Error()
		{
			throw new CompileError($"Can't compile token [{Text}], at", this);
		}
		public void SError(string source)
		{
			throw new CompileError($"Can't compile token [{Text}], at", this, source);
		}
		public CompileError ISError(string source)
		{
			return new CompileError($"Can't compile token [{Text}], at", this, source);
		}
		public CompileError IError()
		{
			return new CompileError($"Can't compile token [{Text}], at", this);
		}
		public void Error(string message)
		{
			throw new CompileError(message, this);
		}
		public void Error(string message, string? source)
		{
			throw new CompileError(message, this, source ?? "<n/a>");
		}
		public CompileError IError(string message)
		{
			return new CompileError(message, this);
		}
		public CompileError IError(string message, string source)
		{
			return new CompileError(message, this, source);
		}

		public static implicit operator string(Token token) => token.Text;
		public static implicit operator TokenType(Token token) => token.Type;

		public static Token Build(SplitItem item)
		{
			string text = item.text;
			object? value = null;
			var type = TokenType.None;
			//text = text.ToLower();
			switch (text)
			{
				case "@": type = TokenType.Email; break;

				case "//": type = TokenType.Comment; break;
				case "/*": type = TokenType.CommentBegin; break;
				case "*/": type = TokenType.CommentEnd; break;

				case "(": type = TokenType.OpenParenthesis; break;
				case ")": type = TokenType.CloseParenthesis; break;
				case "[": type = TokenType.OpenSquares; break;
				case "]": type = TokenType.CloseSquares; break;
				case "{": type = TokenType.OpenBrackets; break;
				case "}": type = TokenType.CloseBrackets; break;
				case "true":
					type = TokenType.TrueLiteral;
					value = true;
					break;
				case "false":
					type = TokenType.FalseLiteral;
					value = false;
					break;
				case "null": type = TokenType.NullLiteral; break;
				case "+": type = TokenType.Plus; break;
				case "-": type = TokenType.Minus; break;
				case "/": type = TokenType.Divide; break;
				case "*": type = TokenType.Multiply; break;
				case "=": type = TokenType.Bind; break;
				case "==": type = TokenType.Equal; break;
				case "!=": type = TokenType.Different; break;
				case ">": type = TokenType.Larger; break;
				case "<": type = TokenType.Lower; break;
				case ">=": type = TokenType.LargerOrEqual; break;
				case "<=": type = TokenType.LowerOrEqual; break;
				case "\n": type = TokenType.EOF; break;
				case ";": type = TokenType.EOF; break;
				case ",": type = TokenType.Comma; break;
				case ":": type = TokenType.Descript; break;
				case "$": type = TokenType.Rich; break;
				case "&": type = TokenType.Reference; break;

				case "->": type = TokenType.Access; break;

				case "=>": type = TokenType.To; break;

				case "AND": case "&&": type = TokenType.And; break;
				case "OR": case "||": type = TokenType.Or; break;

				case ".": type = TokenType.Dot; break;

				case "#": type = TokenType.Hashtag; break;

				case "!": type = TokenType.Exclamation; break;
			}
			if (type == TokenType.None)
			{
				if (text[0] == '"' && text[^1] == '"')
				{
					type = TokenType.StringLiteral;
					value = text.Remove(0, 1).Remove(text.Length - 2, 1);
				}
				else
				{
					if (tryID(text))
					{
						type = TokenType.ID;
						value = text;
						goto end;
					}
					int integer;
					if (int.TryParse(text, out integer))
					{
						type = TokenType.IntegerLiteral;
						value = integer;
						goto end;
					}
					float single;
					if (text[text.Length - 1] == 'f')
					{
						if (float.TryParse(text.Replace('.', ',').Replace("f", ""), out single))
						{
							type = TokenType.FloatLiteral;
							value = single;
							goto end;
						}
					}
					if (float.TryParse(text.Replace('.', ','), out single))
					{
						type = TokenType.FloatLiteral;
						value = single;
						goto end;
					}
					char c;
					if (char.TryParse(text, out c))
					{
						type = TokenType.CharLiteral;
						value = c;
					}
				}
			}
		end:
			return new Token(text, type, item.position, item.line, value);
		}
		public static bool IsValidIdentifier(string text) => tryID(text);
		static bool tryID(string text)
		{
			if (text is "" or null)
				return false;
			if (char.IsDigit(text[0]))
				return false;
			if (text[0] == '.')
				return false;
			foreach (var c in text)
			{
				if (char.IsDigit(c) || char.IsLetter(c) || c is '_' or '.')
					continue;
				return false;
			}
			return true;
		}

		public Token(string text, TokenType type, int position, int line, object? value = null)
		{
			Text = text;
			Type = type;
			Position = position;
			Line = line;
			Value = DynValue.From(value);
		}
		public override string ToString()
		{
			if (Value.Type is not DynType.Null)
				return $"Token: {Text} : {Type} = {Value} [line: {Line}]";
			return $"Token: {Text} : {Type} [line: {Line}]";
		}

		public override bool Equals(object? obj)
		{
			return obj is Token token && Equals(token);
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode() + (Type.GetHashCode() + Position.GetHashCode() + Line.GetHashCode() + Value.GetHashCode());
		}

		public bool Equals(Token other)
		{
			return Text == other.Text && Type == other.Type &&
				Line == other.Line && Position == other.Position;
		}
	}
}

namespace NipahTokenizer
{
	public static class TokenHelper
	{
		public static bool Assert(this Token token, TokenType type, string error = "AUTO",
								 string? source = null)
		{
			bool result = token.Type == type;
			if (error == "AUTO")
				error = $"Expecting '{type.GetString()}', found '{token.Text ?? "NULL"}' at";
			if (!result && error != null)
				token.Error(error, source);
			return result;
		}
		public static bool Assert(this Token token, string text, string error = "AUTO",
								  string? source = null)
		{
			bool result = token.Text == text;
			if (error == "AUTO")
				error = $"Expecting '[{text}]', found '[{token.Text ?? "NULL"}]' at";
			if (!result && error != null)
				token.Error(error, source);
			return result;
		}
		public static bool AssertValue(this Token token, string? error = "AUTO", string? source = null)
		{
			bool result = token.IsValue;
			if (error == "AUTO")
				error = $"Token '[{token.Text}]' is not value, at";
			if (!result && error is not null)
				token.Error(error, source);
			return result;
		}
		public static TokenType Type(this Token token) => token.Type;
		public static object Value(this Token token) => token.Value;
		public static T? Value<T>(this Token token) => token.Value.TrySolve<T>().Solve();

		public static bool IsValue(this Token token) => token.IsValue;
		public static bool IsOperator(this Token token) => token.IsOperator;
		public static bool IsComparer(this Token token) => token.IsComparer;

		public static string GetString(this TokenType type)
		{
			switch (type)
			{
				case TokenType.Access:
					return "->";
				case TokenType.To:
					return "=>";
				case TokenType.And:
					return "&&";
				case TokenType.Bind:
					return "=";
				case TokenType.CloseBrackets:
					return "}";
				case TokenType.CloseParenthesis:
					return ")";
				case TokenType.CloseSquares:
					return "]";
				case TokenType.Comma:
					return ",";
				case TokenType.Comment:
					return "//";
				case TokenType.CommentBegin:
					return "/*";
				case TokenType.CommentEnd:
					return "*/";
				case TokenType.Descript:
					return ":";
				case TokenType.Different:
					return "!=";
				case TokenType.Divide:
					return "/";
				case TokenType.Dot:
					return ".";
				case TokenType.Email:
					return "@";
				case TokenType.EOF:
					return ";";
				case TokenType.Equal:
					return "==";
				case TokenType.Exclamation:
					return "!";
				case TokenType.FalseLiteral:
					return "false";
				case TokenType.Hashtag:
					return "#";
				case TokenType.Larger:
					return ">";
				case TokenType.LargerOrEqual:
					return ">=";
				case TokenType.Lower:
					return "<";
				case TokenType.LowerOrEqual:
					return "<=";
				case TokenType.Minus:
					return "-";
				case TokenType.Multiply:
					return "*";
				case TokenType.NullLiteral:
					return "null";
				case TokenType.OpenBrackets:
					return "{";
				case TokenType.OpenParenthesis:
					return "(";
				case TokenType.OpenSquares:
					return "[";
				case TokenType.Or:
					return "||";
				case TokenType.Plus:
					return "+";
				case TokenType.Reference:
					return "&";
				case TokenType.Rich:
					return "$";
				case TokenType.TrueLiteral:
					return "true";
			}
			return type.ToString();
		}

		public static Token? TryPeek(this Queue<Token> tokens)
		{
			if (tokens.Count > 0)
				return tokens.Peek();
			return null;
		}
		public static Token? TryDequeue(this Queue<Token> tokens)
		{
			if (tokens.Count > 0)
				return tokens.Dequeue();
			return null;
		}
		public static Queue<Token> Add(this Queue<Token> tokens, Token token)
		{
			List<Token> toks = Lists<Token>.Get(tokens.Count + 1);
			toks.AddRange(tokens);
			toks.Add(token);
			var result = new Queue<Token>(toks);
			toks.Return();
			return result;
		}
		public static Queue<Token> Insert(this Queue<Token> tokens, int index, Token token)
		{
			var toks = Lists<Token>.Get(tokens.Count + 1);
			toks.AddRange(tokens);
			toks.Insert(index, token);
			var q = new Queue<Token>(toks);
			toks.Return();
			return q;
		}
		public static Queue<Token> Remove(this Queue<Token> tokens, Token token)
		{
			var toks = Lists<Token>.Get(tokens.Count);
			toks.AddRange(tokens);
			toks.Remove(token);
			var q = new Queue<Token>(toks);
			toks.Return();
			return q;
		}
		public static Queue<Token> RemoveAt(this Queue<Token> tokens, int index)
		{
			var toks = Lists<Token>.Get(tokens.Count);
			toks.AddRange(tokens);
			toks.RemoveAt(index);
			var q = new Queue<Token>(toks);
			toks.Return();
			return q;
		}
	}
}

namespace NipahTokenizer
{
	public class RuntimeError : Exception
	{
		public RuntimeError(string message) : base(message)
		{
		}
	}
}

namespace NipahTokenizer
{
	public class CompileError : Exception
	{
		public override string Message
		{
			get
			{
				if (token != null && source == null)
					return base.Message + $" (Line {token?.Line}, Position {token?.Position})";
				else if (token == null && source != null)
					return base.Message + $" (File {source})";
				else if (token != null && source != null)
					return base.Message + $" (Line {token?.Line}, Position {token?.Position}," +
							   $" File {source})";
				else
					return base.Message;
			}
		}
		public Token? Token => token;
		public string? FileSource => source;
		readonly Token? token;
		readonly string? source;
		public CompileError(string message) : base(message)
		{
		}
		public CompileError(string message, Token? token, string? source = null) : base(message)
		{
			this.token = token;
			this.source = source;
		}
	}
}
