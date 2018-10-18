namespace PSBase
{
    public interface ITokenManager
    {
        void Store(string token, string key);
        string Retrieve(string key);
    }
}