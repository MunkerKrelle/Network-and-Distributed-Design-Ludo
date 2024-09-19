using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern
{
    internal class InputHandler
    {
        private static InputHandler instance;
        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                    instance = new InputHandler();
                return instance;
            }
        }
        private InputHandler() { }
        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private Stack<ICommand> executedCommands = new Stack<ICommand>();
        private Stack<ICommand> undoneCommands = new Stack<ICommand>();
        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }

        private KeyboardState previousKeyState;

        /// <summary>
        /// Checker om der er en knap der passer til en command
        /// </summary>
        public void Execute()
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (var pressedKey in keyState.GetPressedKeys())
            {
                if (keybindsUpdate.TryGetValue(pressedKey, out ICommand cmd))
                {
                    cmd.Execute();
                }
                if (!previousKeyState.IsKeyDown(pressedKey) && keyState.IsKeyDown(pressedKey))
                {
                    if (keybindsButtonDown.TryGetValue(pressedKey, out ICommand cmdBd))
                    {
                        cmdBd.Execute();
                        executedCommands.Push(cmdBd);
                        undoneCommands.Clear();
                    }

                    if (pressedKey == Keys.O)
                    {
                        Redo();
                    }
                    if (pressedKey == Keys.P)
                    {
                        Undo();
                    }
                }
            }
            previousKeyState = keyState;
        }

        /// <summary>
        /// Fjerner commands der er blevet kørt
        /// </summary>
        public void ClearCommands()
        {
            keybindsUpdate.Clear();
            keybindsButtonDown.Clear();
        }

        /// <summary>
        /// Undo for at fortryde en command
        /// </summary>
        private void Undo()
        {
            if (executedCommands.Count > 0)
            {
                ICommand commandToUndo = executedCommands.Pop();
                commandToUndo.Undo();
                undoneCommands.Push(commandToUndo);
            }
        }

        /// <summary>
        /// Redo for at køre den samme command
        /// </summary>
        public void Redo()
        {
            if (undoneCommands.Count > 0)
            {
                ICommand commandToRedo = undoneCommands.Pop();
                commandToRedo.Execute();
                executedCommands.Push(commandToRedo);
            }
        }
    }
}
