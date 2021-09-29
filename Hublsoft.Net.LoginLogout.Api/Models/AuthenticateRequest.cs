using System.ComponentModel.DataAnnotations;

namespace Hublsoft.Net.LoginLogout.Api.Models
{
    public class AuthenticateRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [Required]
        [RegularExpression(@"^[$]2[abxy]?[$](?:0[4-9]|[12][0-9]|3[01])[$][./0-9a-zA-Z]{53}$", ErrorMessage = "Invalid hashed password")]
        public string Password { get; set; }
    }
}