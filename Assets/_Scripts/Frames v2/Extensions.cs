using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FramesV2
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Wrap(this int input, int min, int max, int iterations)
        {
            if (input >= min && input < max) { return input; }

            for (int i = 0; i < iterations; i++)
            {
                if (input < min)
                {
                    input += max - min;
                }
                else if (input >= max)
                {
                    input -= max - min;
                }
            }
            return input;
        }
    }
}