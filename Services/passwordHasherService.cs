using System.Security.Cryptography;
using System.Text;

namespace usersDemo.Services
{
    public class passwordHasherService
    {
        private const int SaltSize = 16; // ขนาดของเกล็ด (salt) ในไบต์

        // เมธอดสำหรับการเข้ารหัสรหัสผ่าน
        public static (string Hash, string Salt) HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);

                string salt = Convert.ToBase64String(saltBytes);
                string hash = ComputeHash(password, salt);

                return (hash, salt);
            }
        }

        // เมธอดสำหรับตรวจสอบรหัสผ่าน
        public static bool VerifyPassword(string password, string salt, string hash)
        {
            string newHash = ComputeHash(password, salt);
            return newHash == hash;
        }

        private static string ComputeHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltBytes = Convert.FromBase64String(salt);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

                Array.Copy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
                Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

