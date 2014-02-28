using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Color_Changer
{
    public class Circle
    {
        private Texture2D texture;
        public Color color;
        public Rectangle position;

        public Circle(Color c, Rectangle r,int radius,GraphicsDevice gd)
        {
            color = c;
            position = r;
            texture = CreateCircle(radius,gd);
        }

        public bool PointIsInRect(Point pt)
        {
            return pt.X >= position.X && pt.X <= position.X + position.Width && pt.Y >= position.Y && pt.Y <= position.Y + position.Height;
        }

        //current object is first select
        //parameter is second select
        public void HandleInteraction(Circle c)
        {
            if (this.color == Color.White)
            {
            }
            else if (c.color == Color.White)
            {
                c.color = this.color;
            }
            else if (isPrimaryColor(this.color) && isPrimaryColor(c.color))
            {
                c.color = CombineColors(this.color, c.color);
            }
            else if ((isPrimaryColor(this.color) && !isPrimaryColor(c.color)) || (!isPrimaryColor(this.color) && isPrimaryColor(c.color)))
            {
                c.color = Color.White;
            }
            else if (!isPrimaryColor(this.color) && !isPrimaryColor(c.color))
            {
                c.color = IntersectionColors(this.color, c.color);
            }
        }

        public static Color IntersectionColors(Color c1, Color c2)
        {
            if (c1 == c2)
                return c1;
            else
            {
                if ((c1 == Color.Purple && c2 == Color.Orange) || (c1 == Color.Orange && c2 == Color.Purple))
                    return Color.Red;
                else if ((c1 == Color.Purple && c2 == Color.Green) || (c1 == Color.Green && c2 == Color.Purple))
                    return Color.Blue;
                else if ((c1 == Color.Orange && c2 == Color.Green) || (c1 == Color.Green && c2 == Color.Orange))
                    return Color.Yellow;
            }
            return Color.White;
        }

        public static Color CombineColors(Color c1, Color c2)
        {
            if (c1 == c2)
                return c1;
            else
            {
                if ((c1 == Color.Blue && c2 == Color.Red) || (c1 == Color.Red && c2 == Color.Blue))
                    return Color.Purple;
                else if ((c1 == Color.Blue && c2 == Color.Yellow) || (c1 == Color.Yellow && c2 == Color.Blue))
                    return Color.Green;
                else if ((c1 == Color.Yellow && c2 == Color.Red) || (c1 == Color.Red && c2 == Color.Yellow))
                    return Color.Orange;
            }
            return Color.White;
        }

        public static bool isPrimaryColor(Color c)
        {
            return c == Color.Red || c == Color.Blue || c == Color.Yellow;
        }

        private Texture2D CreateCircle(int radius,GraphicsDevice gd)
        {
            int outerRadius = radius * 2 + 2;
            Texture2D texture = new Texture2D(gd, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];
            
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            double angleStep = 1f / (radius*3);
            int RAD = radius;
            for (; radius >= 0; radius--)
            {
                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    int x = (int)Math.Round(RAD + radius * Math.Cos(angle));
                    int y = (int)Math.Round(RAD + radius * Math.Sin(angle));

                    data[y * outerRadius + x + 1] = Color.White;
                }
            }

            texture.SetData(data);
            return texture;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, color);
        }

    }
}
