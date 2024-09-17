using Network_Ludo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ComponentPattern
{
    /// <summary>
    /// Component til at lave animater i form af enkelte sprites
    /// </summary>
    public class Animator : Component
    {
        public int CurrentIndex { get; private set; } //Hvilket sprite der er nået til i listen

        private float timeElapsed;

        private SpriteRenderer spriteRenderer;

        private Dictionary<string, Animation> animations = new(); //Dictionary over de forskellige animationer de forskellige GameObjects kan have

        public Animation currentAnimation { get; private set; } //Hvilken animation der kører lige nu

        public Animator(GameObject gameObject) : base(gameObject)
        {
        }

        public override void Awake()
        {
            IsEnabled = true;
        }
        public override void Start()
        {
            spriteRenderer = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
        }

        public override void Update(GameTime gameTime) //Update
        {
            timeElapsed += GameWorld.Instance.DeltaTime;

            CurrentIndex = (int)(timeElapsed * currentAnimation.FPS);

            if (CurrentIndex > currentAnimation.Sprites.Length - 1)
            {
                timeElapsed = 0;
                CurrentIndex = 0;
            }
            spriteRenderer.Sprite = currentAnimation.Sprites[CurrentIndex];
            base.Update(gameTime);
        }

        /// <summary>
        /// Tilføjer en ny animation til listen over GameObjects forskellige animationer 
        /// </summary>
        /// <param name="animation">Ny animationer der skal tilføjes</param>
        public void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);

            if (currentAnimation == null)
            {
                currentAnimation = animation;
            }
        }

        /// <summary>
        /// Bliver brugt af andre klasser til afspille en specifik animation
        /// </summary>
        /// <param name="animationName">Hvilken animation der skal afspilles</param>
        public void PlayAnimation(string animationName)
        {
            if (animationName != currentAnimation.Name)
            {
                currentAnimation = animations[animationName];
                timeElapsed = 0;
                CurrentIndex = 0;
            }
        }
    }

}

/// <summary>
/// Oprettelse af animationer 
/// </summary>
public class Animation
{
    public float FPS { get; private set; }

    public string Name { get; private set; }

    public Texture2D[] Sprites { get; private set; }


    public Animation(string name, Texture2D[] sprites, float fps)
    {
        Sprites = sprites;
        Name = name;
        FPS = fps;
    }
}

