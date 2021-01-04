namespace SpacyDotNet
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Python.Runtime;

    public class Spacy
    {
        private Spacy()
        {
            if (string.IsNullOrEmpty(PathVirtualEnv))
                throw new Exception("You need to define PathVirtualEnv before using the wrapper");
            if (!Directory.Exists(PathVirtualEnv))
                throw new Exception("The directory specified for PathVirtualEnv is invalid");

            Init(PathVirtualEnv);
        }

        private static Spacy _instance;
        private dynamic _nlp;

        public static string PathVirtualEnv
            { get; set; }

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
        public void Init(string pathVirtualEnv)
        {
            string pathVeScripts;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                pathVeScripts = PathVirtualEnv + @"\Scripts";
            else
                pathVeScripts = PathVirtualEnv + @"/bin";
            Environment.SetEnvironmentVariable("PATH", pathVeScripts, EnvironmentVariableTarget.Process);

            var pythonPath = string.Empty;

            var proc = new Process();
            proc.StartInfo.FileName = pathVeScripts + Path.DirectorySeparatorChar + "python";
            proc.StartInfo.Arguments = "-c \"import sys; print(';'.join(sys.path))\"";
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

        public string PipeNames
        {
            get
            {
                using (Py.GIL())
                {
                    var pyPipeNames = _nlp.pipe_names;
                    return pyPipeNames.ToString();                    
                }
            }
        }

        public static Spacy Get()
        {
            if (_instance == null)
                _instance = new Spacy();
            return _instance;
        }

        public void Load(string model)
        {
            using (Py.GIL())
            {
                dynamic sp = Py.Import("spacy");

                var pyString = new PyString(model);
                _nlp = sp.load(pyString);
                return;
            }
        }

        public Doc GetDocument(string text)
        {
            using (Py.GIL())
            {
                var pyString = new PyString(text);
                dynamic doc = _nlp.__call__(pyString);
                return new Doc(doc);
            }
        }
    }
}
