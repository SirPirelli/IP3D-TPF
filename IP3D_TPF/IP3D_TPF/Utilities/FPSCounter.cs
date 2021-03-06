﻿using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    class FPSCounter
    {
        /* frame counter vars */
        SpriteFont _spr_font;
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;
        /*--------------------*/

        public FPSCounter() { }

        public void LoadContent(ContentManager content)
        {
            _spr_font = content.Load<SpriteFont>("Font");
        }

        public void Update(GameTime gameTime)
        {
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
        }

        public void Draw(SpriteFont font ,SpriteBatch spriteBatch, bool collision, int cameraIndex, Tank tank)
        {
            _total_frames++;

            spriteBatch.Begin();

            spriteBatch.DrawString(font,  _fps.ToString(), new Vector2(90.0f, 40.0f), Color.White);

            spriteBatch.End();
        }
    }
}
