namespace C.Tracking.Framework.Validator
{
    public interface IValitMessageProvider
    {
    }

    public interface IValitMessageProvider<TKey> : IValitMessageProvider
    {
        string GetByKey(TKey key);
    }
}
