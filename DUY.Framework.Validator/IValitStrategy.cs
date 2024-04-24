namespace C.Tracking.Framework.Validator
{
    public interface IValitStrategy
    {
        void Fail<TObject>(IValitRule<TObject> rule, IValitResult result, out bool cancel) where TObject : class;
        void Done(IValitResult result);
    }
}
