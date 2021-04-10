using System;
using System.Diagnostics;
using System.IO;
using Python.Runtime;

namespace SpacyDotNet
{
    public static class PythonRuntimeUtils
    {
        /// <summary>
        /// Python.NET project provides a WIKI to initialize the library using virtual environments. See:
        ///     https://github.com/pythonnet/pythonnet/wiki/Using-Python.NET-with-Virtual-Environments
        /// Sadly, I couldn't make the code provided in the official wiki to properly work, so I created my own initialization
        /// I've experienced all problems below
        ///     1) Inability to locate python interpreter
        ///     2) Inability to load Python system libraries
        ///     3) Inability to load Python virtual env libraries (site-packages)
        /// This method aims to solve both 2) and 3) and is an ugly HACK
        /// Using the regular workflow everything is fine; activate virtual environment and run the CPython intepreter. Only Python.NET fails.
        /// Fixing Python.NET itself would be better but for now, I'm just going to copy sys.path
        /// </summary>
        /// <param name="pathVirtualEnv">Path to virtual environment</param>        
        public static void Init(string interpreter, string pathVirtualEnv)
        {
            // SeeCliOptions.Interpreter
            Runtime.PythonDLL = interpreter;

            if (string.IsNullOrEmpty(pathVirtualEnv))
                throw new Exception("You need to define PathVirtualEnv before using the wrapper");
            if (!Directory.Exists(pathVirtualEnv))
                throw new Exception("The directory specified for PathVirtualEnv is invalid");

            string pathVeScripts;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                pathVeScripts = pathVirtualEnv + @"\Scripts";
            else
                pathVeScripts = pathVirtualEnv + @"/bin";
            Environment.SetEnvironmentVariable("PATH", pathVeScripts, EnvironmentVariableTarget.Process);

            var pythonPath = string.Empty;

            var proc = new Process();
            proc.StartInfo.FileName = pathVeScripts + Path.DirectorySeparatorChar + "python";
            proc.StartInfo.Arguments = $"-c \"import sys; print('{Path.PathSeparator}'.join(sys.path))\"";
            proc.StartInfo.RedirectStandardOutput = true;
            if (!proc.Start())
                throw new Exception("Couldn't initialize Python in virtual environment");
            proc.WaitForExit();

            pythonPath = proc.StandardOutput.ReadToEnd();
            pythonPath = pythonPath.Replace(Environment.NewLine, "");
            if (string.IsNullOrEmpty(pythonPath))
                throw new Exception("Couldn't initialize Python.NET");

            Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);
            PythonEngine.PythonPath = pythonPath;
        }
    }
}
