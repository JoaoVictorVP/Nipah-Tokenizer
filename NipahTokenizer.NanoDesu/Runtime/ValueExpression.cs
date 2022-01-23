namespace NipahTokenizer.NanoDesu.Runtime
{
    public class ValueExpression : RuntimeExpression
    {
        public override dynamic Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        public virtual dynamic GetValue()
        {
            return null;
        }
        public virtual void SetValue(dynamic value)
        {

        }
    }
}
