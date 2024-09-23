using Microsoft.Xna.Framework;
using Network_Ludo;
using System;

namespace ComponentPattern
{
    internal class Die : Component
    {
        private Random rnd;
        public int Roll { get; set; }
        private Animator animator;

        public Die(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            animator = GameObject.GetComponent<Animator>() as Animator;
            GameObject.Transform.Scale = new Vector2(.5f, .5f);
            GameObject.IsActive = true;
            rnd = new Random();
        }

        public override void Start()
        {
            GameObject.Transform.Position = new Vector2(100, 800);
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Side1");
            GameObject.Transform.Layer = 0.9f;
        }
        public int RollDie()
        {

            animator.PlayAnimation("RollDie");
            Roll = rnd.Next(1, 7);

            if (Roll == 1)
            {
                animator.PlayAnimation("Idle1");
                
                return Roll;
            }
            else if (Roll == 2)
            {
                animator.PlayAnimation("Idle2");
                return Roll;

            }
            else if (Roll == 3)
            {
                animator.PlayAnimation("Idle3");
                return Roll;

            }
            else if (Roll == 4)
            {
                animator.PlayAnimation("Idle4");
                return Roll;

            }
            else if (Roll == 5)
            {
                animator.PlayAnimation("Idle5");
                return Roll;

            }
            else if (Roll == 6)
            {
                animator.PlayAnimation("Idle6");
                return Roll;

            }
            else
            {
                return Roll;
            }
        }

    }
}
