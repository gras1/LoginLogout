using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Gherkin.Quick;

namespace Hublsoft.Net.LoginLogout.Api.Tests
{
    [ExcludeFromCodeCoverage]
    [FeatureFile("./features/LoginLogoutSpecs.feature")]
    public sealed class UserControllerTests : Feature
    {
        private IHost _host;
        private HttpClient _client;
        private HttpResponseMessage _response;

        public UserControllerTests()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.test.json");
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer()
                           .ConfigureAppConfiguration((context, config) =>
                           {
                               config.AddJsonFile(configPath, optional: true);
                               config.AddEnvironmentVariables();
                           })
                           .UseStartup<Hublsoft.Net.LoginLogout.Api.Startup>()
                           .UseEnvironment("test");
                });
            _host = hostBuilder.Start();
        }

        [Given(@"I am unauthorised")]
        public void I_am_unauthorised()
        {
            _client = _host.GetTestClient();
            _client.DefaultRequestHeaders.Clear();
        }

        [Given(@"I am authorised")]
        public void I_am_authorised()
        {
            _client = _host.GetTestClient();
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("X-api-key", "ac3e456d-e83a-a6d3-a456-96b3a4279e0f");
        }

        [When(@"I post a request to authenticate with a valid email address and password")]
        public async Task I_post_a_request_to_authenticate_with_a_valid_email_address_and_password()
        {
            var payload = "{\"emailaddress\":\"test@test.com\",\"password\":\"test\"}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            _response = null;

            try
            {
                _response = await _client.PostAsync("/api/v1.0/user/authenticate", content);
            }
            catch { }
        }

        [When(@"I post a request to authenticate with an invalid email address and valid password")]
        public async Task I_post_a_request_to_authenticate_with_an_invalid_email_address_and_valid_password()
        {
            var payload = "{\"emailaddress\":\"\",\"password\":\"test\"}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            _response = null;

            try
            {
                _response = await _client.PostAsync("/api/v1.0/user/authenticate", content);
            }
            catch { }
        }

        [When(@"I post a request to authenticate with a valid email address and an invalid password")]
        public async Task I_post_a_request_to_authenticate_with_a_valid_email_address_and_an_invalid_password()
        {
            var payload = "{\"emailaddress\":\"test@test.com\",\"password\":\"\"}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            _response = null;

            try
            {
                _response = await _client.PostAsync("/api/v1.0/user/authenticate", content);
            }
            catch { }
        }

        [Then(@"I expect to receive a (\d+) response")]
        public void I_expect_to_receive_a_z_response(int httpStatusCode)
        {
            ((int)_response.StatusCode).Should().Be(httpStatusCode);
        }
    }
}
