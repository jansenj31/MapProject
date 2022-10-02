using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public class Plane
    {
        public Vector3 position;
        public Vector3 normal;

        public Plane(Vector3 pos, Vector3 normal)
        {
            this.position = pos;
            this.normal = normal;
        }
    }
}
