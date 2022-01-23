using System;

/// <summary>
/// How it works
/// NipahData -> Container of all generated data
/// NipahObject -> Container of properties
/// NipahDataConverter -> Conversor of data types
/// 
/// Schem is like:
///  - #name -> FIELD
///  - name||"name" -> TYPE OF OBJECT && NAME OF OBJECT
///  - {} -> BODY OF OBJECT
///  - @type name || "name" && : -> THE PROPERTY TYPE (OPTIONAL) && THE PROPERTY DECLARATOR
/// </summary>
namespace NipahDataLib
{
    public class NipahProperty
    {
        Type _type;
        string _name;
        object _value;
        public Type type { get => _type; set => setType(value); }
        public string name { get => _name; set => _name = value; }
        public object value { get => _value; set => _value = value; }

        public NipahProperty(string name, Type type = null)
        {
            if (type != null)
                this.type = type;
            else
                this.type = typeof(NipahProperty_Basis);
            _name = name;
        }
        Func<string, object> fromString;
        Func<object, string> toString;

        public object FromString(string from) => fromString(from);
        public object ToString(object to) => toString(to);

        void setType(Type ntype)
        {
            if (ntype == null)
                return;

            var fs = ntype.GetMethod("FromString");
            if (fs == null)
                return;
            fromString = (Func<string, object>)Delegate.CreateDelegate(typeof(Func<string, object>), fs);

            var ts = ntype.GetMethod("ToString", new[] { typeof(object) });
            if (ts == null)
                return;

            toString = (Func<object, string>)Delegate.CreateDelegate(typeof(Func<object, string>), ts);

            _type = ntype;
        }
    }
    struct NipahProperty_Basis
    {
        public static object FromString(string from)
        {
            throw new NotImplementedException("TODO: To implement this function");
        }
        public static string ToString(object to)
        {
            string result = null;
            if (NipahDataConverter.IsGeneric(to))
                NipahDataConverter.GenericConverter_ToString(to, out result);
            return result;
        }
    }
}
