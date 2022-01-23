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
    public class NipahData
    {
        public readonly NipahObject main = new NipahObject();

        public string Serialize()
        {
            return null;
        }
    }
}
