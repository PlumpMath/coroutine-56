using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema; 

namespace mtn
{
    [XmlRoot("CustomDictonary")]
    public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) { return; }

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                object key = reader.GetAttribute("Key");
                object value = reader.GetAttribute("Value");
                this.Add((TKey)key, (TValue)value);
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                writer.WriteStartElement("CustomItems");
                writer.WriteAttributeString("Key", key.ToString());
                writer.WriteAttributeString("Value", this[key].ToString());
                writer.WriteEndElement();
            }
        }
    }
    [XmlRoot("CustomList")]
    public class CustomList<TItem> : List<TItem>, IXmlSerializable
    {
        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) { return; }

            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                object value = reader.GetAttribute("Item");
                this.Add(((TItem)value));
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var val in this)
            {
                writer.WriteStartElement("CustomList");
                writer.WriteAttributeString("Item", val.ToString());
                writer.WriteEndElement();
            }
        }
    }
    public class SerializationUtility
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        
        public static void SerializeObject<T>(T serializableObject, string fileName, bool encrypted=false)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    if (encrypted)
                    {
                        // This resets the memory stream position for the following read operation
                        stream.Seek(0, SeekOrigin.Begin);

                        // Get the bytes
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        var serializedString = System.Text.Encoding.Default.GetString(bytes);
                        // Encrypt your bytes with your chosen encryption method, and write the result instead of the source bytes
                        string encryptedBytes = CryptoUtils.EncryptStringAES(serializedString);
                        File.WriteAllText(fileName, encryptedBytes);
                    }
                    else {
                        xmlDocument.Load(stream);
                        xmlDocument.Save(fileName);
                        stream.Close();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Serialiazing Object: " + ex);
                throw (new SystemException(ex.ToString()));
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string fileName, bool encrypted=false)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                string xmlString = null;
                if (encrypted)
                {
                    xmlString = File.ReadAllText(fileName);
                    xmlString = CryptoUtils.DecryptStringAES(xmlString);
                    xmlDocument.LoadXml(xmlString);
                }
                else {
                    xmlDocument.Load(fileName); 
                }
                xmlString = xmlDocument.OuterXml;
                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Deserializing Object: " + ex);
                throw (new SystemException(ex.ToString()));
            }

            return objectOut;
        }
    }
}
