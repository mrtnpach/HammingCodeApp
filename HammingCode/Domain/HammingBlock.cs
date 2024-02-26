using System;
using System.Linq;

namespace HammingCode.App.Domain
{
    public class HammingBlock
    {
        public Message GetResult(Message message)
        {
            int p = CalculateRequiredParityBits(message.Size);  // Calcula la cantidad de bits de paridad necesarios
            char[] result = GenerateArray(message, p);  // Genera un array con los bit de paridad sin inicializar
            SetParityBits(result, p);                   // Setea los bits de paridad
            Message hammingMessage = new Message(new string(result), isAscii: true);
            return hammingMessage;
        }

        public int CalculateRequiredParityBits(int n)
        {
            int p = 0;
            while (Math.Pow(2, p) < n + p + 1)
            {
                p++;
            }
            return p;
        }

        public int CalculateUsedParityBits(int np)
        {
            int p = 0;
            while (Math.Pow(2, p) < np + 1)
            {
                p++;
            }
            return p;
        }

        private char[] GenerateArray(Message message, int parityBitsCount)
        {
            // Longitud es igual a (n + p)
            int size = message.Size + parityBitsCount;

            // Creo una mascara en donde los bits de paridad son false
            bool[] maskArray = Enumerable.Repeat(true, size).ToArray();
            for (int i = 0; i < parityBitsCount; i++)
            {
                // Calculo la posicion de un bit de paridad, sabiendo que es potencia de 2:
                int position = (int)Math.Pow(2, i) - 1;
                maskArray[position] = false;
            }
            Array.Reverse(maskArray);   // Invierto para mantener la posicion de der. a izq.

            char[] hammingArray = new char[size];
            int k = 0, j = 0;
            for (k = 0; k < size; k++)
            {
                // Solo avanzo el caracter del mensaje si no es un bit de paridad
                if (maskArray[k])
                {
                    hammingArray[k] = message.ASCIIString[j];
                    j++;
                }
            }
            return hammingArray;
        }

        private void SetParityBits(char[] uncalculatedArray, int parityBitsCount)
        {
            Array.Reverse(uncalculatedArray);

            // Usado para determinar si el bit de la posicion analizada debe usarse para el calculo:
            int positionFactor = 1;

            for (int i = 0; i < parityBitsCount; i++)
            {
                int position = (int)Math.Pow(2, i) - 1; // Posicion del iesimo bit de paridad (0 based)
                // Se le suma 1 a position para empezar el calculo al lado del bit de paridad:
                int result = CalculateParityBit(position + 1, positionFactor, uncalculatedArray);
                uncalculatedArray[position] = result == 0 ? '0' : '1';
                positionFactor *= 2;
            }
            Array.Reverse(uncalculatedArray);
        }

        private int CalculateParityBit(int initialPosition, int positionFactor, char[] array)
        {
            int parity = 0;
            for (int currentPosition = initialPosition; currentPosition < array.Length; currentPosition++)
            {
                // Determina si el bit en la posicion dada debe contarse para la paridad.
                // Sumo 1 para hacerlo 1 based:
                var x = currentPosition + 1 & positionFactor;
                if (x != 0)
                {
                    if (array[currentPosition] == '1')
                        parity++;
                }
            }
            return parity % 2;
        }

        // Implemented for up to 32 parity bits only
        public int DetermineErrorPosition(Message message)
        {
            int parityBitsUsed = CalculateUsedParityBits(message.Size);
            if (parityBitsUsed > sizeof(int) * 8)
                throw new Exception("Implementado para un maximo de 32 bits de paridad");

            char[] messageArray = message.ASCIIString.ToCharArray();
            Array.Reverse(messageArray);

            // Usado para determinar si el bit de la posicion analizada debe usarse para el calculo:
            int positionFactor = 1;
            int errorPosition = 0;

            for (int i = 0; i < parityBitsUsed; i++)
            {
                int position = (int)Math.Pow(2, i) - 1; // Posicion del iesimo bit de paridad (0 based)
                // No se le suma 1 a position porque el bit de paridad tambien es usado para el calculo:
                int result = CalculateParityBit(position, positionFactor, messageArray);
                errorPosition = errorPosition | result * positionFactor;  // Seteo el bit de paridad en el indicador de posicion
                positionFactor *= 2;
            }

            return errorPosition;
        }

        public string ExtractInnerMessage(Message hamming)
        {
            int parityBitsUsed = CalculateUsedParityBits(hamming.Size);
            int[] parityPositions = new int[parityBitsUsed];
            for (int i = 0; i < parityBitsUsed; i++)
            {
                parityPositions[i] = (int)Math.Pow(2, i);
            }
            char[] hammingArray = hamming.ASCIIString.ToCharArray();
            char[] contentsArray = new char[hamming.Size - parityBitsUsed];
            int contentsPosition = 0;
            Array.Reverse(hammingArray);
            bool isParityBit = false;
            for (int position = 1; position < hamming.Size + 1; position++)
            {
                for (int i = 0; i < parityBitsUsed; i++)
                {
                    if (position == parityPositions[i])
                    {
                        isParityBit = true;
                        break;
                    }
                }
                if (!isParityBit)
                {
                    contentsArray[contentsPosition] = hammingArray[position - 1];
                    contentsPosition++;
                }
                isParityBit = false;
            }
            Array.Reverse(contentsArray);
            return new string(contentsArray);
        }

        public unsafe string TranslateAsciiStringToString(string asciiString)
        {
            // Por algun motivo hay un bug en mensajes de mas de 4 caracteres ASCII
            // No se bien en que parte se produce, pero es entre el calculo de 
            // stringAsNumber o en la generacion de memoryBuilder.

            if (asciiString.Length > 56)    // long de 64 bits -> 56 bits ascii, el resto de paridad
                return "Implementado solo para mensajes de hasta 7 caracteres.";

            char[] stringCopy = asciiString.ToCharArray();
            Array.Reverse(stringCopy);
            long stringAsNumber = 0;
            for (int position = 0; position < asciiString.Length; position++)
            {
                int value = stringCopy[position] == '1' ? 1 : 0;
                checked { stringAsNumber |= value << position; }  // Shifting causa overflow?
            }

            // Posiblemente esta div. causa el error si el asciiString no tiene 
            // longitud multiplo de 8
            int characterCount = asciiString.Length / 8;
            char[] memoryBuilder = new char[characterCount];
            unsafe
            {
                byte* asciiStart = (byte*)&stringAsNumber;
                for (int i = 0; i < characterCount; i++)
                {
                    char current = (char)asciiStart[i];
                    memoryBuilder[i] = current;
                }
            }

            Array.Reverse(memoryBuilder);
            var result = new string(memoryBuilder);
            return result;
        }
    }
}