using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace Hublsoft.Net.LoginLogout.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private List<string> AcceptableApiKeys = new List<string> { "ac3e456d-e83a-a6d3-a456-96b3a4279e0f" };

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var apiKey = context.HttpContext.Request.Headers["X-api-key"];
            if (string.IsNullOrEmpty(apiKey) || !AcceptableApiKeys.Contains(apiKey))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}