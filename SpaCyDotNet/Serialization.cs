namespace SpacyDotNet
{
    public static class Serialization
    {
        public enum Mode
        {
            Spacy,
            SpacyAndDotNet,
            DotNet
        }

        public static Mode Selected { get; set; } = Mode.Spacy;

        public static string Namespace = "https://github.com/AMArostegui/SpacyDotNet";

        public static string Prefix = "sdn";
    }
}
