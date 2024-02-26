using System;

namespace HammingCode.App.Domain
{
    public class Receiver
    {
        public readonly HammingBlock HammingBlock;
        public Message ReceivedMessage { get; private set; }

        public Receiver(HammingBlock hammingBlock)
        {
            HammingBlock = hammingBlock;
        }

        public void ReceiveMessage(Message message)
        {
            ReceivedMessage = message;
        }

        public bool MessageHasAnError()
        {
            return HammingBlock.DetermineErrorPosition(ReceivedMessage) == 0;
        }

        public int GetErrorPosition()
        {
            return HammingBlock.DetermineErrorPosition(ReceivedMessage);
        }

        public string GetInnerMessage()
        {
            return HammingBlock.ExtractInnerMessage(ReceivedMessage);
        }
    }
}