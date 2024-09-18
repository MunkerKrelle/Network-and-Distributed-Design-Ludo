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

        public GameObject Create(Color color, string name)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite("LudoPiece");
            go.Transform.Position = new Vector2(200, 600);
            go.Transform.Scale = new Vector2(0.1f, 0.1f);
            go.Transform.Color = color;
            go.AddComponent<LudoPiece>(color, name);

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
