using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for serializing message writes to the console.
    /// (write one message at a time, champ :)
    /// </summary>
    class ConsoleWriterActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is Messages.Error.InputError)
            {
                var msg = message as Messages.Error.InputError;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg.Reason);
            }
            else if (message is Messages.Success.InputSuccess)
            {
                var msg = message as Messages.Success.InputSuccess;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg.Reason);
            }
            else
            {
                Console.WriteLine(message);
            }

            Console.ResetColor();
        }
    }
}
