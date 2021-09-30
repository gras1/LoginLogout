using System;
using System.Collections.Generic;
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
            if (registeredUsersRepo == null) {
                throw new ArgumentNullException(nameof(registeredUsersRepo));
            }
            if (registeredUserAuditsRepo == null) {
                throw new ArgumentNullException(nameof(registeredUserAuditsRepo));
            }
            _registeredUsersRepo = registeredUsersRepo;
            _registeredUserAuditsRepo = registeredUserAuditsRepo;
        }

        public async Task<Guid> AuthenticateUserAsync(string emailAddress, string password)
        {
            if (string.IsNullOrEmpty(emailAddress)) {
                throw new ArgumentException(nameof(emailAddress));
            }
            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentException(nameof(password));
            }
            var id = await _registeredUsersRepo.GetIdByEmailAddressAsync(emailAddress);
            if (id == 0)
            {
                return Guid.Empty;
            }
            var userAccountDetails = await _registeredUsersRepo.GetUserAccountDetailsAsync(id, password);
            if (userAccountDetails == null)
            {
                var tasks = new List<Task>();
                tasks.Add(_registeredUsersRepo.IncrementFailedLoginAttemptsAsync(id));
                tasks.Add(_registeredUserAuditsRepo.AddFailedLoginAttemptAuditRecordAsync(id));
                await Task.WhenAll(tasks);

                var loginAttempts = await _registeredUsersRepo.GetFailedLoginAttempsAsync(id);

                if (loginAttempts == 3)
                {
                    await _registeredUsersRepo.LockAccount(id);
                }

                return Guid.Empty;
            }
            return userAccountDetails.PublicId;
        }
    }
}
