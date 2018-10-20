namespace PSBase
{
    public interface ISecretsManager
    {
        void Store(string secret, string id);
        string Retrieve(string id);
        bool Exists(string id);
    }
}