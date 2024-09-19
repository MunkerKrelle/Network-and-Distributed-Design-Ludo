using ComponentPattern;
using Microsoft.Xna.Framework;
using Network_Ludo;
using Network_Ludo.CommandPattern;
using Network_Ludo.ComponentPattern;

namespace ComponentPattern
{
    public enum LudoState
    {
        inBase, outBase, finished
    }


    public class LudoPiece : Component
    {
        string name;

        public LudoPiece(GameObject gameObject, string name, LudoState ludoState) : base(gameObject)
        {
            this.name = name;
        }

        public override void Awake()
        {
            GameObject.IsActive = true;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public void Move()
        {
            
        }
    }
}
