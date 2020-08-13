using MessagePack;
using System.IO;

namespace RDRN_Core.Utils
{
    internal class SerializerBinary
    {
        internal static object DeserializeBinary<T>(byte[] data)
        {
            object output;
            using (var stream = new MemoryStream(data))
            {
                try
                {
                    output = MessagePackSerializer.Deserialize<T>(stream);
                }
                catch
                {
                    return null;
                }
            }
            return output;
        }

        internal static byte[] SerializeBinary(object data)
        {
            using (var stream = new MemoryStream())
            {
                stream.SetLength(0);
                MessagePackSerializer.Serialize(stream, data);
                return stream.ToArray();
            }
        }
    }
}
