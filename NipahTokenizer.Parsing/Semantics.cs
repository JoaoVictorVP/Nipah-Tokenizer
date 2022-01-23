using System.Collections.Generic;

namespace NipahTokenizer.Parsing
{
    public class Semantics
    {

    }
    public class SemanticsProcessor
    {
        public Type type;
        public object[] values;
        public SemanticsProcessorContext context;
        public List<SemanticsProcessor> children = new List<SemanticsProcessor>(32);

        public SemanticsProcessor ContextAs()
        {
            var sem = new SemanticsProcessor
            {
                type = Type.Context,
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor SearchFor(string search)
        {
            var sem = new SemanticsProcessor
            {
                type = Type.SearchFor,
                values = new[] { search },
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor InContext(string contextProperty)
        {
            var sem = new SemanticsProcessor
            {
                type = Type.InContext,
                values = new[] { contextProperty },
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor Object()
        {
            var sem = new SemanticsProcessor
            {
                type = Type.Object,
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor NameAs()
        {
            var sem = new SemanticsProcessor
            {
                type = Type.NameAs,
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor Property(string name)
        {
            var sem = new SemanticsProcessor
            {
                type = Type.Property,
                values = new[] { name },
                context = context
            };
            children.Add(sem);
            return sem;
        }

        public SemanticsProcessor End()
        {
            context.stack.Pop();
            return context.stack.Peek();
        }

        public enum Type
        {
            /// <summary>
            /// Normal, root node wich all other are based in
            /// </summary>
            Root,
            /// <summary>
            /// A context definition node, used to define context on semantics
            /// </summary>
            Context,
            /// <summary>
            /// Inside context get the property '...'
            /// </summary>
            InContext,
            /// <summary>
            /// Defines an object
            /// </summary>
            Object,
            /// <summary>
            /// Sets the property 'Name' of an object
            /// </summary>
            NameAs,
            /// <summary>
            /// Defines a new named proprerty on object
            /// </summary>
            Property,
            /// <summary>
            /// Search for specific tree node named in a way
            /// </summary>
            SearchFor
        }
    }
    public class SemanticsProcessorContext
    {
        public Stack<SemanticsProcessor> stack = new Stack<SemanticsProcessor>(32);
    }
}
