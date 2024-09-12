using ComponentPattern;
using Microsoft.Xna.Framework;
using System;

namespace FactoryPattern
{
    /// <summary>
    /// Fabrik der bygger knapper med forskellige parametre
    /// </summary>
    class ButtonFactory : Factory
    {
        private static ButtonFactory instance;

        public static ButtonFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ButtonFactory();
                }
                return instance;
            }
        }

        /// <summary>
        /// Bygger en ny knap til GameWorld
        /// </summary>
        /// <param name="buttonPosition">Knappes position</param>
        /// <param name="buttonText">Hvilken tekst der skal stå på knappen</param>
        /// <param name="actionFunction">Hvilken Action der skal køre når knappen bliver trykket på</param>
        /// <returns></returns>
        public GameObject Create(Vector2 buttonPosition, string buttonText, Action actionFunction)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite("button");
            go.AddComponent<Button>(buttonPosition, buttonText, actionFunction);

            return go;
        }

        /// <summary>
        /// Default create til interface
        /// </summary>
        /// <returns></returns>
        public override GameObject Create()
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.SetSprite("button");
            go.AddComponent<Weapon>();
            return go;
        }
    }
}
