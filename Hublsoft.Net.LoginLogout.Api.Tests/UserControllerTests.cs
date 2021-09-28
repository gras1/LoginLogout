using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private readonly string _dbConnectionString;
        private static string IncorrectHashedPassword = "$2a$12$p3gGhR1iGQDPaUF4vygnwOEJsvt/r5xo7hgyA6hxJW9DZvdX/JXUK";
        private static string ValidHashedPassword = "$2a$12$jZFwONJ.lkBoR/WEv9vDeuXvZ0q3AvOoeiXtp.N5beBENapOE/Gja";

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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var config = builder.Build();
            _dbConnectionString = config.GetSection("DatabaseOptions")["ConnectionString"];
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
            var payload = "{\"emailaddress\":\"test@test.com\",\"password\":\"" + ValidHashedPassword + "\"}";
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
            var payload = "{\"emailaddress\":\"\",\"password\":\"" + ValidHashedPassword + "\"}";
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

        [When(@"I post a request to authenticate with a valid email address and a valid but incorrect password")]
        public async Task I_post_a_request_to_authenticate_with_a_valid_email_address_and_a_valid_but_incorrect_password()
        {
            var payload = "{\"emailaddress\":\"test@test.com\",\"password\":\"" + IncorrectHashedPassword + "\"}";
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            _response = null;

            try
            {
                _response = await _client.PostAsync("/api/v1.0/user/authenticate", content);
            }
            catch { }
        }

        [And(@"The number of failed login attempts for user (\d+) is (\d+) and status is (\d+)")]
        public async Task The_number_of_failed_login_attempts_for_user_z_is_z_and_status_is_z(int userId, int failedLoginAttempts, int status)
        {
            using (var con = new MySqlConnection(_dbConnectionString))
            {
                await con.OpenAsync();

                var stm = $"DELETE FROM RegisteredUserAudits WHERE Id > 2 ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                
                stm = $"DELETE FROM RegisteredUsers WHERE Id > 1 ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                
                stm = $"UPDATE RegisteredUsers SET `Status` = {status} , LockedOutUntilDateTime = NULL , FailedLoginAttempts = {failedLoginAttempts} WHERE Id = {userId} ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                await con.CloseAsync();
            }
        }

        [Then(@"I expect to receive a (\d+) response")]
        public void I_expect_to_receive_a_z_response(int httpStatusCode)
        {
            ((int)_response.StatusCode).Should().Be(httpStatusCode);
        }

        [And(@"The number of failed login attempts for user (\d+) is (\d+)")]
        public async Task The_number_of_failed_login_attempts_for_user_z_is_z(int userId, int numberOfFailedLoginAttempts)
        {
            var actualNumberOfFailedLoginAttempts = 0;

            using (var con = new MySqlConnection(_dbConnectionString))
            {
                await con.OpenAsync();

                var stm = $"SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ; SELECT FailedLoginAttempts FROM RegisteredUsers WHERE Id = {userId} LIMIT 1 ; SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE READ ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            actualNumberOfFailedLoginAttempts = rdr.GetInt32(rdr.GetOrdinal("FailedLoginAttempts"));
                        }

                        await rdr.CloseAsync();
                    }
                }

                await con.CloseAsync();
            }
            
            actualNumberOfFailedLoginAttempts.Should().Be(numberOfFailedLoginAttempts);
        }

        [And(@"There is (\d+) failed login attempt audit record for user (\d+) where before status is (\d+) and after status is (\d+)")]
        public async Task The_number_of_invalid_login_attempts_is_z(int numberOfFailedLoginAttemptAuditRecords, int userId, byte beforeStatus, byte afterStatus)
        {
            var actualNumberOfFailedLoginAttemptAuditRecords = 0;
            byte actualBeforeStatus = 0;
            byte actualAfterStatus = 0;

            using (var con = new MySqlConnection(_dbConnectionString))
            {
                await con.OpenAsync();

                var stm = $"SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ; SELECT COUNT(Id) AS NumberOfFailedLoginAttemptAuditRecords, StatusBefore, StatusAfter FROM RegisteredUserAudits WHERE RegisteredUserId = {userId} AND Activity = 'Failed login attempt' LIMIT 1 ; SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE READ ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            actualNumberOfFailedLoginAttemptAuditRecords = rdr.GetInt32(rdr.GetOrdinal("NumberOfFailedLoginAttemptAuditRecords"));
                            actualBeforeStatus = rdr.GetByte(rdr.GetOrdinal("StatusBefore"));
                            actualAfterStatus = rdr.GetByte(rdr.GetOrdinal("StatusAfter"));
                        }

                        await rdr.CloseAsync();
                    }
                }

                await con.CloseAsync();
            }
            
            actualNumberOfFailedLoginAttemptAuditRecords.Should().Be(numberOfFailedLoginAttemptAuditRecords);
            actualBeforeStatus.Should().Be(beforeStatus);
            actualAfterStatus.Should().Be(afterStatus);
        }

        /*
        NB: used https://bcrypt-generator.com/ to hash the valid password "test" and incorrect password "password"
        TODO add unit tests for BLL  
        TODO add next Gherkin test that takes user over the 3 failed login threshold, disables their account, with corresponding audit record
        */
    }
}
