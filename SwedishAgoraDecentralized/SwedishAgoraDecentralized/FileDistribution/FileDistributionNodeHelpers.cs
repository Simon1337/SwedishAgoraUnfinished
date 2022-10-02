using SwedishAgoraDecentralized.Shared;
using static SwedishAgoraDecentralized.Shared.Utilities;

namespace SwedishAgoraDecentralized.FileDistribution
{
    internal static class FileDistributionNodeHelpers
    {
        internal static async Task HandleRequestAsync(string prefix, string clientRequest, Func<string[], Task> handleRequest)
        {
            try
            {
                if (clientRequest.StartsWith(prefix))
                {
                    string[] parts = clientRequest.GetParts(prefix);
                    await handleRequest(parts);
                }
            }
            catch(Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal static List<VerifyFileCandidate> ProcessData(string addNodeResponse)
        {
            var originalGroup = addNodeResponse.Split("C ")
                            .Where(c => c.Contains('*'))
                            .Select(c => c.Split('*'))
                            .Select(c => new { checksum = c[0], runtime = int.Parse(c[1]), sid = c[2] })
                            .GroupBy(c => c.checksum)
                            .ToList();

            VerifyFileCandidate bestItem = originalGroup
                .Select(g => new { g, earliestInGroup = g.OrderBy(x => x.runtime).First(x => x.runtime > 0) })
                .Select(g => new VerifyFileCandidate
                {
                    RunTime = g.earliestInGroup.runtime,
                    Checksum = g.earliestInGroup.checksum,
                    Sid = g.earliestInGroup.sid,
                    Count = g.g.Count(),
                })
                .OrderByDescending(s => s.Count)
                .First();

            List<VerifyFileCandidate> itemsToUse = originalGroup
                .First(x => x.Key == bestItem.Checksum)
                .Select(g => new VerifyFileCandidate
                {
                    RunTime = g.runtime,
                    Checksum = g.checksum,
                    Sid = g.sid,
                    Count = 1,
                })
                .OrderByDescending(x => x.Sid == bestItem.Sid)
                .ToList();

            return itemsToUse;
        }

    }
}