using System;

namespace Elsa.Samples.UserRegistration.Web.Models
{
    public class HashedPassword
    {
        public HashedPassword(byte[] hashed, byte[] salt)
        {
            Hashed = Convert.ToBase64String(hashed);
            Salt = Convert.ToBase64String(salt);
        }
        
        public HashedPassword(string hashed, string salt)
        {
            Hashed = hashed;
            Salt = salt;
        }
        
        public string Salt { get; }
        public string Hashed { get; }
    }
}