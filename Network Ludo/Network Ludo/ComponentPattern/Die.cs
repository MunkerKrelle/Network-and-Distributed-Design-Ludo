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
        private float roll;
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
            GameObject.Transform.Position = new Vector2(100, 100);
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Side1");
            GameObject.Transform.Layer = 0.9f;
        }
        public void Roll()
        {

            animator.PlayAnimation("Roll");
            roll = rnd.Next(1, 7);
            GameObject.Transform.Position += new Vector2((roll * 10), 0);

                if (roll == 1)
                {
                    animator.PlayAnimation("Idle1");
                }
                else if (roll == 2)
                {
                    animator.PlayAnimation("Idle2");
                }
                else if (roll == 3)
                {
                    animator.PlayAnimation("Idle3");
                }
                else if (roll == 4)
                {
                    animator.PlayAnimation("Idle4");
                }
                else if (roll == 5)
                {
                    animator.PlayAnimation("Idle5");
                }
                else if (roll == 6)
                {
                    animator.PlayAnimation("Idle6");
                }
            }

        }
    }
