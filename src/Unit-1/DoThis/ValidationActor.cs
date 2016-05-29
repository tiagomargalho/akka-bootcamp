using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail
{
    public class ValidationActor : UntypedActor
    {
        private readonly IActorRef _consoleWriterActor;

        public ValidationActor(IActorRef consoleWriterActor)
        {
            _consoleWriterActor = consoleWriterActor;
        }

        protected override void OnReceive(object msg)
        {
            var message = msg as string;

            if (string.IsNullOrWhiteSpace(message))
            {
                _consoleWriterActor.Tell(new Messages.Error.NullInputError("No input received."));
            }
            else
            {
                if (IsValid(message))
                {
                    _consoleWriterActor.Tell(new Messages.Success.InputSuccess("Thank you! Message was valid."));
                }
                else
                {
                    _consoleWriterActor.Tell(new Messages.Error.ValidationError("Invalid: input had odd number of characters."));
                }
            }

            Sender.Tell(new Messages.Neutral.ContinueProcessing());
        }

        private bool IsValid(string message)
        {
            return message.Length % 2 == 0;
        }
    }
}
