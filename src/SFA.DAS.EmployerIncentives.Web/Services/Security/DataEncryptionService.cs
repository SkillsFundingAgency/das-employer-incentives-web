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
            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = Convert.FromBase64String(_key);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;            
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var encryptedData = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            var data = aes.IV.Concat(encryptedData).ToArray();

            return Convert.ToBase64String(data);
        }
    }
}