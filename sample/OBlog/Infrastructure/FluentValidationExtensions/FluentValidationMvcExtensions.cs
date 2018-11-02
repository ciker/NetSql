using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using OBlog.Validators;

namespace OBlog.Infrastructure.FluentValidationExtensions
{
    public static class FluentValidationMvcExtensions
    {
        /// <summary>
        /// 添加FluentValidation
        /// </summary>
        /// <param name="mvcBuilder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddMyFluentValidation(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<ArticleCreateValidator>();
            });

            //当一个验证失败时，后续的验证不再执行
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            return mvcBuilder;
        }
    }
}
