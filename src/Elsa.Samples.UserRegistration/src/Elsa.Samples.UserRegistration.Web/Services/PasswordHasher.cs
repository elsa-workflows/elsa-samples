using System.Security.Cryptography;
using Elsa.Samples.UserRegistration.Web.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Elsa.Samples.UserRegistration.Web.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public HashedPassword HashPassword(string password)
        {
            // Generate a 128-bit salt using a secure PRNG.
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return HashPassword(password, salt);
        }
        
        public HashedPassword HashPassword(string password, byte[] salt)
        {
            // Derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            var hashed = KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);
            
            return new HashedPassword(hashed, salt);
        }
    }
}