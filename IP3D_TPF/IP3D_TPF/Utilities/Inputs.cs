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
    public class Inputs
    {

        public KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        public MouseState currentMouseState;
        MouseState previousMouseState;

        public Inputs()
        {
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }


        public void Update()
        {

            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();


        }
    }


}
