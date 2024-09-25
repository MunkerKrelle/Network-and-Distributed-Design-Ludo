using Microsoft.Xna.Framework;
using myClientTCP;
using System;

namespace ComponentPattern
{
    /// <summary>
    /// At one point the Die was a part of the game, however, after mergin the game and restructering the code this is no longer the case.
    /// We didn't have time to implement the die again
    /// </summary>
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

        /// <summary>
        /// The many if else statements were tempoary. It was changed to a switch case but this was lost in a merge.
        /// We didn't prioritize changing it back to a switch case, as the die was left out of the final product
        /// </summary>
        /// <returns></returns>
        public int RollDie()
        {

            animator.PlayAnimation("RollDie");
            

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
