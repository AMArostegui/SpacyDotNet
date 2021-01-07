using System;
using SpacyDotNet;

namespace Test
{
    static class SpaCy101
    {
        public static void Run()
        {
            var spacy = new Spacy();
            var nlp = spacy.Load("en_core_web_sm");

            var doc = nlp.GetDocument("Apple is looking at buying U.K. startup for $1 billion");
            foreach (Token token in doc.Tokens)
                Console.WriteLine($"{token.Text} {token.Lemma} {token.PoS} {token.Tag} {token.Dep} {token.Shape} {token.IsAlpha} {token.IsStop}");

            Console.WriteLine("");

            foreach (Span ent in doc.Ents)
                Console.WriteLine($"{ent.Text} {ent.StartChar} {ent.EndChar} {ent.Label}");
        }
    }
}
