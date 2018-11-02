using FluentValidation;
using OBlog.ViewModels;

namespace OBlog.Validators
{
    public class ArticleCreateValidator : AbstractValidator<ArticleCreateModel>
    {
        public ArticleCreateValidator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("请输入文字标题");
        }
    }
}