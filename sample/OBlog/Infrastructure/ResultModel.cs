using NetSql.Query;
using Newtonsoft.Json;

namespace OBlog.Infrastructure
{
    public class ResultModel
    {
        protected ResultModel() { }

        /// <summary>
        /// 处理是否成功
        /// </summary>
        [JsonIgnore]
        public bool Successful { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code => Successful ? 1 : 0;

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 错误
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 创建成功结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultModel Success(object data = null)
        {
            return new ResultModel
            {
                Successful = true,
                Data = data,
                Msg = "success"
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ResultModel Error(string error = null)
        {
            return new ResultModel
            {
                Msg = error ?? "failed"
            };
        }
    }
}
