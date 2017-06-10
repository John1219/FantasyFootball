using System;
using System.IO;
using System.Xml.Serialization;

namespace MyToolbox
{
    public class XmlManager
    {
        /// <summary>
        /// Load an object from an Xml file.
        /// </summary>
        /// <param name="type">The type of the object to load.</param>
        /// <param name="source">The path of the xml file to load from.</param>
        /// <returns>The object loaded from the Xml file.</returns>
        public object LoadFromXml(Type type, string source)
        {
            XmlSerializer deserializer = new XmlSerializer(type);
            TextReader reader = new StreamReader(source);
            object obj = deserializer.Deserialize(reader);
            reader.Close();

            return obj;
        }

        public bool SaveToXml(object obj, string destination)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(obj.GetType());
                using (TextWriter writer = new StreamWriter(destination))
                {
                    x.Serialize(writer, obj);
                }

                return true;
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox);
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }

            return false;
        }
    }
}
