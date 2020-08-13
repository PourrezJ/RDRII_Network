using Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Launcher
{
    public class MainBehaviour
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);


        private static string RDRNFolder = Directory.GetCurrentDirectory() + "\\";

        private PlayerSettings playerSetings;


        public void Start(string[] args)
        {
            playerSetings = new PlayerSettings();

            #region Create splash screen
            SplashScreenThread splashScreen = new SplashScreenThread();
            #endregion

            System.Threading.Thread.Sleep(5500);

            #region Create settings.xml if it does not exist
            if (!File.Exists(RDRNFolder + "settings.xml") || string.IsNullOrWhiteSpace(File.ReadAllText(RDRNFolder + "settings.xml")))
            {
                System.Threading.Thread.Sleep(200);
                var ser = new XmlSerializer(typeof(PlayerSettings));
                using (var stream = File.OpenWrite(RDRNFolder + "settings.xml"))
                {
                    ser.Serialize(stream, playerSetings);
                }
            }
            #endregion

            #region Read settings.xml
            PlayerSettings settings = null;
            settings = PlayerSettings.ReadSettings(RDRNFolder + "settings.xml");
            #endregion

            splashScreen.SetPercent(10);
            splashScreen.SetPercent(15);

            #region Check if RDR2 or RDR2Launcher is running
            if (Process.GetProcessesByName("RDR2").Any())
            {
                MessageBox.Show(splashScreen.SplashScreen, "RDR2 or the RDRII:Network is already running. Please close them before starting RDRII:Network.");
                return;
            }
            #endregion

            #region Check for dependencies
            if (!Environment.Is64BitOperatingSystem)
            {
                MessageBox.Show(splashScreen.SplashScreen, "RDRII:Network does not work on 32bit machines.", "Incompatible");
                return;
            }
            #endregion

            #region Check CEF version
            if (!Directory.Exists(RDRNFolder + "cef") || !File.Exists(RDRNFolder + "cef\\libcef.dll"))
            {
                MessageBox.Show(splashScreen.SplashScreen, "CEF directory or one of the core CEF components is missing from the directory, please reinstall.");
                return;
            }
            #endregion

            #region Check for new client version

            ParseableVersion fileVersion = new ParseableVersion(0, 0, 0, 0);

            var modulePath = Path.Combine(RDRNFolder, "bin\\Scripts\\RDRN_Core.dll");

            if (File.Exists(modulePath))
            {
                fileVersion = ParseableVersion.Parse(FileVersionInfo.GetVersionInfo(modulePath).FileVersion);
            }

            splashScreen.SetPercent(30);

            #endregion

            splashScreen.SetPercent(35);

            #region Check GamePath directory
            if (string.IsNullOrWhiteSpace(settings.GamePath) || !File.Exists(Path.Combine(settings.GamePath, "RDR2.exe")))
            {
                var diag = new OpenFileDialog
                {
                    DefaultExt = ".exe",
                    RestoreDirectory = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                };
                if (diag.ShowDialog() == DialogResult.OK)
                {
                    settings.GamePath = Path.GetDirectoryName(diag.FileName);
                    try
                    {
                        PlayerSettings.SaveSettings(RDRNFolder + "settings.xml", settings);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show(splashScreen.SplashScreen, "Insufficient permissions, Please run as Admin to avoid permission issues. (2)", "Unauthorized access");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            #endregion

            splashScreen.SetPercent(45);

            #region Check required folders and clean up

            #endregion

            #region Patching Game Settings
            var mySettings = GameSettings.LoadGameSettings();

            if (mySettings.Video != null)
            {
                if (mySettings.Video.PauseOnFocusLoss != null)
                    mySettings.Video.PauseOnFocusLoss.Value = "false";
            }
            if (mySettings.AdvancedGraphics != null)
            {
                if (mySettings.AdvancedGraphics != null)
                    mySettings.AdvancedGraphics.API = "kSettingAPI_DX12";
            }

            try
            {
                GameSettings.SaveSettings(mySettings);
            }
            catch
            {
                MessageBox.Show(splashScreen.SplashScreen, "Insufficient permissions, Please run as Admin to avoid permission issues.(8)", "Unauthorized access");
                return;
            }
            #endregion

            splashScreen.SetPercent(90);

            #region Copy over the savegame
            /*
            foreach (var file in Directory.GetFiles(Profiles, "pc_settings.bin", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.Exists((Path.GetDirectoryName(file) + "\\" + "SGTA50000")))
                        MoveFile(Path.GetDirectoryName(file) + "\\" + "SGTA50000", Path.GetDirectoryName(file) + "\\" + "SGTA50000.bak");

                    if (File.Exists(RDRNFolder + "savegame" + "\\" + "SGTA50000"))
                        File.Copy(RDRNFolder + "savegame" + "\\" + "SGTA50000", Path.GetDirectoryName(file) + "\\" + "SGTA50000");
                }
                catch (Exception e)
                {
                    //MessageBox.Show(splashScreen.SplashScreen, "Insufficient permissions, Please run as Admin to avoid permission issues. (4)", "Unauthorized access");
                    MessageBox.Show(splashScreen.SplashScreen, e.ToString(), "Unauthorized access");
                    return;
                }
            }*/
            #endregion

            splashScreen.SetPercent(95);

            #region Launch the Game

            BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(Path.Combine(settings.GamePath, "RDR2.exe"))));
            br.BaseStream.Position = 0x01500000;
            byte[] array = br.ReadBytes(0x35F757);
            string value = BitConverter.ToString(array).Replace("-", string.Empty);

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = value.Contains("737465616D") ? "steam://run/1174180" : Path.Combine(settings.GamePath, "RDR2.exe"),
                UseShellExecute = false,
                //Arguments = "-scOfflineOnly -minmodeapp=rdr2 -user_restart -doneUpgrade"
            };
            Process whatever = Process.Start(info);
            #endregion

            splashScreen.SetPercent(100);
            try
            {
                #region Wait for the Game to launch
                Process rdr2Process = null;


                while (FindWindow("sgaWindow", "Red Dead Redemption 2") == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                }

                //Thread.Sleep(100);

                if (Process.GetProcessesByName("RDR2").Length > 0)
                    rdr2Process = Process.GetProcessesByName("RDR2").FirstOrDefault();

                // Thread.Sleep(100);
                #endregion

                splashScreen.SetPercent(100);
                splashScreen.Stop();

                #region Inject into RDR2
                //Thread.Sleep(25000);
                InjectOurselves(rdr2Process);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #region Restore save game
            /*
            foreach (var file in Directory.GetFiles(Profiles, "pc_settings.bin", SearchOption.AllDirectories))
            {
                try
                {
                    if (File.Exists((Path.GetDirectoryName(file) + "\\" + "SGTA50000")))
                        File.Delete(Path.GetDirectoryName(file) + "\\" + "SGTA50000");

                    if (File.Exists((Path.GetDirectoryName(file) + "\\" + "SGTA50000.bak")))
                        MoveFile(Path.GetDirectoryName(file) + "\\" + "SGTA50000.bak", Path.GetDirectoryName(file) + "\\" + "SGTA50000");
                }
                catch (Exception)
                {
                    MessageBox.Show(splashScreen.SplashScreen, "Insufficient permissions, Please run as Admin to avoid permission issues. (5)", "Unauthorized access");
                    return;
                }
            }*/
            #endregion

        }

        public static void InjectOurselves(Process rdr2)
        {
            Mem m = new Mem();

            try
            {
                m.OpenProcess(rdr2.Id);
                
                m.InjectDll(Path.Combine(RDRNFolder, "bin\\ScriptHookRDR2.dll"));
                m.InjectDll(Path.Combine(RDRNFolder, "bin\\RDRN_Module.dll"));
                /*
                Task.Run(async () =>
                {
                    var native_base = await m.AoBScan("0F B6 C1 48 8D 15 ? ? ? ? 4C 8B C9");
                    if (native_base != null)
                        MessageBox.Show("penis");
                });*/
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Dir and Files utils

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                NoReadonly(file);
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                NoReadonly(dest);
                File.Copy(file, dest, true);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        public static void NoReadonly(string path)
        {
            if (File.Exists(path))
                new FileInfo(path).IsReadOnly = false;
        }

        public static IntPtr FindWindow(string windowName)
        {
            var hWnd = FindWindow(windowName, null);
            return hWnd;
        }
        #endregion

    }
}
