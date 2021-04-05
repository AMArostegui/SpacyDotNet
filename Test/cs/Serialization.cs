using System;
using SpacyDotNet;

namespace Test
{
    static class Serialization
    {
        public static void PrintDoc(Doc adoc)
        {
            foreach (Token word in adoc.Tokens)
            {
                var lexeme = adoc.Vocab[word.Text];
                Console.WriteLine($@"{lexeme.Text} {lexeme.Orth} {lexeme.Shape} {lexeme.Prefix} {lexeme.Suffix} {lexeme.IsAlpha} {lexeme.IsDigit} {lexeme.IsTitle} {lexeme.Lang}");
            }
        }

        public static void Run()
        {
            var spacy = new Spacy();

            var text = "I love coffee";

            // Load base document
            var nlp = spacy.Load("en_core_web_sm");
            var docBase = nlp.GetDocument(text);
            Console.WriteLine("");
            PrintDoc(docBase);

            // Serialize document to disk and bytes
            docBase.ToDisk("doc.out");
            var docBaseBytes = docBase.ToBytes();

            // Serialize using DocBin
            var docBinBase = new DocBin(attrs: new string[] { "ENT_IOB", "POS", "HEAD", "DEP", "ENT_TYPE" }, storeUserData: true);
            docBinBase.Add(docBase);
            var docBinBaseBytes = docBinBase.ToBytes();

            // Restore document from disk
            var doc = new Doc(new Vocab());
            doc.FromDisk("doc.out");
            Console.WriteLine("");
            PrintDoc(doc);

            // Restore document from bytes
            doc = new Doc(new Vocab());
            doc.FromBytes(docBaseBytes);
            Console.WriteLine("");
            PrintDoc(doc);

            // Restore using DocBin
            var docBin = new DocBin();
            docBin.FromBytes(docBinBaseBytes);
            var docs = docBin.GetDocs(nlp.Vocab);
            Console.WriteLine("");
            PrintDoc(docs[0]);
        }
    }
}
