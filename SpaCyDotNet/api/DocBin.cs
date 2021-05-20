using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Python.Runtime;

namespace SpacyDotNet
{
    [Serializable]
    public class DocBin : ISerializable
    {
        private dynamic _pyDocBin;
        private List<Doc> _docs;

        private Serialization _serialization = Serialization.Spacy;

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

        protected DocBin(SerializationInfo info, StreamingContext context)
        {
            if (Serialization.IsSpacy())
            {
                var dummyBytes = new byte[1];

                var bytes = (byte[])info.GetValue("PyObj", dummyBytes.GetType());
                using (Py.GIL())
                {
                    dynamic spacy = Py.Import("spacy");
                    _pyDocBin = spacy.tokens.DocBin.__call__();

                    var pyBytes = ToPython.GetBytes(bytes);
                    _pyDocBin.from_bytes(pyBytes);
                }
            }

            if (Serialization.IsDotNet())
            {
                var tempDocs = new List<Doc>();
                _docs = (List<Doc>)info.GetValue("Docs", tempDocs.GetType());
            }
        }

        public Serialization Serialization
        {
            get => _serialization;

            set
            {
                _serialization = value;
                foreach (var doc in _docs)
                    doc.Serialization = value;
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
            if (Serialization == Serialization.Spacy)
            {
                using (Py.GIL())
                {
                    return Interop.GetBytes(_pyDocBin.to_bytes());
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                var stream = new MemoryStream();
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public void FromBytes(byte[] bytes)
        {
            if (Serialization == Serialization.Spacy)
            {
                using (Py.GIL())
                {
                    var pyObj = ToPython.GetBytes(bytes);
                    _pyDocBin.from_bytes(pyObj);
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                var stream = new MemoryStream(bytes);
                var docBin = (DocBin)formatter.Deserialize(stream);
                Copy(docBin);
            }
        }

        public void ToDisk(string pathFile)
        {
            if (Serialization == Serialization.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(pathFile);
                    _pyDocBin.to_disk(pyPath);
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                using var stream = new FileStream(pathFile, FileMode.Create);
                formatter.Serialize(stream, this);
            }
        }

        public void FromDisk(string pathFile)
        {
            if (Serialization == Serialization.Spacy)
            {
                using (Py.GIL())
                {
                    var pyPath = new PyString(pathFile);
                    _pyDocBin.from_disk(pyPath);
                }
            }
            else
            {
                var formatter = new BinaryFormatter();
                using var stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
                var docBin = (DocBin)formatter.Deserialize(stream);
                Copy(docBin);
            }
        }

        public List<Doc> GetDocs(Vocab vocab)
        {
            return Interop.GetListWrapperObj(_pyDocBin?.get_docs(vocab.PyObj), ref _docs);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (Serialization.IsSpacy())
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(_pyDocBin.to_bytes());
                    info.AddValue("PyObj", pyObj);
                }
            }

            if (Serialization.IsDotNet())
                info.AddValue("Docs", _docs);
        }

        private void Copy(DocBin docBin)
        {
            _pyDocBin = docBin._pyDocBin;
            _docs = docBin._docs;

            if (!Serialization.IsSpacy())
                return;

            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                dynamic pyVocab = spacy.vocab.Vocab.__call__();
                dynamic pyDocs = _pyDocBin.get_docs(pyVocab);

                var i = 0;
                while (true)
                {
                    try
                    {
                        dynamic pyDoc = pyDocs.__next__();
                        _docs[i].PyDoc = pyDoc;
                        i++;
                    }
                    catch (PythonException)
                    {
                        break;
                    }
                }
            }
        }
    }
}
