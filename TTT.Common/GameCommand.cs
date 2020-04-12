using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class GameCommand
    {
        public GameCommand()
        {

        }

        public GameCommand(ISubCommand subCommand)
        {
            CommandType = subCommand.GetType();
            SubCommandString = JsonConvert.SerializeObject(subCommand);
        }

        public Type CommandType { get; set; }

        public string SubCommandString { get; set; }
        
        public ISubCommand SubCommand()
        {
            return JsonConvert.DeserializeObject(SubCommandString, CommandType) as ISubCommand;
        }
    }
}
