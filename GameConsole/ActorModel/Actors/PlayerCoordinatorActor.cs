using Akka.Actor;
using Akka.Persistence;
using GameConsole.ActorModel.Commands;
using GameConsole.ActorModel.Events;

namespace GameConsole.ActorModel.Actors
{
    internal class PlayerCoordinatorActor : ReceivePersistentActor
    {
        private const int DefaultStartingHealth = 100;

        public PlayerCoordinatorActor()
        {
            Command<CreatePlayer>(command =>
            {
                DisplayHelper.WriteLine($"PlayerCoordinatorActor received CreatePlayer command for {command.PlayerName}");
                var @event = new PlayerCreated(command.PlayerName);
                Persist(@event, playerCreatedEvent => 
                {
                    DisplayHelper.WriteLine($"PlayerCoordinatorActor persisted a PlayerCreated for {playerCreatedEvent.PlayerName}");
                    Context.ActorOf(
                        Props.Create(() =>
                                new PlayerActor(playerCreatedEvent.PlayerName, DefaultStartingHealth)), playerCreatedEvent.PlayerName);
                });
            });

            Recover<PlayerCreated>(playerCreatedEvent => 
            {
                DisplayHelper.WriteLine($"PlayerCoordinatorActor replaying a PlayerCreated event for {playerCreatedEvent.PlayerName}");
                Context.ActorOf(
                        Props.Create(() =>
                                new PlayerActor(playerCreatedEvent.PlayerName, DefaultStartingHealth)), playerCreatedEvent.PlayerName);
            });
        }

        public override string PersistenceId => $"player-coordinator";
    }
}