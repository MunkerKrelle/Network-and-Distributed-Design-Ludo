using _2SemesterEksamen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace ComponentPattern
{
    /// <summary>
    /// Klasse for at oprette knapper der kan trykkes på og kører derefter en funktion
    /// </summary>
    public class Button : Component
    {
        private Vector2 minPosition;
        private Vector2 maxPosition;
        private SpriteRenderer sr;
        private Vector2 buttonPosition;
        private Vector2 originText;
        public bool active = true;
        string buttonText;
        public Action buttonAction;

        /// <summary>
        /// Opretter en knap man kan trykke på med musen som kan kører en Action
        /// </summary>
        /// <param name="buttonPosition">Knappens position på skærmen</param>
        /// <param name="buttonText">Tekst der bliver skrevet på knappen</param>
        /// <param name="buttonFunction">Hvilken function der bliver kørt når knappen bliver trykket</param>
        public Button(GameObject gameObject, Vector2 buttonPosition, string buttonText, Action buttonAction) : base(gameObject)
        {
            this.buttonPosition = buttonPosition;
            this.buttonText = buttonText;
            this.buttonAction = buttonAction;
        }

        public override void Update(GameTime gameTime)
        {
            MouseOnButton();
            MousePressed();
            PositionUpdate();
        }

        /// <summary>
        /// Opdatere positionen af boundaries hvor knappen kan trykkes på
        /// </summary>
        public void PositionUpdate()
        {
            minPosition.X = GameObject.Transform.Position.X - (sr.Sprite.Width / 2 * GameObject.Transform.Scale.X);
            minPosition.Y = GameObject.Transform.Position.Y - (sr.Sprite.Height / 2 * GameObject.Transform.Scale.Y);
            maxPosition.X = GameObject.Transform.Position.X + (sr.Sprite.Width / 2 * GameObject.Transform.Scale.X);
            maxPosition.Y = GameObject.Transform.Position.Y + (sr.Sprite.Height / 2 * GameObject.Transform.Scale.Y);
        }
        public override void Awake()
        {
            GameObject.IsActive = true;
        }
        public override void Start()
        {
            Debug.WriteLine("button started");
            sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            GameObject.Transform.Position = buttonPosition;
        }

        /// <summary>
        /// Chekker om mussen er inden for knappes boundaries
        /// </summary>
        public void MouseOnButton()
        {
            if (GameWorld.mouseState.X > minPosition.X && GameWorld.mouseState.Y > minPosition.Y && GameWorld.mouseState.X < maxPosition.X && GameWorld.mouseState.Y < maxPosition.Y)
            {
                GameObject.Transform.Color = Color.LightGray;
            }
            else
            {
                GameObject.Transform.Color = Color.White;
            }
        }

        /// <summary>
        /// Hvis mussen er på knappens boundaries og bliver trykket på, invoker den knappens Action
        /// </summary>
        public void MousePressed()
        {
            if (GameWorld.isPressed == true)
            {
                if (GameWorld.mouseState.X > minPosition.X && GameWorld.mouseState.Y > minPosition.Y && GameWorld.mouseState.X < maxPosition.X && GameWorld.mouseState.Y < maxPosition.Y)
                {
                    GameObject.Transform.Color = Color.Yellow;
                    buttonAction.Invoke();
                }
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 fontLength = GameWorld.font.MeasureString(buttonText);

            originText = new Vector2(fontLength.X / 2f, fontLength.Y / 2f);

            spriteBatch.DrawString(GameWorld.font, buttonText, buttonPosition, Color.Black, 0, originText, 1, SpriteEffects.None, 1f);

            spriteBatch.DrawString(GameWorld.font, $"{minPosition}", new Vector2(buttonPosition.X * 2, buttonPosition.Y + 100), Color.Black, 0, originText, 1, SpriteEffects.None, 1f);
            spriteBatch.DrawString(GameWorld.font, $"{maxPosition}", new Vector2(buttonPosition.X * 2, buttonPosition.Y + 120), Color.Black, 0, originText, 1, SpriteEffects.None, 1f);

        }
        
    }
}
