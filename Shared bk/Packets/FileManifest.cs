using System.Collections.Generic;

namespace Shared.Packets
{
    public enum FileType
    {
        Normal = 0,
        Map = 1,
        Script = 2,
        EndOfTransfer = 3,
        CustomData = 4,
    }

    public class FileManifest
    {
        public Dictionary<string, List<FileDeclaration>> exportedFiles = new Dictionary<string, List<FileDeclaration>>();
    }

    public class FileDeclaration
    {
        public FileDeclaration(string _path, string _hash, FileType _type)
        {
            path = _path;
            hash = _hash;
            type = _type;
        }

        public FileType type { get; set; }
        public string path { get; set; }
        public string hash { get; set; }
    }
}
