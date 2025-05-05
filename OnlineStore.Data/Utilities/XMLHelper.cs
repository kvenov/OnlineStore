using CinemaApp.Data.Utilities.Interfaces;
using System.Text;
using System.Xml.Serialization;

namespace OnlineStore.Data.Utilities
{
    public class XMLHelper : IXmlHelper
    {

        public T? Deserialize<T>(string inputXml, string rootName)
			where T : class
        {
			XmlRootAttribute rootAttribute = new XmlRootAttribute(rootName);
			XmlSerializer serializer = 
				new XmlSerializer(typeof(T), rootAttribute);

			using StringReader xmlReader = new StringReader(inputXml);
			object? deserializedObject = serializer.Deserialize(xmlReader);


			if (deserializedObject == null)
			{
				return null;
			}

			return (T)deserializedObject;
		}

		public T? Deserialize<T>(Stream inputXmlStream, string rootName)
			where T : class
		{
			XmlRootAttribute rootAttribute = new XmlRootAttribute(rootName);
			XmlSerializer serializer =
				new XmlSerializer(typeof(T), rootAttribute);

			object? deserializedObject = serializer.Deserialize(inputXmlStream);


			if (deserializedObject == null)
			{
				return null;
			}

			return (T)deserializedObject;
		}

		public string Serialize<T>(T obj, string rootName, Dictionary<string, string> namespaces = null)
		{
			StringBuilder result = new StringBuilder();

			XmlRootAttribute rootAttr = new XmlRootAttribute(rootName);
			var serializer = new XmlSerializer(typeof(T), rootAttr);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			if (namespaces == null)
			{
				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			}
			else
			{
				foreach (KeyValuePair<string, string> kvp in namespaces)
				{
					xmlSerializerNamespaces.Add(kvp.Key, kvp.Value);
				}
			}

			using StringWriter writer = new StringWriter(result);

			serializer.Serialize(writer, obj, xmlSerializerNamespaces);

			return result.ToString().TrimEnd();
		}

		public void Serialize<T>(T obj, string rootName, Stream outputStream, Dictionary<string, string> namespaces = null)
		{
			XmlRootAttribute rootAttr = new XmlRootAttribute(rootName);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			if (namespaces == null)
			{
				xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			}
			else
			{
				foreach (KeyValuePair<string, string> kvp in namespaces)
				{
					xmlSerializerNamespaces.Add(kvp.Key, kvp.Value);
				}
			}

			var serializer = new XmlSerializer(typeof(T), rootAttr);

			serializer.Serialize(outputStream, obj, xmlSerializerNamespaces);
		}
	}
}
