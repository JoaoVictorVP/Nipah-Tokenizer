using System;

namespace NipahTokenizer
{
	public enum TokenType
	{
		None,

		/// <summary>
		/// Can represent any token (raw)
		/// </summary>
		Any,

		/// <summary>
		/// The email token '@'
		/// </summary>
		Email,
		/// <summary>
		/// The constant token 'const'
		/// </summary>
		/// <summary>
		/// The comment token '//'.
		/// </summary>
		Comment,
		/// <summary>
		/// The comment begin token '/*'.
		/// </summary>
		CommentBegin,
		/// <summary>
		/// The comment end token '*/'.
		/// </summary>
		CommentEnd,
		/// <summary>
		/// The comma character ','.
		/// </summary>
		Comma,
		//Global,
		/// <summary>
		/// The open parenthesis '('
		/// </summary>
		OpenParenthesis,
		/// <summary>
		/// The close parenthesis ')'
		/// </summary>
		CloseParenthesis,
		/// <summary>
		/// The open brackets '{'
		/// </summary>
		OpenBrackets,
		/// <summary>
		/// The close brackets '}'
		/// </summary>
		CloseBrackets,
		/// <summary>
		/// The open squares '['
		/// </summary>
		OpenSquares,
		/// <summary>
		/// The close squares ']'
		/// </summary>
		CloseSquares,
		/// <summary>
		/// true
		/// </summary>
		TrueLiteral,
		/// <summary>
		/// false
		/// </summary>
		FalseLiteral,
		/// <summary>
		/// E.g "Hello World"
		/// </summary>
		StringLiteral,
		/// <summary>
		/// E.g 13
		/// </summary>
		IntegerLiteral,
		/// <summary>
		/// E.g 0,3 || 0.3
		/// </summary>
		FloatLiteral,
		CharLiteral,
		/// <summary>
		/// null
		/// </summary>
		NullLiteral,
		Value,
		/// <summary>
		/// +
		/// </summary>
		Plus,
		/// <summary>
		/// -
		/// </summary>
		Minus,
		/// <summary>
		/// /
		/// </summary>
		Divide,
		/// <summary>
		/// *
		/// </summary>
		Multiply,
		/// <summary>
		/// Represents an id (like: 'aName_Before')
		/// </summary>
		ID,
		/// <summary>
		/// Traditional a '=' b
		/// </summary>
		Bind,
		/// <summary>
		/// a == b
		/// </summary>
		Equal,
		/// <summary>
		/// a != b
		/// </summary>
		Different,
		/// <summary>
		/// a > b
		/// </summary>
		Larger,
		/// <summary>
		/// a , b
		/// </summary>
		Lower,
		/// <summary>
		/// a >= b
		/// </summary>
		LargerOrEqual,
		/// <summary>
		/// a ,= b
		/// </summary>
		LowerOrEqual,
		/// <summary>
		/// The descript token ':'
		/// </summary>
		Descript,
		/// <summary>
		/// The and token 'AND, <![CDATA[&&]]>'
		/// </summary>
		And,
		/// <summary>
		/// The or token 'OR, ||'
		/// </summary>
		Or,
		/// <summary>
		/// The rich token '$'
		/// </summary>
		Rich,
		/// <summary>
		/// The reference token '<![CDATA[&]]>'
		/// </summary>
		Reference,
		/// <summary>
		/// The access token '->'
		/// </summary>
		Access,
		/// <summary>
		/// The 'To' token '=>'
		/// </summary>
		To,
		/// <summary>
		/// The dot token '.'
		/// </summary>
		Dot,
		/// <summary>
		/// The hashtag token '#'
		/// </summary>
		Hashtag,
		/// <summary>
		/// The exclamation token '!'
		/// </summary>
		Exclamation,
		/// <summary>
		/// The End Of Line (like: '; \n \r')
		/// </summary>
		EOF,
		End
	}
}
