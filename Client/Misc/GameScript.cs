using RDRN_Core.Native;

namespace RDRN_Core.Misc
{
    public static class GameScript
    {
        public static void DisableAll(bool disableeditor)
        {
            /*
            if (disableeditor)
            {
                Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "replay_controller");
                Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, "selector");
            }

            for (int i = 0; i < _list.Length; i++)
            {
                Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, _list[i]);
            }*/
        }

        private static int _index;

        public static void Pulse()
        {
            //Function.Call(Hash.TERMINATE_ALL_SCRIPTS_WITH_THIS_NAME, _list[_index]);

            _index = (_index + 1)%_list.Length;
        }
        
        private static readonly string[] _list = new []
        {
            ""
        };
    }
}