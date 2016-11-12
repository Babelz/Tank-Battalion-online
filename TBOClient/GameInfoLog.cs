using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOClient
{
    public enum EntryType : int
    {
        Message = 0,
        Warning,
        Error
    }

    public sealed class GameInfoLog : DrawableGameComponent
    {
        #region Const fields
        private const int EntryDecayTime = 10 * 1000;
        private const float Offset       = 16.0f;
        #endregion

        #region Info log entry class
        private sealed class InfoLogEntry
        {
            public int       elapsed;
            public string    contents;
            public EntryType type;
        }
        #endregion

        #region Static fields
        private static readonly Color[] colors;
        #endregion

        #region Fields
        private readonly List<InfoLogEntry> entries;

        private SpriteBatch spriteBatch;
        
        private SpriteFont font;
        #endregion

        static GameInfoLog()
        {
            colors = new Color[]
            {
                Color.Green,
                Color.Yellow,
                Color.Red
            };
        }

        public GameInfoLog(Game game) 
            : base(game)
        {
            entries = new List<InfoLogEntry>();
        }

        protected override void LoadContent()
        {
            font        = Game.Content.Load<SpriteFont>("info log");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public void AddEntry(EntryType type, string contents)
        {
            entries.Add(new InfoLogEntry()
            {
                contents = contents,
                type     = type
            });
        }

        public override void Update(GameTime gameTime)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];

                if (entry.elapsed >= EntryDecayTime)    entries.RemoveAt(i);
                else                                    entry.elapsed += gameTime.ElapsedGameTime.Milliseconds;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            var position = Vector2.Zero;

            spriteBatch.Begin();

            foreach (var entry in entries)
            {
                spriteBatch.DrawString(font, entry.contents, position, colors[(int)entry.type]);

                position.Y += font.MeasureString(entry.contents).Y + Offset;
            }

            spriteBatch.End();
        }
    }
}
