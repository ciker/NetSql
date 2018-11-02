﻿using System.Text.RegularExpressions;
using FluentValidation.Validators;

namespace Oldli.Fw.FluentValidationExtensions.Validators
{
    public class PhoneValidator : PropertyValidator
    {
        private const string Pattern = @"^1\d{10}$";
        private static Regex _regex;

        public PhoneValidator() : base("手机号无效")
        {
            _regex=new Regex(Pattern);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
                return false;

            return _regex.IsMatch(context.PropertyValue.ToString());
        }
    }
}
