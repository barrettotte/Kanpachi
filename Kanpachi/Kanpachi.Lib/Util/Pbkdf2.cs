using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using DeviceId;

namespace Kanpachi.Lib{

    public static class Pbkdf2{

        private const int hashSize = 32; // 256-bit
        private const int defaultIterations = 10000; 

        public static string Hash(string password){
            return Hash(Encoding.UTF8.GetBytes(password), GetSalt(), defaultIterations);
        }

        // TODO: SecureString password parm

        public static string Hash(string password, string salt){
            return Hash(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt), defaultIterations);
        }

        private static string Hash(byte[] password, byte[] salt, int iterations){
            using(var alg = new Rfc2898DeriveBytes(password, salt, iterations)){
                string hash = Convert.ToBase64String(alg.GetBytes(hashSize));
                return $"{alg.IterationCount}:{Convert.ToBase64String(alg.Salt)}:{hash}";
            }
        }

        // validate hash  TODO: SecureString
        public static bool Validate(string password, string valid){
            var split = valid.Split(':');
            var iterations = Int32.Parse(split[0]);
            var salt = Convert.FromBase64String(split[1]);
            var check = Hash(Encoding.UTF8.GetBytes(password), salt, iterations);

            Console.WriteLine("original => " + valid);
            Console.WriteLine("check    => " + check);
            return valid == check;
        }

        // get salt based on current hardware
        private static byte[] GetSalt(){
            string hardware = new DeviceIdBuilder()
                .AddMotherboardSerialNumber()
                .AddProcessorId()
                .ToString();
            return Encoding.UTF8.GetBytes(hardware);
        }
    }
}
