using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    public class RegisteredUsersRepository : BaseRepository, IRegisteredUsersRepository
    {
        public RegisteredUsersRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions) { }

        public async Task<int> GetIdByEmailAddressAsync(string emailAddress)
        {
            var id = 0;

            using (var con = new MySqlConnection(base.ConnectionString))
            {
                await con.OpenAsync();

                string stm = $"SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ; SELECT Id FROM RegisteredUsers WHERE EmailAddress = '{emailAddress}' LIMIT 1 ; SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE READ ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            id = rdr.GetInt32(rdr.GetOrdinal("Id"));
                        }

                        await rdr.CloseAsync();
                    }
                }

                await con.CloseAsync();
            }
            return id;
        }

        public async Task<UserAccountDetails> GetUserAccountDetailsAsync(int id, string password)
        {
            UserAccountDetails accountDetails = null;

            using (var con = new MySqlConnection(base.ConnectionString))
            {
                await con.OpenAsync();

                string stm = $"SET SESSION TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ; SELECT PublicId, `Status`, LockedOutUntilDateTime, FailedLoginAttempts FROM RegisteredUsers WHERE Id = {id} AND HashedPassword = '{password}' LIMIT 1 ; SET SESSION TRANSACTION ISOLATION LEVEL REPEATABLE READ ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    using (var rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (rdr.Read())
                        {
                            accountDetails = new UserAccountDetails
                            {
                                PublicId = new Guid((byte[])rdr["PublicId"]),
                                Status = (byte)rdr["Status"],
                                LockedOutUntilDateTime = rdr.IsDBNull(rdr.GetOrdinal("LockedOutUntilDateTime")) ? null : rdr.GetDateTime(rdr.GetOrdinal("LockedOutUntilDateTime")),
                                FailedLoginAttempts = (byte)rdr["FailedLoginAttempts"]
                            };
                        }

                        await rdr.CloseAsync();
                    }
                }

                await con.CloseAsync();
            }
            return accountDetails;
        }
        
        public async Task IncrementFailedLoginAttemptsAsync(int id)
        {
            using (var con = new MySqlConnection(base.ConnectionString))
            {
                await con.OpenAsync();

                string stm = $"UPDATE RegisteredUsers SET FailedLoginAttempts = FailedLoginAttempts + 1 WHERE Id = {id} ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                await con.CloseAsync();
            }
        }
    }
}