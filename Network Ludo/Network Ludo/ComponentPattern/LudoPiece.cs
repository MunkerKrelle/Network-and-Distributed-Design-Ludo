using ComponentPattern;
using Microsoft.Xna.Framework;
using Network_Ludo;

namespace ComponentPattern
{
    public enum LudoState
    {
        inBase, outBase, finished
    }


    public class LudoPiece : Component
    {
        Color color;
        string name;

        public LudoPiece(GameObject gameObject, Color color, string name, LudoState ludoState) : base(gameObject)
        {
            this.color = color;
            this.name = name;
        }

        public override void Awake()
        {
            GameObject.IsActive = true;
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
