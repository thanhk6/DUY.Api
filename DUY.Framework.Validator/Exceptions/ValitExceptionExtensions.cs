namespace C.Tracking.Framework.Validator.Exceptions
{
    internal static class ValitExceptionExtensions
    {
        internal static void ThrowIfNull<T>(this T @object)
            where T : class
        {
            if(@object == null)
            {
                throw SemanticExceptions.NullDereferenced();
            }
        }

        internal static void ThrowIfNull<T>(this T @object, string message)
            where T: class
        {
            if(@object == null)
            {
                throw SemanticExceptions.NullDereferenced(message);
            }
        }
    }
}
