namespace HammingCode.App.Domain
{
    public class Transmitter
    {
        public Message LastSent { get; private set; }
        public Message LastSentHamming { get; private set; }
        public readonly HammingBlock HammingBlock;
        public readonly double Bitrate;

        public Transmitter(HammingBlock hammingBlock)
        {
            HammingBlock = hammingBlock;
        }

        public void SendMessage(string message, Receiver destination, bool noisyChannel)
        {
            LastSent = new Message(message, isAscii: false);
            LastSentHamming = HammingBlock.GetResult(LastSent);
            Channel.SendMessage(LastSentHamming, destination, noisyChannel);
        }
    }
}