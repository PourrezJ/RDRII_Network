using Shared.Packets;
using System;
using System.Collections.Generic;
using System.IO;

namespace RDRN_Core.Misc
{
    internal class FileTransferId : IDisposable
    {
        internal static string _DOWNLOADFOLDER_ = Main.RDRNetworkPath + "\\resources\\";

        internal int Id { get; set; }
        internal string Filename { get; set; }
        internal FileType Type { get; set; }
        internal FileStream Stream { get; set; }
        internal int Length { get; set; }
        internal int DataWritten { get; set; }
        internal List<byte> Data { get; set; }
        internal string Resource { get; set; }
        internal string FilePath { get; set; }

        internal FileTransferId(int id, string name, FileType type, int len, string resource)
        {
            Id = id;
            Filename = name;
            Type = type;
            Length = len;
            Resource = resource;

            FilePath = _DOWNLOADFOLDER_ + name;

            if ((type == FileType.Normal || type == FileType.Script) && name != null)
            {
                if (!Directory.Exists(_DOWNLOADFOLDER_ + name.Replace(Path.GetFileName(name), "")))
                    Directory.CreateDirectory(_DOWNLOADFOLDER_ + name.Replace(Path.GetFileName(name), ""));
                Stream = new FileStream(_DOWNLOADFOLDER_ + name,
                    File.Exists(_DOWNLOADFOLDER_ + name) ? FileMode.Truncate : FileMode.CreateNew);
            }

            if (type != FileType.Normal)
            {
                Data = new List<byte>();
            }
        }

        internal void Write(byte[] data)
        {
            Stream?.Write(data, 0, data.Length);

            Data?.AddRange(data);

            DataWritten += data.Length;
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream.Dispose();
            }

            Stream = null;
        }
    }
}
