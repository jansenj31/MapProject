using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public class Border
    {
        public Vector3 v1, v2;

        public Border(Vector2 v1, Vector2 v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }
}
