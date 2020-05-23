using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ifelse.Frames
{
    public class RoundFrame : IFrameableObject
    {
        public RectTransform RectTransform { get; set; }
        public Rect Bounds { get; set; }

        public float[] cornerRadii;
        public int levelOfDetail;

        public List<Vector2> meshPoints = new List<Vector2>();

        //Frame corners are created similar to progressing along a unit circle.
        public RoundFrame(RectTransform rectTransform, float[] cornerRadii, int levelOfDetail)
        {
            this.RectTransform = rectTransform;
            this.Bounds = rectTransform.rect;
            this.cornerRadii = cornerRadii;
            this.levelOfDetail = levelOfDetail;
        }

        private void GetCornerPoints(int cornerNumber, float radius, int levelOfDetail)
        {
            Vector2[] returnPoints = new Vector2[levelOfDetail];

            Vector2 offset = Vector2.zero;
            switch (cornerNumber)
            {
                case 0:
                    offset.Set(Bounds.width / 2 - radius, Bounds.height / 2 - radius);
                    break;
                case 1:
                    offset.Set(-Bounds.width / 2 + radius, Bounds.height / 2 - radius);
                    break;
                case 2:
                    offset.Set(-Bounds.width / 2 + radius, -Bounds.height / 2 + radius);
                    break;
                case 3:
                    offset.Set(Bounds.width / 2 - radius, -Bounds.height / 2 + radius);
                    break;
            }

            //If there's no (or negative) radius, then there's no need for the points
            if (radius > 0)
            {
                for (int i = 0; i < levelOfDetail; i++)
                {
                    float angle = i * Mathf.PI / 2 / levelOfDetail + cornerNumber * Mathf.PI / 2;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius + offset;
                    meshPoints.Add(point);
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
                }
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