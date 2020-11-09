using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Common
{
    public static class ConvertExtension
    {
        private static IHttpContextAccessor _accessor;
        public static HttpContext Current => _accessor.HttpContext;
        internal static void ConvertExtensionConfigure(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            _accessor = httpContextAccessor;
        }
        public static double ToTimestamp(this DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalMilliseconds;
        }
        public static string ToImageUrl(this string src)
        {
            if (src == null)
                return null;
            //HttpContext.Request.s
            //var url = "https://" + HttpContext.Current.Request.Host + "/";
            var url = Current.Request.Scheme + "://" + Current.Request.Host + "/";
            src = src.Replace(@"\", @"/");
            var imageUrl = $"{url}{src}";
            return imageUrl;
        }
    }
}
