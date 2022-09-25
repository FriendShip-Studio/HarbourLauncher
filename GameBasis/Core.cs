using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjBobcat.Class.Helper;
using ProjBobcat.Class.Model.Mojang;
using ProjBobcat.DefaultComponent.Launch;
using ProjBobcat.DefaultComponent.Launch.GameCore;
using ProjBobcat.DefaultComponent.Logging;
using System.IO;
using System.Text.Json.Nodes;
using System.Collections.Generic;

namespace HarbourLauncher_Reloaded.GameBasis
{
    public static class Core
    {
        public static DefaultGameCore core;
        public static string rootPath;
        public static Guid clientToken;

        public static void CoreInit()
        {
            rootPath = Environment.CurrentDirectory + "\\.minecraft";
            var fullRootPath = Path.GetFullPath(rootPath);
            clientToken = Guid.NewGuid();
            core = new DefaultGameCore
            {
                ClientToken = clientToken, // Pick any GUID as you like, and it does not affect launching.
                RootPath = rootPath,
                VersionLocator = new DefaultVersionLocator(fullRootPath, clientToken)
                {
                    LauncherProfileParser = new DefaultLauncherProfileParser(fullRootPath, clientToken),
                    LauncherAccountParser = new DefaultLauncherAccountParser(fullRootPath, clientToken)
                },
                GameLogResolver = new DefaultGameLogResolver()
            };
        }

        public static async Task<VersionManifest?> GetVersionManifestTaskAsync()
        {
            const string vmUrl = "http://launchermeta.mojang.com/mc/game/version_manifest.json";
            var contentRes = await HttpHelper.Get(vmUrl);
            var content = await contentRes.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<VersionManifest>(content);

            return model;
        }

        public static void JavaPathRecord(List<string> javaList)
        {
            rootPath = Environment.CurrentDirectory + "\\HL_config.json";

            string jsonText = File.ReadAllText(rootPath);

            Dictionary<string, dynamic>? configDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

            configDict["javaPath"] = javaList;

            File.WriteAllText(rootPath, JsonConvert.SerializeObject(configDict));

        }

        public static void PlayerNameRecord(string playerName)
        {
            rootPath = Environment.CurrentDirectory + "\\HL_config.json";

            string jsonText = File.ReadAllText(rootPath);

            Dictionary<string, dynamic>? configDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonText);

            configDict["playerName"] = playerName;

            File.WriteAllText(rootPath, JsonConvert.SerializeObject(configDict));
        }

    }
}
