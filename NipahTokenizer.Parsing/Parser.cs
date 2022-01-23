using System;
using System.Collections.Generic;

namespace NipahTokenizer.Parsing
{
    public class Parser
    {
        public List<Fragment> fragments = new List<Fragment>(32);

        public ParserTree Parse(Tokens tokens)
        {
            ParserTree tree = new ParserTree();

        begin:
            var branch = new ParserNode();
            var token = tokens.Next();
            foreach (var fragment in fragments)
            {
                int state = tokens.GetState();
                if (parseFragment(token, tokens, fragment, branch))
                {
                    tree.AddNode(branch);
                }
                else
                {
                    tokens.RestoreState(state);
                    branch = new ParserNode();
                }
            }

            return tree;
        }
        bool parseFragment(Token token, Tokens tokens, Fragment frag, ParserNode tree)
        {
            tree.name = frag.name;

            foreach (var rule in frag.children)
            {
                if (!parseRule(token, tokens, rule, tree))
                {
                    if (rule.error != null)
                        throw new Exception($"Error, {rule.error}, at line {token.line}");
                    return false;
                }
                token = tokens.Next();
            }
            return true;
        }
        bool parseRule(Token token, Tokens tokens, Rule rule, ParserNode tree)
        {
            switch(rule.type)
            {
                case Rule.Type.Match:
                    if(Equals(token.value, rule.values[0]) || Equals(token.text, rule.values[0]))
                    {
                        tree.AddNode(new ParserNode { value = rule.values[0] });
                        return true;
                    }
                    break;
                case Rule.Type.Capture:
                    if (token.value != null || token.text != null)
                    {
                        tree.AddNode(new ParserNode { name = (string)rule.values[0], value = token.value ?? token.text });
                        return true;
                    }
                    break;
                case Rule.Type.Optional:
                    return parseOptional(token, tokens, rule, tree);
            }
            return false;
        }
        bool parseOptional(Token token, Tokens tokens, Rule optional, ParserNode tree)
        {
            var branch = new ParserNode();
            foreach(var child in optional.children)
            {
                var frag = child as Fragment;
                if (frag != null)
                    parseFragment(token, tokens, frag, branch);
                else
                {
                    if (!parseRule(token, tokens, child, branch))
                        break;
                    token = tokens.Next();
                }
            }
            tree.AddNode(branch);

            return true;
        }

        public Fragment Fragment(string name)
        {
            var frag = new Fragment(name) { children = new List<Rule>(32) };

            fragments.Add(frag);

            return frag;
        }
    }
    public class Fragment : Rule
    {
        public string name;

        public Fragment(string name)
        {
            this.name = name;

            type = Type.Container;
        }
    }
    public class Rule
    {
        public Type type;
        public object[] values;
        public ICollection<Rule> children;

        public string error;

        public Fragment Fragment(string name)
        {
            Fragment frag;
            children.Add(frag = new Fragment(name) { children = new List<Rule>(32) });
            return frag;
        }
        public Rule Match(string match, string error = null)
        {
            var rule = new Rule { type = Type.Match, values = new[] { match } };
            children.Add(rule);
            if (error == null)
                error = $"Expecting '{match}'";
            rule.error = error;
            return this;
        }
        public Rule Capture(string name, string error = null)
        {
            var rule = new Rule { type = Type.Capture, values = new[] { name }, error = error };
            children.Add(rule);
            return this;
        }

        public Rule Optional()
        {
            var rule = new Rule { type = Type.Optional, children = new List<Rule>(32) };
            children.Add(rule);
            return rule;
        }

        [Flags]
        public enum Type
        {
            Match = 1,
            Capture = 2,
            Ignore = 4,

            Optional = 8,
            Container = 16
        }
    }
}
