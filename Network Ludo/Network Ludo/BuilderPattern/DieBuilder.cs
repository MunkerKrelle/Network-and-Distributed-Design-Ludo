using ComponentPattern;
using Microsoft.Xna.Framework.Graphics;
using myClientTCP;

namespace BuilderPattern
{
    internal class DieBuilder : IBuilder
    {
        private GameObject gameObject;

        public void BuildGameObject()
        {
            gameObject = new GameObject();

            BuildComponents();
        }

        private void BuildComponents()
        {
            gameObject.AddComponent<Die>();
            gameObject.AddComponent<SpriteRenderer>();

            Animator animator = gameObject.AddComponent<Animator>();
            animator.AddAnimation(BuildAnimation("Idle1", new string[] { "Side1" }));
            animator.AddAnimation(BuildAnimation("Idle2", new string[] { "Side2" }));
            animator.AddAnimation(BuildAnimation("Idle3", new string[] { "Side3" }));
            animator.AddAnimation(BuildAnimation("Idle4", new string[] { "Side4" }));
            animator.AddAnimation(BuildAnimation("Idle5", new string[] { "Side5" }));
            animator.AddAnimation(BuildAnimation("Idle6", new string[] { "Side6" }));
            animator.AddAnimation(BuildAnimation("RollDie", new string[] { "Side1", "Side2", "Side3", "Side4", "Side5", "Side6" }));

        }

        /// <summary>
        /// Bygger en animation ved at indlæse de nødvendige sprites.
        /// </summary>
        /// <param name="animationName">Navnet på animationen.</param>
        /// <param name="spriteNames">En række af sprite-navne, der udgør animationen.</param>
        /// <returns>Den opbyggede animation.</returns>
        private Animation BuildAnimation(string animationName, string[] spriteNames)
        {
            Texture2D[] sprites = new Texture2D[spriteNames.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = ClientGameWorld.Instance.Content.Load<Texture2D>(spriteNames[i]);
            }

            Animation animation = new Animation(animationName, sprites, 5);

            return animation;
        }

        public GameObject GetResult()
        {
            return gameObject;
        }
    }
}
