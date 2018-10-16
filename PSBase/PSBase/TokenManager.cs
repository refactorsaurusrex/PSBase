using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PSBase
{
    public class TokenManager : ITokenManager
    {
        private readonly string _appData;

        public TokenManager() => _appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public void Store(string token, string key)
        {
            var (cipherName, entropyName) = GetTokenFileNames(key);
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(entropy);

            var cipher = ProtectedData.Protect(tokenBytes, entropy, DataProtectionScope.CurrentUser);

            var cipherPath = Path.Combine(_appData, cipherName);
            var entropyPath = Path.Combine(_appData, entropyName);

            File.WriteAllBytes(cipherPath, cipher);
            File.WriteAllBytes(entropyPath, entropy);
        }

        public string Retrieve(string key)
        {
            var (cipherName, entropyName) = GetTokenFileNames(key);

            var cipherPath = Path.Combine(_appData, cipherName);
            var entropyPath = Path.Combine(_appData, entropyName);

            if (!File.Exists(cipherPath) || !File.Exists(entropyPath))
                return string.Empty;

            var cipher = File.ReadAllBytes(cipherPath);
            var entropy = File.ReadAllBytes(entropyPath);

            var resultBytes = ProtectedData.Unprotect(cipher, entropy, DataProtectionScope.CurrentUser);
            var token = Encoding.UTF8.GetString(resultBytes);

            return token;
        }

#warning Need to refactor 'key'. It used to be a proprietary enum
        private (string cipherName, string entropyName) GetTokenFileNames(string key)
        {
            if (key.IsNullOrEmpty())
                throw new ArgumentException("'None' is not a valid token key.", nameof(key));

            var lowerkey = key.ToLowerInvariant();
#warning These strings need to be changed
            return ($"build-cmdlets-{lowerkey}-cipher", $"build-cmdlets-{lowerkey}-entropy");
        }
    }
}