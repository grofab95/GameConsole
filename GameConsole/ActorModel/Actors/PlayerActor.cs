using Akka.Persistence;
using GameConsole.ActorModel.Commands;
using GameConsole.ActorModel.Events;
using System;

namespace GameConsole.ActorModel.Actors
{
    class PlayerActor : ReceivePersistentActor
    {
        private readonly string _playerName;
        private int _health;

        public override string PersistenceId => $"player-{_playerName}";

        public PlayerActor(string playerName, int startingHealth)
        {
            _playerName = playerName;
            _health = startingHealth;

            DisplayHelper.WriteLine($"{_playerName} created");

            Command<HitPlayer>(message => HitPlayer(message));
            Command<DisplayStatus>(message => DisplayPlayerStatus());
            Command<SimulateError>(message => SimulateError());

            Recover<PlayerHit>(message => 
            {
                DisplayHelper.WriteLine($"{_playerName} replaying PlayerHit event from journal");
                _health -= message.DamageTaken;
            });
        }

        private void HitPlayer(HitPlayer command)
        {
            DisplayHelper.WriteLine($"{_playerName} received HitPlayer command");
            var @event = new PlayerHit(command.Damage);
            DisplayHelper.WriteLine($"{_playerName} persisting HitPlayer event");
            Persist(@event, playerHitEvent => 
            {
                DisplayHelper.WriteLine($"{_playerName} persisted PlayerHit event ok, updating actor state");
                _health -= @event.DamageTaken;
            });
        }

        private void DisplayPlayerStatus()
        {
            DisplayHelper.WriteLine($"{_playerName} received DisplayStatus");

            Console.WriteLine($"{_playerName} has {_health} health");
        }

        private void SimulateError()
        {
            DisplayHelper.WriteLine($"{_playerName} received SimulateError");

            throw new ApplicationException($"Simulated exception in player: {_playerName}");
        }
    }
}
