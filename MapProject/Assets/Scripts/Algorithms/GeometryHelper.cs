using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jonas.Geometry
{
    public static class GeometryHelper
    {

        //Where is p in relation to a-b
        // d < 0 -> to the right
        // d == 0 -> on the line
        // d > 0 -> to the left
        public static float GetDeterminant(Vector2 a, Vector2 b, Vector2 p)
        {
            float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

            return determinant;
        }

        public static int ClampedIndex(int index, int listSize)
        {
            index = ((index % listSize) + listSize) % listSize;

            return index;
        }

        public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
        {
            bool isWithinTriangle = false;

            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
            float c = 1 - a - b;

            if (a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f)
            {
                isWithinTriangle = true;
            }

            return isWithinTriangle;
        }

        public static bool IsPointInTriangle(Triangle t, Vector3 p)
        {
            Vector2 p1, p2, p3;
            p1 = new Vector2(t.v1.position.x, t.v1.position.z);
            p2 = new Vector2(t.v2.position.x, t.v2.position.z);
            p3 = new Vector2(t.v3.position.x, t.v3.position.z);

            bool isWithinTriangle = false;

            float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

            float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.y)) / denominator;
            float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.y)) / denominator;
            float c = 1 - a - b;

            if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
            {
                isWithinTriangle = true;
            }

            return isWithinTriangle;
        }

        public static bool IsPointInConvexPolygon(Polygon p, Vector3 v)
        {
            if (p.vertices == null || p.vertices.Count < 3) return false;
            List<Triangle> triangles = Triangulation.TriangulateConvexPolygon(p.vertices);
            foreach (Triangle triangle in triangles)
            {
                if (IsPointInTriangle(triangle, v)) return true;
            }
            return false;
        }

        public static Rect GetListXZBounds(List<Vertex> points)
        {
            float minX, minZ, maxX, maxZ;

            minX = minZ = Mathf.Infinity;
            maxX = maxZ = -Mathf.Infinity;

            foreach (Vertex p in points)
            {
                if (p.position.x < minX) minX = p.position.x;
                if (p.position.x > maxX) maxX = p.position.x;
                if (p.position.z < minZ) minZ = p.position.z;
                if (p.position.z > maxZ) maxZ = p.position.z;
            }

            float width = Mathf.Max(Mathf.Abs(maxX - minX), 1);
            float height = Mathf.Max(Mathf.Abs(maxZ - minZ), 1);

            return new Rect(minX, minZ, width, height);
        }

        public static Vector3 GetLineLineIntersectionPoint(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            float denominator = (b2.z - b1.z) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.z - a1.z);

            float u_a = ((b2.x - b1.x) * (a1.z - b1.z) - (b2.z - b1.z) * (a1.x - b1.x)) / denominator;

            Vector3 i = a1 + u_a * (a2 - a1);

            return new Vector3(i.x, 0, i.z);
        }

        public static Vector3 GetLineLineIntersectionPoint(Vertex v1, Vertex v2, Vertex v3, Vertex v4)
        {
            Vector3 a1 = v1.position;
            Vector3 a2 = v2.position;
            Vector3 b1 = v3.position;
            Vector3 b2 = v4.position;

            float denominator = (b2.z - b1.z) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.z - a1.z);

            float u_a = ((b2.x - b1.x) * (a1.z - b1.z) - (b2.z - b1.z) * (a1.x - b1.x)) / denominator;

            Vector3 i = a1 + u_a * (a2 - a1);

            return new Vector3(i.x, 0, i.z);
        }

        public static bool GetBoundedLineLineIntersectionPoint(Vertex v1, Vertex v2, Vertex v3, Vertex v4, out Vertex intersection)
        {
            Vector3 a1 = v1.position;
            Vector3 a2 = v2.position;
            Vector3 b1 = v3.position;
            Vector3 b2 = v4.position;

            float denominator = (b2.z - b1.z) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.z - a1.z);

            float u_a = ((b2.x - b1.x) * (a1.z - b1.z) - (b2.z - b1.z) * (a1.x - b1.x)) / denominator;

            Vector3 i = a1 + u_a * (a2 - a1);

            intersection = new Vertex(i.x, 0, i.z);

            if ((a1.x > intersection.position.x && a2.x > intersection.position.x)
                || (a1.x < intersection.position.x && a2.x < intersection.position.x)
                || (b1.x > intersection.position.x && b2.x > intersection.position.x)
                || (b1.x < intersection.position.x && b2.x < intersection.position.x)
                || (a1.z > intersection.position.z && a2.z > intersection.position.z)
                || (a1.z < intersection.position.z && a2.z < intersection.position.z)
                || (b1.z > intersection.position.z && b2.z > intersection.position.z)
                || (b1.z < intersection.position.z && b2.z < intersection.position.z)) return false;
            else return true;

        }

        public static Circle GetTriangleCircumCircle(Triangle t)
        {
            Vector3 p1 = GetLineBissector(t.v1.position, t.v2.position);
            Vector3 slope1 = GetInverseVector2(t.v2.position - t.v1.position).normalized;

            Vector3 p2 = GetLineBissector(t.v1.position, t.v3.position);
            Vector3 slope2 = GetInverseVector2(t.v3.position - t.v1.position).normalized;

            Vector3 a1 = p1 - (100f * slope1);
            Vector3 a2 = p1 + (100f * slope1);

            Vector3 b1 = p2 - (100f * slope2);
            Vector3 b2 = p2 + (100f * slope2);

            Vector3 centroid = GetLineLineIntersectionPoint(a1, a2, b1, b2);
            float radius = (centroid - t.v1.position).magnitude;

            return new Circle(centroid, radius);
        }

        public static Vector3 GetInverseVector2(Vector3 v)
        {
            Vector3 vInverse = new Vector3(v.z, 0, -v.x);
            return vInverse;
        }

        public static Vector3 GetLineBissector(Vector3 a, Vector3 b)
        {
            Vector3 ab = b - a;
            Vector3 bissector = a + (0.5f * ab);
            return bissector;
        }
    }
}
