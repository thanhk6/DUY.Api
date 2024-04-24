using System.Collections.Immutable;

namespace C.Tracking.API.Extensions
{
    public static class CollectionExtensions
    {
        public static string Join(this ImmutableArray<string> sentences, string separator)
        {
            return string.Join(separator, sentences);
        }

        public static string JoinNewLine(this ImmutableArray<string> sentences)
        {
            return string.Join(Environment.NewLine, sentences);
        }
    }
}
