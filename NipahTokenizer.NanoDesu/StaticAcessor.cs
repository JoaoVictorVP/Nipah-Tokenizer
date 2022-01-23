using NipahTokenizer.NanoDesu.Runtime;
using System;

namespace NipahTokenizer.NanoDesu
{
    public class StaticAcessor : MiiExpression
    {
        public override dynamic Value
        {
            get
            {
                /*steps.Reset();
                Acessor acessor = new Acessor();
                var stps = (IProgressiveList<Token>)steps.Instantiate();
                var token = stps.Next();
                acessor.Build(token, ref stps, ref NipahRuntime.DATA);
                return acessor;*/
                return null;
            }
        }

        static Predicate<Token> ignore = t => t == TokenType.EOF;
        public static bool Build(Token token, ref IProgressiveList<Token> tokens, out StaticAcessor acessor)
        {
            bool first = false;
            acessor = null;

            if(token == TokenType.ID)
            {
                acessor = new StaticAcessor();
            iterator:
                if(first)
                    token = tokens.Next();
                if (token == TokenType.ID)
                    //acessor.steps.Add(token);
                    acessor.child.Add(new NanoExpression(token));
                token = tokens.Look_Next();
                if (token == TokenType.Dot)
                {
                    tokens.Next();
                    token = tokens.Next();
                    //acessor.steps.Add(token);
                    //acessor.child.Add(new NanoExpression(token));
                    goto iterator;
                }
                else
                {
                    switch(tokens)
                    {
                        case ProgressiveListInstance<Token> tks:
                            tks.Fix();
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        //public readonly ProgressiveList<Token> steps = new ProgressiveList<Token>(32);
    }
}
