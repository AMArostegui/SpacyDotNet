using System.Collections.Generic;
using Python.Runtime;

namespace SpacyDotNet
{
    public class DocBin
    {
        private dynamic _pyDocBin;

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
            using (Py.GIL())
            {
                dynamic pyDoc = doc.PyObj;
                _pyDocBin.add(pyDoc);
            }
        }

        public byte[] ToBytes()
        {
            using (Py.GIL())
            {
                return Helpers.GetBytes(_pyDocBin.to_bytes());
            }
        }

        public void FromBytes(byte[] bytes)
        {
            using (Py.GIL())
            {
                var pyObj = ToPython.GetBytes(bytes);                
                _pyDocBin.from_bytes(pyObj);
            }
        }

        public void ToDisk(string pathFile)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(pathFile);
                _pyDocBin.to_disk(pyPath);
            }
        }

        public void FromDisk(string pathFile)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(pathFile);
                _pyDocBin.from_disk(pyPath);
            }
        }

        public Doc[] GetDocs(Vocab vocab)
        {
            using (Py.GIL())
            {
                dynamic pyDocs = _pyDocBin.get_docs(vocab.PyObj);

                var docs = new List<Doc>();
                while (true)
                {
                    try
                    {
                        dynamic pyDoc = pyDocs.__next__();
                        var doc = new Doc(pyDoc);
                        docs.Add(doc);
                    }
                    catch (PythonException)
                    {
                        break;
                    }
                }

                return docs.ToArray();
            }
        }
    }
}
