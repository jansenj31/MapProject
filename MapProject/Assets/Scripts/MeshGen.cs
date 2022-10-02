using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jonas.Geometry;

public static class MeshGen
{
    public static Mesh GenerateConvexMesh(Polygon poly)
    {
        Mesh newMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        foreach (Vertex v in poly.vertices) vertices.Add(v.position);
        newMesh.vertices = vertices.ToArray();

        int[] triangles = new int[3 * (vertices.Count)];

        for (int i = 2; i < vertices.Count; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i;
            triangles[3 * i + 2] = i - 1;
        }

        newMesh.triangles = triangles;
        newMesh.RecalculateNormals();

        return newMesh;
    }

    public static GameObject GenerateNewMeshObject(Polygon poly, Material mat, GameObject parent = null, bool randomColor = false, string name = "mesh")
    {
        GameObject newObject = new GameObject();
        newObject.name = name;
        if (parent != null) newObject.transform.parent = parent.transform;
        MeshRenderer mr = newObject.AddComponent<MeshRenderer>();
        MeshFilter mf = newObject.AddComponent<MeshFilter>();

        Mesh newMesh = GenerateConvexMesh(poly);
        newMesh.name = name;
        mr.sharedMaterial = mat;
        if (randomColor) mr.sharedMaterial.color = Random.ColorHSV();
        mf.mesh = newMesh;

        return newObject;
    }
}
