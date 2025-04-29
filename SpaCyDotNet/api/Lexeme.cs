using System;
using System.Diagnostics;
using System.Numerics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;
using PythonNetUtils;

namespace SpacyDotNet
{
    public class Lexeme : IXmlSerializable
    {
        private dynamic _pyLexeme;

        private string _text;        
        private string _shape;
        private string _prefix;
        private string _suffix;
        private string _lang;

        private BigInteger? _orth;

        private bool? _isAlpha;
        private bool? _isDigit;
        private bool? _isTitle;

        public Lexeme()
        {
        }

        internal Lexeme(dynamic lexeme)
        {
            _pyLexeme = lexeme;
            _text = null;            
            _shape = null;
            _prefix = null;
            _lang = null;

            _orth = null;

            _isAlpha = null;
            _isDigit = null;
            _isTitle = null;
        }

        public string Text => Interop.GetStringMember(_pyLexeme?.text, ref _text);
        public string Shape => Interop.GetStringMember(_pyLexeme?.shape_, ref _shape);
        public string Prefix => Interop.GetStringMember(_pyLexeme?.prefix_, ref _prefix);
        public string Suffix => Interop.GetStringMember(_pyLexeme?.suffix_, ref _suffix);
        public string Lang => Interop.GetStringMember(_pyLexeme?.lang_, ref _lang);
        public BigInteger Orth => Interop.GetBigIntegerMember(_pyLexeme?.orth, ref _orth);
        public bool IsAlpha => Interop.GetBoolMember(_pyLexeme?.is_alpha, ref _isAlpha);
        public bool IsDigit => Interop.GetBoolMember(_pyLexeme?.is_digit, ref _isDigit);
        public bool IsTitle => Interop.GetBoolMember(_pyLexeme?.is_title, ref _isTitle);

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // TODO: Yet to debug. It's not being used so far
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
            var bytesB64 = reader.ReadElementContentAsString();
            var bytes = Convert.FromBase64String(bytesB64);

            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                _pyLexeme.from_bytes(pyBytes);
            }

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Text");
            _text = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Shape");
            _shape = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Prefix");
            _prefix = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Suffix");
            _suffix = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Lang");
            _lang = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Orth");
            var orth = reader.ReadElementContentAsString();
            _orth = BigInteger.Parse(orth);

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsAlpha");
            _isAlpha = reader.ReadElementContentAsBoolean();            
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsDigit");
            _isDigit = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsTitle");
            _isTitle = reader.ReadElementContentAsBoolean();
        }

        public void WriteXml(XmlWriter writer)
        {
            using (Py.GIL())
            {
                var pyObj = Interop.GetBytes(_pyLexeme.to_bytes());
                writer.WriteElementString("PyObj", pyObj, Serialization.Namespace);
            }

            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Text, Serialization.Namespace);
            writer.WriteElementString("Shape", Shape, Serialization.Namespace);
            writer.WriteElementString("Prefix", Prefix, Serialization.Namespace);
            writer.WriteElementString("Suffix", Suffix, Serialization.Namespace);
            writer.WriteElementString("Lang", Lang, Serialization.Namespace);

            writer.WriteElementString("Orth", Orth.ToString(), Serialization.Namespace);

            writer.WriteStartElement("IsAlpha", Serialization.Namespace);
            writer.WriteValue(IsAlpha);
            writer.WriteEndElement();
            writer.WriteStartElement("IsDigit", Serialization.Namespace);
            writer.WriteValue(IsDigit);
            writer.WriteEndElement();
            writer.WriteStartElement("IsTitle", Serialization.Namespace);
            writer.WriteValue(IsTitle);
            writer.WriteEndElement();
        }
    }
}
