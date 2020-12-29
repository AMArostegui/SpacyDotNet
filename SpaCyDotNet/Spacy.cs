namespace SpacyDotNet
{
    using System;
    using Python.Runtime;

    public class Spacy
    {
        private Spacy()
        {
            if (string.IsNullOrEmpty(PathVirtualEnv))
                throw new InvalidOperationException("You need to define PathVirtualEnv before using this wrapper");            

            var pathVirtualEnvScripts = PathVirtualEnv + @"\Scripts";
            var pathVirtualEnvLib = PathVirtualEnv + @"\Lib";

            // TODO: Probar esto en Linux
            Environment.SetEnvironmentVariable("PATH", pathVirtualEnvScripts, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("PYTHONHOME", pathVirtualEnvScripts, EnvironmentVariableTarget.Process);
            var pythonPath = $"{pathVirtualEnvLib}\\site-packages;{pathVirtualEnvLib}";
            Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);

            PythonEngine.PythonHome = pathVirtualEnvScripts;
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
