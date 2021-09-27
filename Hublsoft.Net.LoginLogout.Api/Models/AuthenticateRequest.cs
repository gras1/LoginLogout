using System.ComponentModel.DataAnnotations;

namespace Hublsoft.Net.LoginLogout.Api.Models
{
    #pragma warning disable 1591
    public class AuthenticateRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
    #pragma warning restore 1591
}