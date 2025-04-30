using Microsoft.AspNetCore.Identity;

namespace Account.Model
{
    public class User : IdentityUser
    {
        public string PolicyNumber { get; set; }
    }
}
