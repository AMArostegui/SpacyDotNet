using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            SerializationMode = (SerializationMode)context.Context;

            if (SerializationMode == SerializationMode.SpacyAndDotNet)
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

            Debug.Assert(SerializationMode != SerializationMode.Spacy);

            var tempDocs = new List<Doc>();
            _docs = (List<Doc>)info.GetValue("Docs", tempDocs.GetType());
        }

        public SerializationMode SerializationMode { get; set; } = SerializationMode.Spacy;

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
            if (SerializationMode == SerializationMode.Spacy)
            {
                using (Py.GIL())
                {
                    return Interop.GetBytes(_pyDocBin.to_bytes());
                }
            }
            else
            {                
                var stream = new MemoryStream();
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, SerializationMode);
                formatter.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public void FromBytes(byte[] bytes)
        {
            if (SerializationMode == SerializationMode.Spacy)
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
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, SerializationMode);
                var docBin = (DocBin)formatter.Deserialize(stream);
                Copy(docBin);
            }
        }

        public void ToDisk(string pathFile)
        {
            if (SerializationMode == SerializationMode.Spacy)
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
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, SerializationMode);                
                formatter.Serialize(stream, this);
            }
        }

        public void FromDisk(string pathFile)
        {
            if (SerializationMode == SerializationMode.Spacy)
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
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.All, SerializationMode);
                var docBin = (DocBin)formatter.Deserialize(stream);
                Copy(docBin);
            }
        }

        public List<Doc> GetDocs(Vocab vocab)
        {
            return Interop.GetListWrapperObj(_pyDocBin?.get_docs(vocab.PyVocab), ref _docs);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var serializationMode = (SerializationMode)context.Context;

            if (serializationMode == SerializationMode.SpacyAndDotNet)
            {
                using (Py.GIL())
                {
                    var pyObj = Interop.GetBytes(_pyDocBin.to_bytes());
                    info.AddValue("PyObj", pyObj);
                }
            }

            Debug.Assert(serializationMode != SerializationMode.Spacy);

            info.AddValue("Docs", _docs);
        }

        private void Copy(DocBin docBin)
        {
            _docs = docBin._docs;

            // I'd rather copy Python object no matter the serialization mode
            // If set to DotNet, the variable will be initialized to null
            // disregarding its current value which might be a default object
            _pyDocBin = docBin._pyDocBin;

            if (SerializationMode == SerializationMode.SpacyAndDotNet)
            {
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
                            _docs[i].Vocab.PyVocab = pyDoc.vocab;
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
}
