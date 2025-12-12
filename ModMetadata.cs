using BoneLib.Notifications;
using MelonLoader;
using MelonLoader.Utils;
using System.Collections;
using UnityEngine.Networking;

namespace freakAssist {
    public struct ModMetadata {
        public string Version;
        public string Author;
        public string Game;
        public string ModLocation;
        public string IconLocation;
        public string Description;

        public void DownloadMod(string fileName, string url) {
            var fileUrl = url + ModLocation;
            MelonCoroutines.Start(DownloadModRoutine(fileName, fileUrl));
        }

        private IEnumerator DownloadModRoutine(string fileName, string fileUrl) {
            Notification noti = new();
            noti.ShowTitleOnPopup = true;
            noti.Title = "FreakAssist";
            noti.Message = $"Downloading Mod {fileName}";
            Notifier.Send(noti);
            MelonLogger.Msg(noti.Message);

            UnityWebRequest downloadRequest = UnityWebRequest.Get(fileUrl);
            
            downloadRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return downloadRequest.SendWebRequest();
            if (downloadRequest.result != UnityWebRequest.Result.Success) {
                noti.Message = $"Error Downloading Mod\n{downloadRequest.error}";
            }
            else {
                var path = Path.Combine(MelonEnvironment.ModsDirectory, fileName);
                DownloadHandlerBuffer buffer = downloadRequest.downloadHandler.TryCast<DownloadHandlerBuffer>();
                File.WriteAllBytes(path, buffer.data);
                noti.Message = $"Downloaded Mod\n{path} (pls restart the gaem)";
            }
            Notifier.Send(noti);
            MelonLogger.Msg(noti.Message);
        }
    }
}
