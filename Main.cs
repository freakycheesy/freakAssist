using BoneLib.BoneMenu;
using Newtonsoft.Json;
using MelonLoader;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[assembly: MelonInfo(typeof(freakAssist.Main), "freakAssist", "1.0.0", "freakycheesy", "https://github.com/freakycheesy/freakAssist.git")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace freakAssist
{
    public class Main : MelonMod
    {
        public static Main Instance;
        public Page Page;
        public Page ModsPage;
        public const string urlPath = "https://github.com/freakycheesy/freakycheesy.github.io/raw/refs/heads/main/";
        public Dictionary<string, ModMetadata> Mods = new();
        public override void OnInitializeMelon()
        {
            Instance = this;
            Page = Page.Root.CreatePage("FreakAssist", Color.green);
            Page.CreateFunction("Refresh", Color.green, () => MelonCoroutines.Start(RequestMods()));
            ModsPage = Page.CreatePage("MODS LIST", Color.green);
            LoggerInstance.Msg("Initialized.");
            MelonCoroutines.Start(RequestMods());
        }

        private IEnumerator RequestMods() {
            UnityWebRequest www = UnityWebRequest.Get($"{urlPath}mods.json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                MelonLogger.Error(www.error);
            }
            else {
                // Show results as text
                var json = www.downloadHandler.text;
                MelonLogger.Msg(www.downloadHandler.text);
                Mods = JsonConvert.DeserializeObject<Dictionary<string, ModMetadata>>(json);
            }
            CreateMods();
        }
        private void CreateMods() {
            ModsPage.RemoveAll();
            foreach (var mod in Mods) {
                if (mod.Value.Game == Application.productName)
                    continue;
                var modPage = ModsPage.CreatePage(mod.Key, Color.green);
                modPage.CreateFunction($"Version: {mod.Value.Version}", Color.green, null);
                modPage.CreateFunction($"Author: {mod.Value.Author}", Color.green, null);
                modPage.CreateFunction($"Description: {mod.Value.Description}", Color.green, null);
                modPage.CreateFunction("Download", Color.green, ()=>mod.Value.DownloadMod(mod.Key + ".dll", urlPath));
            }
        }
    }
}