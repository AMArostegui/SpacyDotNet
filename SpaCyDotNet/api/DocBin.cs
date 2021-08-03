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
    public class DocBin : IXmlSerializable
    {
        private dynamic _pyDocBin;
        private List<Doc> _docs;

        public DocBin()
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                _pyDocBin = spacy.tokens.DocBin.__call__();
            }
        }

        public DocBin(string[] attrs, bool storeUserData)
        {
            using (Py.GIL())
            {
                var pyAttrs = new PyList();
                if (attrs != null)
                {
                    foreach (var att in attrs)
                    {
                        var pyAtt = new PyString(att);
                        pyAttrs.Append(pyAtt);
                    }
                }

                var pyStoreUserDate = new PyInt(storeUserData ? 1 : 0);
                dynamic spacy = Py.Import("spacy");
                _pyDocBin = spacy.tokens.DocBin.__call__(pyAttrs, pyStoreUserDate);
            }
        }

        public void Add(Doc doc)
        {
            if (_docs == null)
                _docs = new List<Doc>();

            _docs.Add(doc);

            using (Py.GIL())
            {
                dynamic pyDoc = doc.PyDoc;
                _pyDocBin.add(pyDoc);
            }
        }

        public byte[] ToBytes()
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    return Interop.GetBytes(_pyDocBin.to_bytes());
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
                    var pyObj = ToPython.GetBytes(bytes);
                    _pyDocBin.from_bytes(pyObj);
                }
            }
            else
            {
                var stream = new MemoryStream(bytes);

                var settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                var reader = XmlReader.Create(stream, settings);

                var docBin = new DocBin();
                docBin.ReadXml(reader);
                Copy(docBin);
            }
        }

        public void ToDisk(string pathFile)
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(pathFile);
                    _pyDocBin.to_disk(pyPath);
                }
            }
            else
            {
                using var stream = new FileStream(pathFile, FileMode.Create);

                var settings = new XmlWriterSettings();
                settings.Indent = true;
                using var writer = XmlWriter.Create(stream, settings);

                WriteXml(writer);
            }
        }

        public void FromDisk(string pathFile)
        {
            if (Serialization.Selected == Serialization.Mode.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(pathFile);
                    _pyDocBin.from_disk(pyPath);
                }
            }
            else
            {
                using var stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

                var settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                var reader = XmlReader.Create(stream, settings);

                var docBin = new DocBin();
                docBin.ReadXml(reader);
                Copy(docBin);
            }
        }

        public List<Doc> GetDocs(Vocab vocab)
        {
            return Interop.GetListFromGenerator(_pyDocBin?.get_docs(vocab.PyVocab), ref _docs);
        }

        private void Copy(DocBin docBin)
        {
            _docs = docBin._docs;

            // I'd rather copy Python object no matter the serialization mode
            // If set to DotNet, the variable will be initialized to null
            // disregarding its current value which might be a default object
            _pyDocBin = docBin._pyDocBin;

            if (Serialization.Selected == Serialization.Mode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");

                    dynamic pyVocab = spacy.vocab.Vocab.__call__();
                    dynamic pyDocs = _pyDocBin.get_docs(pyVocab);

                    dynamic builtins = Py.Import("builtins");
                    dynamic listDocs = builtins.list(pyDocs);

                    var pyCount = new PyInt(builtins.len(listDocs));
                    var count = pyCount.ToInt32();

                    for (var i = 0; i < count; i++)
                    {
                        dynamic pyDoc = listDocs[i];
                        _docs[i].PyDoc = pyDoc;
                        _docs[i].Vocab.PyVocab = pyDoc.vocab;
                    }
                }
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var serializationMode = Serialization.Selected;
            reader.MoveToContent();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:DocBin");
            reader.ReadStartElement();

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
                var bytesB64 = reader.ReadElementContentAsString();
                var bytes = Convert.FromBase64String(bytesB64);

                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    _pyDocBin = spacy.tokens.DocBin.__call__();

                    var pyBytes = ToPython.GetBytes(bytes);
                    _pyDocBin.from_bytes(pyBytes);
                }
            }

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Docs");
            reader.ReadStartElement();
            _docs = new List<Doc>();            

            while (reader.MoveToContent() != XmlNodeType.EndElement)
            {                
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    var doc = new Doc();
                    doc.ReadXml(reader);
                    _docs.Add(doc);
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(Serialization.Prefix, "DocBin", Serialization.Namespace);

            var serializationMode = Serialization.Selected;

            if (serializationMode == Serialization.Mode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(_pyDocBin.to_bytes());
                    var pyObjB64 = Convert.ToBase64String(pyObj);
                    writer.WriteElementString("PyObj", Serialization.Namespace, pyObjB64);
                }
            }

            Debug.Assert(serializationMode != Serialization.Mode.Spacy);

            writer.WriteStartElement("Docs", Serialization.Namespace);
            foreach (var doc in _docs)
                doc.WriteXml(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
