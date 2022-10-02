using System.Security.Cryptography;

namespace SwedishAgoraDecentralized.Shared
{
    public static class Utilities
    {
        public static void Log(string msg) => Console.WriteLine(msg);

        public static async Task<string> CalculateFileHashAsync(string filePath)
        {
            using SHA256 hasher = SHA256.Create();
            string hash = BitConverter.ToString(await hasher.ComputeHashAsync(File.OpenRead(filePath))).Replace("-", string.Empty);
            return hash;
        }
    }
}