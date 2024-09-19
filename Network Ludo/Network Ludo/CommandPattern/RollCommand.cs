using Network_Ludo.ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Ludo.CommandPattern
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
