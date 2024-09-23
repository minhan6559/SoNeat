using SoNeat.src.NEAT;
using System.Text.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace SoNeat.src.Utils
{
    public class Utility
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            // Determine the correct separator for the current OS
            char correctSeparator = Path.DirectorySeparatorChar;
            char incorrectSeparator = (correctSeparator == '/') ? '\\' : '/';

            // Replace incorrect separators with the correct ones
            return path.Replace(incorrectSeparator, correctSeparator);
        }

        // public static void SerializeObject<T>(T serializableObject, string fileName)
        // {
        //     if (serializableObject == null) { return; }

        //     try
        //     {
        //         XmlDocument xmlDocument = new XmlDocument();
        //         XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
        //         using (MemoryStream stream = new MemoryStream())
        //         {
        //             serializer.Serialize(stream, serializableObject);
        //             stream.Position = 0;
        //             xmlDocument.Load(stream);
        //             xmlDocument.Save(fileName);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         //Log exception here
        //     }
        // }

        // public static T DeSerializeObject<T>(string fileName)
        // {
        //     if (string.IsNullOrEmpty(fileName)) { return default; }

        //     T objectOut = default;

        //     try
        //     {
        //         XmlDocument xmlDocument = new XmlDocument();
        //         xmlDocument.Load(fileName);
        //         string xmlString = xmlDocument.OuterXml;

        //         using (StringReader read = new StringReader(xmlString))
        //         {
        //             Type outType = typeof(T);

        //             XmlSerializer serializer = new XmlSerializer(outType);
        //             using (XmlReader reader = new XmlTextReader(read))
        //             {
        //                 objectOut = (T)serializer.Deserialize(reader);
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine(ex.Message);
        //         //Log exception here
        //     }

        //     return objectOut;
        // }
    }
}