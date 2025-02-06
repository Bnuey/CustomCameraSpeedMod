using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FasterMovementMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class FasterMovementBase : BaseUnityPlugin
    {
        private const string modGUID = "Bnuey.FasterMovementMod";
        private const string modName = "Faster Movement Mod";
        private const string modVersion = "1.1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static FasterMovementBase Instance;

        internal ManualLogSource mls;

        private static ConfigEntry<float> CloseSpeed;
        private static ConfigEntry<float> FarSpeed;
        private static ConfigEntry<float> HoldShiftMultiplier;

        private static ConfigEntry<KeyboardShortcut> FastMove;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Faster Movement Mod Enabled");

            harmony.PatchAll();

            CloseSpeed = Config.Bind("General", "Camera Zoomed In Speed", 10f, "How fast the camera moves when fully zoomed in");
            FarSpeed = Config.Bind("General", "Camera Zoomed Out Speed", 10f, "How fast the camera moves when zoomed out");
            HoldShiftMultiplier = Config.Bind("General", "Shift Multiplier", 5f, "Multiplies how fast the camera speed is when holding shift (2 = twice as fast)");
            FastMove = Config.Bind("Hotkeys", "Move Faster", new KeyboardShortcut(KeyCode.LeftShift));
        }

        [HarmonyPatch(typeof(CameraController))]

        internal class CameraControllerBPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            static void fasterMovementPatch(ref float ___manualPanSpeed, ref float ___closeManualPanSpeed)
            {
                ___manualPanSpeed = FarSpeed.Value * HoldShiftMultiplier.Value;
                ___closeManualPanSpeed = CloseSpeed.Value * HoldShiftMultiplier.Value;
            }

            static bool _fastOn = true;

            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            static void UpdatePatch(ref float ___manualPanSpeed, ref float ___closeManualPanSpeed)
            {
                if (FastMove.Value.IsDown())
                {
                    _fastOn = !_fastOn;

                    if (_fastOn)
                    {
                        ___manualPanSpeed = FarSpeed.Value * HoldShiftMultiplier.Value;
                        ___closeManualPanSpeed = CloseSpeed.Value * HoldShiftMultiplier.Value;
                    }
                    else
                    {
                        ___manualPanSpeed = FarSpeed.Value;
                        ___closeManualPanSpeed = CloseSpeed.Value;
                    }



                    
                }

            }
        }

    }
}
