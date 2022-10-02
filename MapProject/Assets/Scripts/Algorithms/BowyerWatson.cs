using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Jonas.Geometry
{
    //TODO: Add failsafes for (nearly) colinear points
    public static class BowyerWatson
    {
        public static List<Triangle> Triangulate(List<Vertex> points)
        {
            List<Triangle> triangles = InitTriangulation(GeometryHelper.GetListXZBounds(points));
            Triangle superTriangle = triangles[0];

            foreach (Vertex v in points)
            {
                triangles = EvaluateTriangulation(triangles, v);
            }

            RemoveSuperTriangle(superTriangle, triangles);

            return triangles;
        }

        public static List<Triangle> InitTriangulation(Rect vertexBB)
        {
            List<Triangle> triangles = new List<Triangle>();
            float minX, minZ, maxX, maxZ;

            Rect bounds = vertexBB;
            minX = bounds.x - bounds.width;
            maxX = bounds.x + 4 * bounds.width;
            minZ = bounds.y - bounds.height;
            maxZ = bounds.y + 4 * bounds.height;

            Vector3 vec1 = new Vector3(minX, 0, minZ);
            Vector3 vec2 = new Vector3(maxX, 0, minZ);
            Vector3 vec3 = new Vector3(minX, 0, maxZ);

            Vertex v1 = new Vertex(vec1);
            Vertex v2 = new Vertex(vec2);
            Vertex v3 = new Vertex(vec3);

            Triangle superTriangle = new Triangle(v1, v2, v3);
            triangles.Add(superTriangle);

            return triangles;
        }

        public static List<Triangle> EvaluateTriangulation(List<Triangle> triangles, Vertex point)
        {
            List<Triangle> invalidTriangles = new List<Triangle>();
            List<Edge> hullEdges = new List<Edge>();

            foreach (Triangle triangle in triangles)
            {
                Circle circumCircle = GeometryHelper.GetTriangleCircumCircle(triangle);
                if (Vector3.Distance(point.position, circumCircle.centroid) < circumCircle.radius)
                {
                    invalidTriangles.Add(triangle);
                }
            }

            foreach (Triangle t1 in invalidTriangles)
            {
                List<Edge> edges = new List<Edge>();
                edges.Add(new Edge(t1.v1, t1.v2));
                edges.Add(new Edge(t1.v2, t1.v3));
                edges.Add(new Edge(t1.v3, t1.v1));

                foreach (Edge e in edges)
                {
                    bool unshared = true;

                    foreach (Triangle t2 in invalidTriangles)
                    {
                        if (t1 == t2) continue;

                        if (t2.ContainsEdge(e))
                        {
                            unshared = false;
                        }
                    }

                    if (unshared)
                    {
                        if (!hullEdges.Contains(e)) hullEdges.Add(e);
                    }
                }
            }

            foreach (Triangle t in invalidTriangles) triangles.Remove(t);

            for (int i = 0; i < hullEdges.Count; i++)
            {
                triangles.Add(new Triangle(point, hullEdges[i].v1, hullEdges[i].v2));
            }

            return triangles;
        }

        public static List<Triangle> RemoveSuperTriangle(Triangle superTriangle, List<Triangle> triangles)
        {
            Vertex v1 = superTriangle.v1;
            Vertex v2 = superTriangle.v2;
            Vertex v3 = superTriangle.v3;

            for (int i = triangles.Count - 1; i >= 0; i--)
            {
                Triangle triangle = triangles[i];
                if (triangle.ContainsVertex(v1) || triangle.ContainsVertex(v2) || triangle.ContainsVertex(v3)) triangles.Remove(triangle);
            }

            return triangles;
        }
    }
}