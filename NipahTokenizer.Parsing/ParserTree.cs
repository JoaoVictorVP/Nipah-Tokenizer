using System.Text;

namespace NipahTokenizer.Parsing
{
    public class ParserTree : Tree<ParserNode>
    {
        protected override void OnToString(StringBuilder context, int indentLevel)
        {
            context.Append("<Parser Tree>");
        }
    }
    public class ParserNode : TreeNode<ParserNode>
    {
        public string name;
        public object value;

        protected override void OnToString(StringBuilder context, int indentLevel)
        {
            if(name == null && value == null)
            {
                context.Append("<Parser Node>");
                return;
            }
            context.Append($"{name} ({value})");
        }

        public ParserNode CopyDeep()
        {
            var copy = new ParserNode { name = name, value = value };
            foreach (ParserNode child in Children)
                copy.AddNode(child.CopyDeep());
            return copy;
        }
    }
}
