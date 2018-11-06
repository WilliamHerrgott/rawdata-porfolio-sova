using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace WebService.Services {
    public class PasswordService {
        private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        public static string GenerateSalt(int size) {
            var buffer = new byte[size];
            _rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        public static string HashPassword(string pwd, string salt, int size) {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                pwd,
                Encoding.UTF8.GetBytes(salt),
                KeyDerivationPrf.HMACSHA256,
                10000,
                size));
        }
    }
}