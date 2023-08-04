
using System.Diagnostics.CodeAnalysis;

namespace CMS.AdminComponents
{
    public class AjaxOption
    {
        /// <summary>
        /// 获取/设置 要上传的参数类
        /// </summary>
        [NotNull]
        public object? Data { get; set; }

        /// <summary>
        /// 获取/设置 传输方式，默认为POST
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// 获取/设置 请求的URL
        /// </summary>
        [NotNull]
        public string? Url { get; set; }
    }
}
