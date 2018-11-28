

using BoundingSpheresTest;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace IP3D_TPF
{
    abstract class CollisionHandler
    {

        //sphere - sphere collision
        public static bool IsColliding(BoundingSphereCls sphere, BoundingSphereCls sphere2)
        {
            if (sphere.Intersects(sphere2)) return true;

            return false;

        }

    }
}
