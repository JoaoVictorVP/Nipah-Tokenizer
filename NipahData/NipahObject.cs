using System;
using System.Collections.Generic;

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
    public class NipahObject
    {
        List<string> exposed = new List<string>(32);
        Dictionary<string, NipahProperty> properties = new Dictionary<string, NipahProperty>(32);

        public List<string> ListExposed => exposed;

        public void Set(string name, object value, Type type = null)
        {
            if (properties.TryGetValue(name, out NipahProperty prop))
            {
                prop.name = name;
                prop.value = value;
                if (type != null)
                    prop.type = type;
            }
            else
                properties[name] = new NipahProperty(name, type) { value = value };
        }
        public object Get(string name)
        {
            if (properties.TryGetValue(name, out NipahProperty prop))
                return prop.value;
            return null;
        }
    }
}
