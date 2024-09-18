using ComponentPattern;
using Microsoft.Xna.Framework;
using Network_Ludo;

namespace ComponentPattern
{
    public class LudoPiece : Component
    {
        Color color;
        string name;

        public LudoPiece(GameObject gameObject, Color color, string name) : base(gameObject)
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
