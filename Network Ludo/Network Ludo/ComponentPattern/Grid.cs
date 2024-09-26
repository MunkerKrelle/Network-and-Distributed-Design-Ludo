using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ComponentPattern
{
    /// <summary>
    /// Represents a grid component that can be used to display a grid of cells in the game.
    /// Each cell can change its color when clicked and the grid is drawn with grid lines.
    /// </summary>
    public class Grid : Component
    {
        private int rows;
        private int cols;
        private int cellSize;
        private Rectangle[,] grid;
        private Color[,] cellColors;

        private Texture2D pixelTexture;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        private SpriteRenderer sr;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid"/> class.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is attached to.</param>
        /// <param name="rows">The number of rows in the grid.</param>
        /// <param name="cols">The number of columns in the grid.</param>
        /// <param name="cellSize">The size of each cell in the grid.</param>
        public Grid(GameObject gameObject, int rows, int cols, int cellSize) : base(gameObject)
        {
            this.rows = rows;
            this.cols = cols;
            this.cellSize = cellSize;

            grid = new Rectangle[rows, cols];
            cellColors = new Color[rows, cols];

            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = new Rectangle(col * cellSize, row * cellSize, cellSize, cellSize);
                    cellColors[row, col] = Color.White;
                }
            }
        }

        /// <summary>
        /// Called when the Grid component is being initialized.
        /// Adds a SpriteRenderer component and sets the default sprite to "Pixel".
        /// </summary>
        public override void Awake()
        {
            base.GameObject.AddComponent<SpriteRenderer>();
            sr = base.GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Pixel");
            base.Awake();
        }

        /// <summary>
        /// Called when the Grid component starts. Can be used to initialize settings before the game loop begins.
        /// </summary>
        public override void Start()
        {
            
        }

        /// <summary>
        /// Updates the grid on each frame. Handles mouse input to toggle cell colors when clicked.
        /// </summary>
        /// <param name="gameTime">The GameTime object providing timing information.</param>
        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();

            // Check for a mouse click and toggle the cell color
            if (_currentMouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                Point mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

                // Check which grid cell was clicked and toggle its color
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (grid[row, col].Contains(mousePosition))
                        {
                            if (cellColors[row, col] == Color.White)
                                cellColors[row, col] = Color.Red;
                            else
                                cellColors[row, col] = Color.White;
                        }
                    }
                }
            }

            _previousMouseState = _currentMouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the grid of cells and the grid lines on the screen.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw the textures.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float cellLayerDepth = 0.5f;  
            float lineLayerDepth = 0.6f;  

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    
                    spriteBatch.Draw(GetTexture(spriteBatch.GraphicsDevice, cellColors[row, col]), grid[row, col], null, cellColors[row, col], 0f, Vector2.Zero, SpriteEffects.None, cellLayerDepth);

                    int lineWidth = 2;

                    
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Left, grid[row, col].Top, lineWidth, cellSize), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Left, grid[row, col].Top, cellSize, lineWidth), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Right - lineWidth, grid[row, col].Top, lineWidth, cellSize), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Left, grid[row, col].Bottom - lineWidth, cellSize, lineWidth), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);
                }
            }
        }

        /// <summary>
        /// Creates a 1x1 texture of the given color.
        /// </summary>
        /// <param name="graphicsDevice">The GraphicsDevice used to create the texture.</param>
        /// <param name="color">The color of the texture.</param>
        /// <returns>A 1x1 Texture2D of the given color.</returns>
        private Texture2D GetTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
    }
}

