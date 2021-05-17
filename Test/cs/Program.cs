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
            PythonRuntimeUtils.Init(cliOps.Interpreter, cliOps.PathVirtualEnv);

            SpaCy101.Run();
            //LinguisticFeatures.Run();
            //ExampleES.Run();
            //Serialization.Run();
            //DisplaCy.Run();
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("You need to specify virtual environment path");
        }

        public class CliOptions
        {
            [Option("interpreter", Required = true, HelpText = "Filename for the interpreter. Usually python38.dll on Windows, libpython3.8.so on Linux and libpython3.8.dylib on Mac.")]
            public string Interpreter { get; set; }

            [Option("venv", Required = true, HelpText = "Set virtual environment path")]
            public string PathVirtualEnv { get; set; }
        }
    }
}
