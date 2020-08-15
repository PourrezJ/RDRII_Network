namespace Shared
{
    public class LauncherSettings
    {
        public static string[] GameParams = new string[8];

        public interface ISubprocessBehaviour
        {
            void Start(string[] args);
        }
    }
}
