using System;
using Momo.Common;

namespace Momo.Domain
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<in T> where T : ICommand
    {
        CommandResult Handle(T command);
    }

    public interface ICommandExecutor
    {
        CommandResult Handle<T>(T command) where T : ICommand;
    }

    public class CommandExecutor : ICommandExecutor
    {
        public CommandResult Handle<T>(T command) where T : ICommand
        {
            var handler = Ioc.Resolve<ICommandHandler<T>>();

            if (handler == null)
                return new CommandResult().Add("No command handler registered for {0}".F(typeof(T).FullName));

            return handler.Handle(command);
        }
    }
}
