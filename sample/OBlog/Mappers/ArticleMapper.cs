using Nelibur.ObjectMapper;
using OBlog.Infrastructure.ObjectMapper;
using System.Collections.Generic;
using OBlog.Domain;
using OBlog.ResultModels;
using OBlog.ViewModels;

namespace OBlog.Mappers
{
    public class ArticleMapper : IObjectMapper
    {
        public void Mapper()
        {
            TinyMapper.Bind<ArticleCreateModel, Article>();
            TinyMapper.Bind<List<Article>, List<ArticleQueryItemModel>>();
        }
    }
}
