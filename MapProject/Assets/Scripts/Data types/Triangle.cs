using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jonas.Geometry
{
    public class Triangle
    {
        public Vertex v1, v2, v3;

        public HalfEdge halfEdge;

        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
            this.v3 = new Vertex(v3);
        }

        public Triangle(HalfEdge halfEdge)
        {
            this.halfEdge = halfEdge;
        }

        public void ChangeOrientation()
        {
            Vertex temp = this.v1;
            this.v1 = this.v2;
            this.v2 = temp;
        }

        public bool ContainsVertex(Vertex v)
        {
            return (v == v1 || v == v2 || v == v3);
        }

        public bool ContainsEdge(Edge e)
        {
            return ContainsVertex(e.v1) && ContainsVertex(e.v2);
        }
    }
}
