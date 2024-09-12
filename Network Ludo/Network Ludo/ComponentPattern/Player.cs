using _2SemesterEksamen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RepositoryPattern;

namespace ComponentPattern
{
    /// <summary>
    /// Repræsenterer spillerkomponenten, der styrer spillerens bevægelser, animationer og interaktioner.
    /// </summary>
    public class Player : Component
    {
        private float speed;
        protected int health;
        public int damage;
        private int currentInvSlot;
        private int scraps;
        Animator animator;
        public Inventory inventory;
        IRepository database = IRepository.currentRepository;

        bool isAlive = true;

        /// <summary>
        /// Får eller sætter spillerens sundhed. Hvis sundheden når 0 eller derunder, vil spilleren dø.
        /// </summary>
        public int Health
        {
            get { return health; }
            set
            {
                health = value;
                if (health <= 0 && isAlive)
                {
                    isAlive = false;
                    Die();
                }
            }
        }

        /// <summary>
        /// Initialiserer en ny instans af <see cref="Player"/> klassen med det specificerede game object.
        /// </summary>
        /// <param name="gameObject">Det game object, som spilleren er knyttet til.</param>
        public Player(GameObject gameObject) : base(gameObject)
        {

        }



        /// <summary>
        /// Flytter spilleren i henhold til den angivne hastighedsvektor.
        /// </summary>
        /// <param name="velocity">Hastighedsvektoren, som spilleren skal flytte sig med.</param>
        public void Move(Vector2 velocity)
        {
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();

            }

            velocity *= speed;

            GameObject.Transform.PlayerPointMove(velocity);

            if (velocity.X > 0)
            {
                animator.PlayAnimation("PlayerMove");
                GameObject.Transform.SpriteEffect = SpriteEffects.None;
                if (inventory.weaponsList.Count > 0)
                {
                    inventory.weaponsList[currentInvSlot].GameObject.Transform.SpriteEffect = SpriteEffects.None;
                }
            }
            else if (velocity.X < 0)
            {
                animator.PlayAnimation("PlayerMove");
                GameObject.Transform.SpriteEffect = SpriteEffects.FlipHorizontally;
                if (inventory.weaponsList.Count > 0)
                {
                    inventory.weaponsList[currentInvSlot].GameObject.Transform.SpriteEffect = SpriteEffects.FlipHorizontally;
                }
            }
        }

        /// <summary>
        /// Initialiserer spillerens egenskaber, når komponenten vågner.
        /// </summary>
        public override void Awake()
        {
            GameObject.IsActive = true;
            speed = 100;
            health = 10000;
            scraps = database.UpdateScraps();
            animator = GameObject.GetComponent<Animator>() as Animator;
            animator.PlayAnimation("Idle");
            GameObject.Transform.Scale = new Vector2(3f, 3f);
            inventory = GameObject.GetComponent<Inventory>() as Inventory;
            inventory.Active = true;
        }

        /// <summary>
        /// Initialiserer spillerens startposition og grafiske elementer.
        /// </summary>
        public override void Start()
        {
            GameObject.Transform.Position = new Vector2(300, 300);
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Player/Idle/Idle1");
            GameObject.Transform.Layer = 0.9f;
        }

        /// <summary>
        /// Opdaterer spillerens tilstand i hver frame.
        /// </summary>
        /// <param name="gameTime">Spillets tid, der er gået siden sidste opdatering.</param>
        public override void Update(GameTime gameTime)
        {
            if (currentInvSlot >= inventory.weaponsList.Count)
            {
                currentInvSlot = 0;
            }

            if (inventory.weaponsList.Count != 0 && animator.currentAnimation.Name != "Attack")
            {
                inventory.weaponsList[currentInvSlot].GameObject.Transform.Layer = 0.5f;
                inventory.weaponsList[currentInvSlot].GameObject.Transform.Position = GameObject.Transform.Position;
            }
            else if (inventory.weaponsList.Count != 0 && animator.currentAnimation.Name == "Attack")
            {
                inventory.weaponsList[currentInvSlot].GameObject.Transform.Layer = 1f;
                if (inventory.weaponsList[currentInvSlot].GameObject.Transform.SpriteEffect == SpriteEffects.None)
                {
                    inventory.weaponsList[currentInvSlot].GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X + (animator.CurrentIndex * 8), inventory.weaponsList[currentInvSlot].GameObject.Transform.Position.Y);
                }
                else
                {
                    inventory.weaponsList[currentInvSlot].GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X - (animator.CurrentIndex * 8), inventory.weaponsList[currentInvSlot].GameObject.Transform.Position.Y);
                }
            }

            if (Database.playerItemsUpdated == true)
            {
                scraps = database.UpdateScraps();
                inventory.AddItem(database.AddToInventory());
                Database.playerItemsUpdated = false;

            }
        }

        public void ChangeItem()
        {
            foreach (var item in inventory.weaponsList)
            {
                item.GameObject.Transform.Position = new Vector2(-100, -100);
            }
            currentInvSlot++;
        }

        /// <summary>
        /// Udfører spillerens angrebsanimation.
        /// </summary>
        public void Attack()
        {
            animator.PlayAnimation("Attack");
            if (inventory.weaponsList.Count > 0)
            {
                damage = 1 + inventory.weaponsList[currentInvSlot].Damage;
            }
            else
            {
                damage = 1;
            }
        }

        /// <summary>
        /// Håndterer kollisioner med andre objekter.
        /// </summary>
        /// <param name="col">Kollisionen, som spilleren er involveret i.</param>
        public override void OnCollisionEnter(Collider col)
        {
            Enemy enemy = (Enemy)col.GameObject.GetComponent<Enemy>();

            if (enemy != null && animator.currentAnimation.Name == "Attack" && animator.CurrentIndex < 3)
            {
                enemy.Health -= damage;
            }


            base.OnCollisionEnter(col);
        }

        /// <summary>
        /// Håndterer spillerens død ved at vise en respawn-knap og destruere spillerens game object.
        /// </summary>
        private void Die()
        {
            GameWorld.Instance.CreateRespawnButton(); 
            GameWorld.Instance.Destroy(GameObject);  
        }

        /// <summary>
        /// Genopliver spilleren ved at genoprette helbred og position.
        /// </summary>
        public void Respawn()
        {
            Health = 10000;
            GameObject.Transform.Position = new Vector2(300, 300);
            GameWorld.Instance.Instantiate(GameObject);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(GameWorld.font, $"Health: {health}\nPlayer Scraps: {scraps}", new Vector2(1100, 50), Color.Black);
        }


    }
}
