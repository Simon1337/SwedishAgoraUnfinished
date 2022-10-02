using SwedishAgoraDecentralized.FileDistribution;

namespace SwedishAgoraDecentralized
{
    internal sealed class Program
    {
        public static async void Main()
        {
            var node = FileDistributionNode.StartNew();

            await Task.Delay(Timeout.Infinite);
        }
    }
}