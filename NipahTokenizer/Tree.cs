using System.Collections.Generic;
using System.Text;

namespace NipahTokenizer
{
    /// <summary>
    /// An alias for TreeNode
    /// </summary>
    public class Tree : TreeNode
    {

    }
    public class Tree<TNode> : TreeNode<TNode> where TNode : TreeNode
    {

    }
    public class TreeNode<TNode> : TreeNode where TNode : TreeNode
    {
        public TreeNode<TNode> AddNode(TNode node) => base.AddNode(node) as TreeNode<TNode>;
        public TreeNode<TNode> RemoveNode(TNode node) => base.RemoveNode(node) as TreeNode<TNode>;
    }
    /// <summary>
    /// The base class for creating trees with C#
    /// </summary>
    public class TreeNode
    {
        public List<TreeNode> Children = new List<TreeNode>(32);

        public TreeNode AddNode(TreeNode node)
        {
            Children.Add(node);

            return this;
        }
        public TreeNode RemoveNode(TreeNode node)
        {
            Children.Remove(node);

            return this;
        }

        protected virtual void OnToString(StringBuilder context, int indentLevel) { }

        protected void Indent(StringBuilder context, int level)
        {
            if (level == 0) return;
            for (int i = 0; i < level; i++)
                context.Append("  ");
            context.Append("-> ");
        }
        protected void ToStringChildren(StringBuilder context, int indentLevel)
        {
            indentLevel++;
            foreach (var child in Children)
            {
                Indent(context, indentLevel);
                child.OnToString(context, indentLevel);
                context.AppendLine();

                child.ToStringChildren(context, indentLevel);
            }
        }

        public override string ToString()
        {
            var ctx = new StringBuilder();

            OnToString(ctx, 0);
            ctx.AppendLine();

            ToStringChildren(ctx, 0);

            return ctx.ToString();
        }
    }
}
