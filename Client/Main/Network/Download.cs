using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using RDRN_Core.Native;
using RDRN_Core.Streamer;
using RDRN_Core.Util;
using Shared;
using Lidgren.Network;
using Newtonsoft.Json;

namespace RDRN_Core
{
    public partial class Main
    {
        private Thread _httpDownloadThread;
        private bool _cancelDownload;

        private void StartFileDownload(string address)
        {
            _cancelDownload = false;

            _httpDownloadThread?.Abort();
            _httpDownloadThread = new Thread((ThreadStart)delegate
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        var manifestJson = wc.DownloadString(address + "/manifest.json");

                        var obj = JsonConvert.DeserializeObject<FileManifest>(manifestJson);

                        wc.DownloadProgressChanged += (sender, args) =>
                        {
                            _threadsafeSubtitle = "Downloading " + args.ProgressPercentage;
                        };

                        foreach (var resource in obj.exportedFiles)
                        {
                            if (!Directory.Exists(FileTransferId._DOWNLOADFOLDER_ + resource.Key))
                                Directory.CreateDirectory(FileTransferId._DOWNLOADFOLDER_ + resource.Key);

                            for (var index = resource.Value.Count - 1; index >= 0; index--)
                            {
                                var file = resource.Value[index];
                                if (file.type == FileType.Script) continue;

                                var target = Path.Combine(FileTransferId._DOWNLOADFOLDER_, resource.Key, file.path);

                                if (File.Exists(target))
                                {
                                    var newHash = DownloadManager.HashFile(target);

                                    if (newHash == file.hash) continue;
                                }

                                wc.DownloadFileAsync(
                                    new Uri($"{address}/{resource.Key}/{file.path}"), target);

                                while (wc.IsBusy)
                                {
                                    Thread.Yield();
                                    if (!_cancelDownload) continue;
                                    wc.CancelAsync();
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Exception(ex, "HTTP FILE DOWNLOAD");
                }
            });
        }

        public static void InvokeFinishedDownload(List<string> resources)
        {
            var confirmObj = Client.CreateMessage();
            confirmObj.Write((byte)PacketType.ConnectionConfirmed);
            confirmObj.Write(true);
            confirmObj.Write(resources.Count);

            for (int i = 0; i < resources.Count; i++)
            {
                confirmObj.Write(resources[i]);
            }

            Client.SendMessage(confirmObj, NetDeliveryMethod.ReliableOrdered, (int)ConnectionChannel.SyncEvent);

            HasFinishedDownloading = true;
            //Function.Call((Hash)0x10D373323E5B9C0D); //_REMOVE_LOADING_PROMPT
            Function.Call(Hash.DISPLAY_RADAR, true);


            LogManager.WriteLog("Changement de skin");

            Util.Util.SetPlayerSkin(PedHash.mp_male);
        }
    }
}
