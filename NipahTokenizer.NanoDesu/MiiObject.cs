using System.Collections;
using System.Collections.Generic;

namespace NipahTokenizer.NanoDesu
{
    /// <summary>
    /// A complex, dynamic defined object, to use in code
    /// </summary>
    public struct MiiObject : IMiiObject, IValue
    {
        Dictionary<string, object> members;

        public object this[string member]
        {
            get => Get(member);
            set => Set(member, value);
        }

        public object Value => this;

        public void Set(string name, object value) => members[name] = value;
        public object Get(string name)
        {
            members.TryGetValue(name, out object value);
            return value;
        }

        public MiiObject(bool initialize) : this()
        {
            if (initialize)
                members = new Dictionary<string, object>(32);
        }
    }
    public interface IMiiObject
    {
        void Set(string member, object value);
        object Get(string member);
    }
    public struct DataStructure
    {
        public readonly object source;
        public readonly Type type;

        public bool NULL => source == null || type == Type.None;

        public object Get(string key)
        {
            switch(type)
            {
                case Type.IData:
                    return ((IData)source)[key];
                case Type.IMiiObject:
                    return ((IMiiObject)source).Get(key);
                case Type.IDictionary:
                    return ((IDictionary)source)[key];
                case Type.IDictionary_SO:
                    object value;
                    if (!((IDictionary<string, object>)source).TryGetValue(key, out value))
                        NipahRuntime.Error($"Can't find '{key}' within object or dictionary");
                    return value;
                    //return ((IDictionary<string, object>)source)[key];
            }
            return null;
        }
        public void Set(string key, object value)
        {
            switch(type)
            {
                case Type.IData:
                    ((IData)source)[key] = value;
                    break;
                case Type.IMiiObject:
                    ((IMiiObject)source).Set(key, value);
                    break;
                case Type.IDictionary:
                    ((IDictionary)source)[key] = value;
                    break;
                case Type.IDictionary_SO:
                    ((IDictionary<string, object>)source)[key] = value;
                    break;
            }
        }

        public static bool New(object source, out DataStructure result)
        {
            var type = Determine(source);
            if(type == Type.None)
            {
                result = default;
                return false;
            }
            result = new DataStructure(source, type);
            return true;
        }
        public static Type Determine(object source)
        {
            if (source is IDictionary<string, object>)
                return Type.IDictionary_SO;
            else if (source is IDictionary)
                return Type.IDictionary;
            else if (source is IData)
                return Type.IData;
            else if (source is IMiiObject)
                return Type.IMiiObject;
            else return Type.None;
        }

        DataStructure(object source, Type type)
        {
            this.source = source;

            this.type = type;
        }
        public enum Type
        {
            None,
            IDictionary_SO,
            IDictionary,
            IData,
            IMiiObject
        }
    }
    public interface IData
    {
        object this[string key] { get; set; }
        bool TryGetValue(string key, out object value);
    }
    public interface IValue
    {
        object Value { get; }
    }
}
