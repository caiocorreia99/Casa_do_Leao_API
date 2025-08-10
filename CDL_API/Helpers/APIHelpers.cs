using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace CDL.Api.Helpers
{
    public class APIHelper
    {

        private static Regex MailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,})+)$");

        public static string EncryptSHA256(string plainText)
        {

            byte[] data = Encoding.ASCII.GetBytes(plainText);
            data = new SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }

        public static string GenerateRandomPassword(int len = 8, string cypherPass = null)
        {
            if (len < 2) len = 2;

            if (string.IsNullOrEmpty(cypherPass))
            {
                throw new InvalidOperationException("CypherPass was not passed");
            }

            string baseString = Guid.NewGuid().ToString("d");
            string generatedString = EncryptAES(baseString, cypherPass);

            return generatedString[..len];
        }

        public static bool ValidMail(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return false;
            }

            return MailRegex.Match(mail).Success;
        }

        public static string EncryptAES(string plainText, string passPhrase)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(passPhrase);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new();
                using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new(cryptoStream))
                {
                    streamWriter.Write(plainText);
                }

                array = memoryStream.ToArray();
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptAES(string cipherText, string passPhrase)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(passPhrase);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            return streamReader.ReadToEnd();
        }

        public static string ToBase64(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(string text64)
        {
            if (string.IsNullOrEmpty(text64))
            {
                return null;
            }

            var bytes = Convert.FromBase64String(text64);
            return Encoding.UTF8.GetString(bytes);
        }

    }
}
