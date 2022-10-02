using SwedishAgoraDecentralized.FileDistribution;

namespace SwedishAgoraDecentralized
{
    internal sealed class Program
    {
        public static async Task Main()
        {
            var node = FileDistributionNode.StartNew();

            await Task.Delay(Timeout.Infinite);
        }
    }
}