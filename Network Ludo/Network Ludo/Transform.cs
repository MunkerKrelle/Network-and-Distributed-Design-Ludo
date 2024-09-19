using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Network_Ludo
{
    /// <summary>
    /// Bliver brugt til at kunne ændre alle aspekter af GameObejctsne samlet, frem for at skulle gøre det hver for sig
    /// </summary>
    public class Transform
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; } = 0f;
        public Vector2 Scale { get; set; } = new Vector2(1, 1);
        public Color Color { get; set; } = Color.White;
        public float Layer { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public void Transformer(Vector2 Position, float Rotaion, Vector2 Scale, Color Color, float Layer)
        {
            this.Position = Position;
            this.Rotation = Rotaion;
            this.Scale = Scale;
            this.Color = Color;
            this.Layer = Layer;
        }

        /// <summary>
        /// Oversætter en Position til en andens position
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector2 translation)
        {
            if (!float.IsNaN(translation.X) && !float.IsNaN(translation.Y))
            {
                Position += translation;
            }
        }

        /// <summary>
        /// OVersættter en Vector2 til en Point variable
        /// </summary>
        /// <param name="a">Positionen der skal oversættes</param>
        /// <returns></returns>
        public Point VectorToPointConverter(Vector2 a)
        {
            Point PTPosition = new Point((int)a.X / 100, (int)a.Y / 100);
            return PTPosition;
        }

        /// <summary>
        /// Bevæger spilleren til et nyt point 
        /// </summary>
        /// <param name="a">Hvilken positionen spilleren har nu</param>
        public void PlayerPointMove(Vector2 a)
        {
            Point PTPosition = new Point((int)a.X, (int)a.Y);
            Position += new Vector2(PTPosition.X, PTPosition.Y);
        }
    }
}
