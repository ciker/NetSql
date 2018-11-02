using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nelibur.ObjectMapper;
using OBlog.Domain;
using OBlog.Domain.Repositories;
using OBlog.Infrastructure;
using OBlog.IServices;
using OBlog.ResultModels;
using OBlog.ViewModels;

namespace OBlog.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<ResultModel> Create(ArticleCreateModel model)
        {
            var article = TinyMapper.Map<Article>(model);
            if (await _articleRepository.AddAsync(article))
            {
                return ResultModel.Success(article.Id);
            }

            return ResultModel.Error();
        }

        public async Task<ResultModel> Query(ArticleQueryModel model)
        {
            var result = new ArticleQueryResultModel();
            var paging = model.Paging();
            var list = await _articleRepository.Query(model.Title, paging);
            if (list.Any())
            {
                result.Total = paging.TotalCount;
                result.Rows = TinyMapper.Map<List<ArticleQueryItemModel>>(list);
            }
            return ResultModel.Success(result);
        }
    }
}
