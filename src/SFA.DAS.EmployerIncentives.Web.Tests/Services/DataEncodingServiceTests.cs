using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services;
using System;
using System.IO;
using System.Security.Cryptography;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services
{
    public class DataEncodingServiceTests
    {
        private const string TestAesKey = "P5T1NjQ1xqo1FgFM8RG+Yg==";
        private const string OriginalRaw = "DW5T8V|P0007983|Angus Sinclair|07365363563|ag@gmail.com|Harold Shipman|07356356353|hs@gmail.com|Fred West|07265625622|fs@gmail.com|Ian Brady|01367363783|ib@gmail.com|apps=53";

        [Test]
        public void CanEncryptStringDataPrefixedWithRandomIv()
        {
            // Arrange
            var service1 = new DataEncryptionService(TestAesKey);

            // Act
            var encrypted = service1.Encrypt(OriginalRaw);

            // Assert
            Decrypt(encrypted).Should().Be(OriginalRaw);
        }

        private static string Decrypt(string ivAndData)
        {
            string decrypted;
            var iv = Convert.FromBase64String(ivAndData.Substring(0, 24));
            var cipher = Convert.FromBase64String(ivAndData.Substring(24));

            using (var aes = new AesManaged())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.Key = Convert.FromBase64String(TestAesKey);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.IV = iv;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using var ms = new MemoryStream(cipher);
                var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                decrypted = sr.ReadToEnd();
            }

            return decrypted;
        }
    }
}
