using AntlerShed.EnemySkinKit.Patches;
using AntlerShed.SkinRegistry;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AntlerShed.EnemySkinKit
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("antlershed.lethalcompany.enemyskinregistry")]
    public sealed class EnemySkinKit : BaseUnityPlugin
    {
        public const string modGUID = "AntlerShed.LethalCompany.EnemySkinKit";
        public const string modName = "Vanilla Enemy Skin Kit";
        public const string modVersion = "1.3.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static ManualLogSource SkinKitLogger { get; private set; } = BepInEx.Logging.Logger.CreateLogSource(modGUID);
        private static ConfigEntry<LogLevel> logLevel;
        internal static LogLevel LogLevelSetting => logLevel.Value;

        private void Awake()
        {
            harmony.PatchAll(typeof(AudioSourcePatch));
            logLevel = Config.Bind("LogLevel", "Logging", LogLevel.ERROR, "Verbosity setting of the logger.");
        }
    }

    internal enum LogLevel
    {
        NONE,
        ERROR,
        WARN,
        INFO
    }
}

