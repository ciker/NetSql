using System.Threading.Tasks;
using OBlog.Infrastructure;
using OBlog.ViewModels;

namespace OBlog.IServices
{
    public interface IArticleService
    {
        Task<ResultModel> Query(ArticleQueryModel model);

        Task<ResultModel> Create(ArticleCreateModel model);
    }
}
