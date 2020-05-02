using System.Threading;

namespace Launcher
{
    public class SplashScreenThread
    {
        private Thread _thread;

        private delegate void CloseForm();
        private delegate void SetPercentDel(int newPercent);

        public SplashScreen SplashScreen;

        public SplashScreenThread()
        {
            _thread = new Thread(Show);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void SetPercent(int newPercent)
        {
            while (SplashScreen == null) Thread.Sleep(10);
            if (SplashScreen.InvokeRequired)
            {
                SplashScreen.Invoke(new SetPercentDel(SetPercent), newPercent);

            }
            else
            {
                SplashScreen.progressBar1.Value = newPercent;
            }
        }

        public void Stop()
        {
            if (SplashScreen.InvokeRequired)
                SplashScreen.Invoke(new CloseForm(Stop));
            else
                SplashScreen.Close();
        }

        public void Show()
        {
            SplashScreen = new SplashScreen();
            SplashScreen.ShowDialog();
        }
    }
}
