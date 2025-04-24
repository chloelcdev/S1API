using System;
using System.IO;
using System.Reflection;
using MelonLoader;

[assembly: MelonInfo(typeof(S1APILoader.S1APILoader), "S1APILoader", "{VERSION_NUMBER}", "KaBooMa")]

namespace S1APILoader
{
    public class S1APILoader : MelonPlugin
    {
        private const string BuildFolderName = "S1API";
            
        public override void OnPreModsLoaded()
        {
            string? pluginsFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (pluginsFolder == null)
                throw new Exception("Failed to identify plugins folder.");
            
            string buildsFolder = Path.Combine(pluginsFolder, BuildFolderName);

            string activeBuild = MelonUtils.IsGameIl2Cpp() ? "Il2Cpp" : "Mono";
            MelonLogger.Msg($"Loading S1API for {activeBuild}...");
            
            string s1APIBuildFile = Path.Combine(buildsFolder, $"S1API.{activeBuild}.dll");

            // FIX: https://github.com/KaBooMa/S1API/issues/30
            // Manual assembly loading versus file manipulation.
            // Thunderstore doesn't pick it up if we do file manipulation.
            Assembly assembly = Assembly.LoadFile(s1APIBuildFile);
            MelonAssembly melonAssembly = MelonAssembly.LoadMelonAssembly(s1APIBuildFile, assembly);
            foreach (MelonBase melon in melonAssembly.LoadedMelons)
                melon.Register();
            
            MelonLogger.Msg($"Successfully loaded S1API for {activeBuild}!");
        }
    }
}