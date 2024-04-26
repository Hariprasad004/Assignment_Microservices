namespace Services.AuthAPI.Models
{

    public class BcryptPasswordHash : IPassWordHash
    {
        public bool VerifyPassword(string password, string HashesPwd)
        {
            return BCrypt.Net.BCrypt.Verify(password, HashesPwd);
        }

        public string GetHashedPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
