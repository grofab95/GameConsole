namespace GameConsole.ActorModel.Commands
{
    internal class HitPlayer
    {
        public int Damage { get; }

        public HitPlayer(int damage)
        {
            Damage = damage;
        }
    }
}