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
                var formatter = new XmlSerializer(typeof(Doc));
                formatter.Serialize(stream, this);
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
                var formatter = new XmlSerializer(typeof(Doc));                
                var doc = (Doc)formatter.Deserialize(stream);
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
                var stream = new MemoryStream();
                var formatter = new XmlSerializer(typeof(Doc));
                formatter.Serialize(stream, this);
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
                var formatter = new XmlSerializer(typeof(Doc));
                var doc = (Doc)formatter.Deserialize(stream);
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

            Debug.Assert(reader.Name == "Doc");
            reader.ReadStartElement();

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                var dummyBytes = new byte[1];

                Debug.Assert(reader.Name == "PyObj");
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

            Debug.Assert(reader.Name == "Text");
            _text = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == "Vocab");
            _vocab = new Vocab(null);
            _vocab.ReadXml(reader);

            Debug.Assert(reader.Name == "Tokens");
            _tokens = new List<Token>();
            reader.ReadStartElement();

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.Name == "Token");
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

            foreach (var token in _tokens)
                token.RestoreHead(_tokens);

            Debug.Assert(reader.Name == "Sentences");
            _sentences = new List<Span>();
            reader.ReadStartElement();

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.Name == "Sent");
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

            Debug.Assert(reader.Name == "NounChunks");
            _nounChunks = new List<Span>();
            reader.ReadStartElement();

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.Name == "NounChunk");
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

            Debug.Assert(reader.Name == "Ents");
            _ents = new List<Span>();
            reader.ReadStartElement();

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {
                Debug.Assert(reader.Name == "Ent");
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
            var serializationMode = Serialization.Selected;

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(PyDoc.to_bytes());
                    var pyObjB64 = Convert.ToBase64String(pyObj);
                    writer.WriteElementString("PyObj", pyObjB64);
                }
            }

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);

            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Text);
            writer.WriteStartElement("Vocab");
            Vocab.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Tokens");
            foreach (var token in Tokens)
            {
                writer.WriteStartElement("Token");
                token.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Sentences");
            foreach (var sent in Sents)
            {
                writer.WriteStartElement("Sent");
                sent.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("NounChunks");
            foreach (var nounChunk in NounChunks)
            {
                writer.WriteStartElement("NounChunk");
                nounChunk.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteStartElement("Ents");
            foreach (var ent in Ents)
            {
                writer.WriteStartElement("Ent");
                ent.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
