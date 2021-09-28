using System;
using System.Threading.Tasks;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    public interface IRegisteredUsersRepository
    {
        Task<int> GetIdByEmailAddressAsync(string emailAddress);

        Task<UserAccountDetails> GetUserAccountDetailsAsync(int id, string password);

        Task IncrementFailedLoginAttemptsAsync(int id);
    }
}