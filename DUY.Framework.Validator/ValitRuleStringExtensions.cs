using System;
using System.Text.RegularExpressions;
using C.Tracking.Framework.Validator.Errors;

namespace C.Tracking.Framework.Validator
{
    public static class ValitRuleStringExtensions
    {
        private static string EmailRegularExpression => @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public static IValitRule<TObject, string> IsEqualTo<TObject>(this IValitRule<TObject, string> rule, string value) where TObject : class
            => rule.Satisfies(p => !String.IsNullOrEmpty(p) && !String.IsNullOrEmpty(value) && p == value).WithDefaultMessage(ErrorMessages.IsEqualTo, value);

        public static IValitRule<TObject, string> MinLength<TObject>(this IValitRule<TObject, string> rule, int length) where TObject : class
            => rule.Satisfies(p => !String.IsNullOrEmpty(p) && p.Length >= length).WithDefaultMessage(ErrorMessages.MinLength, length);

        public static IValitRule<TObject, string> MaxLength<TObject>(this IValitRule<TObject, string> rule, int length) where TObject : class
            => rule.Satisfies(p => !String.IsNullOrEmpty(p) && p.Length <= length).WithDefaultMessage(ErrorMessages.MaxLength, length);

        public static IValitRule<TObject, string> Matches<TObject>(this IValitRule<TObject, string> rule, string regularExpression) where TObject : class
            => rule.Satisfies(p => !String.IsNullOrEmpty(p) && !String.IsNullOrEmpty(regularExpression) && Regex.IsMatch(p, regularExpression)).WithDefaultMessage(ErrorMessages.Matches, regularExpression);

        public static IValitRule<TObject, string> Email<TObject>(this IValitRule<TObject, string> rule) where TObject : class
            => rule.Satisfies(p => !String.IsNullOrEmpty(p) && !String.IsNullOrEmpty(EmailRegularExpression) && Regex.IsMatch(p, EmailRegularExpression)).WithDefaultMessage(ErrorMessages.Email);

        public static IValitRule<TObject, string> Required<TObject>(this IValitRule<TObject, string> rule) where TObject : class
            => rule.Satisfies(p => !string.IsNullOrEmpty(p)).WithDefaultMessage(ErrorMessages.Required);
    }
}