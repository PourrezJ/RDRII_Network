using System.IO;
using System.Xml.Serialization;

namespace Shared
{
    public class PlayerSettings
    {
        public bool CEFDevtool { get; set; }

        public string GamePath { get; set; }
        public static string DisplayName { get; set; }

        public PlayerSettings()
        {

        }

        public static PlayerSettings ReadSettings(string path)
        {
            var ser = new XmlSerializer(typeof(PlayerSettings));

            PlayerSettings settings = new PlayerSettings();

            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path)) settings = (PlayerSettings)ser.Deserialize(stream);
            }

            return settings;
        }

        public static void SaveSettings(string path, PlayerSettings set)
        {
            var ser = new XmlSerializer(typeof(PlayerSettings));
            if (File.Exists(path))
            {
                using (var stream = new FileStream(path, FileMode.Truncate)) ser.Serialize(stream, set);
            }
            else
            {
                using (var stream = new FileStream(path, FileMode.Create)) ser.Serialize(stream, set);
            }
        }
    }
}
