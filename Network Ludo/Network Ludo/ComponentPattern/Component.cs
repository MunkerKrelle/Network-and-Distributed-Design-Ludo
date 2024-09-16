using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ComponentPattern
{
    /// <summary>
    /// Component bliver brugt til sammensætte objekter i træstrukturer og derefter arbejde med disse strukturer, som om de var individuelle objekter 
    /// </summary>
    public abstract class Component : ICloneable
    {
        public bool IsEnabled { get; set; }

        public GameObject GameObject { get; private set; }

        public Component(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        // Metode der kaldes når komponenten bliver oprettet
        public virtual void Awake()
        {
            
        }

        // Metode der kaldes når spillet starter
        public virtual void Start()
        {

        }

        public virtual void LoadContent()
        {

        }
        // Metode der kaldes for at opdatere komponenten
        public virtual void Update(GameTime gameTime)
        {

        }

        // Metode der kaldes for at tegne komponenten
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        // Metode der laver en klon af komponenten
        public virtual object Clone()
        {
            Component component = (Component)MemberwiseClone();
            component.GameObject = GameObject;
            return component;
        }

        // Metode der sætter en ny GameObject for komponenten
        public virtual void SetNewGameObject(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        //// Metode der kaldes når der sker en kollision med en anden Collider
        //public virtual void OnCollisionEnter(Collider col)
        //{

        //}
    }
}
