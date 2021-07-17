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
    }
}
