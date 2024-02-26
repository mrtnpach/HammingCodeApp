using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammingCode.App.Domain
{
    public static class HammingGenerator
    {
        // Consider using a HammingCodedMessage class with an AddError method
        public static string GetResult(Message message)
        {
            int p = CalculateParityBits(message.Size);
            char[] result = GenerateArray(message, p);
            SetParityBits(result, p);
            return new string(result);
        }

        public static int CalculateParityBits(int n)
        {
            int p = 0;
            while (Math.Pow(2, p) < n + p + 1)
            {
                p++;
            }
            return p;
        }

        private static char[] GenerateArray(Message message, int parityBitsCount)
        {
            int size = message.Size + parityBitsCount;
            bool[] maskArray = Enumerable.Repeat(true, size).ToArray();
            for (int i = 0; i < parityBitsCount; i++)
            {
                int position = (int)Math.Pow(2, i) - 1;
                maskArray[position] = false;
            }
            Array.Reverse(maskArray);
            char[] hammingArray = new char[size];
            int k = 0, j = 0;
            for (k = 0; k < size; k++)
            {
                if (maskArray[k])
                {
                    hammingArray[k] = message.ASCIIString[j];
                    j++;
                }
            }
            return hammingArray;
        }

        private static void SetParityBits(char[] emptyArray, int parityBitsCount)
        {
            char[] copyArray = new char[emptyArray.Length];
            Array.Copy(emptyArray, copyArray, emptyArray.Length);
            Array.Reverse(copyArray);
            int positionFactor = 1;
            for (int i = 0; i < parityBitsCount; i++)
            {
                int position = (int)Math.Pow(2, i) - 1;
                int result = CalculateParityBit(position + 1, positionFactor, copyArray);
                copyArray[position] = result == 0 ? '0' : '1';
                positionFactor *= 2;
            }
            Array.Reverse(copyArray);
            Array.Copy(copyArray, emptyArray, emptyArray.Length);
        }

        private static int CalculateParityBit(int initialPosition, int positionFactor, char[] array)
        {
            int parity = 0;
            for (int i = initialPosition; i < array.Length; i++)
            {
                var x = i + 1 & positionFactor;
                if (x != 0)
                {
                    if (array[i] == '1')
                        parity++;
                }
            }
            return parity % 2;
        }

        // Implemented for up to 32 parity bits only
        public static int DetermineErrorPosition(Message message)
        {
            int parityBitsCount = CalculateParityBits(message.Size);
            if (parityBitsCount > sizeof(int) * 8)
                throw new Exception("Implementado para un maximo de 32 bits de paridad");
            int errorPosition = 0;

            char[] messageArray = message.RealMessage.ToCharArray();
            char[] copyArray = new char[message.RealMessage.Length];
            Array.Copy(messageArray, copyArray, messageArray.Length);
            Array.Reverse(copyArray);

            int positionFactor = 1;
            for (int i = 0; i < parityBitsCount; i++)
            {
                int position = (int)Math.Pow(2, i) - 1;
                int result = CalculateParityBit(position, positionFactor, copyArray);
                errorPosition = errorPosition | result * positionFactor;
                positionFactor *= 2;
            }

            return errorPosition;
        }
    }
}
