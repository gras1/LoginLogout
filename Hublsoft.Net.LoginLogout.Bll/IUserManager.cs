using System;
using System.Threading.Tasks;

namespace Hublsoft.Net.LoginLogout.Bll
{
    public interface IUserManager
    {
        Task<Guid> AuthenticateUserAsync(string emailAddress, string password);
    }
}