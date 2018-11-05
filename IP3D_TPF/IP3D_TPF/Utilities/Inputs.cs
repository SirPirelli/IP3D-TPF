using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    //nesta classe é q temos q ter metodos para confirmar se uma tecla esta a ser pressionada
    class Inputs
    {

        KeyboardState keyboardState;
        MouseState mouseState;

        public KeyboardState KeyboardState { get { return keyboardState; } }
        public MouseState MouseState { get { return mouseState; } }

        public Inputs() { }

        public void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
    }
}
