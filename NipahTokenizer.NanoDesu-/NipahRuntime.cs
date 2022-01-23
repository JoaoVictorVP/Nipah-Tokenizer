using System;
using System.Collections.Generic;

namespace NipahTokenizer.NanoDesu
{
    public class NipahRuntime
    {
        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
    public class NipahCompiler
    {
        public NipahWorld world;
        public List<IMiiExpression> expressions = new List<IMiiExpression>(32);

        public void SetWorld(NipahWorld world) => this.world = world;

        static Tokenizer tokenizer = new Tokenizer();
        public void Compile(string code)
        {
            try {
                var _tokens_ = new ProgressiveList<Token>(tokenizer.Tokenize(code, true));
                var tokens = processTokens(_tokens_);
            }
            catch(Exception ex) { world.ERROR(ex); }
        }
        ProgressiveList<IMiiExpression> processTokens(ProgressiveList<Token> tokens)
        {
            var final = new ProgressiveList<IMiiExpression>(tokens.Count);
            Predicate<Token> clausule = t => t == TokenType.EOF;
            Token token;
            while(token = tokens.Next(clausule))
            {
                int state = tokens.GetState();
                Acessor acessor = new Acessor();
                var _tokens = (IProgressiveList<Token>)tokens.Instantiate();
                if(acessor.Build(token, ref _tokens, ref world.data))
                    final.Add(acessor);
                else
                    final.Add(new NanoExpression(token));
            }

            return final;
        }
    }
    public struct NipahWorld
    {
        public DataStructure data;
        List<string> errors;
        public void IterateErrors(Action<string> errorIterator) => errors.ForEach(errorIterator);
        public void ERROR(Exception ex) => errors.Add(ex.ToString());

        public NipahWorld(DataStructure data)
        {
            this.data = data;
            errors = new List<string>(32);
        }
    }
}
