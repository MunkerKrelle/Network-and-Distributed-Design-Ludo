using myClientTCP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentPattern
{
    /// <summary>
    /// Indlæse hvilket sprite et GameObject har
    /// </summary>
    class SpriteRenderer : Component
    {
        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
        }

        public Texture2D Sprite { get; set; }

        public Vector2 Origin { get; set; }

        public override void Start()
        {
            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
        }

        public void SetSprite(string spriteName)
        {
            Sprite = ClientGameWorld.Instance.Content.Load<Texture2D>(spriteName);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameObject.IsActive == true)
            {
                spriteBatch.Draw(Sprite, GameObject.Transform.Position, null, GameObject.Transform.Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, GameObject.Transform.SpriteEffect, GameObject.Transform.Layer);
            }
        }
    }
}
