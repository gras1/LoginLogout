using System;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    public class UserAccountDetails
    {
        public Guid PublicId {get;set;}

        public byte Status {get;set;}

        public DateTime? LockedOutUntilDateTime {get;set;}

        public byte FailedLoginAttempts {get;set;}
    }
}