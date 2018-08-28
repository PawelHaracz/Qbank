using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;

namespace Qbank.Core.Command
{

    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext _context;

        public CommandDispatcher(IComponentContext context)
        {
            _context = context;
        }

        public async Task<Guid> DispatchAsync<T>(T command) where T : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command),
                    $"Command: '{typeof(T).Name}' can not be null.");
            }
            var handler = _context.Resolve<ICommandHandler<T>>();
            return await handler.HandleAsync(command);
        }
    }

}