using System;
using System.Collections.Generic;
using S = NipahTokenizer.Separator;

namespace NipahTokenizer
{
	public class Tokenizer
	{
		public static bool isValue(TokenType token)
		{
			switch (token)
			{
				case TokenType.IntegerLiteral: return true;
				case TokenType.FloatLiteral: return true;
				case TokenType.TrueLiteral: return true;
				case TokenType.FalseLiteral: return true;
				case TokenType.StringLiteral: return true;
				case TokenType.ID: return true;
			}
			return false;
		}
		public static bool isValueGross(TokenType token)
		{
			switch (token)
			{
				case TokenType.IntegerLiteral: return true;
				case TokenType.FloatLiteral: return true;
				case TokenType.TrueLiteral: return true;
				case TokenType.FalseLiteral: return true;
				case TokenType.StringLiteral: return true;
				case TokenType.NullLiteral: return true;
			}
			return false;
		}
		public static bool isComparisson(TokenType token)
		{
			switch (token)
			{
				case TokenType.Equal: return true;
				case TokenType.Different: return true;
				case TokenType.Larger: return true;
				case TokenType.Lower: return true;
				case TokenType.LargerOrEqual: return true;
				case TokenType.LowerOrEqual: return true;
			}
			return false;
		}
		public static bool isConditional(TokenType token)
		{
			switch (token)
			{
				case TokenType.And: return true;
				case TokenType.Or: return true;
			}
			return false;
		}
		public static bool isOperator(TokenType token)
		{
			switch (token)
			{
				case TokenType.Plus: return true;
				case TokenType.Minus: return true;
				case TokenType.Divide: return true;
				case TokenType.Multiply: return true;
				case TokenType.OpenParenthesis: return true;
				case TokenType.CloseParenthesis: return true;
			}
			return false;
		}
		public static bool isMathOperator(TokenType token)
		{
			switch (token)
			{
				case TokenType.Plus: return true;
				case TokenType.Minus: return true;
				case TokenType.Divide: return true;
				case TokenType.Multiply: return true;
			}
			return false;
		}
		public static List<char> begins = new List<char>()
		{
			//'"', '<', '[', '(', '{', '\''
			'"', '\'', '§'
		};
		public static List<char> ends = new List<char>()
		{
			//'"', '>', ']', ')', '}', '\''
			'"', '\'', '§'
		};
		public static List<char> special = new List<char>()
		{
			//'<','>','(',')','[',']','{','}'
		};
		public static List<bool> ables = new List<bool>()
		{
			true, true, true, true, true, true
		};
		public static List<bool> intern = new List<bool>()
		{
			true, true, true, true, true, true
		};
		public static Tokenizer single => _single;
		static Tokenizer _single = new Tokenizer();
		public event Action<List<Token>> tokensProcessor;
		public event Action<List<SplitItem>> splitProcessor;
		public event Action<Token> tokenProcessor;
		public static event SplitProcessor finalSplitProcessor;
		public List<Token> Tokenize(string entry, bool removeLineBreaks = true)
		{
			//entry = entry.Replace("\n","");
			entry = entry.Replace("\r", "");
			var tokens = new List<Token>();
			var pieces = splitString(entry, defaultSeparators);
			splitProcessor?.Invoke(pieces);
			foreach (var piece in pieces)
			{
				var token = Token.build(piece);
				tokenProcessor?.Invoke(token);
				tokens.Add(token);
			}
			tokens.ForEach(token =>
			{
				string str = token.value as string;
				if (str != null)
				{
					str = str.Replace("''", "\"");
					str = str.Replace('£', '\'');
					token.value = str;
				}
			});
			if (removeLineBreaks)
				tokens.RemoveAll(token => token.type == TokenType.LineBreak);
			tokensProcessor?.Invoke(tokens);
			return tokens;
		}
		public void GeneralizeValue(List<Token> tokens)
		{
			tokens.ForEach(token =>
			{
				if (isValue(token.type))
					token.type = TokenType.Value;
			});
		}
		public void GeneralizeValue_Gross(List<Token> tokens)
		{
			tokens.ForEach(token =>
			{
				if (isValueGross(token.type))
					token.type = TokenType.Value;
			});
		}
		static S[] defaultSeparators = {
			new S(' ', false), new S('*'), new S('/'), new S('+'), new S('-'),
			new S('('), new S(')'), new S('\n'), new S(','), new S(';'),
			new S('='), new S('{'), new S('}'),
			new S('['), new S(']'), new S(':'), new S('<'), new S('>'), new S('\t', false),
			new S('&'), new S('|'), new S('$'), new S('@'), new S('.'), new S('#'),
			new S('!')
		};

		// ¬



		public static List<SplitItem> splitString(string text, params S[] separatorsArr)
		{
			List<S> separators = new List<S>(separatorsArr);
			List<SplitItem> list = new List<SplitItem>();
			string cur = "";
			Stack<int> opens = new Stack<int>();
			int count = text.Length;
			int line = 1;
			int curEnd = 0;
			int position = -1;
			for (int i = 0; i < count; i++)
			{
				var c = text[i];
				position++;
				curEnd = position;
				if (c == '\n')
				{
					line++;
					position = 0;
					curEnd = 0;
				}
				if (special.Contains(c))
				{
					list.Add(new SplitItem(c.ToString(), position, line));
					continue;
				}
				var separator = separators.Find(s => s.separator == c);
				if (separator != null && opens.Count == 0)
				{
					if (separator.include)
						if (!separator.addSep)
							cur += c;
					list.Add(new SplitItem(cur, position, line));
					if (separator.include && separator.addSep)
						list.Add(new SplitItem(c.ToString(), position, line));
					cur = "";
					continue;
				}
				if (begins.Contains(c) && (opens.Count > 0 && begins[opens.Peek()] != c || opens.Count == 0))
				{
					opens.Push(begins.IndexOf(c));
					if (ables[opens.Peek()])
						cur += c;
					continue;
				}
				if (opens.Count == 0)
				{
					cur += c;
				}
				else
				{
					int pop = opens.Peek();
					char end = ends[pop];
					bool can = intern[pop];
					if (can && (c != end || ables[pop]))
						cur += c;
					if (c == end)
					{
						opens.Pop();
					}
				}
			}
			if (cur != "")
				list.Add(new SplitItem(cur, curEnd, line));
			list.RemoveAll((obj) => obj == "");
			applyList(list);
			return list;
		}
		public static bool AcceptSeparatedID = false;
		static void applyList(List<SplitItem> list)
		{
			Queue<SplitItem> tokens = new Queue<SplitItem>(list);
			list.Clear();
			SplitItem back = null;
			while (tokens.Count > 0)
			{
				SplitItem token = tokens.Dequeue();
				if (tokens.Count > 0)
				{
					SplitItem next = tokens.Peek();
					if (token == "=" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("==", token.position, token.line));
						continue;
					}
					if (token == "/" && next == "/")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("//", token.position, token.line));
						continue;
					}
					if (token == "/" && next == "*")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("/*", token.position, token.line));
						continue;
					}
					if (token == "*" && next == "/")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("*/", token.position, token.line));
						continue;
					}
					if (token == "!" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("!=", token.position, token.line));
						continue;
					}
					if (token == ">" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem(">=", token.position, token.line));
						continue;
					}
					if (token == "<" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("<=", token.position, token.line));
						continue;
					}
					if (token == "-" && next == ">")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("->", token.position, token.line));
						continue;
					}

					if (token == "&" && next == "&")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("&&", token.position, token.line));
						continue;
					}
					if (token == "|" && next == "|")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("||", token.position, token.line));
						continue;
					}
					if (finalSplitProcessor != null)
					{
						FinalSplit final;
						if (finalSplitProcessor(token, next, out final))
						{
							back = null;
							tokens.Dequeue();
							list.Add(new SplitItem(final.result, token.position, token.line));
						}
					}
					/* Optional Negative Number Parser */
					if (token == "-")
					{
						long num;
						if (long.TryParse(next, out num))
						{
							back = null;
							tokens.Dequeue();
							list.Add(token + next);
							continue;
						}
					}
					if (token == "=" && next == ">")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("=>", token.position, token.line));
						continue;
					}
					/* Optional Dotted Number Parser */
					if (token == ".")
					{
						long num;
						if (long.TryParse(back, out num))
						{
							if (long.TryParse(next, out num))
							{
								tokens.Dequeue();
								list.RemoveAt(list.Count - 1);
								list.Add(back + token + next);
								continue;
							}
						}
					}
					/* Optional Separated-By-Character ID */
					if (AcceptSeparatedID)
					{
						if (token == "-" && back != "-" && next != "-" &&
						   Token.IsValidIdentifier(back) && Token.IsValidIdentifier(next))
						{
							tokens.Dequeue();
							list.RemoveAt(list.Count - 1);
							list.Add(back + token + next);
							back = list[list.Count - 1];
							continue;
						}
					}
				}
				back = token;
				list.Add(token);
			}
		}
	}
	public class Separator
	{
		public char separator;
		public bool include;
		public bool addSep;

		public Separator(char separator, bool include = true, bool addSeparated = true)
		{
			this.separator = separator;
			this.include = include;
			addSep = addSeparated;
		}
	}
	public class SplitItem
	{
		public string text;
		public int position;
		public int line;

		public static implicit operator string(SplitItem item) => item.text;

		public static SplitItem operator +(SplitItem a, SplitItem b)
		{
			var c = new SplitItem(a.text + b.text, a.position > b.position ? a.position : b.position,
								  a.line > b.line ? a.line : b.line);
			return c;
		}

		public SplitItem(string text, int position, int line)
		{
			this.text = text;
			this.position = position;
			this.line = line;
		}

		public override string ToString()
		{
			return $"\"{text}\", at (Position: {position}, Line: {line})";
		}
	}
	public struct FinalSplit
	{
		public string result;
	}
	public delegate bool SplitProcessor(SplitItem token, SplitItem next,
										out FinalSplit result);
}
