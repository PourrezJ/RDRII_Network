using Shared.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace RDRN_Core.Misc
{
    internal static class DownloadManager
    {
        private static ScriptCollection PendingScripts = new ScriptCollection() { ClientsideScripts = new List<ClientsideScript>() };

        internal static Dictionary<string, string> FileIntegrity = new Dictionary<string, string>();

        private static string[] _allowedFiletypes = new[]
        {
            "audio/basic",
            "audio/mid",
            "audio/wav",
            "image/gif",
            "image/jpeg",
            "image/pjpeg",
            "image/png",
            "image/x-png",
            "image/tiff",
            "image/bmp",
            "video/avi",
            "video/mpeg",
            "audio/mpeg",
            "text/plain",
            "application/x-font-ttf",
        };

        internal static string HashFile(string path)
        {
            byte[] myData;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                myData = md5.ComputeHash(stream);
            }

            return myData.Select(byt => byt.ToString("x2")).Aggregate((left, right) => left + right);
        }

        internal static bool CheckFileIntegrity()
        {
            foreach (var pair in FileIntegrity)
            {
                byte[] myData;

                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(FileTransferId._DOWNLOADFOLDER_ + pair.Key))
                {
                    myData = md5.ComputeHash(stream);
                }

                string hash = myData.Select(byt => byt.ToString("x2")).Aggregate((left, right) => left + right);

                LogManager.WriteLog(LogLevel.Trace, "GOD: " + pair.Value + " == " + hash);

                if (hash != pair.Value) return false;
            }

            return true;
        }

        private static FileTransferId CurrentFile;
        internal static bool StartDownload(int id, string path, FileType type, int len, string md5hash, string resource)
        {
            if (CurrentFile != null)
            {
                LogManager.WriteLog(LogLevel.Trace, "CurrentFile isn't null -- " + CurrentFile.Type + " " + CurrentFile.Filename);
                return false;
            }

            if ((type == FileType.Normal || type == FileType.Script) && Directory.Exists(FileTransferId._DOWNLOADFOLDER_ + path.Replace(Path.GetFileName(path), "")) &&
                File.Exists(FileTransferId._DOWNLOADFOLDER_ + path))
            {
                byte[] myData;

                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(FileTransferId._DOWNLOADFOLDER_ + path))
                {
                    myData = md5.ComputeHash(stream);
                }

                string hash = myData.Select(byt => byt.ToString("x2")).Aggregate((left, right) => left + right);

                //FileIntegrity.Set(path, md5hash);

                if (hash == md5hash)
                {
                    if (type == FileType.Script)
                    {
                        PendingScripts.ClientsideScripts.Add(LoadScript(path, resource, File.ReadAllText(FileTransferId._DOWNLOADFOLDER_ + path)));
                    }

                    LogManager.WriteLog(LogLevel.Trace, "HASH MATCHES, RETURNING FALSE");
                    return false;
                }
            }

            CurrentFile = new FileTransferId(id, path, type, len, resource);
            return true;
        }

        internal static ClientsideScript LoadScript(string file, string resource, string script)
        {
            var csScript = new ClientsideScript
            {
                Filename = Path.GetFileNameWithoutExtension(file)?.Replace('.', '_'),
                ResourceParent = resource,
                Script = script
            };


            return csScript;
        }

        internal static void Cancel()
        {
            CurrentFile = null;
        }

        internal static void DownloadPart(int id, byte[] bytes)
        {
            if (CurrentFile == null || CurrentFile.Id != id)
            {
                return;
            }

            CurrentFile.Write(bytes);
            if (CurrentFile.Type != FileType.EndOfTransfer)
            {
                //Main.LoadingPromptText();
                /*
                Main.LoadingPromptText("Downloading " +
                    ((CurrentFile.Type == FileType.Normal || CurrentFile.Type == FileType.Script)
                        ? CurrentFile.Filename
                        : CurrentFile.Type.ToString()) + ": " +
                    (CurrentFile.DataWritten / (float)CurrentFile.Length).ToString("P"));*/
            }

        }

        internal static void End(int id)
        {/*
            if (CurrentFile == null || CurrentFile.Id != id)
            {
                Util.Util.SafeNotify($"END Channel mismatch! We have {CurrentFile?.Id} and supplied was {id}");
                return;
            }

            try
            {
                if (CurrentFile.Type == FileType.Map)
                {
                    var obj = Main.DeserializeBinary<ServerMap>(CurrentFile.Data.ToArray()) as ServerMap;
                    if (obj == null)
                    {
                        Util.Util.SafeNotify("ERROR DOWNLOADING MAP: NULL");
                    }
                    else
                    {
                        Main.AddMap(obj);
                    }
                }
                else if (CurrentFile.Type == FileType.Script)
                {
                    try
                    {
                        var scriptText = Encoding.UTF8.GetString(CurrentFile.Data.ToArray());
                        var newScript = LoadScript(CurrentFile.Filename, CurrentFile.Resource, scriptText);
                        PendingScripts.ClientsideScripts.Add(newScript);
                    }
                    catch (ArgumentException)
                    {
                        CurrentFile.Dispose();
                        if (File.Exists(CurrentFile.FilePath))
                        {
                            try { File.Delete(CurrentFile.FilePath); }
                            catch { }
                        }
                    }
                }
                else if (CurrentFile.Type == FileType.EndOfTransfer)
                {
                    if (Main.JustJoinedServer)
                    {
                        World.RenderingCamera = null;
                        Main.MainMenu.TemporarilyHidden = false;
                        Main.MainMenu.Visible = false;
                        Main.JustJoinedServer = false;
                    }

                    List<string> AffectedResources = new List<string>();
                    AffectedResources.AddRange(PendingScripts.ClientsideScripts.Select(cs => cs.ResourceParent));

                    Main.StartClientsideScripts(PendingScripts);
                    PendingScripts.ClientsideScripts.Clear();

                    Main.InvokeFinishedDownload(AffectedResources);
                }
                else if (CurrentFile.Type == FileType.CustomData)
                {
                    string data = Encoding.UTF8.GetString(CurrentFile.Data.ToArray());

                    JavascriptHook.InvokeCustomDataReceived(CurrentFile.Resource, data);
                }
            }
            finally
            {
                CurrentFile.Dispose();

                if (CurrentFile.Type == FileType.Normal && File.Exists(CurrentFile.FilePath))
                {
                    var mime = MimeTypes.GetMimeType(File.ReadAllBytes(CurrentFile.FilePath), CurrentFile.FilePath);

                    if (!_allowedFiletypes.Contains(mime))
                    {
                        try { File.Delete(CurrentFile.FilePath); }
                        catch { }

                        Screen.ShowNotification("Disallowed file type: " + mime + "~n~" + CurrentFile.Filename);
                    }
                }

                CurrentFile = null;
            }*/
        }
    }
}
