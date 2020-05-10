using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace TTT.Common
{
    public class GameCommand
    {
        public GameCommand()
        {

        }

        public GameCommand(ISubCommand subCommand, Guid commandId)
        {
            CommandType = subCommand.GetType();
            CommandId = commandId;
            SubCommandString = JsonConvert.SerializeObject(subCommand);
        }

        public Type CommandType { get; set; }

        public string SubCommandString { get; set; }

        public Guid CommandId { get; set; }
        
        public ISubCommand SubCommand()
        {
            return JsonConvert.DeserializeObject(SubCommandString, CommandType) as ISubCommand;
        }

        public static bool TryParse(string message, out GameCommand gameCommand)
        {
            gameCommand = JsonConvert.DeserializeObject<GameCommand>
                (message, new JsonSerializerSettings()
                    {
                        Error = (object sender, ErrorEventArgs eventArgs) =>
                        {
                            //Fail silently
                            eventArgs.ErrorContext.Handled = true;
                        }
                    }
                );
            return gameCommand != null;
        }
    }
}
