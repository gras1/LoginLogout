using System;
using System.Threading.Tasks;
using Hublsoft.Net.LoginLogout.DataAccess;

namespace Hublsoft.Net.LoginLogout.Bll
{
    public class UserManager : IUserManager
    {
        private readonly IRegisteredUsersRepository _registeredUsersRepo;
        private readonly IRegisteredUserAuditsRepository _registeredUserAuditsRepo;

        public UserManager(IRegisteredUsersRepository registeredUsersRepo, IRegisteredUserAuditsRepository registeredUserAuditsRepo)
        {
            _registeredUsersRepo = registeredUsersRepo;
            _registeredUserAuditsRepo = registeredUserAuditsRepo;
        }

        public async Task<Guid> AuthenticateUserAsync(string emailAddress, string password)
        {
            var id = await _registeredUsersRepo.GetIdByEmailAddressAsync(emailAddress);
            if (id == 0)
            {
                return Guid.Empty;
            }
            var userAccountDetails = await _registeredUsersRepo.GetUserAccountDetailsAsync(id, password);
            if (userAccountDetails == null)
            {
                await _registeredUsersRepo.IncrementFailedLoginAttemptsAsync(id);
                await _registeredUserAuditsRepo.AddFailedLoginAttemptAuditRecordAsync(id);
                return Guid.Empty;
            }
            return userAccountDetails.PublicId;
        }
    }
}
