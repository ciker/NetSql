using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OBlog.Infrastructure;
using OBlog.IServices;
using OBlog.ViewModels;

namespace OBlog.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ArticleController : BaseController
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<ResultModel> Query([FromQuery]ArticleQueryModel model)
        {
            return _articleService.Query(model);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ResultModel> Create([FromBody]ArticleCreateModel model)
        {
            return _articleService.Create(model);
        }
    }
}