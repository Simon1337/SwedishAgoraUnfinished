using SwedishAgoraDecentralized.FileDistribution.DataModel;
using SwedishAgoraDecentralized.Shared;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static SwedishAgoraDecentralized.FileDistribution.FileDistributionNodeHelpers;
using static SwedishAgoraDecentralized.Shared.Utilities;
using static SwedishAgoraDecentralized.Shared.Protocol;
using static SwedishAgoraDecentralized.FileDistribution.FileDistributionNodeHelpers;

namespace SwedishAgoraDecentralized.FileDistribution
{
    public sealed class FileDistributionNode
    {
        private TimeSpan RunTime => _stopwatch.Elapsed;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly string _uniqueServerId = Guid.NewGuid().ToString();

        private readonly FileDistributionNodeConfiguration _config;
        private readonly Database _database;

        private FileDistributionNode(FileDistributionNodeConfiguration config)
        {
            _config = config;
            _database = new Database(config.ConnectionString, config.BackupPath);
        }

        public static FileDistributionNode StartNew(FileDistributionNodeConfiguration? config = null)
        {
            FileDistributionNode instance = new(config ?? new());
            instance.StartAsync().FireAndForget();
            return instance;
        }

        private async Task StartAsync()
        {
            try
            {
                var isSuccessful = await RunStartupScriptAsync();
                if (!isSuccessful) return;

                TcpListener server = new(IPAddress.IPv6Any, SocketPort);
                string? checksumCache = null; //Dictionary<string, object> cachedVariables

                while (true)
                {
                    try
                    {
                        using Socket result = await server.AcceptSocketAsync();
                        using NetworkStream responseStream = new (result);
                        string clientRequest = await responseStream.ReadStringAsync();

                        LazyTask<string> hash = new(async () =>
                        {
                            await _database.BackupAsync();
                            return await CalculateFileHashAsync(_config.BackupPath);
                        });

                        //GetChecksum
                        if (clientRequest.StartsWith("GC "))
                        {
                            byte[] response = Encoding.GetBytes($"C {checksumCache ?? await hash}*{RunTime.Minutes}*{_uniqueServerId}");
                            await responseStream.WriteAsync(response);
                            checksumCache = null;
                        }

                        //File
                        await HandleRequestAsync("F ", clientRequest, async parts =>
                        {
                            var reqSid = parts[0];
                            if (_uniqueServerId == reqSid)
                            {
                                using FileStream fileStream = File.OpenRead(_config.BackupPath);

                                byte[] memoryBuffer = new byte[PacketSize];
                                while (await fileStream.ReadAsync(memoryBuffer) != 0)
                                {
                                    await responseStream.WriteAsync(memoryBuffer);
                                }
                            }
                        });

                        //GetPayoutDetails
                        await HandleRequestAsync("GPD", clientRequest, async parts =>
                        {
                            byte[] response = Encoding.GetBytes($"PD {_uniqueServerId}");
                            await responseStream.WriteAsync(response);
                        });

                        //AcceptProfits
                        await HandleRequestAsync("AP", clientRequest, async parts => 
                        {

                        });

                        //AddProductLine
                        await HandleRequestAsync("APL ", clientRequest, async parts =>
                        {
                            ProductLine line = new()
                            {
                                Name = parts[0],
                                Description = parts[1],
                                ImagePath = parts[2],
                                PublicKey = parts[3],
                                LatestChangeSignature = parts[4],
                                QuantityRemaining = int.Parse(parts[5]),
                                UnitPrice = decimal.Parse(parts[6]),
                            };

                            if (!await _database.InsertObjectAsync(line))
                                Log("Insert ProductLine Failed");

                            var currentChecksum = checksumCache = await hash;

                            Task.Run(async () =>
                            {
                                try
                                {
                                    using UdpClient broadClient = new() { EnableBroadcast = true, };
                                    if (!await VerifyFileAsync(broadClient, currentChecksum))
                                        Log("Unable to ensure file integrity");
                                }
                                catch (Exception ex)
                                {
                                    Log(ex.ToString());
                                }
                            }).FireAndForget();
                        });
                        //ChangeProductLine
                        //GetProductLine
                        //GetProductLineList
                        //GetProductTypeList
                    }
                    catch(Exception e) when (e is ObjectDisposedException or SocketException or IOException)
                    {
                        Log(e.ToString());
                        server = new(SocketPort);
                    }
                    catch (Exception ex)
                    {
                        Log("Exception occured serving request:");
                        Log(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Failed to start server:");
                Log(ex.ToString());
            }
        }

        private async Task<bool> VerifyFileAsync(UdpClient client, string? currentChecksum = null) => (await VerifyFileWithResponseAsync(client, currentChecksum)).Item1;
        private async Task<(bool, string?)> VerifyFileWithResponseAsync(UdpClient client, string? currentChecksum = null)
        {
            //C <checksum>*<runtime(minutes)>*<uniqueserverid>
            var getchkResp = await client.SendBroadCastMessageAsync(Encoding, $"GC {_uniqueServerId}");
            if (!(getchkResp?.Contains("C ") ?? false))
            {
                if (getchkResp != null)
                    Log("Invalid format: " + getchkResp);
                return (false, getchkResp);
            }


            List<VerifyFileCandidate> itemsToUse = ProcessData(getchkResp);
            if (itemsToUse.Count > 0 && itemsToUse.First().Checksum == currentChecksum)
                return (true, getchkResp);

            foreach (VerifyFileCandidate item in itemsToUse)
            {
                if (!await TryDownloadFileAsync(client, item))
                    continue;

                var hash = await CalculateFileHashAsync(_config.BackupPath);
                if (hash == item.Checksum)
                {
                    await _database.RestoreAsync();
                    return (true, getchkResp);
                }
            }

            return (false, getchkResp);
        }

        private async Task<bool> RunStartupScriptAsync()
        {
            bool fileExist = File.Exists(_config.BackupPath);
            using UdpClient broadClient = new() { EnableBroadcast = true, };
            var (success, getChecksumResponse) = await VerifyFileWithResponseAsync(broadClient);
            return success || (fileExist && getChecksumResponse == null); 
        }

        private async Task<bool> TryDownloadFileAsync(NetworkStream communicationStream, VerifyFileCandidate itemToUse)
        {
            try
            {
                Log("Inform the network, this is the file we intend to use and get the data...");
                //TODO we changed UDP -> TCP:
                //_ = await broadClient.SendBroadCastMessageAsync(Protocol.Encoding, $"F {itemToUse.Sid}*{itemToUse.Checksum}", skipResponse: true); //F for File
                //NetworkStream netStream = broadClient.GetStream();
                using StreamWriter fileStream = new(_config.BackupPath, append: true) { AutoFlush = true };
                while (communicationStream.DataAvailable && communicationStream.Socket.Connected)
                {
                    byte[] memorybuffer = new byte[PacketSize];
                    _ = await netStream.ReadAsync(memorybuffer);
                    await fileStream.WriteAsync(Protocol.Encoding.TryGetString(memorybuffer));
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                return false;
            }

            return true;
        }
    }
}