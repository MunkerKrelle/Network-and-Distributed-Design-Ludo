using ComponentPattern;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Ludo.ComponentPattern
{
    internal class Die : Component
    {
        private Random rnd;
        private int roll;
        private Animator animator;
        private float fps;

        public Die(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            animator = GameObject.GetComponent<Animator>() as Animator;
            GameObject.Transform.Scale = new Vector2(.5f, .5f);
            GameObject.IsActive = true;
        }

        public override void Start()
        {
            GameObject.Transform.Position = new Vector2(100, 100);
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Side1");
            GameObject.Transform.Layer = 0.9f;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public void Roll()
        {
            roll = rnd.Next(1, 7);
            //BRIK RYK ALT EFTER VÆRDI AF ROLL
        }
    }
}
