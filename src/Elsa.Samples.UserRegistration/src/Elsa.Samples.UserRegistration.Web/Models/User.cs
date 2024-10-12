namespace Elsa.Samples.UserRegistration.Web.Models
{
    public class User
    {
        public string Id           { get; set; }
        public string Name         { get; set; }
        public string Email        { get; set; }
        public string Password     { get; set; }
        public string PasswordSalt { get; set; }
        public bool   IsActive     { get; set; }
    }
}