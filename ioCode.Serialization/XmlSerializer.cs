//------------------------------------------------------------------------------ 
// 
// File provided for Reference Use Only by ioCode (c) 2022.
// Copyright (c) ioCode. All rights reserved.
//
// Author: Gokhan Erdogdu
// 
//------------------------------------------------------------------------------
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ioCode.Serialization
{
    public sealed class XmlSerializer<T>
    {
        private XmlSerializer m_baseSerializer = null;

        private void Initialize(XmlSerializer xmlSerializer, XmlSerializerNamespaces xmlSerializerNamespaces)
        {
            m_baseSerializer = xmlSerializer;
            Namespaces = xmlSerializerNamespaces;
        }

        public XmlSerializer()
        {
            XmlSerializerNamespaces xmlSerializerNamespaces = null;
            Type genericType = typeof(T);
            PropertyInfo propertyInfo = genericType.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList()
                .Find(x => x.CanRead && x.CanWrite && (x.CustomAttributes.ToList().Exists(y => y.AttributeType == typeof(XmlNamespaceDeclarationsAttribute))));
            if (propertyInfo != null)
            {
                T obj = Activator.CreateInstance<T>();
                xmlSerializerNamespaces = (XmlSerializerNamespaces)propertyInfo.GetValue(obj);
            }
            else
            {
                xmlSerializerNamespaces = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) });
            }

            Initialize(
                new XmlSerializer(typeof(T)),
                xmlSerializerNamespaces
            );
        }

        public XmlSerializer(XmlSerializerNamespaces ns)
        {
            Initialize(
                new XmlSerializer(typeof(T)),
                ns
            );
        }

        public XmlSerializer(XmlTypeMapping xmlTypeMapping)
        {
            Initialize(
                new XmlSerializer(xmlTypeMapping),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(string defaultNamespace)
        {
            Initialize(
                new XmlSerializer(typeof(T), defaultNamespace),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(Type[] extraTypes)
        {
            Initialize(
                new XmlSerializer(typeof(T), extraTypes),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(XmlAttributeOverrides overrides)
        {
            Initialize(
                new XmlSerializer(typeof(T), overrides),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(XmlRootAttribute root)
        {
            Initialize(
                new XmlSerializer(typeof(T), root),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
        {
            Initialize(
                new XmlSerializer(typeof(T), overrides, extraTypes, root, defaultNamespace),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializer(XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
        {
            Initialize(
                new XmlSerializer(typeof(T), overrides, extraTypes, root, defaultNamespace, location),
                new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, string.Empty) })
            );
        }

        public XmlSerializerNamespaces Namespaces { get; set; }

        public T Deserialize(string xml)
        {
            TextReader reader = new StringReader(xml);
            return Deserialize(reader);
        }

        public T Deserialize(TextReader reader)
        {
            T o = (T)m_baseSerializer.Deserialize(reader);
            reader.Close();
            return o;
        }

        public string Serialize(T rootclass)
        {
            Stream stream = WriterSerialize(rootclass);
            stream.Seek(0, SeekOrigin.Begin);
            string xml = new StreamReader(stream).ReadToEnd();
            stream.Close();
            return xml.Trim();
        }

        private Stream WriterSerialize(T rootclass)
        {
            XmlWriterSettings writerSettings = new XmlWriterSettings()
            {
                NewLineHandling = NewLineHandling.Entitize,
                Encoding = Encoding.UTF8,
                Indent = true
            };

            MemoryStream ms = new MemoryStream();
            using (System.Xml.XmlWriter w = System.Xml.XmlWriter.Create(ms, writerSettings))
            {
                m_baseSerializer.Serialize(w, rootclass, Namespaces);
                w.Flush();
            }

            return ms;
        }

        #region Public Static Methods

        public static T DeserializeObject(string value)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>();
            try
            {
                return serializer.Deserialize(value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>();
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlSerializerNamespaces ns)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(ns);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlTypeMapping xmlTypeMapping)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(xmlTypeMapping);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, string defaultNamespace)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(defaultNamespace);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, Type[] extraTypes)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(extraTypes);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlAttributeOverrides overrides)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlRootAttribute root)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(root);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides, extraTypes, root, defaultNamespace);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static T ReadFile(string file, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides, extraTypes, root, defaultNamespace, location);
            try
            {
                string xml = string.Empty;
                using (StreamReader reader = new StreamReader(file))
                {
                    xml = reader.ReadToEnd();
                    reader.Close();
                }
                return serializer.Deserialize(xml);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return default(T);
        }

        public static string SerializeObject(T config)
        {
            XmlSerializer<T> serializer = new XmlSerializer<T>();
            try
            {
                return serializer.Serialize(config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: ReadFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return null;
        }

        public static bool WriteFile(string file, T config)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>();
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlSerializerNamespaces ns)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(ns);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlTypeMapping xmlTypeMapping)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(xmlTypeMapping);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, string defaultNamespace)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(defaultNamespace);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, Type[] extraTypes)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(extraTypes);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlAttributeOverrides overrides)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlRootAttribute root)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(root);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides, extraTypes, root, defaultNamespace);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        public static bool WriteFile(string file, T config, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
        {
            bool ok = false;
            XmlSerializer<T> serializer = new XmlSerializer<T>(overrides, extraTypes, root, defaultNamespace, location);
            try
            {
                string xml = serializer.Serialize(config);
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    writer.Write(xml.Trim());
                    writer.Flush();
                    writer.Close();
                }
                ok = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: WriteFile error. [{0}] {1}", ex.HResult, ex.Message);
            }
            return ok;
        }

        #endregion // Public Static Methods
    }
}
