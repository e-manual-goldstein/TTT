using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TTT.Common
{
    public class ControllerManager
    {
        IServiceProvider _serviceProvider;
        Logger _logger;
        Type[] _controllerTypes;
        Dictionary<Type, MethodInfo> _actionDictionary = new Dictionary<Type, MethodInfo>();

        public ControllerManager(IServiceProvider serviceProvider, Logger logger, Type[] controllerTypes)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _controllerTypes = controllerTypes;
        }

        public string ExecuteCommand(GameCommand gameCommand)
        {
            _logger.Debug($"Executing Command: {gameCommand.CommandType.Name}");
            if (!_actionDictionary.ContainsKey(gameCommand.CommandType))
            {
                if (!FindAction(gameCommand.CommandType))
                {
                    _actionDictionary.Remove(gameCommand.CommandType);
                    throw new ApplicationException("Unknown Command");
                };
            }
            var action = _actionDictionary[gameCommand.CommandType];
            return CreateControllerAndExecuteCommand(action.DeclaringType, gameCommand);
        }

        private bool FindAction(Type argumentType)
        {
            _actionDictionary[argumentType] = _controllerTypes.SelectMany(conType => conType.GetMethods())
                .SingleOrDefault(m => m.GetParameters().FirstOrDefault()?.ParameterType == argumentType);
            return _actionDictionary[argumentType] != null;
        }

        private string CreateControllerAndExecuteCommand(Type controllerType, GameCommand gameCommand)
        {
            var controller = _serviceProvider.GetService(controllerType);
            if (controller == null)
            {
                throw new ApplicationException("Controller not found");
            }
            _actionDictionary[gameCommand.CommandType].Invoke(controller, new object[] { gameCommand.SubCommand() });
            return "";
        }
    }
}
