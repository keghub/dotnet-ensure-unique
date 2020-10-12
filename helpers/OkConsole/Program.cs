using System;

namespace OkConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = args.Length > 0 ? args[0] : "World";

            Console.WriteLine($"Hello {target}!");
        }
    }
}
