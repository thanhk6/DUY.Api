using C.Tracking.Framework.Validator.Exceptions;

namespace C.Tracking.Framework.Validator.Rules
{
    internal static class ValitRuleAccessorExtensions
    {
        internal static IValitRuleAccessor<TObject, TProperty> GetAccessor<TObject, TProperty>(this IValitRule<TObject, TProperty> rule) where TObject : class
        {
            var accessor = rule as IValitRuleAccessor<TObject, TProperty>;
            accessor.ThrowIfNull("Rule doesn't have an accessor");

            return accessor;
        }
    }
}
