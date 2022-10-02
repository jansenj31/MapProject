using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public class Site
    {
        public Vector3 position;

        public Site(Vertex v)
        {
            position = v.position;
        }
    }
}
