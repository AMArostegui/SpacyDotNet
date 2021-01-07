using System;
using System.Collections.Generic;
using System.Text;
using SpacyDotNet;

namespace Test
{
    static class DisplaCy
    {
        public static void Run()
        {
            var spacy = new Spacy();
            var nlp = spacy.Load("en_core_web_sm");

            var doc = nlp.GetDocument("Apple is looking at buying U.K. startup for $1 billion");
            var displacy = new Displacy();
            displacy.Serve(doc, "dep");
        }
    }
}
