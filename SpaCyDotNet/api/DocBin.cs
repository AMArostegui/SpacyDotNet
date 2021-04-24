using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class DocBin
    {
        private dynamic _docBin;

        public DocBin()
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                _docBin = spacy.tokens.DocBin.__call__();
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
                _docBin = spacy.tokens.DocBin.__call__(pyAttrs, pyStoreUserDate);
            }
        }

        public void Add(Doc doc)
        {
            using (Py.GIL())
            {
                dynamic pyDoc = doc.PyObj;
                _docBin.add(pyDoc);
            }
        }

        public byte[] ToBytes()
        {
            using (Py.GIL())
            {
                return ToPythonHelpers.GetBytes(_docBin.to_bytes());
            }
        }

        public void FromBytes(byte[] bytes)
        {
            using (Py.GIL())
            {
                var pyObj = ToDotNetHelpers.GetBytes(bytes);                
                _docBin.from_bytes(pyObj);
            }
        }

        public void ToDisk(string pathFile)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(pathFile);
                _docBin.to_disk(pyPath);
            }
        }

        public void FromDisk(string pathFile)
        {
            using (Py.GIL())
            {
                var pyPath = new PyString(pathFile);
                _docBin.from_disk(pyPath);
            }
        }

        public Doc[] GetDocs(Vocab vocab)
        {
            using (Py.GIL())
            {
                dynamic pyDocs = _docBin.get_docs(vocab.PyObj);

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
