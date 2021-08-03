using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SpacyDotNet
{
    public class Span : IXmlSerializable
    {
        private dynamic _pySpan;

        private string _text;
        private string _label;
        private int? _startChar;
        private int? _endChar;

        public Span()
        {
            // Needed to use generics
        }

        internal Span(dynamic sentence)
        {
            _pySpan = sentence;
            _startChar = null;
            _endChar = null;
        }

        public string Text
        {
            get
            {
                return Interop.GetString(_pySpan?.text, ref _text);
            }
        }

        public string Label
        {
            get
            {
                return Interop.GetString(_pySpan?.label_, ref _label);
            }
        }

        public int StartChar
        {
            get
            {
                return Interop.GetInt(_pySpan?.start_char, ref _startChar);
            }
        }

        public int EndChar
        {
            get
            {
                return Interop.GetInt(_pySpan?.end_char, ref _endChar);
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Text");
            _text = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Label");
            _label = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:StartChar");
            _startChar = reader.ReadElementContentAsInt();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:EndChar");
            _endChar = reader.ReadElementContentAsInt();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Serialization.Namespace, Text);
            writer.WriteElementString("Label", Serialization.Namespace, Label);
            writer.WriteStartElement("StartChar", Serialization.Namespace);
            writer.WriteValue(StartChar);
            writer.WriteEndElement();
            writer.WriteStartElement("EndChar", Serialization.Namespace);
            writer.WriteValue(EndChar);
            writer.WriteEndElement();
        }
    }
}
