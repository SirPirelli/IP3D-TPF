

using BoundingSpheresTest;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace IP3D_TPF
{
    abstract class CollisionHandler
    {

        /// <summary>
        /// Checks for intersection between two <see cref="BoundingSphereCls"/>.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="sphere2"></param>
        /// <returns></returns>
        public static bool IsColliding(BoundingSphereCls sphere, BoundingSphereCls sphere2)
        {
            if (sphere.Intersects(sphere2)) return true;

            return false;

        }

    }
}
