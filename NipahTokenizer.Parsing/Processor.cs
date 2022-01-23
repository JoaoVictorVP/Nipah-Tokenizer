using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NipahTokenizer.Parsing
{
    public static class Processor
    {

        public static PostProcessorNode MakePostProcess() => new PostProcessorNode(true);

        public class PostProcessorNode : TreeNode<PostProcessorNode>
        {
            public Type type;
            public object[] values;


            public PostProcessorNode SearchFor(string search, bool deep = true)
            {
                var node = new PostProcessorNode
                {
                    type = Type.SearchFor,
                    values = new object[] { processInput(search), deep },
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode InChildren()
            {
                var node = new PostProcessorNode
                {
                    type = Type.Children,
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode If(string match)
            {
                (match, string mode) = processInput(match);
                var node = new PostProcessorNode
                {
                    type = Type.If,
                    values = new[] { match, mode },
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode Else()
            {
                if (type != Type.If && type != Type.ElseIf)
                    throw new System.Exception("Expecting an 'If' or 'ElseIf' node before 'Else'");
                var node = new PostProcessorNode
                {
                    type = Type.Else,
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode ElseIf(string match)
            {
                if(type != Type.If)
                    throw new System.Exception("Expecting an 'If' node before 'ElseIf'");
                (match, string mode) = processInput(match);
                var node = new PostProcessorNode
                {
                    type = Type.ElseIf,
                    values = new[] { match, mode },
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode Emit(int offset = 0)
            {
                var node = new PostProcessorNode
                {
                    type = Type.Emit,
                    values = new object[] { offset },
                    stack = stack
                };
                Children.Add(node);
                return this;
            }
            public PostProcessorNode EmitSelf()
            {
                var node = new PostProcessorNode
                {
                    type = Type.Emit,
                    values = new object[] { -1 },
                    stack = stack
                };
                Children.Add(node);
                return this;
            }
            public PostProcessorNode Header(string header)
            {
                var node = new PostProcessorNode
                {
                    type = Type.Header,
                    values = new[] { header },
                    stack = stack
                };
                Children.Add(node);
                return this;
            }
            public PostProcessorNode Branch(string name)
            {
                var node = new PostProcessorNode
                {
                    type = Type.Branch,
                    values = new[] { name },
                    stack = stack
                };
                stack.Push(node);
                Children.Add(node);
                return node;
            }
            public PostProcessorNode End()
            {
                // detaches last and return current (the parent of this)
                stack.Pop();
                // if else or else if, detaches else|elseif and the parent of these <if>, in order to prevent unnecessary typing
                if (type == Type.Else || type == Type.ElseIf)
                    stack.Pop();
                return stack.Peek();
            }


            (string match, string mode) processInput(string input)
            {
                string mode = "text";
                if (input.Length > 2 && input[0] == 'r' && input[1] == ':')
                {
                    input = input.Remove(0, 2);
                    mode = "regex";
                }
                return (input, mode);
            }

            #region BUILD
            PostProcessorNode context;
            public ParserTree Build(ParserTree source)
            {
                var tree = new ParserTree();

                buildOn(source, tree);

                return tree;
            }
            void buildOn(TreeNode<ParserNode> source, TreeNode<ParserNode> tree)
            {
                ParserNode pnode;
                switch(type)
                {
                    case Type.SearchFor:
                        var search = ((string match, string mode))values[0];
                        bool deep = (bool)values[1];

                        var searchResults = new List<ParserNode>(32);
                        if(deep)
                        {
                            void iterateOver(List<TreeNode> children)
                            {
                                foreach(ParserNode child in children)
                                {
                                    if (isMatch(search.match, search.mode, child.name) || isMatch(search.match, search.mode, child.value as string))
                                        searchResults.Add(child);
                                    iterateOver(child.Children);
                                }
                            }
                            iterateOver(source.Children);
                        }
                        else
                        {
                            foreach (ParserNode child in source.Children)
                                if (isMatch(search.match, search.mode, child.name) || isMatch(search.match, search.mode, child.value as string))
                                    searchResults.Add(child);
                        }

                        foreach(var result in searchResults)
                        {
                            var branch = new ParserNode { name = result.name, value = result.value };
                            foreach (PostProcessorNode child in Children)
                                child.buildOn(result, branch);
                            if(branch.Children.Count > 0)
                                tree.AddNode(branch);
                        }

                        return;
                    case Type.Children:
                        foreach(ParserNode childNode in source.Children)
                        {
                            var branch = new ParserNode { name = childNode.name, value = childNode.value };
                            foreach (PostProcessorNode child in Children)
                                child.buildOn(childNode, branch);
                            if (branch.Children.Count > 0)
                                tree.AddNode(branch);
                        }
                        return;
                    case Type.If:
                    case Type.ElseIf:
                        pnode = source as ParserNode;
                        if(pnode != null)
                        {
                            string match = (string)values[0];
                            string mode = (string)values[1];
                            if(isMatch(match, mode, pnode.name) || isMatch(match, mode, pnode.value as string))
                            {
                                var branch = new ParserNode { name = pnode.name, value = pnode.value};
                                foreach (PostProcessorNode child in Children)
                                    if(child.type != Type.ElseIf && child.type != Type.Else)
                                        child.buildOn(source, branch);
                                if (branch.Children.Count > 0)
                                    tree.AddNode(branch);
                            }
                            else
                            {
                                foreach(PostProcessorNode child in Children)
                                {
                                    if (child.type == Type.ElseIf || child.type == Type.Else)
                                        child.buildOn(source, tree);
                                }
                            }
                        }
                        return;
                    case Type.Else:
                        pnode = source as ParserNode;
                        if (pnode != null)
                        {
                            var branch = new ParserNode { name = pnode.name, value = pnode.value };
                            foreach (PostProcessorNode child in Children)
                                child.buildOn(source, branch);
                            if (branch.Children.Count > 0)
                                tree.AddNode(branch);
                        }
                        return;

                    case Type.Emit:
                        int offset = (int)values[0];
                        if (offset >= source.Children.Count)
                            return;
                        pnode = (offset < 0 ? source : source.Children[offset]) as ParserNode;
                        if (pnode != null)
                        {
                            var branch = pnode.CopyDeep();
                            tree.AddNode(branch);
                        }
                        return;
                    case Type.Header:
                        string header = (string)values[0];
                        tree.AddNode(new ParserNode { name = header });
                        return;
                    case Type.Branch:
                        string name = (string)values[0];
                        var selfBranch = new ParserNode { name = name };
                        foreach (PostProcessorNode child in Children)
                            child.buildOn(source, selfBranch);
                        tree.AddNode(selfBranch);
                        return;
                }

                foreach (PostProcessorNode child in Children)
                    child.buildOn(source, tree);
            }

            bool isMatch(string match, string mode, string input)
            {
                if (input == null)
                    return false;
                switch(mode)
                {
                    case "text":
                        return match == input;
                    case "regex":
                        return Regex.IsMatch(input, match);
                }
                return false;
            }

            #endregion

            Stack<PostProcessorNode> stack;
            public PostProcessorNode(bool root = false)
            {
                if (root)
                {
                    stack = new Stack<PostProcessorNode>(32);
                    stack.Push(this);
                }
            }
            public enum Type
            {
                Root,
                SearchFor,
                Children,
                If,
                Else,
                ElseIf,
                Emit,
                Header,
                Branch
            }
        }
    }
}
