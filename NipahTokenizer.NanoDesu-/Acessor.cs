namespace NipahTokenizer.NanoDesu
{
    public struct Acessor : IMiiExpression, IValue
    {
        //string name;
        //object source;
        ProgressiveList<AcessorStep> steps;

        public object Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public ExpressionType type => ExpressionType.Unary;

        public object GetValue()
        {
            steps.Reset();
            object current = null;
            while (steps.TryNext(out AcessorStep step))
            {
                if (DataStructure.New(current, out DataStructure data))
                    step.data = data;
                current = step.GetValue();
            }
            return current;
        }

        public void SetValue(object value)
        {
            steps.Reset();
            object current = null;
            DataStructure data = default;
            while (steps.TryNext(out AcessorStep step))
            {
                if (DataStructure.New(current, out data))
                {
                    step.data = data;
                }
                current = step.GetValue();
            }
            data.Set(steps.Last().name, value);
        }

        /// <summary>
        /// Build an acessor by providing a initial token, and a passed-by-ref list of tokens
        /// </summary>
        /// <param name="token">The current token of <paramref name="tokens"/></param>
        /// <param name="tokens">The tokens list</param>
        public bool Build(Token token, ref IProgressiveList<Token> tokens, ref DataStructure data, bool firstWasSetted = true)
        {
            //if (!firstWasSetted)
            //    token = tokens.Next();
            steps = new ProgressiveList<AcessorStep>(32);

        begin:
            if (firstWasSetted)
                firstWasSetted = false;
            else
                token = tokens.Next();
            if (!token.Assert(TokenType.ID))
                return false;
            steps.Add(new AcessorStep((string)token.value, data));
            token = tokens.Look_Next();
            if (token == TokenType.Dot)
            {
                token = tokens.Next();
                goto begin;
            }
            switch(tokens)
            {
                case ProgressiveListInstance<Token> tks:
                    tks.Fix();
                    break;
            }
            return true;
        }

        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
    public struct AcessorStep
    {
        public readonly string name;
        public DataStructure data;

        public object GetValue() => data.Get(name);
        public void SetValue(object value) => data.Set(name, value);

        public AcessorStep(string name, DataStructure data = default)
        {
            this.name = name;
            this.data = data;
        }
    }
}
