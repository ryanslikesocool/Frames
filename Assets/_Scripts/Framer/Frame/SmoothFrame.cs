using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framer
{
    public class SmoothFrame : IFrameableObject
    {
        public RectTransform bounds;

        public float[] cornerRadii;
        public int levelOfDetail;

        public List<Vector2> meshPoints = new List<Vector2>();

        //Frame corners are created similar to progressing along a unit circle.
        public SmoothFrame(RectTransform bounds, float[] cornerRadii, int levelOfDetail)
        {
            this.bounds = bounds;
            this.cornerRadii = cornerRadii;
            this.levelOfDetail = levelOfDetail;
        }

        void GetCornerPoints(int cornerNumber, float radius, int levelOfDetail)
        {
            Vector2[] returnPoints = new Vector2[levelOfDetail];

            Vector2 offset = Vector2.zero;
            switch (cornerNumber)
            {
                case 0:
                    offset.Set(bounds.sizeDelta.x / 2 - radius, bounds.sizeDelta.y / 2 - radius);
                    break;
                case 1:
                    offset.Set(-bounds.sizeDelta.x / 2 + radius, bounds.sizeDelta.y / 2 - radius);
                    break;
                case 2:
                    offset.Set(-bounds.sizeDelta.x / 2 + radius, -bounds.sizeDelta.y / 2 + radius);
                    break;
                case 3:
                    offset.Set(bounds.sizeDelta.x / 2 - radius, -bounds.sizeDelta.y / 2 + radius);
                    break;
            }

            //If there's no (or negative) radius, then there's no need for the points
            if (radius > 0)
            {
                float pi = Mathf.PI;
                //Big old messy for loop for super ellipses
                for (float angle = pi / 2 * cornerNumber; angle < pi / 2 * (cornerNumber + 1); angle += 1f / levelOfDetail)
                {
                    float n = 2.4f; //This is the superness.  I find 2.4f to be a good number
                    float na = 2 / n;
                    float angleSine = Mathf.Sin(angle);
                    float angleCosine = Mathf.Cos(angle);
                    float x = Mathf.Pow(Mathf.Abs(angleCosine), na) * radius * Mathf.Sign(angleCosine);
                    float y = Mathf.Pow(Mathf.Abs(angleSine), na) * radius * Mathf.Sign(angleSine);
                    meshPoints.Add(new Vector2(x, y) + offset);
                }

                Vector2 extraPoint = Vector2.zero;
                switch (cornerNumber)
                {
                    case 0:
                        extraPoint.Set(offset.x, offset.y + radius);
                        break;
                    case 1:
                        extraPoint.Set(offset.x - radius, offset.y);
                        break;
                    case 2:
                        extraPoint.Set(offset.x, offset.y - radius);
                        break;
                    case 3:
                        extraPoint.Set(offset.x + radius, offset.y);
                        break;
                }
                meshPoints.Add(extraPoint);
            }
            else
            {
                meshPoints.Add(offset);
            }
        }

        public Mesh CreateMesh()
        {
            meshPoints.Clear();

            for (int i = 0; i < 4; i++)
            {
                GetCornerPoints(i, cornerRadii[i], levelOfDetail);
            }

            Vector2[] vertices2D = meshPoints.ToArray();

            //Use Triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D);
            int[] indices = tr.Triangulate();

            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
            }

            //Create the mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            //Set up game object with mesh
            return mesh;
        }
    }
}