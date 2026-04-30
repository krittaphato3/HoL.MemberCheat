using BepInEx;
using HarmonyLib;
using UnityEngine;
using YuanAPI;

namespace HoLMod.MemberCheat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            YuanLogger.LogInfo("MemberCheat v4.0 loaded! F8 to open.");
            gameObject.AddComponent<MainUI>();
        }
    }
}