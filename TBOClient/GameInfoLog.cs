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

        private SpriteFont font;
        #endregion

        public GameInfoLog(Game game) 
            : base(game)
        {
            entries = new List<InfoLogEntry>();
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("console");
        }

        public override void Update(GameTime gameTime)
        {
            for (var i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                
            }
        }
        public override void Draw(GameTime gameTime)
        {
        }
    }
}
