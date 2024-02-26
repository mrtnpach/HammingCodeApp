# HammingCodeApp
Calculadora de **código Hamming**[^1] desarrollada en .NET Core 3.1 y Windows Forms.

![Captura de pantalla de la calculadora de código Hamming](https://imgur.com/a/IFTYfG4)

La aplicación simula la generación del código Hamming en el emisor y su posterior decodificación en el receptor. Es posible introducir ruido en el canal de comunicación, produciendo un error de 1 bit sobre una posición aleatoria.
El receptor es capaz de determinar la posición del error.

El algoritmo usado para la generación emplea operaciones bit-wise simples en diversos pasos. Esto incluye la implementación de type punning para la conversión del mensaje a un número sobre el cual se aplican las transformaciones.
Nota: Esto introduce limitaciones sujetas a la longitud del mensaje a enviar. Si su longitud es mayor a 4 caracteres, no es posible extraee el contenido del mensaje recibido. Si la longitud es mayor a 7 caracteres, el algoritmo no funciona, ya que la cadena - incluyendo los bits de paridad - debe ser de menos de 64 bits. Estas cuestiones pueden ser resueltas mejorando el algoritmo.

´´´
public class HammingBlock
{
    public unsafe string TranslateAsciiStringToString(string asciiString)
    {
        [...]
        char[] stringCopy = asciiString.ToCharArray();
        Array.Reverse(stringCopy);
        long stringAsNumber = 0;
        for (int position = 0; position < asciiString.Length; position++)
        {
            int value = stringCopy[position] == '1' ? 1 : 0;
            checked { stringAsNumber |= value << position; }
        }
        [...]
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
        [...]
    }
}

´´´

[^1]: Wikipedia. _Hamming Code_. Accedido el 26 de febrero, 2024. https://en.wikipedia.org/wiki/Hamming_code

