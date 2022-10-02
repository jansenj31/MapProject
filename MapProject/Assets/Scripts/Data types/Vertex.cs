using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jonas.Geometry
{
    [System.Serializable]
    public class Vertex
    {
        [SerializeField]
        public Vector3 position;
        public HalfEdge halfEdge;
        public Triangle triangle;
        public Vertex prevVertex, nextVertex;
        public bool isReflex, isConvex, isEar;

        public Vertex(Vector3 pos)
        {
            position = pos;
        }

        public Vertex()
        {
            position = Vector3.zero;
        }

        public Vertex(float x, float y, float z)
        {
            position = new Vector3(x, y, z);
        }

        public static explicit operator Vertex(Vector3 v) => new Vertex(v);
        public static implicit operator Vector3(Vertex v) => new Vector3(v.position.x, v.position.y, v.position.z);


        public Vector2 GetPos2D_XZ()
        {
            Vector2 pos_2d_XZ = new Vector2(position.x, position.z);
            return pos_2d_XZ;
        }
    }
}