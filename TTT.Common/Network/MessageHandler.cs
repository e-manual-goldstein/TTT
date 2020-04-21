using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;

namespace TTT.Common
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
            gameCommand = JsonConvert.DeserializeObject<GameCommand>(message, new JsonSerializerSettings() { Error = ErrorHandler });
            return gameCommand != null;
        }

        private void ErrorHandler(object sender, ErrorEventArgs eventArgs)
        {
            eventArgs.ErrorContext.Handled = true;
            _logger.Warning($"{sender}");
        }

    }
}
