using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ComponentPattern
{
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
        public override void Awake()
        {
            base.GameObject.AddComponent<SpriteRenderer>();
            sr = base.GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Pixel");
            base.Awake();
        }
        public override void Start()
        {
            
        }


        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();

            if (_currentMouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                
                Point mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

                
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            float cellLayerDepth = 0.5f;  // Layer depth for cells (tiles)
            float lineLayerDepth = 0.6f;  // Layer depth for grid lines (drawn over tiles)

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Tegn selve cellen med en lavere layerDepth (baggrund)
                    spriteBatch.Draw(
                        GetTexture(spriteBatch.GraphicsDevice, cellColors[row, col]),
                        grid[row, col],
                        null,
                        cellColors[row, col],
                        0f,
                        Vector2.Zero,
                        SpriteEffects.None,
                        cellLayerDepth
                    );

                    int lineWidth = 2;  

                    
                    spriteBatch.Draw(sr.Sprite,new Rectangle(grid[row, col].Left, grid[row, col].Top, lineWidth, cellSize), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);

                    
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Left, grid[row, col].Top, cellSize, lineWidth), null, Color.Black, 0f,Vector2.Zero, SpriteEffects.None, lineLayerDepth);

                    
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Right - lineWidth, grid[row, col].Top, lineWidth, cellSize), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth
                    );

                    
                    spriteBatch.Draw(sr.Sprite, new Rectangle(grid[row, col].Left, grid[row, col].Bottom - lineWidth, cellSize, lineWidth), null, Color.Black, 0f, Vector2.Zero, SpriteEffects.None, lineLayerDepth);
                }
            }
        }





        private Texture2D GetTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
    }
}

