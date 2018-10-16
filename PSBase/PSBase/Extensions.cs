using System;
using System.IO;
using System.Security.Cryptography;

namespace PSBase
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static T ToEnum<T>(this string value) where T : Enum => (T)Enum.Parse(typeof(T), value, true);

        public static bool CanBeParsedTo<T>(this string value) where T : struct, Enum => Enum.TryParse<T>(value, true, out _);

        public static string GetSha1Hash(this FileInfo file)
        {
            using (var sha1 = SHA1.Create())
            {
                using (var stream = file.OpenRead())
                {
                    var hash = sha1.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string GetMd5Hash(this FileInfo file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = file.OpenRead())
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}