using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FasterMovementMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class FasterMovementBase : BaseUnityPlugin
    {
        private const string modGUID = "Bnuey.FasterMovementMod";
        private const string modName = "Faster Movement Mod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static FasterMovementBase Instance;

        internal ManualLogSource mls;

        private static ConfigEntry<float> CloseSpeed;
        private static ConfigEntry<float> FarSpeed;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Faster Movement Mod Enabled");

            harmony.PatchAll();

            CloseSpeed = Config.Bind("General", "Camera Zoomed In Speed", 50f, "How fast the camera moves when fully zoomed in");
            FarSpeed = Config.Bind("General", "Camera Zoomed Out Speed", 50f, "How fast the camera moves when zoomed out");
        }

        [HarmonyPatch(typeof(CameraController))]

        internal class CameraControllerBPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            static void fasterMovementPatch(ref float ___manualPanSpeed, ref float ___closeManualPanSpeed)
            {
                ___manualPanSpeed = FarSpeed.Value;
                ___closeManualPanSpeed = CloseSpeed.Value;
            }
        }

    }
}
