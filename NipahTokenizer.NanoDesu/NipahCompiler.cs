using System;
using System.Collections.Generic;

namespace NipahTokenizer.NanoDesu
{
    public static class NipahCompiler
    {
        public static readonly Predicate<Token> ignoreToken = token => token == TokenType.EOF;
        public static readonly Predicate<IMiiExpression> ignoreExp = exp => exp is NanoExpression && ((NanoExpression)exp).token == TokenType.EOF;
        public static List<NipahCompilerPiece> pieces = new List<NipahCompilerPiece>(32)
        {
            new InvokeCompiler()
        };
        static Tokenizer tokenizer = new Tokenizer();
        public static void Compile(string code)
        {
            var tokens = new ProgressiveList<Token>(tokenizer.Tokenize(code));
            var expressions = processTokens(tokens);
            expressions = processExpressions(expressions);
        }
        static ProgressiveList<IMiiExpression> processTokens(ProgressiveList<Token> tokens)
        {
            Token token;
            var expressions = new ProgressiveList<IMiiExpression>(tokens.Count);
            while(token = tokens.Next())
            {
                var tks = (IProgressiveList<Token>)tokens.Instantiate();
                if (StaticAcessor.Build(token, ref tks, out StaticAcessor acessor))
                    expressions.Add(acessor);
                else
                    expressions.Add(new NanoExpression(token));
            }
            return expressions;
        }
        static ProgressiveList<IMiiExpression> processExpressions(ProgressiveList<IMiiExpression> expressions)
        {
            ProgressiveList<IMiiExpression> result = new ProgressiveList<IMiiExpression>(expressions.Count);
            IMiiExpression left = null;
            IMiiExpression exp;
            while((exp = expressions.Next()) != null)
            {
                foreach(var piece in pieces)
                {
                    if (piece.Process(left, exp, expressions, result))
                        continue;
                }
                left = exp;
/*                switch(exp)
                {
                    case NanoExpression nano:

                        break;
                    case StaticAcessor acessor:

                        break;
                }*/
            }
            return result;
        }
    }
    public class NanoExpression : MiiExpression
    {
        public Token token;

        public override dynamic Value => token.value;

        public NanoExpression(Token token)
        {
            this.token = token;
        }
    }
    public class DesuExpression : MiiExpression
    {
        public readonly byte opCode;
        public readonly object value;
        public DesuExpression(byte opCode, object value = null)
        {
            this.opCode = opCode;
            this.value = value;
        }
    }
    public class MiiExpression : IMiiExpression
    {
        public IMiiExpression left;
        public IMiiExpression right;
        public IMiiExpression next;
        public List<IMiiExpression> child = new List<IMiiExpression>(32);

        public virtual dynamic Value { get; set; }
    }
}
