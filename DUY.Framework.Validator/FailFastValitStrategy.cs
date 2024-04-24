namespace C.Tracking.Framework.Validator
{
    public class FailFastValitStrategy : IValitStrategy
    {
        public void Fail<TObject>(IValitRule<TObject> rule, IValitResult result, out bool cancel)
            where TObject : class
        {
            cancel = true;
        }

        public void Done(IValitResult result)
        {
        }
    }
}
