using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PSBase
{
    internal class SecretsManager : ISecretsManager
    {
        public void Store(string secret, string id)
        {
            var (cipherPath, entropyPath) = GetTokenFilePaths(id);
            var tokenBytes = Encoding.UTF8.GetBytes(secret);

            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(entropy);

            var cipher = ProtectedData.Protect(tokenBytes, entropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(cipherPath, cipher);
            File.WriteAllBytes(entropyPath, entropy);
        }

        public string Retrieve(string id)
        {
            var (cipherPath, entropyPath) = GetTokenFilePaths(id);

            if (!File.Exists(cipherPath) || !File.Exists(entropyPath))
                return string.Empty;

            var cipher = File.ReadAllBytes(cipherPath);
            var entropy = File.ReadAllBytes(entropyPath);

            var resultBytes = ProtectedData.Unprotect(cipher, entropy, DataProtectionScope.CurrentUser);
            var secret = Encoding.UTF8.GetString(resultBytes);

            return secret;
        }

        public bool Exists(string id)
        {
            var (cipherPath, entropyPath) = GetTokenFilePaths(id);
            return File.Exists(cipherPath) && File.Exists(entropyPath);
        }

        private (string cipherPath, string entropyPath) GetTokenFilePaths(string id)
        {
            if (id.IsNullOrEmpty() || id.Length < 10)
                throw new ArgumentNullException(nameof(id), "You must provide a unique id containing at least 10 characters.");

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var hash = id.GetSha1Hash();
            var cipherPath = Path.Combine(appData, $"{hash}-cipher");
            var entropyPath = Path.Combine(appData, $"{hash}-entropy");
            return (cipherPath, entropyPath);
        }
    }
}