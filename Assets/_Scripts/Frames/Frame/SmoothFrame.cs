using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ifelse.Frames
{
    public class SmoothFrame : IFrameableObject
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get { return RectTransform.rect; } }

        public FrameCornerType CornerType { get { return FrameCornerType.Smooth; } }
        public Triangulator Triangulator { get; }

        public float[] cornerRadii;
        public int levelOfDetail;

        public List<Vector2> meshPoints = new List<Vector2>();
        public List<Vector2> meshUVs = new List<Vector2>();

        //Frame corners are created similar to progressing along a unit circle.
        public SmoothFrame(RectTransform rectTrans, float[] cornerRadii, int levelOfDetail)
        {
            this.RectTransform = rectTrans;
            this.cornerRadii = cornerRadii;
            this.levelOfDetail = levelOfDetail;

            Triangulator = new Triangulator();
        }

        private void GetCornerPoints(int cornerNumber, float radius, int levelOfDetail)
        {
            Vector2 offset = Vector2.zero;
            Vector2 size = new Vector2(Bounds.width, Bounds.height);
            Vector2 extents = size * 0.5f;
            switch (cornerNumber)
            {
                case 0:
                    offset.Set(Bounds.width * 0.5f - radius, Bounds.height * 0.5f - radius);
                    break;
                case 1:
                    offset.Set(-Bounds.width * 0.5f + radius, Bounds.height * 0.5f - radius);
                    break;
                case 2:
                    offset.Set(-Bounds.width * 0.5f + radius, -Bounds.height * 0.5f + radius);
                    break;
                case 3:
                    offset.Set(Bounds.width * 0.5f - radius, -Bounds.height * 0.5f + radius);
                    break;
            }

            //If there's no (or negative) radius, then there's no need for the points
            if (radius > 0)
            {
                float pi = Mathf.PI;
                for (float angle = pi * 0.5f * cornerNumber; angle < pi * 0.5f * (cornerNumber + 1); angle += 1f / levelOfDetail)
                {
                    float n = 2.4f; //This is the superness.  I find 2.4f to be a good number
                    float na = 2 / n;
                    float angleSine = Mathf.Sin(angle);
                    float angleCosine = Mathf.Cos(angle);
                    float x = Mathf.Pow(Mathf.Abs(angleCosine), na) * radius * Mathf.Sign(angleCosine);
                    float y = Mathf.Pow(Mathf.Abs(angleSine), na) * radius * Mathf.Sign(angleSine);
                    Vector2 xy = new Vector2(x, y) + offset;
                    meshPoints.Add(xy);
                    meshUVs.Add((xy - extents) / size);
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
                if (!meshPoints.Contains(extraPoint))
                {
                    meshPoints.Add(extraPoint);
                    meshUVs.Add((extraPoint - extents) / size);
                }
            }
            else
            {
                meshPoints.Add(offset);
                meshUVs.Add((offset - extents) / size);
            }
        }

        public void CreateMesh(Mesh mesh)
        {
            meshPoints.Clear();
            meshUVs.Clear();

            for (int i = 0; i < 4; i++)
            {
                GetCornerPoints(i, cornerRadii[i], levelOfDetail);
            }

            //Use Triangulator to get indices for creating triangles
            Triangulator.SetPoints(meshPoints);
            int[] indices = Triangulator.Triangulate();

            Vector3[] vertices = new Vector3[meshPoints.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(meshPoints[i].x, meshPoints[i].y, 0);
            }

            //Create the mesh
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            mesh.Clear();
            mesh.name = $"Frame {GetHashCode()} - Smooth";
            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, meshUVs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.Optimize();
        }
    }
}