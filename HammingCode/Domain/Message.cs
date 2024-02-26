using System;
using System.Text;

namespace HammingCode.App.Domain
{
    public class Message
    {
        protected Message()
        {

        }

        public Message(string message, bool isAscii)
        {
            if (string.IsNullOrEmpty(message))
                throw new Exception("Mensaje no valido.");

            if (isAscii)
            {
                //_ascii = Encoding.ASCII.GetBytes(message);
                _ascii = null;
                _asciiString = message;
                //RealMessage = Encoding.ASCII.GetString(_ascii);
            }
            else
            {
                RealMessage = message;
                ASCII = Encoding.ASCII.GetBytes(message);
                ASCIIString = GetAsciiString(ASCII);
            }
        }

        protected string GetAsciiString(byte[] ascii)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in ascii)
            {
                string byteString = Convert.ToString(b, 2).PadLeft(8, '0');
                builder.Append(byteString);
            }
            return builder.ToString();
        }

        protected byte[] _ascii;
        protected string _asciiString;
        protected double _bitrate;

        public byte[] ASCII { get => _ascii; protected set { _ascii = value; } }
        public string ASCIIString { get => _asciiString; protected set { _asciiString = value; } }
        public int Size => ASCIIString.Length;
        public string RealMessage { get; private set; }
        public double Bitrate => _bitrate;
    }
}
