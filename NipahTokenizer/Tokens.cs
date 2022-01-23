using System;
using System.Collections.Generic;

namespace NipahTokenizer
{
	public class Tokens : ProgressiveList<Token>
	{
		public Tokens(List<Token> source) : base(source)
        {

        }

		public Tokens RemoveComments()
		{
			var list = List();
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
			List(list);
			Reset();
			return this;
		}
	}
}
