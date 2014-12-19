using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Core.XML
{
    /// <summary>
    /// This class provides some utility methods to convert the xml and object.
    /// </summary>
    public sealed class XmlConvertor
    {
        private XmlConvertor()
        {
        }

        /// <summary>
        /// Converts the xml string to the specified object.
        /// </summary>
        /// <param name="xml">The xml string.</param>
        /// <param name="type">The object type.</param>
        /// <returns>The object deserialized from the xml string.</returns>
        public static object XmlToObject(string xml, Type type)
        {
            if (null == xml)
            {
                throw new ArgumentNullException("xml");
            }
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            object obj = null;
            var serializer = new XmlSerializer(type);
            var strReader = new StringReader(xml);
            XmlReader reader = new XmlTextReader(strReader);

            try
            {
                obj = serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ie)
            {
                throw new InvalidOperationException("Can not convert xml to object", ie);
            }
            finally
            {
                reader.Close();
            }
            return obj;
        }

        /// <summary>
        /// Converts the object to xml string.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="toBeIndented"><c>true</c> if wants the xml string was indented, otherwise <c>false</c>.</param>
        /// <returns>The xml string.</returns>
        public static string ObjectToXml(object obj, bool toBeIndented)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            var encoding = new UTF8Encoding(false);
            var serializer = new XmlSerializer(obj.GetType());
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, encoding);
            writer.Formatting = (toBeIndented ? Formatting.Indented : Formatting.None);

            try
            {
                serializer.Serialize(writer, obj);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("Can not convert object to xml.");
            }
            finally
            {
                writer.Close();
            }

            var xml = encoding.GetString(stream.ToArray());
            return xml;
        }
    }
}