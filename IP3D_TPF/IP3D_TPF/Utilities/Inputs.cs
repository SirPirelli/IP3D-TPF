using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    /// <summary>
    /// <see langword="class"/> that registers and processes user input.
    /// </summary>
    public class Inputs
    {
        /* FIELDS -------------------------------- */
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        /* PROPERTIES ---------------------------- */
        public KeyboardState CurrentKeyboardState       { get { return currentKeyboardState; } private set { currentKeyboardState = value; } }
        public KeyboardState PreviousKeyboardState      { get { return previousKeyboardState; } private set { previousKeyboardState = value; } }

        public MouseState CurrentMouseState             { get { return currentMouseState; } private set { currentMouseState = value; } }
        public MouseState PreviousMouseState            { get { return previousMouseState; } private set { previousMouseState = value; } }
        /* --------------------------------------- */


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

        #region KEYBOARD FUNCTIONS

        /// <summary>
        /// Released the key in the frame function is called.
        /// </summary>
        /// <param name="key">Key we want to check.</param>
        /// <returns></returns>
        public bool ReleasedKey(Keys key)
        {
            if (previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyUp(key)) return true;

            return false;
        }

        /// <summary>
        /// In the last check key was not being pressed.
        /// </summary>
        /// <param name="key">Key we want to check.</param>
        /// <returns>If true, key is being pressed.</returns>
        public bool StartedPressingKey(Keys key)
        {
            if (previousKeyboardState.IsKeyUp(key) && currentKeyboardState.IsKeyDown(key)) return true;

            return false;
        }

        /// <summary>
        /// Check if key is being continuosly pressed.
        /// </summary>
        /// <param name="key">Key we want to check.</param>
        /// <returns></returns>
        public bool Check(Keys key)
        {
            if (previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key)) return true;

            return false;
        }

        #endregion

        #region MOUSE FUNCTIONS

        public Vector2 GetMouseDeltaPosition(Vector2 position)
        {
            Vector2 delta = new Vector2(currentMouseState.X - position.X,
                                        currentMouseState.Y - position.Y);

            return delta;
        }

        public void SetMousePosition(Vector2 position)
        {
            Mouse.SetPosition((int)position.X, (int)position.Y);
        }

        #endregion
    }


}
