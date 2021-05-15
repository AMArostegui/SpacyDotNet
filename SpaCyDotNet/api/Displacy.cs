using System;
using System.Collections.Generic;
using System.Text;
using Python.Runtime;

namespace SpacyDotNet
{
    public class Displacy
    {
        public Displacy()
        {
        }

        public void Serve(Doc doc, string style)
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");

                var pyDoc = doc.PyDoc;                
                var pyStyle = new PyString(style);
                spacy.displacy.serve(pyDoc, pyStyle);
            }
        }
    }
}
