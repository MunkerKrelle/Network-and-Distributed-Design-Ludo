using Microsoft.Xna.Framework;

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

        public void Move(int roll)
        {
            GameObject.Transform.Position += new Vector2((10 * roll), 0);
        }
    }
}
