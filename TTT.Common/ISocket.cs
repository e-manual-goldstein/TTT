using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public interface ISocket
    {
        bool IsOpen { get; }

        void Send(GameCommand gameCommand);
        void SendReceipt(GameCommand gameCommand);
    }
}
