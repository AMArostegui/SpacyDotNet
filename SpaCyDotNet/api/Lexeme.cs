﻿using System;
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

        public string Text => ToClr.GetMember<string>(_pyLexeme?.text, ref _text);
        public string Shape => ToClr.GetMember<string>(_pyLexeme?.shape_, ref _shape);
        public string Prefix => ToClr.GetMember<string>(_pyLexeme?.prefix_, ref _prefix);
        public string Suffix => ToClr.GetMember<string>(_pyLexeme?.suffix_, ref _suffix);
        public string Lang => ToClr.GetMember<string>(_pyLexeme?.lang_, ref _lang);
        public BigInteger Orth => ToClr.GetMember<BigInteger?>(_pyLexeme?.orth, ref _orth);
        public bool IsAlpha => ToClr.GetMember<bool?>(_pyLexeme?.is_alpha, ref _isAlpha);
        public bool IsDigit => ToClr.GetMember<bool?>(_pyLexeme?.is_digit, ref _isDigit);
        public bool IsTitle => ToClr.GetMember<bool?>(_pyLexeme?.is_title, ref _isTitle);

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
            var pyBytes = ToPy.GetBytes(bytes);

            using (Py.GIL())
            {                
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
                var pyObj = ToClr.GetBytes(_pyLexeme.to_bytes());
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
