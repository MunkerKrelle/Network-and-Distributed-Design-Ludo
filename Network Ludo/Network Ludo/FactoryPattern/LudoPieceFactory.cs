using ComponentPattern;
using Network_Ludo.ComponentPattern;
using System.Drawing;

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

        public GameObject Create(Color color)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            //sr.SetSprite();
            go.Transform.Layer = 0.7f;
            go.AddComponent<LudoPiece>(color);

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
            sr.SetSprite("");
            go.Transform.Layer = 0.7f;
            go.AddComponent<LudoPiece>();
            return go;
        }
    }
}
