using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Jonas.Geometry
{
    public static class Triangulation
    {

        // Simple triangulation for convex polygons
        // using triangles that all connect to a vertex p1
        public static List<Triangle> TriangulateConvexPolygon(List<Vertex> points)
        {
            List<Triangle> triangles = new List<Triangle>();
            Vertex p1 = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                triangles.Add(new Triangle(p1, points[i - 1], points[i]));
            }

            return triangles;
        }

        // 'Dumb' triangulation for random points inside of a convex polygon
        public static List<Triangle> SimpleTriangulation(List<Vertex> points)
        {
            List<Vertex> convexHull = JarvisMarch.GetConvexHull(points);
            List<Vertex> vertices = points.Except(convexHull).ToList();

            List<Triangle> triangles = TriangulateConvexPolygon(convexHull);

            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex currentPoint = vertices[i];
                Vector2 p = new Vector2(currentPoint.position.x, currentPoint.position.z);

                for (int j = 0; j < triangles.Count; j++)
                {
                    Triangle t = triangles[j];

                    Vector2 p1 = new Vector2(t.v1.position.x, t.v1.position.z);
                    Vector2 p2 = new Vector2(t.v2.position.x, t.v2.position.z);
                    Vector2 p3 = new Vector2(t.v3.position.x, t.v3.position.z);

                    if (GeometryHelper.IsPointInTriangle(p1, p2, p3, p))
                    {
                        //Create 3 new triangles
                        Triangle t1 = new Triangle(t.v1, t.v2, currentPoint);
                        Triangle t2 = new Triangle(t.v2, t.v3, currentPoint);
                        Triangle t3 = new Triangle(t.v3, t.v1, currentPoint);

                        //Remove the old triangle
                        triangles.Remove(t);

                        //Add the new triangles
                        triangles.Add(t1);
                        triangles.Add(t2);
                        triangles.Add(t3);

                        break;
                    }
                }
            }



            return triangles;
        }
    }
}
