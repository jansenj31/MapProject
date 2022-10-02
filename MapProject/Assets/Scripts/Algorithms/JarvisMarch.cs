using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Jonas.Geometry;

public static class JarvisMarch
{
    public static List<Vertex> GetConvexHull(List<Vertex> vertices)
    {
        if (vertices == null || vertices.Count < 3) return null;
        //else if (vertices.Count == 3) return vertices;

        List<Vertex> convexHull = new List<Vertex>();
        List<Vertex> points = new List<Vertex>(vertices);
        List<Vertex> colinearPoints = new List<Vertex>();

        Vertex startVertex = vertices[0];

        for (int i = 1; i < vertices.Count; i++)
        {
            Vector3 testPos = vertices[i].position;

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startVertex.position.x || (Mathf.Approximately(testPos.x, startVertex.position.x) && testPos.z < startVertex.position.z))
            {
                startVertex = vertices[i];
            }
        }

        convexHull.Add(startVertex);
        points.Remove(startVertex);

        Vertex currentVertex = convexHull[0];

        int counter = 0;

        while (true)
        {

            if (counter == 2 || points.Count < 1) points.Add(startVertex);

            int index = Random.Range(0, points.Count);
            Vertex nextVertex = points[index];

            //To 2d space so we can see if a point is to the left is the vector ab
            Vector2 a = currentVertex.GetPos2D_XZ();
            Vector2 b = nextVertex.GetPos2D_XZ();

            //Test if there's a point to the right of ab, if so then it's the new b
            for (int i = 0; i < points.Count; i++)
            {
                //Dont test the point we picked randomly
                if (points[i].Equals(nextVertex))
                {
                    continue;
                }

                Vector2 c = points[i].GetPos2D_XZ();

                float relation = GeometryHelper.GetDeterminant(a, b, c);

                //Colinear points
                //Cant use exactly 0 because of floating point precision issues
                //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
                float accuracy = 0.00000001f;

                if (relation < accuracy && relation > -accuracy)
                {
                    colinearPoints.Add(points[i]);
                }
                //To the right = better point, so pick it as next point on the convex hull
                else if (relation < 0f)
                {
                    nextVertex = points[i];

                    b = nextVertex.GetPos2D_XZ();

                    //Clear colinear points
                    colinearPoints.Clear();
                }
            }

            //If we have colinear points
            if (colinearPoints.Count > 0)
            {
                colinearPoints.Add(nextVertex);

                //Sort this list, so we can add the colinear points in correct order
                colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n.position - currentVertex.position)).ToList();

                convexHull.AddRange(colinearPoints);

                currentVertex = colinearPoints[colinearPoints.Count - 1];

                //Remove the points that are now on the convex hull
                for (int i = 0; i < colinearPoints.Count; i++)
                {
                    points.Remove(colinearPoints[i]);
                }

                colinearPoints.Clear();
            }
            else
            {
                convexHull.Add(nextVertex);
                points.Remove(nextVertex);
                currentVertex = nextVertex;
            }


            if (currentVertex.Equals(startVertex))
            {
                convexHull.RemoveAt(convexHull.Count - 1);
                break;
            }

            counter++;
        }
        return convexHull;
    }
}
