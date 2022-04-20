using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TestForm
{
    [XmlRoot("sample_item")]
    public class SampleItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [Browsable(false)]
        [XmlElement("key")]
        public Guid Key { get; set; } = Guid.NewGuid();

        public override string ToString()
        {
            return $"[\"{Name}\": \"{Value}\"]";
        }
    }
}
