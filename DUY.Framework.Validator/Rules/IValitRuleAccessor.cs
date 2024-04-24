using System;
using System.Linq.Expressions;
using C.Tracking.Framework.Validator.Errors;

namespace C.Tracking.Framework.Validator.Rules
{
    internal interface IValitRuleAccessor
    {
        IValitMessageProvider GetMessageProvider();
        IValitMessageProvider<TKey> GetMessageProvider<TKey>();
        void AddError(ValitRuleError error);
        void AddTags(params string[] tags);
        bool HasPredicate();
    }

    internal interface IValitRuleAccessor<TObject, TProperty> : IValitRuleAccessor where TObject : class
    {
        Expression<Func<TObject, TProperty>> PropertySelector { get; }
        IValitRule<TObject, TProperty> PreviousRule { get; }
        void SetPredicate(Predicate<TProperty> predicate);
        void AddCondition(Predicate<TObject> condition);
    }
}
