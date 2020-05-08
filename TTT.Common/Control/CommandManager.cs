using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TTT.Common
{
    public class CommandManager
    {
        Logger _logger;
        public CommandManager(Logger logger)
        {
            _logger = logger;
        }

        Dictionary<Guid, bool> _commandReceiptLog = new Dictionary<Guid, bool>();
        object lockObject = new object();
        public GameCommand CreateCommand(ISubCommand subCommand)
        {
            _logger.Debug($"Creating Command: {subCommand}");
            var commandId = Guid.NewGuid();
            lock (lockObject)
            {
                lock (_commandReceiptLog)
                {
                    _commandReceiptLog[commandId] = false;
                }
            }
            return new GameCommand(subCommand, commandId);
        }

        public bool IsProcessed(GameCommand gameCommand)
        {
            if (!_commandReceiptLog.ContainsKey(gameCommand.CommandId))
                return false;
            lock (lockObject)
            {
                lock (_commandReceiptLog)
                {
                    return _commandReceiptLog[gameCommand.CommandId];
                }
            }
        }

        public void SendVerify(ISocket socket, GameCommand gameCommand)
        {
            if (!_commandReceiptLog.ContainsKey(gameCommand.CommandId))
                throw new ArgumentException("Cannot send unregistered command. Use CreateCommand to registed commands before sending.");
            while (!_commandReceiptLog[gameCommand.CommandId] && socket.IsOpen)
            {
                _logger.Debug($"Sending verified command: {gameCommand.CommandType.Name}");
                socket.Send(gameCommand);
                Thread.Sleep(500);
            }
        }

        public void LogReceipt(Guid commandId)
        {
            lock (lockObject)
            {
                lock (_commandReceiptLog)
                {
                    _commandReceiptLog[commandId] = true;
                }
            }
        }
    }
}
