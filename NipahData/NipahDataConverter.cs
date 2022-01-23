using System;
/// <summary>
/// How it works
/// NipahData -> Container of all generated data
/// NipahObject -> Container of properties
/// NipahDataConvert -> Conversor of data types
/// 
/// Schem is like:
///  - #name -> FIELD
///  - name||"name" -> TYPE OF OBJECT && NAME OF OBJECT
///  - {} -> BODY OF OBJECT
///  - @type name || "name" && : -> THE PROPERTY TYPE (OPTIONAL) && THE PROPERTY DECLARATOR
/// </summary>
namespace NipahDataLib
{
    public class NipahDataConverter
    {
        public static object GenericConverter_FromString(string text, Type type)
        {
            if (type == typeof(string))
                return text;

            if (type == typeof(double) || text.Contains("d"))
                return double.Parse(text.Replace("d", ""));
            else if (type == typeof(long) || text.Contains("l"))
                return long.Parse(text.Replace("l", ""));
            else if (type == typeof(decimal) || text.Contains("dec"))
                return decimal.Parse(text.Replace("dec", ""));

            if (int.TryParse(text, out int ires))
                return ires;
            if (float.TryParse(text, out float fres))
                return fres;

            return null;
        }
        public static bool GenericConverter_ToString(object generic, out string result)
        {
            bool isGeneric = IsGeneric(generic);
            if(isGeneric)
            {
                result = "";
                if (generic is string)
                    result = (string)generic;
                else if (generic is bool || generic is int || generic is float)
                    result = generic.ToString();
                else if (generic is double)
                    result = generic.ToString() + 'd';
                else if (generic is long)
                    result = generic.ToString() + 'l';
                else if (generic is decimal)
                    result = generic + "dec";
            }else
                result = null;
            return isGeneric;
        }
        public static bool IsGeneric(object test)
        {
            return test is bool || test is int || test is float || test is double || test is long || test is decimal || test is string;
        }
    }
}
