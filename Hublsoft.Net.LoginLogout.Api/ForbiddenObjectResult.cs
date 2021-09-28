using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Hublsoft.Net.LoginLogout.Api
{
    #pragma warning disable 1591
    [ExcludeFromCodeCoverage]
    public class ForbiddenObjectResult : ObjectResult
    {
        public ForbiddenObjectResult(object value) : base(value)
        {
            StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
    #pragma warning restore 1591
}
