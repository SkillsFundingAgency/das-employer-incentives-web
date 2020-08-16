using System;
using System.IO;
using System.Security.Cryptography;

namespace SFA.DAS.EmployerIncentives.Web.Services.Security
{
    public class DataEncryptionService : IDataEncryptionService
    {
        private readonly string _key;

        public DataEncryptionService(string key)
        {
            _key = key;
        }

        public string Encrypt(string raw)
        {
            byte[] encrypted;
            string iv;

            using (var aes = GetAes())
            {
                iv = Convert.ToBase64String(aes.IV);
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using var ms = new MemoryStream();
                var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using (var sw = new StreamWriter(cs)) sw.Write(raw);

                encrypted = ms.ToArray();
            }

            return iv + Convert.ToBase64String(encrypted);
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