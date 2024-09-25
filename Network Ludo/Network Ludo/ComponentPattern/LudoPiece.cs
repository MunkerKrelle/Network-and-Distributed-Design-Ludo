using Microsoft.Xna.Framework;

namespace ComponentPattern
{
    public enum LudoState
    {
        inBase, outBase, finished
    }


    public class LudoPiece : Component
    {
        public string name;
        public Color color;

        public LudoPiece(GameObject gameObject, string name, Color color) : base(gameObject)
        {
            this.name = name;
            this.color = color;
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
