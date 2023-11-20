using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushCamera;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
public class Plugin : BaseUnityPlugin {
    public static ManualLogSource Log = null!;
    public static Harmony Harmony = null!;

    public static ConfigEntry<float> BaseSpeed = null!;
    public static ConfigEntry<float> ShiftMultiplier = null!;
    public static ConfigEntry<float> CtrlMultiplier = null!;
    public static ConfigEntry<float> MouseSensitivity = null!;

    public static ConfigEntry<KeyCode> Key = null!;

    public static bool Active = false;
    public static List<int> RewiredMaps = new();

    private void Awake() {
        Log = this.Logger;

        Key = this.Config.Bind(
            "Freecam",
            "Key",
            KeyCode.RightAlt,
            "The key to toggle freecam."
        );
        BaseSpeed = this.Config.Bind(
            "Freecam",
            "BaseSpeed",
            5f,
            "The speed you move at in freecam by default."
        );
        ShiftMultiplier = this.Config.Bind(
            "Freecam",
            "ShiftMultiplier",
            10f,
            "The speed multiplier when holding shift."
        );
        CtrlMultiplier = this.Config.Bind(
            "Freecam",
            "CtrlMultiplier",
            0.1f,
            "The speed multiplier when holding ctrl."
        );
        MouseSensitivity = this.Config.Bind(
            "Freecam",
            "MouseSensitivity",
            5f,
            "The mouse sensitivity when in freecam."
        );

        Harmony = new Harmony("BombRushCamera.Harmony");
        Harmony.PatchAll();
    }

    private void Update() {
        if (Input.GetKeyDown(Key.Value)) {
            Active = !Active;

            var gameInput = Core.Instance.GameInput;
            var player = gameInput.rewiredMappingHandler.GetRewiredPlayer();
            var maps = player.controllers.maps.GetAllMaps().Where(x => x.enabled);

            // Save and restore Rewired maps so we don't have input colliding
            if (Active) {
                RewiredMaps = maps.Select(x => x.id).ToList();
                gameInput.DisableAllControllerMaps();
            } else {
                foreach (var map in RewiredMaps) gameInput.EnableControllerMap(map);
            }
        }
    }
}
