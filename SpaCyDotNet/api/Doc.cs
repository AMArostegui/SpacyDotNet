using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Doc : IXmlSerializable
    {
        private string _text;

        private Vocab _vocab;

        private List<Token> _tokens;

        private List<Span> _sentences;
        private List<Span> _nounChunks;
        private List<Span> _ents;

        public Doc()
        {
        }

        public Doc(Vocab vocab)
        {
            _vocab = vocab;

            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                dynamic pyVocab = vocab.PyVocab;
                PyDoc = spacy.tokens.doc.Doc.__call__(pyVocab);
            }
        }

        internal Doc(dynamic doc)
        {
            PyDoc = doc;
            _vocab = null;
        }

        internal Doc(dynamic doc, string text)
        {
            PyDoc = doc;
            _vocab = null;
            _text = text;
        }

        internal dynamic PyDoc { get; set; }

        public string Text
        {
            get
            {
                return Interop.GetString(PyDoc?.text, ref _text);
            }
        }

        public List<Token> Tokens
        {
            get
            {
                return Interop.GetListFromCollection(PyDoc, ref _tokens);
            }
        }

        public List<Span> Sents
        {
            get
            {
                return Interop.GetListFromGenerator(PyDoc?.sents, ref _sentences);
            }
        }

        public List<Span> NounChunks
        {
            get
            {
                return Interop.GetListFromGenerator(PyDoc?.noun_chunks, ref _nounChunks);
            }
        }

        public List<Span> Ents
        {
            get
            {
                return Interop.GetListFromGenerator(PyDoc?.ents, ref _ents);
            }
        }

        public Vocab Vocab
        {
            get
            {
                if (_vocab != null)
                    return _vocab;

                using (Py.GIL())
                {
                    var vocab = PyDoc.vocab;
                    _vocab = new Vocab(vocab);
                    return _vocab;
                }
            }
        }

        public void ToDisk(string path)
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(path);
                    PyDoc.to_disk(pyPath);
                }
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Create);

                var settings = new XmlWriterSettings();
                settings.Indent = true;
                using var writer = XmlWriter.Create(stream, settings);

                WriteXml(writer);
            }
        }

        public void FromDisk(string path)
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(path);
                    PyDoc.from_disk(pyPath);
                }
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                var settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                var reader = XmlReader.Create(stream, settings);

                var doc = new Doc();
                doc.ReadXml(reader);
                Copy(doc);
            }
        }

        public byte[] ToBytes()
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    return Interop.GetBytes(PyDoc.to_bytes());
                }
            }
            else
            {
                using var stream = new MemoryStream();

                var settings = new XmlWriterSettings();
                settings.Indent = true;
                using var writer = XmlWriter.Create(stream, settings);                

                WriteXml(writer);
                writer.Flush();
                return stream.ToArray();
            }
        }

        public void FromBytes(byte[] bytes)
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyBytes = ToPython.GetBytes(bytes);
                    PyDoc.from_bytes(pyBytes);
                }
            }
            else
            {
                var stream = new MemoryStream(bytes);

                var settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                var reader = XmlReader.Create(stream, settings);

                var doc = new Doc();
                doc.ReadXml(reader);
                Copy(doc);
            }
        }

        private void Copy(Doc doc)
        {
            // I'd rather copy Python object no matter the serialization mode
            // If set to DotNet, the variable will be initialized to null
            // disregarding its current value which might be a default object
            PyDoc = doc.PyDoc;

            _text = doc._text;
            _vocab = doc._vocab;
            _tokens = doc._tokens;
            _sentences = doc._sentences;
            _nounChunks = doc._nounChunks;
            _ents = doc._ents;                
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var serializationMode = Serialization.Selected;
            reader.MoveToContent();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Doc");
            reader.ReadStartElement();

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
                var bytesB64 = reader.ReadElementContentAsString();
                var bytes = Convert.FromBase64String(bytesB64);
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    dynamic pyVocab = spacy.vocab.Vocab.__call__();
                    PyDoc = spacy.tokens.doc.Doc.__call__(pyVocab);

                    var pyBytes = ToPython.GetBytes(bytes);
                    PyDoc.from_bytes(pyBytes);
                    _vocab = new Vocab(PyDoc.vocab);
                }
            }

            Debug.Assert(Serialization.Selected != Serialization.Mode.Spacy);

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Text");
            _text = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Vocab");
            _vocab = new Vocab(null);
            _vocab.ReadXml(reader);

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Tokens");
            _tokens = new List<Token>();
            var isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmpty)
            {
                while (reader.MoveToContent() != XmlNodeType.EndElement)
                {
                    Debug.Assert(reader.Name == $"{Serialization.Prefix}:Token");
                    reader.ReadStartElement();
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        var token = new Token();
                        token.ReadXml(reader);
                        _tokens.Add(token);
                        reader.ReadEndElement();
                    }
                }

                reader.ReadEndElement();
            }

            foreach (var token in _tokens)
                token.RestoreHead(_tokens);

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Sentences");
            _sentences = new List<Span>();
            isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmpty)
            {
                while (reader.MoveToContent() != XmlNodeType.EndElement)
                {
                    Debug.Assert(reader.Name == $"{Serialization.Prefix}:Sent");
                    reader.ReadStartElement();
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        var sent = new Span();
                        sent.ReadXml(reader);
                        _sentences.Add(sent);
                        reader.ReadEndElement();
                    }
                }

                reader.ReadEndElement();
            }

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:NounChunks");
            _nounChunks = new List<Span>();
            isEmpty = reader.IsEmptyElement;
            reader.ReadStartElement();

            if (!isEmpty)
            {
                while (reader.MoveToContent() != XmlNodeType.EndElement)
                {
                    Debug.Assert(reader.Name == $"{Serialization.Prefix}:NounChunk");
                    reader.ReadStartElement();
                    if (reader.NodeType != XmlNodeType.EndElement)
                    {
                        var nChunk = new Span();
                        nChunk.ReadXml(reader);
                        _nounChunks.Add(nChunk);
                        reader.ReadEndElement();
                    }
                }

                reader.ReadEndElement();
            }

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Ents");
            _ents = new List<Span>();
            reader.ReadStartElement();

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.Name == $"{Serialization.Prefix}:Ent");
                reader.ReadStartElement();
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    var ent = new Span();
                    ent.ReadXml(reader);
                    _ents.Add(ent);
                    reader.ReadEndElement();
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Serialization.Prefix, "Doc", Serialization.Namespace);

            var serializationMode = Serialization.Selected;

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(PyDoc.to_bytes());
                    var pyObjB64 = Convert.ToBase64String(pyObj);
                    writer.WriteElementString("PyObj", Serialization.Namespace, pyObjB64);
                }
            }

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);

            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Serialization.Namespace, Text);
            writer.WriteStartElement("Vocab", Serialization.Namespace);
            Vocab.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Tokens", Serialization.Namespace);
            foreach (var token in Tokens)
            {
                writer.WriteStartElement("Token", Serialization.Namespace);
                token.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Sentences", Serialization.Namespace);
            foreach (var sent in Sents)
            {
                writer.WriteStartElement("Sent", Serialization.Namespace);
                sent.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("NounChunks", Serialization.Namespace);
            foreach (var nounChunk in NounChunks)
            {
                writer.WriteStartElement("NounChunk", Serialization.Namespace);
                nounChunk.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Ents", Serialization.Namespace);
            foreach (var ent in Ents)
            {
                writer.WriteStartElement("Ent", Serialization.Namespace);
                ent.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
