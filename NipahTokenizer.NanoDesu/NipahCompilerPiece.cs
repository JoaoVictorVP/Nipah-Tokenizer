using System;
using System.Runtime.CompilerServices;

namespace NipahTokenizer.NanoDesu
{
    public abstract class NipahCompilerPiece
    {
        public abstract bool Process(IMiiExpression left, IMiiExpression exp, ProgressiveList<IMiiExpression> expressions, ProgressiveList<IMiiExpression> result);
    }
    public class InvokeCompiler : NipahCompilerPiece
    {
        public override bool Process(IMiiExpression left, IMiiExpression exp, ProgressiveList<IMiiExpression> expressions, ProgressiveList<IMiiExpression> result)
        {
            var invoke = new DesuExpression(0, "invoke");
            var leftAcessor = left as StaticAcessor;
            if (leftAcessor != null)
            {
                NanoExpression nano = exp as NanoExpression;
                if (nano != null && nano.token == TokenType.OpenParenthesis)
                {
                    var closures = new Closures<IMiiExpression>(iexp =>
                    {
                        var _nano = iexp as NanoExpression;
                        return _nano != null && _nano.token == TokenType.OpenParenthesis;
                    }, iexp =>
                    {
                        var _nano = iexp as NanoExpression;
                        return _nano != null && _nano.token == TokenType.CloseParenthesis;
                    }, true);
                    invoke.left = leftAcessor;
                    //MiiExpression current = invoke;
                    var checker = expressions.Instantiate();
                    while((exp = checker.Next(NipahCompiler.ignoreExp)) != null)
                    {
                        if (closures.Check(exp))
                        {
                            if (closures.IsClosed)
                            {
                                //leftAcessor.right = exp;
                                checker.Fix();
                                break;
                            }
                            else
                                continue;
                        }
                        switch(exp)
                        {
                            case StaticAcessor acessor:
                                invoke.child.Add(acessor);
                                break;
                            case NanoExpression nanoExp:
                                invoke.child.Add(nanoExp);
                                break;
                        }
                    }
                }
                else
                    return false;
                result.Add(invoke);
                return true;
            }
            return false;
        }
    }
    public struct Closures<T>
    {
        public bool IsOpen => open > 0;
        public bool IsClosed => open <= 0;
        public Predicate<T> openCase, closeCase;
        int open;

        public void Open() => open++;
        public void Close() => open--;

        public bool Check(T token)
        {
            if (openCase(token))
            {
                open++;
                return true;
            }
            else if (closeCase(token))
            {
                open--;
                return true;
            }
            return false;
        }
        public Closures(Predicate<T> openCase, Predicate<T> closeCase, bool startsOpen)
        {
            this.openCase = openCase;
            this.closeCase = closeCase;
            open = 0;
            if (startsOpen)
                open++;
        }
        public Closures(bool startsOpen)
        {
            openCase = null;
            closeCase = null;
            open = 0;
            if(startsOpen)
                open++;
        }
    }
}
