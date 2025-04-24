using Python.Runtime;
using PythonNetUtils;
using System;

namespace SpacyDotNet
{
    public class Spacy
    {
        public Spacy()
        {
            if (!PythonRt.IsInitialized)
            {
                throw new InvalidOperationException("Initialize runtime before usage");
            }
        }

        public Lang Load(string model)
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                var pyString = new PyString(model);
                var nlp = spacy.load(pyString);
                return new Lang(nlp);
            }
        }
    }
}
