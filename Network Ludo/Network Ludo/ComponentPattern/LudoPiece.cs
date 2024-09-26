using Microsoft.Xna.Framework;

namespace ComponentPattern
{
    /// <summary>
    /// The playable piece for the game
    /// </summary>
    public class LudoPiece : Component
    {
        public string name; //Name of the player controlling
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
    }
}
