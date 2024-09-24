using myClientTCP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FactoryPattern;
using System.Collections.Generic;

namespace ComponentPattern
{
    /// <summary>
    /// Repræsenterer spillerkomponenten, der styrer spillerens bevægelser, animationer og interaktioner.
    /// </summary>
    public class Player : Component
    {
        private Animator animator;

        public List<GameObject> ludoPieces = new List<GameObject>();

        public string playerName;

        public Color color;
        public Vector2 pos; 

        /// <summary>
        /// Initialiserer en ny instans af <see cref="Player"/> klassen med det specificerede game object.
        /// </summary>
        /// <param name="gameObject">Det game object, som spilleren er knyttet til.</param>
        public Player(GameObject gameObject, string playerName, Color color, Vector2 pos) : base(gameObject)
        {
            this.playerName = playerName;
            this.color = color;
            this.pos = pos;
        }

        public override void Awake()
        {
            for (int i = 0; i < 4; i++)
            {
                ludoPieces.Add(LudoPieceFactory.Instance.Create(color, playerName));
                ludoPieces[i].Transform.Position = new Vector2(pos.X - 100 + 70 * i, pos.Y + 50);
                ClientGameWorld.Instance.Instantiate(ludoPieces[i]);

            }
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
