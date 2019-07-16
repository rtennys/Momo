using System;

namespace Momo.Domain
{
    public interface ICommand
    {
    }

    public interface ICommandHandler<in T>
    {
        DomainResult Handle(T command);
    }

    public interface ICommandExecutor
    {
        DomainResult Execute<T>(T command) where T : ICommand;
    }

    public class CommandExecutor : ICommandExecutor
    {
        public DomainResult Execute<T>(T command) where T : ICommand
        {
            var handler = Ioc.Resolve<ICommandHandler<T>>();

            if (handler == null)
            {
                object arg0 = typeof(T).FullName;
                return new DomainResult().Add($"No command handler registered for {arg0}");
            }

            return handler.Handle(command);
        }
    }
}
