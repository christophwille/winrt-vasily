using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Vasily.ThirdParty
{
    //
    // http://social.msdn.microsoft.com/Forums/nl-BE/winappswithcsharp/thread/4b847d71-9cd5-4457-add9-f68e457b23ff
    //
    public static class SerializationHelper
    {
        public static T DeserializeFromString<T>(string data)
        {
            using (var stringReader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
        }

        public static string SerializeToString<T>(T value)
        {
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, value);
                return stringWriter.ToString();
            }
        }
    }
}
