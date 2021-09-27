# LoginLogout

## Thought process and steps I took to create the Login/Logout MVP

After cloning the repo from Terminal/Command Line ....  
`dotnet new webapi -f net5.0 -n Hublsoft.Net.LoginLogout.Api`  

this follows a naming convention for solutions/projects/namespaces [company].[technology].[project].[layer]  
then add other projects:  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.Shared`  

this is for the DTOs  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.Bll`  

this is for the business logic layer  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.DataAccess`  

this is for the data access layer  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.Api.Tests`  

test project for the API  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.Bll.Tests`  

test project for the business logic layer  

`dotnet new classlib -f net5.0 -n Hublsoft.Net.LoginLogout.DataAccess.Tests`  

test project for the data access layer  

Now to add references between the projects  

`dotnet add ./Hublsoft.Net.LoginLogout.Bll/Hublsoft.Net.LoginLogout.Bll.csproj reference ./Hublsoft.Net.LoginLogout.DataAccess/Hublsoft.Net.LoginLogout.DataAccess.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Bll/Hublsoft.Net.LoginLogout.Bll.csproj reference ./Hublsoft.Net.LoginLogout.Shared/Hublsoft.Net.LoginLogout.Shared.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Api/Hublsoft.Net.LoginLogout.Api.csproj reference ./Hublsoft.Net.LoginLogout.Bll/Hublsoft.Net.LoginLogout.Bll.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Api/Hublsoft.Net.LoginLogout.Api.csproj reference ./Hublsoft.Net.LoginLogout.Shared/Hublsoft.Net.LoginLogout.Shared.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.DataAccess.Tests/Hublsoft.Net.LoginLogout.DataAccess.Tests.csproj reference ./Hublsoft.Net.LoginLogout.DataAccess/Hublsoft.Net.LoginLogout.DataAccess.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Bll.Tests/Hublsoft.Net.LoginLogout.Bll.Tests.csproj reference ./Hublsoft.Net.LoginLogout.Bll/Hublsoft.Net.LoginLogout.Bll.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Bll.Tests/Hublsoft.Net.LoginLogout.Bll.Tests.csproj reference ./Hublsoft.Net.LoginLogout.Shared/Hublsoft.Net.LoginLogout.Shared.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Api.Tests/Hublsoft.Net.LoginLogout.Api.Tests.csproj reference ./Hublsoft.Net.LoginLogout.Api/Hublsoft.Net.LoginLogout.Api.csproj`
`dotnet add ./Hublsoft.Net.LoginLogout.Api.Tests/Hublsoft.Net.LoginLogout.Api.Tests.csproj reference ./Hublsoft.Net.LoginLogout.Shared/Hublsoft.Net.LoginLogout.Shared.csproj`

Now to create a solution file and add the projects to it 

`dotnet new sln --name Hublsoft.Net.LoginLogout`
`dotnet sln add ./Hublsoft.Net.LoginLogout.Api/Hublsoft.Net.LoginLogout.Api.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.Shared/Hublsoft.Net.LoginLogout.Shared.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.Bll/Hublsoft.Net.LoginLogout.Bll.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.DataAccess/Hublsoft.Net.LoginLogout.DataAccess.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.Api.Tests/Hublsoft.Net.LoginLogout.Api.Tests.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.Bll.Tests/Hublsoft.Net.LoginLogout.Bll.Tests.csproj`
`dotnet sln add ./Hublsoft.Net.LoginLogout.DataAccess.Tests/Hublsoft.Net.LoginLogout.DataAccess.Tests.csproj`

### Api.Tests project

As part of the job spec and during the interview behaviour driven development was discussed so next add the appropriate nuget packages to the Api.Tests project:  

`cd Hublsoft.Net.LoginLogout.Api.Tests`
`dotnet add package Microsoft.NET.Test.Sdk`
`dotnet add package xunit`
`dotnet add package xunit.runner.visualstudio`
`dotnet add package coverlet.collector`
`dotnet add package FluentAssertions`
`dotnet add package Microsoft.AspNetCore.Mvc.Testing`

- coverlet.collector can be used to derive code coverage from tests
- After some research about running BDD tests in .NET 5, the Xunit.Gherkin.Quick package seemed to do this fine without using something a full framework such as Specflow  

`dotnet add package Xunit.Gherkin.Quick`

- Create a folder called `features` under the Hublsoft.Net.LoginLogout.Api.Tests folder  
- Add a file called LoginLogoutSpecs.feature  
- Add the following to Hublsoft.Net.LoginLogout.Api.Tests.csproj:  


    <ItemGroup>
        <None Update="features\LoginLogoutSpecs.feature">
          <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
        </None>
    </ItemGroup>

Add an appsettings.test.json file at the root of the project with the following content:

`{ "AllowedHosts": "*" }`  

This gets the Api.Tests project ready for Gherkin syntax feature tests.  

The first scenario took some time to think about how to word the given .. when .. then. I wanted to test that if an unathorised request was made to the UserController authenticate method (not supplying a custom header token in the request header), then a 401 unauthorised response code is returned. I also wanted to word it in such a way that each part of the given .. when .. then could be re-used in other tests.  

I renamed Class1.cs to UserControllerTests.cs and reference the feature file as an attribute on the class `[FeatureFile("./features/LoginLogoutSpecs.feature")]`. I add logic to the constructor to create a test web application. In the I_am_unauthorised method the Http Client is created but no custom header token is added.  

NB: because the user controller doesn't exist yet in the Api project, when the test is run, a 404 response code so the first test fails. Now to go on and get this first test passing.  

### Api project

Now to get the Api project ready:  

`cd Hublsoft.Net.LoginLogout.Api`
`dotnet add package Microsoft.AspNetCore.Mvc.Versioning`
`dotnet add package Swashbuckle.AspNetCore`
`dotnet add package Swashbuckle.AspNetCore.Annotations`
`dotnet add package Swashbuckle.AspNetCore.Filters`

- Microsoft.AspNetCore.Mvc.Versioning provides full control over API versioning (more than you get out of the box)  
- Swashbuckle is a Swagger generator that builds SwaggerDocument objects directly from your routes, controllers, and models  

In an attempt to get proper versioning and swagger documentation created (from the outset). I added `<GenerateDocumentationFile>true</GenerateDocumentationFile>` to the property group in the Api.csproj file. I added the following code to Startup:

    services.AddApiVersioning(config =>
    {
        config.AssumeDefaultVersionWhenUnspecified = true;
        config.DefaultApiVersion = new ApiVersion(1, 0);
        config.ReportApiVersions = true;
    });
    services.AddSwaggerGen(c => {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
        c.ExampleFilters();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hublsoft.Net.LoginLogout.Api", Version = "v1" });
    });
    services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());`  

I deleted anything to do with the out-of-the-box weather controller.

I created an Authorize attribute to satisfy the requirement of passing a custom header token to all requests to the api. All hard-coded for now but could come out of a json config file, a database or some other storage medium.:

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    
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

I created a new controller called UserController. To this I added the versioning annotations:

    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]

I added the following authentication method:

    /// <summary>
    /// Retrieves the public id for the user given the request contains a valid api-key and a valid email address and password.
    /// </summary>
    /// <response code="200">Returns the public id for the user</response>
    /// <response code="400">If the request is malformed (email address is not supplied or is an empty string, password is not supplied or is an empty string, email address is not properly formatted)</response>
    /// <response code="401">If the request does not contain valid api-key</response>
    /// <response code="403">The user could not be found (incorrect email address and password combination)</response>
    /// <response code="500">An unhandled exception occurs</response>
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        return Ok(System.Guid.NewGuid());
    }

- The xml comments help build the swagger documentation

The Authenticate method has been stubbed out to return a new guid - this will be replaced as more tests are added.

The 4th feature test requires me to implement the database and the data access and bll projects because I need to check it for the number of failed login attempts. This will mean that I need to add some unit tests as well. This all needs to be done from the "bottom upwards".

- Run the script called "Setup and populate LoginLogout database.sql"

To be able to connect to the MySql database run `dotnet add package MySql.Data`