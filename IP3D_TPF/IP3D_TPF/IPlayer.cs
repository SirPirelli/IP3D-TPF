using Microsoft.Xna.Framework;


namespace IP3D_TPF
{
    public interface IPlayer
    {
        int Player { get; }

      void UpdateInputs(Inputs inputs, GameTime gameTime);
    }
}
