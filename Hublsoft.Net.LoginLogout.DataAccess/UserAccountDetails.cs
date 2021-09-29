using System;
using System.Diagnostics.CodeAnalysis;

namespace Hublsoft.Net.LoginLogout.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class UserAccountDetails
    {
        public Guid PublicId {get;set;}

        public byte Status {get;set;}

        public DateTime? LockedOutUntilDateTime {get;set;}

        public byte FailedLoginAttempts {get;set;}
    }
}