using System;
using System.Collections.Generic;

namespace NipahTokenizer.NanoDesu.Runtime
{
    public class InvokeExpression : ValueExpression
    {
        public Acessor function;
        public List<IRuntimeExpression> arguments = new List<IRuntimeExpression>(32);
        public override dynamic GetValue()
        {
            var func = function.GetValue();
            switch(func)
            {
                case Delegate mref:
                    return mref.DynamicInvoke(buildArgs());
            }
            throw new NotImplementedException("Can't find any invoke implementation on passed function");
        }
        object[] cachedArgs;
        object[] buildArgs()
        {
            int count = arguments.Count;
            object[] args = cachedArgs ?? new object[count];
            for (int i = 0; i < count; i++)
            {
                var argument = arguments[i];
                switch(argument)
                {
                    case StaticAcessor statAcc:
                        argument = arguments[i] = statAcc.Value;
                        break;
                }
                args[i] = argument;
            }
            cachedArgs = args;
            return args;
        }
    }
}
