using Python.Runtime;
using SpacyDotNet;

namespace Test
{
    static class Misc
    {
        public static void Run()
        {
            var spacy = new Spacy();

            var nlp = spacy.Load("es_core_news_lg");
            var text = "Equipaje para el transporte de animales";

            //var nlp = spacy.Load("en_core_web_sm");
            //var text = "I love coffe";



            var doc1 = nlp.GetDocument(text);
            //var fs1 = new FastSerializable();
            //var bytes = fs1.ToBytes(doc1);

            Serialization.PrintDoc(doc1);

            //var fs2 = new FastSerializable();
            //var doc2 = fs2.FromBytes(bytes);

            //Serialization.PrintDoc(doc2);
        }
    }
}
