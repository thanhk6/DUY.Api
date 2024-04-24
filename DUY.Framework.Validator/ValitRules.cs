﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using C.Tracking.Framework.Validator.Exceptions;
using C.Tracking.Framework.Validator.MessageProvider;
using C.Tracking.Framework.Validator.Rules;
namespace C.Tracking.Framework.Validator
{
    public class ValitRules<TObject> :
        IValitRules<TObject>,
        IValitRulesMessageProvider<TObject>,
        IValitRulesStrategyPicker<TObject>
        where TObject : class
    {
        private TObject _object;
        private readonly List<IValitRule<TObject>> _rules;
        private IValitStrategy _strategy;
        private IValitMessageProvider _messageProvider;

        private ValitRules(IEnumerable<IValitRule<TObject>> rules)
        {
            _rules = rules?.ToList() ?? new List<IValitRule<TObject>>();
            _strategy = default(DefaultValitStrategies).Complete;
            _messageProvider = new EmptyMessageProvider();
        }

        public static IValitRulesMessageProvider<TObject> Create()
           => new ValitRules<TObject>(Enumerable.Empty<IValitRule<TObject>>());

        public static IValitRulesMessageProvider<TObject> Create(IValitRules<TObject> rules)
        {
            rules.ThrowIfNull();
            return new ValitRules<TObject>(rules.GetAllRules());
        }

        IValitRulesStrategyPicker<TObject> IValitRulesMessageProvider<TObject>.WithMessageProvider<TKey>(IValitMessageProvider<TKey> messageProvider)
        {
            _messageProvider = messageProvider;
            return this;
        }

        IValitRules<TObject> IValitRulesStrategyPicker<TObject>.WithStrategy(IValitStrategy strategy)
        {
            _strategy = strategy;
            return this;
        }
        IValitRules<TObject> IValitRules<TObject>.Ensure<TProperty>(Expression<Func<TObject, TProperty>> selector, Func<IValitRule<TObject, TProperty>, IValitRule<TObject, TProperty>> ruleFunc)
        {
            selector.ThrowIfNull();
            ruleFunc.ThrowIfNull();

            AddEnsureRules(selector, ruleFunc);
            return this;
        }

        IValitRules<TObject> IValitRules<TObject>.Ensure<TProperty>(Expression<Func<TObject, TProperty>> selector, IValitator<TProperty> valitator)
        {
            selector.ThrowIfNull();
            valitator.ThrowIfNull();

            var nestedValitRule = new NestedObjectValitRule<TObject, TProperty>(selector, valitator, _strategy);
            _rules.Add(nestedValitRule);
            return this;
        }

        IValitRules<TObject> IValitRules<TObject>.EnsureFor<TProperty>(Expression<Func<TObject, IEnumerable<TProperty>>> selector, Func<IValitRule<TObject, TProperty>, IValitRule<TObject, TProperty>> ruleFunc)
        {
            selector.ThrowIfNull();
            ruleFunc.ThrowIfNull();

            var collectionValitRule = new CollectionValitRule<TObject, TProperty>(selector, ruleFunc, _strategy, _messageProvider);
            _rules.Add(collectionValitRule);
            return this;
        }

        IValitRules<TObject> IValitRules<TObject>.EnsureFor<TProperty>(Expression<Func<TObject, IEnumerable<TProperty>>> selector, IValitator<TProperty> valitator)
        {
            selector.ThrowIfNull();
            valitator.ThrowIfNull();

            var collectionValitRule = new NestedObjectCollectionValitRule<TObject, TProperty>(selector, valitator, _strategy);
            _rules.Add(collectionValitRule);
            return this;
        }

        IValitRules<TObject> IValitRules<TObject>.For(TObject @object)
        {
            @object.ThrowIfNull();

            _object = @object;
            return this;
        }

        IEnumerable<IValitRule<TObject>> IValitRules<TObject>.GetAllRules()
            => _rules;

        IEnumerable<IValitRule<TObject>> IValitRules<TObject>.GetTaggedRules()
            => _rules.Where(r => r.Tags.Any());

        IEnumerable<IValitRule<TObject>> IValitRules<TObject>.GetUntaggedRules()
            => _rules.Where(r => !r.Tags.Any());

        IValitResult IValitRules<TObject>.Validate()
            => Validate(_rules);

        IValitResult IValitRules<TObject>.Validate(params string[] tags)
        {
            tags.ThrowIfNull();
            var taggedRules = _rules.Where(r => r.Tags.Intersect(tags).Any());

            return Validate(taggedRules);
        }

        IValitResult IValitRules<TObject>.Validate(Func<IValitRule<TObject>, bool> predicate)
        {
            predicate.ThrowIfNull(ValitExceptionMessages.NullPredicate);
            var taggedRules = _rules.Where(predicate);

            return Validate(taggedRules);
        }

        private IValitResult Validate(IEnumerable<IValitRule<TObject>> rules)
            => rules.ValidateRules(_strategy, _object);

        private void AddEnsureRules<TProperty>(Expression<Func<TObject, TProperty>> propertySelector, Func<IValitRule<TObject, TProperty>, IValitRule<TObject, TProperty>> ruleFunc)
        {
            var lastEnsureRule = ruleFunc(new ValitRule<TObject, TProperty>(propertySelector, _messageProvider));
            var ensureRules = lastEnsureRule.GetAllEnsureRules();
            _rules.AddRange(ensureRules);
        }
    }
}