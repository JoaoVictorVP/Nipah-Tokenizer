using System;
using System.Collections.Generic;
using System.Text;

namespace NipahTokenizer.NanoDesu
{
    /// <summary>
    /// A inheritable class to define custom expressions (do not implement a constructor)
    /// </summary>
    public abstract class MiiExpression : IMiiExpression
    {
        public abstract ExpressionType type { get; }
        public ProgressiveList<IMiiExpression> components;
        /// <summary>
        /// The resulting value of running this expression (optional)
        /// </summary>
        public object Result;
        public abstract bool Setup(IProgressiveList<IMiiExpression> tokens);
        public abstract void Run();

        public static T NewExpression<T>() where T : MiiExpression
        {
            var exp = Activator.CreateInstance<T>();
            exp.components = new ProgressiveList<IMiiExpression>(32);
            return exp;
        }
    }
    public enum ExpressionType
    {
        Unary,
        Binary,
        Ternary
    }
    public interface IMiiExpression
    {
        ExpressionType type { get; }
        void Run();
    }
    public static class _IMiiExpression_
    {
        public static bool Is<T>(this IMiiExpression mii) => mii is T;
        public static IMiiExpression Is<T>(this IMiiExpression mii, out bool result)
        {
            result = mii is T;
            return mii;
        }
        public static MiiExpressionChecker<T> Is<T>(this IMiiExpression mii, Predicate<T> predicate)
        {
            T result = default;
            if(mii is T) 
                result = (T)mii;
            else
                predicate = t => false;

            return new MiiExpressionChecker<T>(result, predicate);
        }
        public static MiiExpressionChecker<T> Is<T>(this IMiiExpression mii, Predicate<T> predicate, out T result)
        {
            result = default;
            if (mii is T)
                result = (T)mii;
            else
                predicate = t => false;

            return new MiiExpressionChecker<T>(result, predicate);
        }
        public static T As<T>(this IMiiExpression mii) => (T)mii;
        public static MiiExpressionChecker Checker(this IMiiExpression mii, Predicate<IMiiExpression> checker) => new MiiExpressionChecker(mii, checker);
    }
    public struct MiiExpressionChecker
    {
        IMiiExpression target;
        Predicate<IMiiExpression> checker;
        public static implicit operator bool(MiiExpressionChecker exp) => exp.checker(exp.target);

        public MiiExpressionChecker(IMiiExpression target, Predicate<IMiiExpression> checker) 
        {
            this.target = target;
            this.checker = checker;
        }
    }
    public struct MiiExpressionChecker<TExpression>
    {
        TExpression target;
        Predicate<TExpression> checker;
        public static implicit operator bool(MiiExpressionChecker<TExpression> exp) => exp.checker(exp.target);

        public MiiExpressionChecker(TExpression target, Predicate<TExpression> checker) 
        {
            this.target = target;
            this.checker = checker;
        }
    }
}
namespace NipahTokenizer.NanoDesu.Expressions
{
    public class MiiBinding : MiiExpression
    {
        public override ExpressionType type => ExpressionType.Binary;

        Acessor from;
        IValue value;
        public override bool Setup(IProgressiveList<IMiiExpression> tokens)
        {
            return tokens.Next().Is<Acessor>(a => tokens.Next().Is<NanoExpression>(t => t.token == TokenType.Bind && (value = tokens.Next() as IValue) != null), out from);
        }

        public override void Run()
        {
            from.SetValue(value.Value);
        }
    }
}
