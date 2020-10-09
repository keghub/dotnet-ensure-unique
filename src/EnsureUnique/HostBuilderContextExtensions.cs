using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Hosting;

namespace EMG.Tools.EnsureUnique
{
    internal static class HostBuilderContextExtensions
    {
        public static bool TryGetOptionValue<TValue>(this HostBuilderContext context, Option option, out TValue value)
        {
            if (context.Properties.TryGetValue(typeof(InvocationContext), out var obj) &&
            obj is InvocationContext ctx &&
            ctx.ParseResult.HasOption(option) &&
            ctx.ParseResult.ValueForOption<TValue>(option) is TValue optionValue)
            {
                value = optionValue;
                return true;
            }

            value = default;
            return false;
        }
    }
}
