using System;

namespace NipahTokenizer
{
	public enum TokenType
	{
		None,

		/// <summary>
		/// The email token '@'
		/// </summary>
		Email,
		//Say,
		//Echo,
		//Chose,
		//Choice,
		//Define,
		//Scope,
		//Loop,
		/// <summary>
		/// The constant token 'const'
		/// </summary>
		//Constant,
		//Set,
		//Invoke,
		//Input,
		//Label,
		//Goto,
		//To,
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
		//Function,
		//Return,
		//Field,
		//Type,
		//Then,
		//If,
		//Else,
		//For,
		//ForEach,
		//While,
		/// <summary>
		/// void
		/// </summary>
		VoidLiteral,
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
		//Public,
		//Private,
		//Static,
		//Internal,
		//Export,
		ID,
		//Variable,
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
		/// The and token 'AND, e-commercial.e-commercial'
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
		/// The reference token 'E-Commercial'
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
		/// The End Of Line (; \n \r)
		/// </summary>
		EOF,
		/// <summary>
		/// The line break (when LineBreakCountAsEOF disabled, then \n will be this)
		/// </summary>
		LineBreak,
		End
	}
}
