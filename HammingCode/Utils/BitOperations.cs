using System;
using System.Collections.Generic;
using System.Text;

namespace HammingCode.App.Utils
{
    public static class BitOperations
    {
        public static void SetBit(this decimal bits, int position, bool value)
        {
            unsafe
            {

                long bit = value ? 1 : 0;
                bit = bit << (int)Math.Pow(2, position);
                decimal neutral = *(decimal*)&bit;
            }
        }
    }
}
