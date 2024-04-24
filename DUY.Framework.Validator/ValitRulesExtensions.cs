using C.Tracking.Framework.Validator.Exceptions;
using C.Tracking.Framework.Validator.Validators;

namespace C.Tracking.Framework.Validator
{
    public static class ValitRulesExtensions
    {
        public static IValitator<TObject> CreateValitator<TObject>(this IValitRules<TObject> valitRules) where TObject : class
        {
            valitRules.ThrowIfNull();
            return new Valitator<TObject>(valitRules);
        }
    }
}
