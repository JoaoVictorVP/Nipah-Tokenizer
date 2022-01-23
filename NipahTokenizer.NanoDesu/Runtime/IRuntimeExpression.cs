using System;

namespace NipahTokenizer.NanoDesu.Runtime
{
    public interface IRuntimeExpression : IMiiExpression
    {
        dynamic Value { get; set; }
    }
    public abstract class RuntimeExpression : IRuntimeExpression
    {
        public abstract dynamic Value { get; set; }
        public IRuntimeExpression left => _left;
        public IRuntimeExpression right => _right;
        public IRuntimeExpression child => _child;
        IRuntimeExpression _left, _right, _child;

        public void SetNext(IRuntimeExpression next, NextExpressions wich)
        {
            switch(wich)
            {
                case NextExpressions.Left:
                    _left = next;
                    break;
                case NextExpressions.Right:
                    _right = next;
                    break;
                case NextExpressions.Child:
                    _child = next;
                    break;
            }
        }

        public static TExpression Create<TExpression>(IRuntimeExpression left, IRuntimeExpression right, IRuntimeExpression child) where TExpression : RuntimeExpression
        {
            var exp = Activator.CreateInstance<TExpression>();
            exp._left = left; exp._right = right; exp._child = child;
            return exp;
        }
    }
    public enum NextExpressions
    {
        Left,
        Right,
        Child
    }
}
