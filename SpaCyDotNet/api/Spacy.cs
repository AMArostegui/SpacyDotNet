namespace SpacyDotNet
{
    using Python.Runtime;

    public class Spacy
    {
        public Spacy()
        {
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
