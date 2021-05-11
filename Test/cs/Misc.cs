using SpacyDotNet;
using System;

namespace Test
{
    static class Misc
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

            var nlp = spacy.Load("es_core_news_lg");
            var text = "Equipaje para el transporte de animales";

            var doc1 = nlp.GetDocument(text);
            var ser1 = new SerializableEx();
            var bytes = ser1.ToBytes(doc1);

            PrintDoc(doc1);

            var ser2 = new SerializableEx();
            var doc2 = ser2.FromBytes(bytes);

            PrintDoc(doc2);
        }
    }
}
