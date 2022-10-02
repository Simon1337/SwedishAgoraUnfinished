using System.Text;

namespace SwedishAgoraDecentralized.FileDistribution
{
    public sealed class FileDistributionNodeConfiguration
    {
        public string? RecieveAddress { get; set; }
        public string PrefferedCurrencySymbol { get; set; } = "BTC";
        public string BackupPath { get; set; } = "C:\\backup.sql";
        public string ConnectionString { get; set; } = "server=localhost;user=root;pwd=root;database=swedish_agora;";
        public string ProtocolEncoding { get; set; } = "ASCII";
    }
}