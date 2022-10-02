using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Jonas.Geometry
{
    public static class Voronoi
    {
        [System.Obsolete("Deprecated, GenerateBoundedVoronoi fixes issues with missing poly's")]
        public static List<Polygon> GenerateVoronoiGraph(List<Vertex> points, Bounds bounds)
        {
            List<Polygon> graph = new List<Polygon>();

            List<Vertex> boundsVertices = new List<Vertex>();
            float minX = bounds.min.x;
            float minZ = bounds.min.z;
            float maxX = bounds.max.x;
            float maxZ = bounds.max.z;

            boundsVertices.Add(new Vertex(minX, 0, minZ));
            boundsVertices.Add(new Vertex(minX, 0, maxZ));
            boundsVertices.Add(new Vertex(maxX, 0, maxZ));
            boundsVertices.Add(new Vertex(maxX, 0, minZ));

            points.Add(new Vertex(0f, 0f, bounds.size.x * 10f));
            points.Add(new Vertex(0f, 0f, -bounds.size.x * 10f));
            points.Add(new Vertex(bounds.size.y * 10f, 0f, 0f));
            points.Add(new Vertex(-bounds.size.y * 10f, 0f, 0f));

            List<Triangle> delaunay = BowyerWatson.Triangulate(points);

            foreach (Vertex v in points)
            {
                List<Triangle> connectedTriangles = new List<Triangle>();
                foreach (Triangle t in delaunay) if (t.ContainsVertex(v)) connectedTriangles.Add(t);
                List<Vertex> polygonVertices = new List<Vertex>();
                foreach (Triangle t in connectedTriangles) polygonVertices.Add((Vertex)GeometryHelper.GetTriangleCircumCircle(t).centroid);
                Polygon newPoly = new Polygon(JarvisMarch.GetConvexHull(polygonVertices), v);
                if (newPoly.vertices != null) graph.Add(newPoly);
            }

            foreach (Polygon poly in graph)
            {
                if (poly.vertices == null || poly.vertices.Count < 3) continue;

                List<Vertex> clippedVertices = new List<Vertex>();
                List<Vertex> newVertices = new List<Vertex>();

                for (int i = 0; i < boundsVertices.Count; i++)
                {
                    Vertex v1 = boundsVertices[i];
                    Vertex v2 = boundsVertices[GeometryHelper.ClampedIndex(i + 1, boundsVertices.Count)];

                    for (int j = 0; j < poly.vertices.Count; j++)
                    {
                        Vertex v3 = poly.vertices[j];
                        Vertex v4 = poly.vertices[GeometryHelper.ClampedIndex(j + 1, poly.vertices.Count)];

                        if (!bounds.Contains(v3.position) && !bounds.Contains(v4.position))
                        {
                            clippedVertices.Add(v3);
                            clippedVertices.Add(v4);
                        }
                        else if (!bounds.Contains(v3.position))
                        {
                            Vertex intersection = new Vertex();
                            if (GeometryHelper.GetBoundedLineLineIntersectionPoint(v1, v2, v3, v4, out intersection))
                            {
                                clippedVertices.Add(v3);
                                newVertices.Add(intersection);
                            }
                        }
                        else if (!bounds.Contains(v4.position))
                        {
                            Vertex intersection = new Vertex();
                            if (GeometryHelper.GetBoundedLineLineIntersectionPoint(v1, v2, v3, v4, out intersection))
                            {
                                clippedVertices.Add(v4);
                                newVertices.Add(intersection);
                            }
                        }

                    }
                }

                foreach (Vertex v in newVertices) poly.vertices.Add(v);
                foreach (Vertex v in clippedVertices) if (!newVertices.Contains(v)) poly.vertices.Remove(v);
            }

            foreach (Vertex v in boundsVertices)
            {
                float minDist = Mathf.Infinity;
                Vertex closest = null;

                foreach (Vertex v2 in points)
                {
                    float dist = Vector3.Distance(v.position, v2.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = v2;
                    }
                }

                foreach (Polygon poly in graph)
                {
                    if (poly.site == closest)
                    {
                        poly.vertices.Add(v);
                        Debug.Log("Added corner" + poly.site.position);
                        break;
                    }
                }
            }

            for (int i = graph.Count - 1; i >= 0; i--)
            {
                Polygon p = graph[i];
                if (p.vertices == null || p.vertices.Count == 0) graph.Remove(p);
                p.vertices = JarvisMarch.GetConvexHull(p.vertices);
            }
            return graph;
        }

        public static List<Polygon> GenerateBoundedVoronoi(List<Vertex> points, Polygon bounds)
        {
            List<Polygon> graph = new List<Polygon>();

            Rect bb = GeometryHelper.GetListXZBounds(points);
            points.Add(new Vertex(0f, 0f, bb.size.x * 10f));
            points.Add(new Vertex(0f, 0f, -bb.size.x * 10f));
            points.Add(new Vertex(bb.size.y * 10f, 0f, 0f));
            points.Add(new Vertex(-bb.size.y * 10f, 0f, 0f));

            List<Triangle> delaunay = BowyerWatson.Triangulate(points);

            foreach (Vertex v in points)
            {
                List<Triangle> connectedTriangles = new List<Triangle>();
                foreach (Triangle t in delaunay) if (t.ContainsVertex(v)) connectedTriangles.Add(t);
                List<Vertex> polygonVertices = new List<Vertex>();
                foreach (Triangle t in connectedTriangles) polygonVertices.Add((Vertex)GeometryHelper.GetTriangleCircumCircle(t).centroid);
                Polygon newPoly = new Polygon(JarvisMarch.GetConvexHull(polygonVertices), v);
                if (newPoly.vertices != null) graph.Add(newPoly);
            }

            foreach (Polygon poly in graph)
            {
                if (poly.vertices == null || poly.vertices.Count < 3) continue;

                List<Vertex> clippedVertices = new List<Vertex>();
                List<Vertex> newVertices = new List<Vertex>();

                poly.vertices = ClipPolygon(poly, bounds).vertices;
            }

            for (int i = graph.Count - 1; i >= 0; i--)
            {
                Polygon p = graph[i];
                if (p.vertices == null || p.vertices.Count == 0) graph.Remove(p);
                p.vertices = JarvisMarch.GetConvexHull(p.vertices);
            }

            Debug.Log(points.Count - graph.Count - bounds.vertices.Count);
            return graph;
        }

        static Polygon ClipPolygon(Polygon input, Polygon bounds)
        {
            List<Vertex> clippedVertices = new List<Vertex>();
            List<Vertex> addedVertices = new List<Vertex>();
            Polygon output = new Polygon(input.vertices);

            for (int i = 0; i < bounds.vertices.Count; i++)
            {
                Vertex v1 = bounds.vertices[i];
                Vertex v2 = bounds.vertices[GeometryHelper.ClampedIndex(i + 1, bounds.vertices.Count)];

                for (int j = 0; j < output.vertices.Count; j++)
                {
                    Vertex v3 = output.vertices[j];
                    Vertex v4 = output.vertices[GeometryHelper.ClampedIndex(j + 1, output.vertices.Count)];

                    if (!bounds.Contains(v3.position) && !bounds.Contains(v4.position))
                    {
                        Vertex intersection = new Vertex();
                        if (GeometryHelper.GetBoundedLineLineIntersectionPoint(v1, v2, v3, v4, out intersection))
                        {
                            addedVertices.Add(intersection);
                        }
                        clippedVertices.Add(v3);
                        clippedVertices.Add(v4);
                    }

                    else if (!bounds.Contains(v3.position))
                    {
                        Vertex intersection = new Vertex();
                        if (GeometryHelper.GetBoundedLineLineIntersectionPoint(v1, v2, v3, v4, out intersection))
                        {
                            clippedVertices.Add(v3);
                            addedVertices.Add(intersection);
                        }
                    }
                    else if (!bounds.Contains(v4.position))
                    {
                        Vertex intersection = new Vertex();
                        if (GeometryHelper.GetBoundedLineLineIntersectionPoint(v1, v2, v3, v4, out intersection))
                        {
                            clippedVertices.Add(v4);
                            addedVertices.Add(intersection);
                        }
                    }
                }
            }

            foreach (Vertex v in bounds.vertices)
            {
                if (output.Contains(v)) output.vertices.Add(v);
            }

            foreach (Vertex v in clippedVertices) output.vertices.Remove(v);
            foreach (Vertex v in addedVertices) output.vertices.Add(v);

            return new Polygon(output.vertices);
        }
    }
}