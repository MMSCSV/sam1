using System.Runtime.Serialization;
using System.Xml.Linq;

namespace CareFusion.Dispensing
{
    public static class XElementSerializer
    {
        public static XElement ToXElement<T>(T input) 
            where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));

            var doc = new XDocument();

            using (var writer = doc.CreateWriter())
            {
                serializer.WriteObject(writer, input);
            }

            return doc.Root;
        }

        public static T FromXElement<T>(XElement messageXml)
            where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = messageXml.CreateReader())
            {
                return (T)serializer.ReadObject(reader);
            }
        }
    }
}
