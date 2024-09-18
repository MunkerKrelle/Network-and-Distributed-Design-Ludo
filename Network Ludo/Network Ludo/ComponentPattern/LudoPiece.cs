using ComponentPattern;
using Microsoft.Xna.Framework;
using Network_Ludo;
using Network_Ludo.CommandPattern;
using Network_Ludo.ComponentPattern;

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

        public void Move()
        {
            GameObject.Transform.Position += new Vector2((Roll * 10), 0);
        }
    }
}
