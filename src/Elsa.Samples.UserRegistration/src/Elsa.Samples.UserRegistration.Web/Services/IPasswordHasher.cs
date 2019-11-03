using Elsa.Samples.UserRegistration.Web.Models;

namespace Elsa.Samples.UserRegistration.Web.Services
{
    public interface IPasswordHasher
    {
        HashedPassword HashPassword(string password);
        HashedPassword HashPassword(string password, byte[] salt);
    }
}