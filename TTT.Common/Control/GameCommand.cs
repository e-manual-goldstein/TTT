﻿using Newtonsoft.Json;
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

        public static bool TryParse(string message, out GameCommand gameCommand)
        {
            gameCommand = JsonConvert.DeserializeObject<GameCommand>(message, 
                new JsonSerializerSettings() 
                { 
                    Error = (object sender, ErrorEventArgs eventArgs) => 
                    { 
                        //Fail silently
                        eventArgs.ErrorContext.Handled = true; 
                    } 
                });
            return gameCommand != null;
        }

        private static void ErrorHandler(object sender, ErrorEventArgs eventArgs)
        {
            
            //_logger.Warning($"{eventArgs.ErrorContext.Error.Message}");
        }
    }
}
