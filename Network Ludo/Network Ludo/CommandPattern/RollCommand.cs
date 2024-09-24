using ComponentPattern;
using myClientTCP;

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
            ClientGameWorld.Instance.CheckState(die.RollDie());
        }

        public void Undo()
        {
        }
    }
}
