#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Color_Changer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        enum GameLocation { PLAYING, MENU}
        private GameLocation location = GameLocation.MENU;

        private Rectangle playButtonRect = new Rectangle(100, 100, 150, 50);
        private Texture2D playButtonTexture;
        private bool playButtonDown = false;

        private Rectangle randButtonRect = new Rectangle(100, 200, 150, 50);
        private Texture2D randButtonTexture;
        private bool randButtonDown = false;

        private const int ROWSIZE = 3, COLSIZE = 3;
        private const int CIRCLE_RADIUS = 50;
        private const int MARGIN = 10;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Circle[,] circles;
        Circle[,] solution;
        Point boardOffset = new Point(50, 75);
        Point solutionOffset = new Point(600, 100);
        private Random rand = new Random();

        Circle from = null;
        bool firstFound = false;
        bool backDown = false;

        List<string> levels;
        int currLevel = 0;

        private class Tuple
        {
            public Point p;
            public Color c;
            public Tuple(Point p1, Color c1)
            {
                p = p1;
                c = c1;
            }
        }
        Stack<Tuple> lastMoves;

        enum Colors { RED, ORANGE, YELLOW, GREEN, BLUE, PURPLE }

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            circles = new Circle[ROWSIZE, COLSIZE];
            solution = new Circle[ROWSIZE, COLSIZE];

            if (levels != null)
            {
                InitalizeLevels(levels);
                string level = levels[0];
                InitializeBoard(circles, level.Substring(0, level.IndexOf(';')));
                InitializeSolution(solution, level.Substring(level.IndexOf(';') + 1));
            }
            else
            {
                InitializeBoard(circles, "");
                InitializeSolution(solution, "");
            }
            lastMoves = new Stack<Tuple>();
            base.Initialize();
        }

        private void GotoNextLevel()
        {
            currLevel += 1;
            if (currLevel > levels.Count - 1)
                location = GameLocation.MENU;
            else
            {
                string level = levels[currLevel];
                InitializeBoard(circles, level.Substring(0, level.IndexOf(';')));
                InitializeSolution(solution, level.Substring(level.IndexOf(';') + 1));
                lastMoves = new Stack<Tuple>();
            }
        }

        private void InitializeBoard(Circle[,] board, string level)
        {
            if (level == null || level.Length == 0)
            {
                int d = 2 * CIRCLE_RADIUS;
                for (int row = 0; row < ROWSIZE; row++)
                    for (int col = 0; col < COLSIZE; col++)
                        board[row, col] = new Circle(EnumToColor((Colors)(rand.Next(0, 3) * 2)), new Rectangle(col * (d + MARGIN) + boardOffset.X, row * (d + MARGIN) + boardOffset.Y, d, d), CIRCLE_RADIUS, GraphicsDevice);
            }
            else
            {
                int d = 2 * CIRCLE_RADIUS;
                for (int row = 0; row < ROWSIZE; row++)
                    for (int col = 0; col < COLSIZE; col++)
                        board[row, col] = new Circle(CharToColor(level[3 * row + col]), new Rectangle(col * (d + MARGIN) + boardOffset.X, row * (d + MARGIN) + boardOffset.Y, d, d), CIRCLE_RADIUS, GraphicsDevice);
            }
        }

        private void InitializeSolution(Circle[,] board, string level)
        {
            if (level == null || level.Length == 0)
            {
                int d = CIRCLE_RADIUS;
                for (int row = 0; row < ROWSIZE; row++)
                    for (int col = 0; col < COLSIZE; col++)
                        board[row, col] = new Circle(EnumToColor((Colors)(rand.Next(0, 3) * 2 + 1)), new Rectangle(col * (d + MARGIN / 2) + solutionOffset.X, row * (d + MARGIN / 2) + solutionOffset.Y, d, d), CIRCLE_RADIUS / 2, GraphicsDevice);
            }
            else
            {
                int d = CIRCLE_RADIUS;
                for (int row = 0; row < ROWSIZE; row++)
                    for (int col = 0; col < COLSIZE; col++)
                        board[row, col] = new Circle(CharToColor(level[3 * row + col]), new Rectangle(col * (d + MARGIN / 2) + solutionOffset.X, row * (d + MARGIN / 2) + solutionOffset.Y, d, d), CIRCLE_RADIUS / 2, GraphicsDevice);
            }
        }

        private void InitalizeLevels(List<string> levels)
        {
            levels.Add("RBYBYRYRB;PPPGGGOOO");
            levels.Add("RBYBRBYBR;POGPRPGOP");
            levels.Add("RBYYRBBYR;OPOGOPGGO");
        }

        #region EnumToColor

        private Color EnumToColor(Colors c)
        {
            switch (c)
            {
                case Colors.RED:
                    return Color.Red;
                case Colors.ORANGE:
                    return Color.Orange;
                case Colors.YELLOW:
                    return Color.Yellow;
                case Colors.GREEN:
                    return Color.Green;
                case Colors.BLUE:
                    return Color.Blue;
                case Colors.PURPLE:
                    return Color.Purple;
                default:
                    return Color.White;
            }
        }

        #endregion

        #region CharToColor

        private Color CharToColor(char c)
        {
            switch (c)
            {
                case 'R':
                    return Color.Red;
                case 'O':
                    return Color.Orange;
                case 'Y':
                    return Color.Yellow;
                case 'G':
                    return Color.Green;
                case 'B':
                    return Color.Blue;
                case 'P':
                    return Color.Purple;
                default:
                    return Color.White;
            }
        }

        #endregion

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playButtonTexture = this.Content.Load<Texture2D>("play.png");
            randButtonTexture = this.Content.Load<Texture2D>("random.png");
        }

        protected override void UnloadContent()
        {

        }

        private void UndoMove()
        {
            if (lastMoves.Count > 0)
            {
                Tuple t = lastMoves.Pop();
                circles[t.p.X, t.p.Y].color = new Color(t.c.R, t.c.G, t.c.B);
            }
        }

        private bool SolutionFound()
        {
            for (int row = 0; row < ROWSIZE; row++)
                for (int col = 0; col < COLSIZE; col++)
                    if (circles[row, col].color != solution[row, col].color)
                        return false;
            return true;
        }

        public bool PointIsInRect(Rectangle r, Point pt)
        {
            return pt.X >= r.X && pt.X <= r.X + r.Width && pt.Y >= r.Y && pt.Y <= r.Y + r.Height;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                location = GameLocation.MENU;

            if (location == GameLocation.MENU)
            {
                MouseState mouse = Mouse.GetState();
                Point m = new Point(mouse.X,mouse.Y);
                if (mouse.LeftButton == ButtonState.Pressed && PointIsInRect(playButtonRect, m))
                    playButtonDown = true;

                if (mouse.LeftButton == ButtonState.Released && playButtonDown && PointIsInRect(playButtonRect, m))
                {
                    levels = new List<string>();
                    playButtonDown = false;
                    location = GameLocation.PLAYING;
                    Initialize();
                }

                if (mouse.LeftButton == ButtonState.Pressed && PointIsInRect(randButtonRect, m))
                    randButtonDown = true;

                if (mouse.LeftButton == ButtonState.Released && randButtonDown && PointIsInRect(randButtonRect, m))
                {
                    levels = null;
                    randButtonDown = false;
                    location = GameLocation.PLAYING;
                    Initialize();
                }
            }
            else if (location == GameLocation.PLAYING)
            {
                if (SolutionFound())
                {
                    if (levels == null)
                        location = GameLocation.MENU;
                    else
                        GotoNextLevel();
                }
                MouseState mouse = Mouse.GetState();
                KeyboardState keyboard = Keyboard.GetState();

                if (keyboard.IsKeyDown(Keys.Back))
                    backDown = true;
                if (backDown && keyboard.IsKeyUp(Keys.Back))
                {
                    backDown = false;
                    UndoMove();
                }

                if (mouse.LeftButton == ButtonState.Pressed && !firstFound)
                {
                    Point pt = new Point(mouse.X, mouse.Y);
                    foreach (Circle c in circles)
                        if (c.PointIsInRect(pt))
                        {
                            from = c;
                            firstFound = true;
                            break;
                        }
                }

                if (mouse.LeftButton == ButtonState.Released && firstFound)
                {
                    Circle to = null;
                    Point pt = new Point(mouse.X, mouse.Y);
                    foreach (Circle c in circles)
                        if (c.PointIsInRect(pt))
                        {
                            to = c;
                            break;
                        }
                    if (to != null)
                    {
                        double dist = Math.Sqrt((to.position.X - from.position.X) * (to.position.X - from.position.X) + (to.position.Y - from.position.Y) * (to.position.Y - from.position.Y));
                        if (dist == (2 * CIRCLE_RADIUS + MARGIN))
                        {
                            lastMoves.Push(new Tuple(new Point((to.position.Y - boardOffset.Y) / (2 * CIRCLE_RADIUS + MARGIN), (to.position.X - boardOffset.X) / (2 * CIRCLE_RADIUS + MARGIN)), to.color));
                            from.HandleInteraction(to);
                        }
                    }
                    firstFound = false;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (location == GameLocation.MENU)
            {
                spriteBatch.Draw(playButtonTexture, playButtonRect, Color.White);
                spriteBatch.Draw(randButtonTexture, randButtonRect, Color.White);
            }
            else if (location == GameLocation.PLAYING)
            {
                foreach (Circle c in circles)
                    c.Draw(spriteBatch);
                foreach (Circle c in solution)
                    c.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
