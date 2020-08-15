using MessagePack;

namespace ResuMPServer.Constant
{
    [MessagePackObject]
    internal class ChatData
    {
        [Key(0)]
        public long Id { get; set; }
        [Key(1)]
        public string Sender { get; set; }
        [Key(2)]
        public string Message { get; set; }
    }
}