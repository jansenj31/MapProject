using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    [System.Serializable]
    public class Polygon
    {
        [SerializeField]
        public List<Vertex> vertices;
        public Vertex site;

        public Polygon(List<Vertex> vertices)
        {
            this.vertices = JarvisMarch.GetConvexHull(vertices);
        }

        public Polygon(List<Vector3> points)
        {
            foreach (Vector3 p in points) vertices.Add((Vertex)p);
            this.vertices = JarvisMarch.GetConvexHull(vertices);
        }
        public Polygon(List<Vertex> vertices, Vertex site)
        {
            this.vertices = vertices;
            this.site = site;
        }

        public Polygon(Bounds bounds)
        {
            List<Vertex> bVerts = new List<Vertex>() {
                new Vertex(bounds.center.x - bounds.extents.x, 0, bounds.center.z - bounds.extents.z),
                new Vertex(bounds.center.x - bounds.extents.x, 0, bounds.center.z + bounds.extents.z),
                new Vertex(bounds.center.x + bounds.extents.x, 0, bounds.center.z - bounds.extents.z),
                new Vertex(bounds.center.x + bounds.extents.x, 0, bounds.center.z + bounds.extents.z)
            };

            this.vertices = JarvisMarch.GetConvexHull(bVerts);
        }

        public bool Contains(Vector3 point)
        {
            return GeometryHelper.IsPointInConvexPolygon(this, point);
        }
    }
}
