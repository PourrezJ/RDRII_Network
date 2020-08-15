using MessagePack;

namespace Shared
{
    [MessagePackObject]
    public class DataDownloadStart
    {
        [Key(0)]
        public int Id { get; set; }

        [Key(1)]
        public byte FileType { get; set; }

        [Key(2)]
        public string FileName { get; set; }

        [Key(3)]
        public string ResourceParent { get; set; }

        [Key(4)]
        public int Length { get; set; }

        [Key(5)]
        public string Md5Hash { get; set; }
    }
}