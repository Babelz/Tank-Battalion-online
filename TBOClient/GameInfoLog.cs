using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBOClient
{
    public sealed class GameInfoLog : DrawableGameComponent
    {
        #region Info log entry class
        private sealed class InfoLogEntry
        {
            public int    elapsed;
            public string contents;
        }
        #endregion

        #region Fields
        private readonly List<InfoLogEntry> entries;

        private readonly SpriteFont font;
        #endregion

        public GameInfoLog(Game game) 
            : base(game)
        {
            entries = new List<InfoLogEntry>();
        }

        protected override void LoadContent()
        {
            Game.Content.Load<SpriteFont>("console");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
