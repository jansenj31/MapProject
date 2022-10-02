using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jonas.Geometry;
using System.Linq;

//[ExecuteInEditMode]
public class MapGen2D : MonoBehaviour
{
    [SerializeField]
    Bounds bounds;

    [Range(5, 5000)]
    [SerializeField]
    int areaCount;

    [SerializeField]
    Material mat;

    [SerializeField]
    float falloffStrength, scale, perlinStrength;

    List<GameObject> meshObjects;
    List<Polygon> polys;
    List<Vector3> points;

    [SerializeField]
    int seed = 42;
    [SerializeField]
    bool randomSeed;

    float xOffset, zOffset;

    private void Start()
    {
        if (!randomSeed) Random.InitState(seed);
        xOffset = Random.Range(-100000f, 100000f);
        zOffset = Random.Range(-100000f, 100000f);

        points = RandomPointGenerator.GenerateRandomPoints(bounds.center, areaCount, bounds.size.x, 0, bounds.size.z);
        List<Vertex> vertices = (from point in points select (Vertex)point).ToList();

        polys = Voronoi.GenerateBoundedVoronoi(vertices, new Polygon(bounds));
        meshObjects = (from poly in polys select MeshGen.GenerateNewMeshObject(poly, mat, this.gameObject, true)).ToList();

        GenerateColorMap();
    }

    private void Update()
    {
        GenerateColorMap();
    }

    void GenerateColorMap()
    {
        List<float> heightMap = new List<float>();

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 point = points[i];
            float height = perlinStrength * Mathf.PerlinNoise(scale * point.x + xOffset, scale * point.z + zOffset)
                           + (1f - falloffStrength * calculateFalloff(point.x, point.z, bounds.center, bounds.extents));
            height = normalise(height, perlinStrength);
            heightMap.Add(height);
            meshObjects[i].GetComponent<MeshRenderer>().material.color = height >= 0.5f ? Color.green * Random.Range(0.9f, 1.1f) : Color.blue * Random.Range(0.95f, 1.05f);
        }
    }

    float calculateFalloff(float x, float z, Vector3 center, Vector3 extents)
    {
        float falloff = 0;
        falloff = Mathf.Max(Mathf.Abs(normalise(center.x - x, extents.x)), Mathf.Abs(normalise(center.z - z, extents.z)));
        return Evaluate(falloff);
    }

    float Evaluate(float value)
    {
        float a = 12.6f;
        float b = 6f;
        value = Mathf.Clamp(value, 0f, 1f);

        return 1 / (1 + Mathf.Exp(-a * value + b));
    }

    float normalise(float value, float range)
    {
        float normalised = value / range;
        return Mathf.Abs(normalised);
    }
}
