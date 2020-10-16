using System;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace Tests
{
    public class CommandHandlerSpy : ICommandHandler
    {
        public InvocationContext InvocationContext {get; private set;}

        public Task<int> InvokeAsync(InvocationContext context)
        {
            InvocationContext = context ?? throw new ArgumentNullException(nameof(context));

            return Task.FromResult(0);
        }
    }
}