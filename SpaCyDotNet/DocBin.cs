using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class DocBin
    {
        private dynamic _docBin;

        public DocBin(string[] attrs = null, bool storeUserData = false)
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
                _docBin = spacy.tokens.DocBin.__call__(attrs == null ? Runtime.None : pyAttrs, pyStoreUserDate);
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
                return Utils.GetBytes(_docBin.to_bytes());
            }
        }

        public void FromBytes(byte[] bytes)
        {
            using (Py.GIL())
            {
                var jare = new byte[bytes.Length];
                bytes.CopyTo(jare, 0);

                //var pyObj = bytes.ToPython();
                var pyObj = jare.ToPython();

                //var kaka = new PyList();
                //var kakaBuff = kaka.GetBuffer();
                //kakaBuff.Write(bytes, 0, bytes.Length);

                var kakaBuff = pyObj.GetBuffer();
                //kakaBuff.Write(bytes, 0, bytes.Length);

                dynamic zlib = Py.Import("zlib");
                dynamic kakak = zlib.decompress(kakaBuff);
                


                
                //_docBin.from_bytes(pyObj);
            }
        }

        public Doc[] GetDocs(Vocab vocab)
        {
            using (Py.GIL())
            {
                dynamic pyDocs = _docBin.get_docs(vocab.PyObj());

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
