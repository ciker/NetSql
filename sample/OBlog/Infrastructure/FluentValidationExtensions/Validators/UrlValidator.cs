﻿using System.Text.RegularExpressions;
using FluentValidation.Validators;

namespace Oldli.Fw.FluentValidationExtensions.Validators
{
    public class UrlValidator : PropertyValidator
    {
        private const string Pattern = @"^(https?)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
        private static Regex _regex;

        public UrlValidator() : base("URL地址无效")
        {
            _regex = new Regex(Pattern);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
                return false;

            return _regex.IsMatch(context.PropertyValue.ToString());
        }
    }
}
