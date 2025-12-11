using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EatWork.Mobile.Utils
{
    public static class Cryptography
    {
        private static readonly string encryptionKey_ = "e@workP@$$w0rD";
        private static Rfc2898DeriveBytes pdb_;

        public static string Encrypt(this string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                var bytes = Encoding.Unicode.GetBytes(input);

                using (var encryptor = Aes.Create())
                {
                    pdb_ = new Rfc2898DeriveBytes(encryptionKey_,
                        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.BlockSize = 128;
                    encryptor.Key = pdb_.GetBytes(32);
                    encryptor.IV = pdb_.GetBytes(16);

                    using (var memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            cryptoStream.Write(bytes, 0, bytes.Length);

                        input = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }

            return input;
        }

        public static string Decrypt(this string cipherText)
        {
            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                var cipherBytes = Convert.FromBase64String(cipherText);

                using (var encryptor = Aes.Create())
                {
                    pdb_ = new Rfc2898DeriveBytes(encryptionKey_,
                        new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

                    encryptor.Key = pdb_.GetBytes(32);
                    encryptor.IV = pdb_.GetBytes(16);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }

                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }

            return cipherText;
        }

        public static bool TryParseJson<T>(this string obj, out T result)
        {
            try
            {
                // Validate missing fields of object
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Error;

                result = JsonConvert.DeserializeObject<T>(obj, settings);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }
    }
}