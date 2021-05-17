namespace SpacyDotNet
{
    public enum Serialization
    {
        Spacy,
        SpacyAndDotNet,
        DotNet
    }

    public static class SerializationEx
    {
        public static bool IsSpacy(this Serialization value)
        {
            return value == Serialization.Spacy || value == Serialization.SpacyAndDotNet;
        }

        public static bool IsDotNet(this Serialization value)
        {
            return value == Serialization.SpacyAndDotNet || value == Serialization.DotNet;
        }
    }
}
