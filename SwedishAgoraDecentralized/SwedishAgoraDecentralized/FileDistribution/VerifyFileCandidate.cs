namespace SwedishAgoraDecentralized.FileDistribution
{
    internal sealed class VerifyFileCandidate
    {
        public int RunTime { get; set; }
        public string Checksum { get; set; } = string.Empty;
        public string Sid { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}