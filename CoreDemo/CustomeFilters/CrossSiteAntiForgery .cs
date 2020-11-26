using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.CustomeFilters
{
    public class CrossSiteAntiForgery: ResultFilterAttribute
    {
        public const string Cookie = "XSRF-TOKEN";
        public const string Header = "X-XSRF-TOKEN";

        private IAntiforgery _antiforgery;
        public CrossSiteAntiForgery(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            AntiforgeryTokenSet tokens = _antiforgery.GetAndStoreTokens(context.HttpContext);
            context.HttpContext.Response.Cookies.Append(Cookie, tokens.RequestToken, new CookieOptions() { HttpOnly = false });
        }
    }
}
