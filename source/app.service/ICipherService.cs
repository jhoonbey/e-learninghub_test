namespace app.service
{
    public interface ICipherService
    {
        string Encrypt(string key, string input);
        string Decrypt(string key, string input);
    }
}
