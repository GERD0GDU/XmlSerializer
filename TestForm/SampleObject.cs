using System.Xml.Serialization;

namespace TestForm
{
    [XmlRoot("sample_object")]
    public class SampleObject
    {
        [XmlArray("items"), XmlArrayItem("item")]
        public SampleItem[] Items { get; set; }
    }
}
