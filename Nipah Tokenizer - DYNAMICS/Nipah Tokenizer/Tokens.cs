using System;
using System.Collections.Generic;

namespace NipahTokenizer
{
	public struct Tokens
	{
		public ProgressiveList<Token> Public => tokens;
		ProgressiveList<Token> tokens;

		public void UpdateBegin() => tokens.UpdateBegin();
		public int GetState() => tokens.GetState();
		public void RestoreState(int state) => tokens.RestoreState(state);

		public void Reset()
		{
			tokens.Reset();
		}

		/// <summary>
		/// Return the token that pointer is in
		/// </summary>
		/// <returns>The this.</returns>
		public Token This()
		{
			return tokens.This();
		}
		/// <summary>
		/// Return a token and advance the pointer
		/// </summary>
		/// <returns>The next.</returns>
		public Token Next()
		{
			return tokens.Next();
		}
		/// <summary>
		/// Return a token and backwards the pointer
		/// </summary>
		/// <returns>The back.</returns>
		public Token Back()
		{
			return tokens.Back();
		}

		public bool VerifyNexts(ProgressiveList<Token> nexts)
		{
			return tokens.VerifyNexts(nexts);
		}
		public bool VerifyNexts(ProgressiveList<string> nexts, bool dontWorryAboutOrder = false,
								Func<Token, Tokens, bool> checker = null)
		{
			bool ret = true;
			int state = GetState();
			bool begun = true;
			if (dontWorryAboutOrder)
				begun = false;
			Token token;
			while ((token = tokens.Next()) != null)
			{
				var ntoken = nexts.Next();
				if (ntoken == null)
				{
					if (!begun)
						return false;
					return ret;
				}
				if (token.text != ntoken)
				{
					if (!begun)
					{
						nexts.Back();
						continue;
					}
					ret = false;
					break;
				}
				else
				{
					if (!begun)
					{
						if (checker == null)
							begun = true;
						else if (checker != null)
						{
							int prev = GetState();
							if (!checker(token, this))
								nexts.Back();
							else
							{
								begun = true;
								return true;
							}
							RestoreState(prev);
						}
					}
				}
			}
			RestoreState(state);
			if (!begun)
				ret = false;
			return ret;
		}
		/// <summary>
		/// Previews the next token without moving pointer
		/// </summary>
		/// <returns>The next.</returns>
		public Token Preview_Next()
		{
			return tokens.Preview_Next();
		}
		/// <summary>
		/// Previews the back token without moving pointer
		/// </summary>
		/// <returns>The back.</returns>
		public Token Preview_Back()
		{
			return tokens.Preview_Back();
		}

		/// <summary>
		/// Move's the pointer to the next index
		/// </summary>
		public void PointerNext() => tokens.PointerNext();
		/// <summary>
		/// Move's the pointer to a back index
		/// </summary>
		public void PointerBack() => tokens.PointerBack();

		public int Pointer => tokens.Pointer;

		public Tokens SplitUntil(Predicate<Token> predicate,
								 bool includeTruth = true, bool removeSelf = true,
								 int startingIndex = 0)
		{
			var pl = tokens.SplitUntil(predicate, includeTruth, removeSelf, startingIndex);
			return new Tokens(pl);
		}

		public Tokens(List<Token> tokens)
		{
			this.tokens = new ProgressiveList<Token>(tokens);
		}
		public Tokens(ProgressiveList<Token> tokens)
		{
			this.tokens = tokens;
		}

		public Tokens RemoveComments()
		{
			var list = tokens.List();
			bool insideComment = false;
			bool lineComment = false;
			list.RemoveAll(t =>
			{
				if (!lineComment && t.type == TokenType.CommentBegin)
				{
					insideComment = true;
					return true;
				}
				if (insideComment)
				{
					if (t.type == TokenType.CommentEnd)
					{
						insideComment = false;
						return true;
					}
				}
				else
				{
					if (t.type == TokenType.Comment)
					{
						lineComment = true;
						return true;
					}
					if (lineComment && t.type == TokenType.EOF)
					{
						lineComment = false;
						return true;
					}
				}
				return insideComment || lineComment;
			});
			tokens.List(list);
			tokens.Reset();
			return this;
		}
	}
}
