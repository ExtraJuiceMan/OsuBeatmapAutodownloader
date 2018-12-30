using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bancho2
{
    public static class HashTool
    {
        private static readonly MD5 md5 = MD5.Create();

        public static string GenerateMd5Hash(string str) => GetMd5String(Encoding.UTF8.GetBytes(str));
        public static string GetMd5String(byte[] data) => String.Join("", md5.ComputeHash(data).Select(x => x.ToString("x2")));
    }
}
