namespace SpacyDotNet
{
    using System;
    using System.IO;
    using Python.Runtime;

    public class Spacy
    {
        private Spacy()
        {
            if (string.IsNullOrEmpty(PathVirtualEnv))
                throw new InvalidOperationException("You need to define PathVirtualEnv before using the wrapper");
            if (!Directory.Exists(PathVirtualEnv))
                throw new InvalidOperationException("The directory specified for PathVirtualEnv is invalid");

            var pathVeScripts = PathVirtualEnv + @"\Scripts";
            var pathVeLib = PathVirtualEnv + @"\Lib";

            // TODO: Try this on a Linux machine
            Environment.SetEnvironmentVariable("PATH", pathVeScripts, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathVeScripts, EnvironmentVariableTarget.Process);
            var pythonPath = $"{pathVeLib}\\site-packages;{pathVeLib}";
            Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);

            PythonEngine.PythonHome = pathVeScripts;
            PythonEngine.PythonPath = pythonPath;
        }

        private static Spacy _instance;
        private dynamic _nlp;

        public static string PathVirtualEnv
            { get; set; }

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

        public static Spacy GetSpacy()
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
