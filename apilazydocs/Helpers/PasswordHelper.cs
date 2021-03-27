using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ApiLazyDoc.Helpers
{
    class PasswordHelper
    {
        private const int SaltByteSize = 32;
        private const int HashByteSize = 32;
        private const int Iterations = 4096;

        private static string GetSalt()
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            byte[] b_salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(b_salt);
            return Convert.ToBase64String(b_salt);
        }

        public static string GetPasswordHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Le password est obligatorie");

            string salt = GetSalt();

            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] derived;

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations))
            {
                derived = pbkdf2.GetBytes(HashByteSize);
            }

            return string.Format("{0}:{1}:{2}", Iterations, Convert.ToBase64String(derived), Convert.ToBase64String(saltBytes));
        }

        public static bool VerifyPasswordHash(string password, string hash)
        {
            try
            {
                string[] parts = hash.Split(new char[] { ':' });

                byte[] saltBytes = Convert.FromBase64String(parts[2]);
                byte[] derived;

                int iterations = Convert.ToInt32(parts[0]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    saltBytes,
                    iterations))
                {
                    derived = pbkdf2.GetBytes(HashByteSize);
                }

                string new_hash = string.Format("{0}:{1}:{2}", Iterations, Convert.ToBase64String(derived), Convert.ToBase64String(saltBytes));
                var samePassword = string.Compare(hash, new_hash);
                return samePassword == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
