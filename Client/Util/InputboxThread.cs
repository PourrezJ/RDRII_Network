using System;
using System.Collections.Generic;
using RDRN_Core;

namespace RDRN_Core.Util
{
    public class InputboxThread : Script
    {
        public InputboxThread()
        {
            Tick += (sender, args) =>
            {
                if (ThreadJumper.Count > 0)
                {
                    ThreadJumper.Dequeue().Invoke();
                }
            };
        }

        public static string GetUserInput(string defaultText, Action spinner)
        {
            string output = null;

            ThreadJumper.Enqueue(delegate
            {
                output = Game.GetUserInput("", defaultText, 99);
            });

            Main.BlockControls = true;

            Script.Yield();

            while (output == null)
            {
                spinner.Invoke();
                Script.Yield();
            }
            Main.BlockControls = false;
            return output;
        }

        public static string GetUserInput(Action spinner)
        {
            return GetUserInput("", spinner);
        }

        public static Queue<Action> ThreadJumper = new Queue<Action>();
    }
}