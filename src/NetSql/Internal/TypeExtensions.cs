using System.Reflection;

namespace NetSql.Internal
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// 判断属性是否是静态的
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsStatic(this PropertyInfo property) => (property.GetMethod ?? property.SetMethod).IsStatic;
    }
}
