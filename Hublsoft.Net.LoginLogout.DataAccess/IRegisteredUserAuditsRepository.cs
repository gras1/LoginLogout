using System.Threading.Tasks;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    public interface IRegisteredUserAuditsRepository
    {
        Task AddFailedLoginAttemptAuditRecordAsync(int id);
    }
}