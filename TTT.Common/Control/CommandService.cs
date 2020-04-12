using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TTT.Common.Control
{
    public class CommandService
    {
        Logger _logger;
        Type[] _controllerTypes;
        Dictionary<Type, MethodInfo> _actionDictionary = new Dictionary<Type, MethodInfo>();

        public CommandService(Logger logger, Type[] controllerTypes)
        {
            _logger = logger;
            //validate these
            _controllerTypes = controllerTypes;
        }

        public string ExecuteCommand(GameCommand gameCommand)
        {
            if (!_actionDictionary.ContainsKey(gameCommand.CommandType))
            {
                if (!FindAction(gameCommand.CommandType))
                    return "Unknown command";
            }
            return CreateControllerAndExecuteCommand(_actionDictionary[gameCommand.CommandType].DeclaringType);
        }

        private bool FindAction(Type argumentType)
        {
            _actionDictionary[argumentType] = _controllerTypes.SelectMany(conType => conType.GetType().GetMethods())
                .SingleOrDefault(m => m.GetParameters().FirstOrDefault()?.ParameterType == argumentType);
            return _actionDictionary[argumentType] != null;
        }

        private string CreateControllerAndExecuteCommand(Type controllerType)
        {
            //var controller = Activator.CreateInstance(controllerType, );
            //_actionDictionary[gameCommand.CommandType].Invoke(, new object[] { gameCommand.SubCommand() });
            throw new NotImplementedException();
        }
    }
}
