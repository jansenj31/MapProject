using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public struct Circle
    {
        public Vector3 centroid;
        public float radius;

        public Circle(Vector3 c, float r)
        {
            centroid = c;
            radius = r;
        }
    }
}
