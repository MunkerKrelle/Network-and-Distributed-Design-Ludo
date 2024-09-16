using Network_Ludo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentPattern
{
    /// <summary>
    /// Repræsenterer spillerkomponenten, der styrer spillerens bevægelser, animationer og interaktioner.
    /// </summary>
    public class Player : Component
    {
        Animator animator;

        /// <summary>
        /// Initialiserer en ny instans af <see cref="Player"/> klassen med det specificerede game object.
        /// </summary>
        /// <param name="gameObject">Det game object, som spilleren er knyttet til.</param>
        public Player(GameObject gameObject) : base(gameObject)
        {

        }

        public override void Awake()
        {

        }

        /// <summary>
        /// Initialiserer spillerens startposition og grafiske elementer.
        /// </summary>
        public override void Start()
        {

        }

        /// <summary>
        /// Opdaterer spillerens tilstand i hver frame.
        /// </summary>
        /// <param name="gameTime">Spillets tid, der er gået siden sidste opdatering.</param>
        public override void Update(GameTime gameTime)
        {
        }
    }
}
