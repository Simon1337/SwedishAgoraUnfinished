using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace SwedishAgoraDecentralized
{
    internal static class Utilities
    {
        internal static void Log(string msg) => Console.WriteLine(msg);

        internal static async Task<string> CalculateFileHashAsync(string filePath)
        {
            using SHA256 hasher = SHA256.Create();
            string hash = BitConverter.ToString(await hasher.ComputeHashAsync(File.OpenRead(filePath))).Replace("-", string.Empty);
            return hash;
        }
    }
}