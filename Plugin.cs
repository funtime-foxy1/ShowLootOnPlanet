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
    public enum OffsetGUI
    {
        Original,
        Lowered,
        Lowered2,
        Uppered,
        Uppered2
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
        public static BepInEx.Configuration.ConfigEntry<OffsetGUI> offset;
        public static BepInEx.Configuration.ConfigEntry<float> fontSize;
        public static BepInEx.Configuration.ConfigEntry<float> lifetime;

        public static ColorType colorValue;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            log = BepInEx.Logging.Logger.CreateLogSource(GUID);

            log.LogInfo("The mod has awakened!");

            color = Config.Bind("Customize", "Color", ColorType.Green, "Change the color of the text!");
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<ColorType>(color, new EnumDropDownOptions { RequiresRestart = false }));
            color.Value = colorValue;

            lifetime = Config.Bind("Customize", "Lifetime", 3f, "How long does it last? ShipLoot: 5");
            LethalConfigManager.AddConfigItem(new FloatStepSliderConfigItem(lifetime, new FloatStepSliderOptions { RequiresRestart = false, Step = .25f, Min = 1f, Max = 6f }));

            offset = Config.Bind("Support", "Offset", OffsetGUI.Original, "Just in case for other mods intersect. (If needed, may turn into a slider)");
            LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<OffsetGUI>(offset, new EnumDropDownOptions { RequiresRestart = false }));

            fontSize = Config.Bind("Customize", "Font Size", 19f, "Change the size of the text.");
            LethalConfigManager.AddConfigItem(new FloatStepSliderConfigItem(fontSize, new FloatStepSliderOptions { RequiresRestart = false, Step = .25f, Min = 6, Max = 24 }));

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
