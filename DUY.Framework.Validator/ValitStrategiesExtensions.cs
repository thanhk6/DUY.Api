using System;

namespace C.Tracking.Framework.Validator
{
    public static class ValitStrategyExtensions
    {
        public static IValitRules<TObject> WithStrategy<TObject>(this IValitRulesStrategyPicker<TObject> that, Func<DefaultValitStrategies, IValitStrategy> picker)
            where TObject : class
        {
            var strat = picker(new DefaultValitStrategies());

            return that.WithStrategy(strat);
        }
    }
}
