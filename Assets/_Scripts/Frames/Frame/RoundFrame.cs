using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Frames
{
    public class RoundFrame : IFrameableObject
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get; set; }

        public FrameCornerType CornerType { get { return FrameCornerType.Round; } }
        public Triangulator Triangulator { get; }

        public float[] cornerRadii;
        public int levelOfDetail;

        public List<Vector2> meshPoints = new List<Vector2>();
        public List<int> meshIndices = new List<int>();
        public List<Vector2> meshUVs = new List<Vector2>();

        //Frame corners are created similar to progressing along a unit circle.
        public RoundFrame(RectTransform rectTransform, float[] cornerRadii, int levelOfDetail)
        {
            this.RectTransform = rectTransform;
            this.Bounds = rectTransform.rect;
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
                for (int i = 0; i < levelOfDetail; i++)
                {
                    float angle = i * Mathf.PI * 0.5f / levelOfDetail + cornerNumber * Mathf.PI * 0.5f;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + offset;
                    meshPoints.Add(point);
                    meshUVs.Add((point - extents) / size);
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
            meshIndices.Clear();
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
            mesh.name = $"Frame {GetHashCode()} - Round";
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