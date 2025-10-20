using System.Security.Cryptography;
using System.Text;

namespace Reserva_de_equipos.Utils
{
    public static class Encrypt
    {
        public static string GetSHA256(string str)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(str ?? "");
            var hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }
    }
}
