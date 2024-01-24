using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ShowLootOnPlanet.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using Color = UnityEngine.Color;

namespace ShowLootOnPlanet
{
    public enum ColorType
    {
        White,
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple,
        Pink
    }
    [BepInDependency("ainavt.lc.lethalconfig")]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class PlanetModBase : BaseUnityPlugin
    {
        private const string GUID = "funfoxrr.PlanetLoot";
        private const string NAME = "Show Planet Loot";
        private const string VERSION = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(GUID);

        public static PlanetModBase Instance;

        internal static ManualLogSource log;

        public static BepInEx.Configuration.ConfigEntry<ColorType> color;
        public static ColorType colorValue;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            log = BepInEx.Logging.Logger.CreateLogSource(GUID);

            log.LogInfo("The mod has awakened!");

            color = Config.Bind("General", "Color", ColorType.Green, "Change the color of the text!");
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<ColorType>(color, new EnumDropDownOptions {}));
            color.Value = colorValue;

            color.SettingChanged += Color_SettingChanged;

            harmony.PatchAll(typeof(PlanetModBase));
            harmony.PatchAll(typeof(ScanPatch));
        }

        private void Color_SettingChanged(object sender, EventArgs e)
        {
            colorValue = color.Value;
        }
    }
}
