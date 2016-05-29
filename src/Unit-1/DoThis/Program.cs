using System;
﻿using Akka.Actor;

namespace WinTail
{
    class Program
    {
        public static ActorSystem MyActorSystem;

        static void Main(string[] args)
        {
            MyActorSystem = ActorSystem.Create("MyActorSystem");

            var consoleWriterProps = Props.Create<ConsoleWriterActor>();
            var consoleWriterActor = MyActorSystem.ActorOf(consoleWriterProps);

            Props tailCoordinatorProps = Props.Create(() => new TailCoordinatorActor());
            IActorRef tailCoordinatorActor = MyActorSystem.ActorOf(tailCoordinatorProps, "tailCoordinatorActor");

            var validationActorProps = Props.Create(() => new FileValidationActor(consoleWriterActor, tailCoordinatorActor));
            var validationActor = MyActorSystem.ActorOf(validationActorProps);

            var consoleReaderProps = Props.Create<ConsoleReaderActor>(validationActor);
            var consoleReaderActor = MyActorSystem.ActorOf(consoleReaderProps);

            consoleReaderActor.Tell(ConsoleReaderActor.StartCommand);

            MyActorSystem.WhenTerminated.Wait();
        }
    }
}
