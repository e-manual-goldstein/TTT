using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TTT.Common
{
    /// <summary>
    /// The AbstractController class receives incoming messages to the device. It is implemented on both Client and Host. Inbound messages arrive in the form of COMMANDS
    /// </summary>
    public class AbstractController
    {
        Dictionary<Type, MethodInfo> _actionDictionary = new Dictionary<Type, MethodInfo>();
        protected Logger _logger;
        protected Action<string> PostBackAction;

        public AbstractController(Logger logger)
        {
            _logger = logger;
        }

        public void ExecuteCommand(GameCommand gameCommand, Action<string> postbackMessage)
        {
            if (!_actionDictionary.ContainsKey(gameCommand.CommandType))
            {
                if (!FindAction(gameCommand.CommandType))
                    return;
            }
            PostBackAction = postbackMessage;
            _actionDictionary[gameCommand.CommandType].Invoke(this, new object[] { gameCommand.SubCommand() });
            PostBackAction = null;
        }

        private bool FindAction(Type argumentType)
        {
            _actionDictionary[argumentType] = GetType().GetMethods().SingleOrDefault(m => m.GetParameters().FirstOrDefault()?.ParameterType == argumentType);
            return _actionDictionary[argumentType] != null;
        }
    }
}
