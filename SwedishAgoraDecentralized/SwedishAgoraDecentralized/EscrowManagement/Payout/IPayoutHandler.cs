using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwedishAgoraDecentralized.EscrowManagement.Payout
{
    internal interface IPayoutHandler
    {
        Task<bool> PayoutAsync(string toRecieveAddress, string fromPrivateKey, decimal amount);
    }
}
