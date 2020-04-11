using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;

namespace TTT.Host.Control
{
    public class MessageHandler
    {
        public bool TryParse(string message, out IGameCommand gameCommand)
        {
            try
            {
                gameCommand = JsonConvert.DeserializeObject(message) as IGameCommand;
                return gameCommand != null;
            }
            catch
            {
                gameCommand = null;
                return false;
            }
        }
    }
}
