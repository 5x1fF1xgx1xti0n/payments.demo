using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payments.API.Hash
{
    public class Hasher : IHasher
    {
        private const int saltSize = 16;
        private const int keySize = 32;
        private readonly byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };
        private const string key = "ABCD1EFGHI23456";

        public string Encrypt(string data)
        {
            var bytes = Encoding.Unicode.GetBytes(data);

            using(Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
                encryptor.Key = pdb.GetBytes(keySize);
                encryptor.IV = pdb.GetBytes(saltSize);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }

                    data = Convert.ToBase64String(ms.ToArray());
                }
            }

            return data;
        }

        public string Decrypt(string data)
        {
            var bytes = Convert.FromBase64String(data);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
                encryptor.Key = pdb.GetBytes(keySize);
                encryptor.IV = pdb.GetBytes(saltSize);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.Close();
                    }

                    data = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return data;
        }
    }
}
