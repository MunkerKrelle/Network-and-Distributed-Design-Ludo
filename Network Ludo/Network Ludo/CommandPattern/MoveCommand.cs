using ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    internal class MoveCommand : ICommand
    {
        private LudoPiece piece;
        public void Execute()
        {
            piece.Move();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
