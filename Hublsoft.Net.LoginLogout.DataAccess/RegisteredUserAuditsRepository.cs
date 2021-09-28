using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    public class RegisteredUserAuditsRepository : BaseRepository, IRegisteredUserAuditsRepository
    {
        public RegisteredUserAuditsRepository(IOptions<DatabaseOptions> databaseOptions) : base(databaseOptions) { }
        
        public async Task AddFailedLoginAttemptAuditRecordAsync(int id)
        {
            using (var con = new MySqlConnection(base.ConnectionString))
            {
                await con.OpenAsync();

                string stm = $"INSERT INTO RegisteredUserAudits (`TimeStamp`, RegisteredUserId, Activity, StatusBefore, StatusAfter) VALUES ('{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}', {id}, 'Failed login attempt', 1, 1) ;";

                using (var cmd = new MySqlCommand(stm, con))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                await con.CloseAsync();
            }
        }
    }
}