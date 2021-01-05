using System;
using SpacyDotNet;
using CommandLine;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CliOptions>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }

        static void RunOptions(CliOptions cliOps)
        {
            Spacy.PathBase = cliOps.PathBase;
            Spacy.PathVirtualEnv = cliOps.PathVirtualEnv;

            var separator = "____________________________________________________________________________";
            var text = @"Cuando Sebastian Thrun empezó a trabajar en coches de conducción autónoma, en 2007, para ";
            text += "Google, muy poca gente fuera de la empresa le tomó en serio. “Podría contaros como CEOs muy ";
            text += "veteranos de las empresas automotrices más grandes de América me daban la mano para después ";
            text += "ignorarme porque no merecía la pena hablar conmigo”, comentaba Thrun, en una entrevista a Recode ";
            text += "a principios de semana";

            var spacy = Spacy.Get();
            spacy.Load("es_core_news_sm");

            var doc = spacy.GetDocument(text);

            Console.WriteLine("Pipeline:");
            Console.WriteLine(spacy.PipeNames);
            Console.WriteLine(separator);

            Console.WriteLine("Tokenization");
            Console.Write("[");
            foreach (var token in doc.Tokens)
                Console.Write("'" + token.Text + "', ");
            Console.WriteLine("\b\b]");
            Console.WriteLine(separator);

            Console.WriteLine("Pos");
            Console.Write("[");
            foreach (var token in doc.Tokens)
                Console.Write("'" + token.PoS + "', ");
            Console.WriteLine("\b\b]");
            Console.WriteLine(separator);

            Console.WriteLine("PoS[0]:");
            var token0 = doc.Tokens[0];
            Console.WriteLine("Fine-grained POS tag " + token0.PoS);
            Console.WriteLine("Coarse-grained POS tag " + token0.Tag);
            Console.WriteLine("Word shape " + token0.Shape);
            Console.WriteLine("Alphabetic characters? " + token0.IsAlpha);
            Console.WriteLine("Punctuation mark? " + token0.IsPunct);
            Console.WriteLine("Digit? " + token0.IsDigit);
            Console.WriteLine("Like a number? " + token0.LikeNum);
            Console.WriteLine("Like an email address? " + token0.LikeEMail);
            Console.WriteLine(separator);

            Console.WriteLine("Lemmatization:");
            Console.Write("[");
            foreach (var token in doc.Tokens)
                Console.Write("'" + token.Lemma + "', ");
            Console.WriteLine("\b\b]");
            Console.WriteLine(separator);

            Console.WriteLine("Sentences:");
            Console.Write("[");
            foreach (var sentence in doc.Sents)
                Console.Write("'" + sentence.Text + "', ");
            Console.WriteLine("\b\b]");
            Console.WriteLine(separator);

            Console.WriteLine("Noun Phrases:");
            Console.Write("[");
            foreach (var nounChunk in doc.NounChunks)
                Console.Write("'" + nounChunk.Text + "', ");
            Console.WriteLine("\b\b]");
            Console.WriteLine(separator);

            Console.WriteLine("Entities (Named entities, phrases and concepts):");
            foreach (var entity in doc.Ents)
                Console.WriteLine("Entity: " + entity.Text + "\tLabel: " + entity.Label);
            Console.WriteLine(separator);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("You need to specify virtual environment path");
        }

        public class CliOptions
        {
            [Option("base", Required = false, HelpText = "Set base intepreter path. Useful mostly to try the 'official' initiaization code")]
            public string PathBase { get; set; }

            [Option("venv", Required = true, HelpText = "Set virtual environment path")]
            public string PathVirtualEnv { get; set; }
        }
    }
}
