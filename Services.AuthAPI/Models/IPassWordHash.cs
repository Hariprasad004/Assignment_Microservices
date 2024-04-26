namespace Services.AuthAPI.Models
{
    public interface IPassWordHash
    {
        public string GetHashedPassword(string password);
        public bool VerifyPassword(string password, string HashesPwd);
    }
}
