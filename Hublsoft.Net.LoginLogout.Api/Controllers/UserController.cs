using Microsoft.AspNetCore.Mvc;
using Hublsoft.Net.LoginLogout.Api.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hublsoft.Net.LoginLogout.Bll;

namespace Hublsoft.Net.LoginLogout.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    #pragma warning disable 1591
    public class UserController : ControllerBase
    #pragma warning restore 1591
    {
        private readonly IUserManager _userManager;

        #pragma warning disable 1591
        public UserController(IUserManager userManager)
        #pragma warning restore 1591
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves the public id for the user given the request contains a valid api-key and a valid email address and password.
        /// </summary>
        /// <response code="200">Returns the public id for the user</response>
        /// <response code="400">If the request is malformed (email address is not supplied or is an empty string, password is not supplied or is an empty string, email address is not properly formatted)</response>
        /// <response code="401">If the request does not contain valid api-key</response>
        /// <response code="403">The user could not be found, incorrect email address and password combination</response>
        /// <response code="500">An unhandled exception occurs</response>
        [HttpPost("authenticate")]
        public async Task<ObjectResult> AuthenticateAsync(AuthenticateRequest model)
        {
            var publicId = await _userManager.AuthenticateUserAsync(model.EmailAddress, model.Password);
            if (publicId == Guid.Empty)
            {
                return new ForbiddenObjectResult("The user could not be found, incorrect email address and password combination");
            }
            return new OkObjectResult(publicId);
        }
    }
}