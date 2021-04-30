using System;
using System.Collections.Generic;
using SpacyDotNet;

namespace Test
{
    static class LinguisticFeatures
    {
        public static void Run()
        {
            var spacy = new Spacy();
            var nlp = spacy.Load("en_core_web_sm");

            var text = "Autonomous cars shift insurance liability toward manufacturers";
            var doc = nlp.GetDocument(text);

            foreach (var token in doc.Tokens)
            {
                var childs = new List<string>();
                token.Children.ForEach(c => childs.Add(c.Text));
                Console.WriteLine($"{token.Text} {token.Dep} {token.Head.Text} [{string.Join(", ", childs)}]");
            }                
        }
    }
}
