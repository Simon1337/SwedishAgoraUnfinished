using System.Text;

namespace SwedishAgoraDecentralized.Shared
{
    public static class Protocol
    {
        public const int MaxMessageSize = 1024;
        public const int PacketSize = 1024;
        public const int SocketPort = 8888;
        public static readonly Encoding Encoding = Encoding.ASCII;
    }
}
