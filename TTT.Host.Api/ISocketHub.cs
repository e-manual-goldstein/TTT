using System;
using System.Net;
using System.Threading.Tasks;
using TTT.Common;

namespace TTT.Host.Api
{
    public interface ISocketHub
    {
        int ServerPort { get; }
        IPAddress ServerAddress { get; }
        void BroadcastCommand(GameCommand gameCommand);
        Task<Guid> ConnectAsync(bool useThreading = false);
        Task<Guid> BeginListening();
        void SendCommand(Guid userId, GameCommand message);
    }
}
