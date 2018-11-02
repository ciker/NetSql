using FluentValidation;
using Oldli.Fw.FluentValidationExtensions.Validators;

namespace OBlog.Infrastructure.FluentValidationExtensions
{
    public static class ValidatorExtensions
    {
        /// <summary>
        /// 验证手机号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> Phone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PhoneValidator());
        }

        /// <summary>
        /// 验证URL地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UrlValidator());
        }
    }
}
