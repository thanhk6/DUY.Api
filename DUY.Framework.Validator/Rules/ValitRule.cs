using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using C.Tracking.Framework.Validator.Errors;
using C.Tracking.Framework.Validator.Exceptions;
using C.Tracking.Framework.Validator.Result;

namespace C.Tracking.Framework.Validator.Rules
{
    internal class ValitRule<TObject, TProperty> : IValitRule<TObject, TProperty>, IValitRuleAccessor<TObject, TProperty> where TObject : class
    {
        public IEnumerable<string> Tags => _tags;

        Expression<Func<TObject, TProperty>> IValitRuleAccessor<TObject, TProperty>.PropertySelector => _propertySelector;
        IValitRule<TObject, TProperty> IValitRuleAccessor<TObject, TProperty>.PreviousRule => _previousRule;

        private readonly Expression<Func<TObject, TProperty>> _propertySelector;
        private Predicate<TProperty> _predicate;
        private readonly List<Predicate<TObject>> _conditions;
        private readonly IValitRule<TObject, TProperty> _previousRule;
        private readonly List<ValitRuleError> _errors;
        private readonly List<string> _tags;
        private readonly IValitMessageProvider _messageProvider;

        internal ValitRule(IValitRule<TObject, TProperty> previousRule) : this()
        {
            var previousRuleAccessor = previousRule.GetAccessor();
            _propertySelector = previousRuleAccessor.PropertySelector;
            _previousRule = previousRule;
            _messageProvider = previousRuleAccessor.GetMessageProvider();
        }

        internal ValitRule(Expression<Func<TObject, TProperty>> propertySelector, IValitMessageProvider messageProvider) : this()
        {
            _propertySelector = propertySelector;
            _messageProvider = messageProvider;
        }

        private ValitRule()
        {
            _errors = new List<ValitRuleError>();
            _conditions = new List<Predicate<TObject>>();
            _tags = new List<string>();
        }

        void IValitRuleAccessor<TObject, TProperty>.SetPredicate(Predicate<TProperty> predicate)
            => _predicate = predicate;

        bool IValitRuleAccessor.HasPredicate()
            => _predicate != null;

        void IValitRuleAccessor.AddError(ValitRuleError error)
        {
            var areAnyDefaultMessages = _errors.Any(e => e.IsDefault);

            if(error.IsDefault && areAnyDefaultMessages)
            {
                _errors.Clear();
            }
            
            _errors.Add(error);
        }

        void IValitRuleAccessor<TObject, TProperty>.AddCondition(Predicate<TObject> condition)
            => _conditions.Add(condition);

        void IValitRuleAccessor.AddTags(params string[] tags)
            => _tags.AddRange(tags);

        IValitMessageProvider IValitRuleAccessor.GetMessageProvider()
            => _messageProvider;

        IValitMessageProvider<TKey> IValitRuleAccessor.GetMessageProvider<TKey>()
            => _messageProvider as IValitMessageProvider<TKey>;

        public IValitResult Validate(TObject @object)
        {
            @object.ThrowIfNull();

            var property = _propertySelector.Compile().Invoke(@object);
            var hasAllConditionsFulfilled = true;

            foreach(var condition in _conditions)
                hasAllConditionsFulfilled &= condition(@object);

            var isSatisfied = _predicate?.Invoke(property) != false;
            var errors = _errors.Where(e => !e.IsDefault).Any() ? _errors.Where(e => !e.IsDefault) : _errors;

            return !hasAllConditionsFulfilled || isSatisfied ? ValitResult.Success : ValitResult.Fail(errors.ToArray());
        }
    }
}
