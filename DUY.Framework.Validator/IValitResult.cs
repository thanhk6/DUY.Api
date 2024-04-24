using System.Collections.Immutable;

namespace C.Tracking.Framework.Validator
{
    public interface IValitResult
    {
        bool Succeeded { get; }

        ImmutableArray<string> ErrorMessages { get; }

        ImmutableArray<int> ErrorCodes { get; }
    }
}
