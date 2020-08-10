using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

namespace ifelse.Frames.v2
{
    public class FrameJobs
    {
        public void CalculateRoundFrame(RoundFrame frame)
        {

        }

        [BurstCompile]
        private struct CalcualteFrameVerticesJob : IJobParallelFor
        {
            [ReadOnly] public int CornerDetail;
            [ReadOnly] public float Step;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> CornerRadii;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float3> Offsets;

            [WriteOnly] public NativeList<float3> Vertices;

            public void Execute(int index)
            {
                int cornerIndex = index / CornerDetail;
                float3 offset = Offsets[cornerIndex];
                float radius = CornerRadii[cornerIndex];

                if (radius > 0)
                {
                    float angle = math.PI * 2 * Step * index;

                    float3 cornerPoint = new float3
                    (
                        math.cos(angle),
                        math.sin(angle),
                        0
                    );
                    cornerPoint *= radius;
                    cornerPoint += offset;

                    Vertices.Add(cornerPoint);
                }
                else if ((index - 1) / CornerDetail < cornerIndex)
                {
                    Vertices.Add(offset);
                }
            }
        }

        [BurstCompile]
        private struct CalculateFrameIndicesJob : IJobParallelFor
        {
            [ReadOnly] public int CornerDetail;

            public void Execute(int index)
            {

            }
        }
    }
}