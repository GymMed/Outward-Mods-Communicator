using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OutwardModsCommunicator
{
    [XmlRoot("ConfigOverrides")]
    public class ConfigOverrides
    {
        [XmlElement("Mod")]
        public List<ModOverride> Mods { get; set; } = new List<ModOverride>();
    }

    public class ModOverride
    {
        [XmlAttribute("GUID")]
        public string ModGUID { get; set; } = "";

        [XmlElement("Section")]
        public List<SectionOverride> Sections { get; set; } = new List<SectionOverride>();
    }

    public class SectionOverride
    {
        [XmlAttribute("Name")]
        public string Name { get; set; } = "";

        [XmlElement("Entry")]
        public List<EntryOverride> Entries { get; set; } = new List<EntryOverride>();
    }

    public class EntryOverride
    {
        [XmlAttribute("Key")]
        public string Key { get; set; } = "";

        [XmlAttribute("Value")]
        public string Value { get; set; } = "";
    }
}
