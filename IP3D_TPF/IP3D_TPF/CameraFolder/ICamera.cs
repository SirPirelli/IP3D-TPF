using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    public interface ICamera
    {

        Vector3 Position { get; set; }
        Vector3 Target { get; set; }
        Matrix ViewMatrix { get; set; }
        Matrix ProjectionMatrix { get; set; }
        float NearPlaneDistance { get; set; }
        float FarPlaneDistance { get; set; }
        float FieldOfViewDegrees { get; set; }

    }
}
