namespace C.Tracking.Framework.Validator
{
    public interface IValitRulesStrategyPicker<TObject> : IValitRules<TObject>
        where TObject : class
    {
        IValitRules<TObject> WithStrategy(IValitStrategy strategy);
    }
}
