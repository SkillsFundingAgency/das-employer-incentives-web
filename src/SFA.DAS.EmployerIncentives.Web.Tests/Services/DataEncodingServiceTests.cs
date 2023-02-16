using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerIncentives.Web.Services.Security;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace SFA.DAS.EmployerIncentives.Web.Tests.Services
{
    public class DataEncodingServiceTests
    {
        private const string TestAesKey = "P5T1NjQ1xqo1FgFM8RG+Yg==";
        private const string OriginalRaw = "DW5T8V|P0007983|Angus Sinclair|07365363563|ag@gmail.com";

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
            var bytes = Convert.FromBase64String(ivAndData);
            var iv = bytes.Take(16).ToArray();

            using var aes = Aes.Create();
            aes.KeySize = 128;
            aes.BlockSize = 128;
            aes.Key = Convert.FromBase64String(TestAesKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var rawData = decryptor.TransformFinalBlock(bytes, 16, bytes.Length - 16);

            return System.Text.Encoding.UTF8.GetString(rawData);
        }
    }
}
