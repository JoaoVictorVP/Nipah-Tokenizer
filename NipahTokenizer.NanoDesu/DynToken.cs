using System;

namespace NipahTokenizer.NanoDesu
{
    public class DynToken
    {
        public Token token => tokens.This();
        public Token next => tokens.Look_Next();
        public Token previous => tokens.Look_Back();
        public Predicate<Token> discardThese;

        ProgressiveListInstance<Token> tokens;
        IProgressiveList<Token> _tokens;

        public bool advance()
        {
            if (discardThese != null)
                return tokens.TryNext(discardThese, out _);
            return tokens.TryNext(out _);
        }

        public void consume()
        {
            var persistent = _tokens as IPersistentList<int>;
            if (persistent != null) persistent.RestoreState(tokens.Pointer);
            else
                while (tokens.Pointer != _tokens.Pointer) 
                    _tokens.Next();
        }

        public DynToken(IProgressiveList<Token> tokens, Predicate<Token> discardThese = null)
        {
            this._tokens = tokens;
            this.tokens = tokens.Instantiate();
            this.discardThese = discardThese;
        }

        public static implicit operator Token (DynToken dyn) => dyn.token;
    }
}
