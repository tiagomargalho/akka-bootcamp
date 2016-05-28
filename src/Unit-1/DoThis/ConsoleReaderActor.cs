using System;
using Akka.Actor;

namespace WinTail
{
    /// <summary>
    /// Actor responsible for reading FROM the console. 
    /// Also responsible for calling <see cref="ActorSystem.Shutdown"/>.
    /// </summary>
    class ConsoleReaderActor : UntypedActor
    {
        public const string StartCommand = "start";
        public const string ExitCommand = "exit";
        private IActorRef _consoleWriterActor;

        public ConsoleReaderActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object message)
        {
            if (message.Equals(StartCommand))
            {
                DoPrintInstructions();
            }
            else if (message is Messages.Error.InputError)
            {
                _consoleWriterActor.Tell(message as Messages.Error.InputError);
            }

            GetAndValidateInput();
        }

        private void GetAndValidateInput()
        {
            var message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message))
            {
                Self.Tell(new Messages.Error.NullInputError("No input received."));
            }
            else if (!string.IsNullOrEmpty(message) && String.Equals(message, ExitCommand, StringComparison.OrdinalIgnoreCase))
            {
                // shut down the system (acquire handle to system via
                // this actors context)
                Context.System.Shutdown();
                return;
            }
            else
            {
                if (IsValid(message))
                {
                    // send input to the console writer to process and print
                    _consoleWriterActor.Tell(new Messages.Success.InputSuccess("Thank you! Message was valid."));

                    // continue reading messages from the console
                    Self.Tell(new Messages.Neutral.ContinueProcessing());
                }
                else
                {
                    Self.Tell(new Messages.Error.ValidationError("Invalid: input had odd number of characters."));
                }
            }
        }

        private void DoPrintInstructions()
        {
            Console.WriteLine("Write whatever you want into the console!");
            Console.WriteLine("Some entries will pass validation, and some won't...\n\n");
            Console.WriteLine("Type 'exit' to quit this application at any time.\n");
        }

        private bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}