using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class GameCommand
    {
        string _subCommand;

        public GameCommand()
        {

        }

        public GameCommand(ISubCommand subCommand)
        {
            CommandType = subCommand.GetType();
            _subCommand = JsonConvert.SerializeObject(subCommand);
        }

        public Type CommandType { get; set; }

        public string SubCommandString
        {
            get => _subCommand;
            set => _subCommand = value;
        }
        
        public ISubCommand SubCommand()
        {
            return JsonConvert.DeserializeObject(_subCommand, CommandType) as ISubCommand;
        }
    }
}
