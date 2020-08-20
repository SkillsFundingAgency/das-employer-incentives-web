using System;
using System.Linq;
using System.Security.Cryptography;

namespace SFA.DAS.EmployerIncentives.Web.Services.Security
{
    public class DataEncryptionService : IDataEncryptionService
    {
        private readonly string _key;

        public DataEncryptionService(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("DataEncryptionServiceKey is not configured.");
            _key = key;
        }

        public string Encrypt(string raw)
        {
            using var aes = GetAes();
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var encryptedData = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            var data = aes.IV.Concat(encryptedData).ToArray();

            return Convert.ToBase64String(data);
        }

        private AesManaged GetAes()
        {
#pragma warning disable S5542 // Encryption algorithms should be used with secure mode and padding scheme
            return new AesManaged
            {
                KeySize = 128,
                BlockSize = 128,
                Key = Convert.FromBase64String(_key),
                Mode = CipherMode.CBC, // TODO: Review this (see https://docs.microsoft.com/en-us/dotnet/standard/security/vulnerabilities-cbc-mode)
                Padding = PaddingMode.PKCS7
            };
#pragma warning restore S5542 // Encryption algorithms should be used with secure mode and padding scheme
        }
    }
}