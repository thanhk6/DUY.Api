using System.Collections.Generic;

namespace C.Tracking.Framework.Validator
{
    public interface IValitRule
    {
        IEnumerable<string> Tags { get; }
    }

    public interface IValitRule<TObject> : IValitRule where TObject : class
    {
        IValitResult Validate(TObject @object);
    }
    public interface IValitRule<TObject, TProperty> : IValitRule<TObject> where TObject : class
    {
    }
}
