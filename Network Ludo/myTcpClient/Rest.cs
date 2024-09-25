using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoServer
{
    public class Rest
    {
        List<Color> colors = new List<Color> { Color.White, Color.Black, Color.Red, Color.Purple, Color.PaleGreen, Color.Yellow, Color.Orange, Color.Pink };

        string colorCode = "151312";

        public void StringToColor(string colorString)
        {


            if (colorCode.Length == 3 &&
    byte.TryParse(colorString[0], out byte r) &&
    byte.TryParse(colorString[1], out byte g) &&
    byte.TryParse(colorString[2], out byte b))
            {
                Color color = new Color(r, g, b);
                // Use the color as needed
            }
        }

        public bool GetColor(Color color)
        {
            foreach (Color c in colors)
            {
                if (c == color)
                {
                    colors.Remove(c);
                    return true;
                }
            }
            return false;
        }
    }
}
