using System;
using System.Collections.Generic;
using System.Text;

namespace HammingCode.App.Domain
{
    public static class Channel
    {
        public static void SendMessage(Message message, Receiver receiver, bool isNoisy = false)
        {
            if (isNoisy)
                message = AddError(message);
            receiver.ReceiveMessage(message);
        }

        public static Message AddError(Message message)
        {
            Random rdm = new Random();
            int position = rdm.Next(0, message.Size);
            char[] asciiStringCopy = message.ASCIIString.ToCharArray();
            asciiStringCopy[position] = message.ASCIIString[position] == '1' ? '0' : '1';
            return new Message(new string(asciiStringCopy), isAscii: true);
        }
    }
}
