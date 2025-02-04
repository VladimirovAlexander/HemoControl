using Account.Model;

namespace Account.Interfaces
{
    public interface ITokenService
    {   
        public string GenerateToken(User user);
    }
}
