using ComponentPattern;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Ludo.ComponentPattern
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

        public Grid(GameObject gameObject, int rows, int cols, int cellSize) : base(gameObject)
        {
            this.rows = rows;
            this.cols = cols;
            this.cellSize = cellSize;

            // Initialize the grid and cell colors
            grid = new Rectangle[rows, cols];
            cellColors = new Color[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = new Rectangle(col * cellSize, row * cellSize, cellSize, cellSize);
                    cellColors[row, col] = Color.White; // Default cell color
                }
            }
        }

        public override void Start()
        {
            
        }

        public override void LoadContent()
        {
            // Create a 1x1 pixel texture for drawing lines in the LoadContent method
            pixelTexture = new Texture2D(GameWorld.Instance.GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();

            if (_currentMouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                // Get the mouse position
                Point mousePosition = new Point(_currentMouseState.X, _currentMouseState.Y);

                // Check if any cell was clicked
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (grid[row, col].Contains(mousePosition))
                        {
                            // Toggle cell color
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
            // Draw each cell in the grid
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Draw the cell
                    spriteBatch.Draw(GetTexture(spriteBatch.GraphicsDevice, cellColors[row, col]), grid[row, col], cellColors[row, col]);

                    // Draw the vertical line (right side of the cell)
                    spriteBatch.Draw(pixelTexture, new Rectangle(grid[row, col].Right, grid[row, col].Top, 1, cellSize), Color.Black);

                    // Draw the horizontal line (bottom side of the cell)
                    spriteBatch.Draw(pixelTexture, new Rectangle(grid[row, col].Left, grid[row, col].Bottom, cellSize, 1), Color.Black);
                }
            }

            // Draw the right-most vertical and bottom-most horizontal border lines
            spriteBatch.Draw(pixelTexture, new Rectangle(grid[0, cols - 1].Right, grid[0, cols - 1].Top, 1, rows * cellSize), Color.Black);
            spriteBatch.Draw(pixelTexture, new Rectangle(grid[rows - 1, 0].Left, grid[rows - 1, 0].Bottom, cols * cellSize, 1), Color.Black);
        }

        // Utility function to create a 1x1 texture with a specific color
        private Texture2D GetTexture(GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
    }
}

