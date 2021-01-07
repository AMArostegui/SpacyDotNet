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
            PythonRuntimeUtils.Init(cliOps.PathVirtualEnv);

            //ExampleES.Run();
            SpaCy101.Run();
            //DisplaCy.Run();
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
