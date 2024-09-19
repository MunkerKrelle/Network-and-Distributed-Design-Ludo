using ComponentPattern;
using Network_Ludo;

namespace CommandPattern
{
    internal class RollCommand : ICommand
    {
        private Die die;

        public RollCommand(Die die)
        {
            this.die = die;
        }
        public void Execute()
        {
            GameWorld.Instance.CheckState(die.RollDie());
        }

        public void Undo()
        {
        }
    }
}
