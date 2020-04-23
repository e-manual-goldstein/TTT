using System;
using System.Threading.Tasks;
using TTT.Common;

namespace TTT.Host.Api
{
    public interface ISocketHub
    {
        void BroadcastCommand(GameCommand gameCommand);
        Task<Guid> ConnectAsync(bool useThreading = false);
    }
}
