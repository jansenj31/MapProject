using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomPointGenerator
{
    public static List<Vector3> GenerateRandomPoints(Vector3 center, int pointCount, float xSize = 0, float ySize = 0, float zSize = 0)
    {
        List<Vector3> points = new List<Vector3>();

        float minX, maxX, minY, maxY, minZ, maxZ;

        minX = center.x - xSize / 2;
        maxX = center.x + xSize / 2;
        minY = center.y - ySize / 2;
        maxY = center.y + ySize / 2;
        minZ = center.z - zSize / 2;
        maxZ = center.z + zSize / 2;

        for (int i = 0; i < pointCount; i++)
        {
            Vector3 newPoint = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            points.Add(newPoint);
        }

        return points;
    }
}
