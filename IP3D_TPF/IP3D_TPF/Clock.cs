using Microsoft.Xna.Framework;

namespace IP3D_TPF
{
    public class Clock
    {
        private double totalTimeInSeconds;

        public double TotalTimeInSeconds { get => totalTimeInSeconds; }

        public Clock()
        {
            totalTimeInSeconds = 0d;
        }

        public void Update(GameTime gametime)
        {
            totalTimeInSeconds += gametime.ElapsedGameTime.TotalSeconds;
            
        }
        
    }
}
