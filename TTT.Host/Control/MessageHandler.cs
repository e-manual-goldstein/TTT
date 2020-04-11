using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;

namespace TTT.Host.Control
{
    public class MessageHandler
    {
        Logger _logger;

        public MessageHandler(Logger logger)
        {
            _logger = logger;
        }

        public bool TryParse(string message, out GameCommand gameCommand)
        {
            try
            {
                gameCommand = JsonConvert.DeserializeObject<GameCommand>(message);
                return gameCommand != null;
            }
            catch (Exception ex)
            {
                _logger.Log($"{ex}");
                gameCommand = null;
                return false;
            }
        }

    }
}
