using ComponentPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FactoryPattern
{
    /// <summary>
    /// Fabrik til opbygning af våben
    /// </summary>
    class LudoPieceFactory : Factory
    {
        private static LudoPieceFactory instance;

        public static LudoPieceFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LudoPieceFactory();
                }
                return instance;
            }
        }

        private GameObject prototype;

        public GameObject Create(string name, Color color)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite("LudoPiece");
            go.Transform.Scale = new Vector2(0.05f, 0.05f);
            go.Transform.Color = color;
            go.Transform.Layer = 1f;
            go.AddComponent<LudoPiece>(name, color);

            return go;
        }

        /// <summary>
        /// Default create til fabrik interface
        /// </summary>
        /// <returns></returns>
        public override GameObject Create()
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite("LudoPiece");
            go.Transform.Layer = 0.7f;
            go.AddComponent<LudoPiece>();
            return go;
        }
    }
}
