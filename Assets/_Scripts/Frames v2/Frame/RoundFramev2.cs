using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace ifelse.Frames.v2
{
    public class RoundFramev2 : IFrameObjectv2
    {
        public RectTransform RectTransform { get; set; }
        public Rect Rect { get { return RectTransform.rect; } }
        public float3 Extents { get { return new float3(Rect.width * 0.5f, Rect.height * 0.5f, 0); } }

        public RoundFramev2(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
        }

        public void GenerateFrame(Mesh mesh, int detail, float[] cornerRadii)
        {
            float turnStep = (math.PI * 0.5f) / (detail - 1);
            int vertexCount = 4 + detail * 4;

            float3 extents = Extents;
            float3[] cornerExtents = new float3[4]
            {
                new float3(extents.x - cornerRadii[0], extents.y - cornerRadii[0], 0),
                new float3(-extents.x + cornerRadii[1], extents.y - cornerRadii[1], 0),
                new float3(-extents.x + cornerRadii[2], -extents.y + cornerRadii[2], 0),
                new float3(extents.x - cornerRadii[3], -extents.y + cornerRadii[3], 0)
            };

            int indexCount = detail * 12 + 30;

            NativeArray<float3> vertices = new NativeArray<float3>(vertexCount, Allocator.TempJob);
            NativeArray<int> indices = new NativeArray<int>(indexCount, Allocator.TempJob);

            indices[indexCount - 6] = detail * 3 + 2;
            indices[indexCount - 5] = detail * 2 + 1;
            indices[indexCount - 4] = detail * 1 + 0;
            indices[indexCount - 3] = detail * 4 + 3;
            indices[indexCount - 2] = detail * 3 + 2;
            indices[indexCount - 1] = detail * 1 + 0;

            JobHandle dependency = new CalculateVerticesJob
            {
                TurnStep = turnStep,
                VerticesPerCorner = detail + 1,
                Extents = new NativeArray<float3>(cornerExtents, Allocator.TempJob),
                CornerRadii = new NativeArray<float>(cornerRadii, Allocator.TempJob),
                Vertices = vertices,
            }.Schedule(vertexCount, 32);

            int cornerIndexCount = (indexCount - 30) / 3;
            dependency = new CalculateIndicesJob
            {
                OuterVertexCount = detail,
                CornerVertexCount = detail + 1,
                Indices = indices
            }.Schedule(cornerIndexCount, 32, dependency);

            dependency = new CalculateQuadIndicesJob
            {
                StartIndex = cornerIndexCount * 3,
                CornerVertexCount = detail + 1,
                VertexCount = vertexCount,
                Indices = indices
            }.Schedule(4, 32, dependency);

            dependency.Complete();

            mesh.SetVertices(vertices);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            vertices.Dispose();
            indices.Dispose();
        }

        [BurstCompile]
        private struct CalculateVerticesJob : IJobParallelFor
        {
            [ReadOnly] public float TurnStep;
            [ReadOnly] public int VerticesPerCorner;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float3> Extents;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> CornerRadii;

            [WriteOnly] public NativeArray<float3> Vertices;

            public void Execute(int index)
            {
                int cornerIndex = index / VerticesPerCorner;
                int vertexIndex = index % VerticesPerCorner;

                float turn = TurnStep * vertexIndex;

                float3 extent = Extents[cornerIndex];
                float3 point = new float3(math.cos(turn) * math.sign(extent.x), math.sin(turn) * math.sign(extent.y), 0) * CornerRadii[cornerIndex];

                if (vertexIndex < VerticesPerCorner - 1)
                {
                    Vertices[index] = extent + point;
                }
                else
                {
                    Vertices[index] = extent;
                }
            }
        }

        [BurstCompile]
        private struct CalculateIndicesJob : IJobParallelFor
        {
            [ReadOnly] public int OuterVertexCount;
            [ReadOnly] public int CornerVertexCount;

            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> Indices;

            public void Execute(int index)
            {
                int cornerIndex = index / OuterVertexCount;
                int cornerStartIndex = cornerIndex * CornerVertexCount;
                int vertexIndex = cornerStartIndex + index % OuterVertexCount;
                int indexIndex = index * 3;

                if (cornerIndex % 2 == 1)
                {
                    Indices[indexIndex + 2] = cornerStartIndex + OuterVertexCount;
                    Indices[indexIndex + 1] = vertexIndex + 1;
                    Indices[indexIndex + 0] = vertexIndex;
                }
                else
                {
                    Indices[indexIndex + 0] = cornerStartIndex + OuterVertexCount;
                    Indices[indexIndex + 1] = vertexIndex + 1;
                    Indices[indexIndex + 2] = vertexIndex;
                }
            }
        }

        [BurstCompile]
        private struct CalculateQuadIndicesJob : IJobParallelFor
        {
            [ReadOnly] public int StartIndex;
            [ReadOnly] public int CornerVertexCount;
            [ReadOnly] public int VertexCount;

            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> Indices;

            public void Execute(int index)
            {
                int quadStartIndex = StartIndex + index * 6;
                int indexStart = (index + 1) * CornerVertexCount - 1;

                int a;
                int b;
                int c;
                int d;

                if (index % 2 == 0)
                {
                    a = (indexStart - 1).Wrap(0, VertexCount, 1);
                    b = (indexStart).Wrap(0, VertexCount, 1);
                    c = (indexStart + CornerVertexCount - 1).Wrap(0, VertexCount, 1);
                    d = (indexStart + CornerVertexCount).Wrap(0, VertexCount, 1);
                }
                else
                {
                    a = (indexStart - CornerVertexCount + 1).Wrap(0, VertexCount, 1);
                    b = (indexStart).Wrap(0, VertexCount, 1);
                    c = (indexStart + 1).Wrap(0, VertexCount, 1);
                    d = (indexStart + CornerVertexCount).Wrap(0, VertexCount, 1);
                }

                Indices[quadStartIndex + 0] = a;
                Indices[quadStartIndex + 1] = b;
                Indices[quadStartIndex + 2] = c;
                Indices[quadStartIndex + 3] = b;
                Indices[quadStartIndex + 4] = d;
                Indices[quadStartIndex + 5] = c;
            }
        }

        [BurstCompile]
        private struct CaclculateUVsJob : IJobParallelFor
        {
            public void Execute(int index)
            {

            }
        }
    }
}