namespace C.Tracking.Framework.Validator
{
    public interface IValitRulesMessageProvider<TObject> : IValitRulesStrategyPicker<TObject>
        where TObject : class
    {
        IValitRulesStrategyPicker<TObject> WithMessageProvider<TKey>(IValitMessageProvider<TKey> messageProvider);
    }
}
