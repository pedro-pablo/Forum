using System.Security.Cryptography;

namespace Forum
{
    public static class Hashing
    {
        public static byte[] GerarSalt()
        {
            byte[] salt = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static byte[] GerarHashSenha(string senha, byte[] salt)
        {
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, 100000))
            {
                hash = pbkdf2.GetBytes(32);
            }

            return hash;
        }

        public static byte[] GerarHashArquivo(byte[] arquivo)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(arquivo);
            }

            return hash;
        }
    }
}